using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using AnlabMvc.Models.Order;
using Anlab.Core.Domain;
using Anlab.Core.Models;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;
using AnlabMvc.Extensions;
using AnlabMvc.Models.Roles;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Anlab.Core.Models.AggieEnterpriseModels;
using Anlab.Core.Services;

namespace AnlabMvc.Controllers
{
    [Authorize]
    public class OrderController : ApplicationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderService _orderService;
        private readonly IOrderMessageService _orderMessageService;
        private readonly ILabworksService _labworksService;
        private readonly IFinancialService _financialService;
        private IAggieEnterpriseService _aggieEnterpriseService;
        private readonly IDocumentSigningService _documentSigningService;
        private readonly AggieEnterpriseSettings _aeSettings;
        private readonly AppSettings _appSettings;

        private const string processingCode = "PROC";

        public OrderController(ApplicationDbContext context, IOrderService orderService, IOrderMessageService orderMessageService, ILabworksService labworksService, IFinancialService financialService, IOptions<AppSettings> appSettings, IOptions<AggieEnterpriseSettings> aeSettings, IAggieEnterpriseService aggieEnterpriseService, IDocumentSigningService documentSigningService)
        {
            _context = context;
            _orderService = orderService;
            _orderMessageService = orderMessageService;
            _labworksService = labworksService;
            _financialService = financialService;
            _aggieEnterpriseService = aggieEnterpriseService;
            _documentSigningService = documentSigningService;
            _aeSettings = aeSettings.Value;
            _appSettings = appSettings.Value;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _context.Orders.Where(a => a.CreatorId == CurrentUserId).Select(a => new OrderListModel
            {
                Id              = a.Id,
                RequestNum      = a.RequestNum,
                Project         = a.Project,
                Status          = a.Status,
                Paid            = a.Paid,
                Created         = a.Created,
                Updated         = a.Updated,
                ShareIdentifier = a.ShareIdentifier
            }).ToArrayAsync();

            return View(model);
        }

        public async Task<IActionResult> Favorites()
        {
            var savedOrders = await _context.SavedOrders.Where(a => a.UserId == CurrentUserId).Select(a => new OrderListModel
            {
                Id              = a.Order.Id,
                RequestNum      = a.Order.RequestNum,
                Project         = a.Order.Project,
                Status          = a.Order.Status,
                Paid            = a.Order.Paid,
                Created         = a.Order.Created,
                Updated         = a.Order.Updated,
                ShareIdentifier = a.Order.ShareIdentifier
            }).ToArrayAsync();

            return View(savedOrders);
        }

        [HttpPost]
        public async Task<IActionResult> SaveLink(Guid id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(a => a.ShareIdentifier == id);
            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Finalized && order.Status != OrderStatusCodes.Complete)
            {
                return NotFound();
            }

            if (await _context.SavedOrders.AnyAsync(a => a.OrderId == order.Id && a.UserId == CurrentUserId))
            {
                Message = "You have already added this to your saved orders";
                return RedirectToAction("Link", "Results", new {id});
            }

            //TODO: Maybe allow user to specify a short note, saved in the SavedOrders table.

            var user = _context.Users.Single(a => a.Id == CurrentUserId);

            var savedOrder = new SavedOrder();
            savedOrder.OrderId = order.Id;
            //savedOrder.Order = order;
            savedOrder.UserId = CurrentUserId;
            //savedOrder.User = user;

            await _context.AddAsync(savedOrder);
            await _context.SaveChangesAsync();

            Message = "Order saved to your list";
            return RedirectToAction("Link", "Results", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLink(Guid id)
        {
            var order = await _context.Orders.SingleOrDefaultAsync(a => a.ShareIdentifier == id);
            if (order == null)
            {
                return NotFound();
            }

            var savedOrder = await _context.SavedOrders.SingleOrDefaultAsync(a => a.OrderId == order.Id && a.UserId == CurrentUserId);
            if (savedOrder == null)
            {
                return NotFound();
            }

            _context.SavedOrders.Remove(savedOrder);
            await _context.SaveChangesAsync();

            Message = "Saved order removed from list";
            return RedirectToAction("Favorites");
        }

        public async Task<IActionResult> Create()
        {
            var joined = await _orderService.PopulateTestItemModel();
            var proc = await _labworksService.GetPrice(processingCode);

            var user = _context.Users.Single(a => a.Id == CurrentUserId);

            var model = new OrderEditModel {
                TestItems = joined.ToArray(),
                InternalProcessingFee = Math.Ceiling(proc.Cost),
                ExternalProcessingFee = Math.Ceiling(proc.Cost * _appSettings.NonUcRate),
                Defaults = new OrderEditDefaults {
                    DefaultAccount = user.Account?.ToUpper(),
                    DefaultEmail = user.Email,
                    DefaultCompanyName = user.CompanyName,
                    DefaultAcAddr = user.BillingContactAddress,
                    DefaultAcEmail = user.BillingContactEmail,
                    DefaultAcName = user.BillingContactName,
                    DefaultAcPhone = user.BillingContactPhone,
                },
                UseCoa = _aeSettings.UseCoA,
            };

            if (!string.IsNullOrWhiteSpace(user.ClientId))
            {
                //Has a default client id, so try to get defaults:
                var defaults = await _labworksService.GetClientDetails(user.ClientId);
                if (defaults != null)
                {
                    model.Defaults.DefaultAccount = model.Defaults.DefaultAccount ?? defaults.DefaultAccount;
                    model.Defaults.DefaultClientId = defaults.ClientId;
                    model.Defaults.DefaultClientIdName = defaults.Name;
                    model.Defaults.DefaultSubEmail = defaults.SubEmail;
                    model.Defaults.DefaultCopyEmail = defaults.CopyEmail;

                }
            }

            return View(model);
        }

        private async Task<User> GetAdminUser()
        {
            var user = await _context.Users.SingleAsync(a => a.Id == CurrentUserId);
            if (User.IsInRole(RoleCodes.Admin))
            {
                return user;
            }
            return null;
        }

        private bool AllowAdminOverride()
        {
            if (User.IsInRole(RoleCodes.Admin))
            {
                return true;
            }
            return false;
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null) {
                return NotFound();
            }
            if (order.CreatorId != CurrentUserId)
            {
                if (AllowAdminOverride() == false)
                {
                    ErrorMessage = "You don't have access to this order.";
                    return RedirectToAction("Index");
                }

                Message = "Warning, Admin editing order for user.";
            }

            if (order.Status != OrderStatusCodes.Created)
            {
                ErrorMessage = "You can't edit an order that has been confirmed.";
                return RedirectToAction("Index");
            }

            var joined = await _orderService.PopulateTestItemModel();
            var proc = await _labworksService.GetPrice(processingCode);

            var model = new OrderEditModel {
                TestItems = joined.ToArray(),
                Order = order,
                InternalProcessingFee = Math.Ceiling(proc.Cost),
                ExternalProcessingFee = Math.Ceiling(proc.Cost * _appSettings.NonUcRate),
                Defaults = new OrderEditDefaults
                {
                    DefaultEmail = order.Creator.Email
                },
                UseCoa = _aeSettings.UseCoA,
            };

            return View(model);
        }

        public async Task<IActionResult> Copy(Guid id, bool adminCopy = false)
        {
            var orderToCopy = await _context.Orders.SingleOrDefaultAsync(o => o.ShareIdentifier == id);
            if (orderToCopy == null)
            {
                return NotFound();
            }

            var user = _context.Users.Single(a => a.Id == CurrentUserId);

            var order = await _orderService.DuplicateOrder(orderToCopy, adminCopy);
            order.CreatorId = CurrentUserId;
            order.Creator = user;
            order.ShareIdentifier = Guid.NewGuid();

            if (adminCopy)
            {
                if (!User.IsInRole(RoleCodes.Admin))
                {
                    throw new Exception("Permissions Missing");
                }
                order.CreatorId = orderToCopy.CreatorId;
                order.Creator = orderToCopy.Creator;
                order.ClientId = orderToCopy.ClientId;
                order.ClientName = orderToCopy.ClientName;
                order.Status = OrderStatusCodes.Confirmed;
                var orderDetails = order.GetOrderDetails();
                var labComments = new StringBuilder(orderDetails.LabComments);
                labComments.AppendLine();
                labComments.AppendLine($"Duplicated from {orderToCopy.Id}");
                orderDetails.LabComments = labComments.ToString();
                order.SaveDetails(orderDetails);
                order.SavedTestDetails = orderToCopy.SavedTestDetails; //Use the test codes the original order was created from

                order.History.Add(new History
                {
                    Action = "Copied - Admin",
                    Status = order.Status,
                    ActorId = user.NormalizedUserName,
                    ActorName = user.Name,
                    JsonDetails = order.JsonDetails,
                    Notes = $"Copied from Order {orderToCopy.Id}",
                });
            }
            else
            {

                order.History.Add(new History
                {
                    Action = "Copied",
                    Status = order.Status,
                    ActorId = order.Creator.NormalizedUserName,
                    ActorName = order.Creator.Name,
                    JsonDetails = order.JsonDetails,
                    Notes = $"Copied from Order {orderToCopy.Id}",
                });
            }

            _context.Add(order);
            await _context.SaveChangesAsync();
            if (adminCopy)
            {
                return RedirectToAction("AddRequestNumber", "Lab", new {id = order.Id});
            }

            var originalDetails = orderToCopy.GetOrderDetails();
            if (!string.IsNullOrWhiteSpace(originalDetails.AdditionalInfo))
            {
                Message = "Note! Comments not copied.";
            }
            return RedirectToAction("Edit", new {id = order.Id});

        }


        [HttpPost]
        public async Task<IActionResult> Save(OrderSaveModel model)
        {
            User adminUser = null;
            if (!ModelState.IsValid)
            {
                var errors = new List<string>();
                foreach (var result in ModelState.Values)
                {
                    foreach (var errs in result.Errors)
                    {
                        errors.Add(errs.ErrorMessage);
                    }
                }

                //TODO: A better way to return errors. Or maybe not, they shouldn't really ever happen.
                return Json(new { success = false, message = "There were problems with your order. Unable to save. Errors: " + string.Join("--", errors) });
            }

            var idForRedirection = 0;

            if (model.OrderId.HasValue)
            {
                var orderToUpdate = await _context.Orders.Include(i => i.Creator).SingleAsync(a => a.Id == model.OrderId.Value);
                if (orderToUpdate.CreatorId != CurrentUserId)
                {
                    adminUser = await GetAdminUser();
                    if (adminUser == null)
                    {
                        return Json(new { success = false, message = "This is not your order." });
                    }
                }
                if (orderToUpdate.Status != OrderStatusCodes.Created)
                {
                    return Json(new { success = false, message = "This has been confirmed and may not be updated." });
                }

                var allTests = await _orderService.PopulateTestItemModel();
                orderToUpdate.SaveTestDetails(allTests);

                _orderService.PopulateOrder(model, orderToUpdate);

                orderToUpdate.History.Add(new History
                {
                    Action = "Edited",
                    Status = orderToUpdate.Status,
                    ActorId = adminUser != null ? adminUser.NormalizedUserName : orderToUpdate.Creator.NormalizedUserName,
                    ActorName = adminUser != null ? adminUser.Name : orderToUpdate.Creator.Name,
                    JsonDetails = orderToUpdate.JsonDetails,
                    Notes = adminUser != null ? "Admin Override" : string.Empty
                });

                idForRedirection = model.OrderId.Value;
                await _context.SaveChangesAsync();
            }
            else
            {
                var user = _context.Users.Single(a => a.Id == CurrentUserId);
                var order = new Order
                {
                    CreatorId = CurrentUserId,
                    Creator = user,
                    Status = OrderStatusCodes.Created,
                    ShareIdentifier = Guid.NewGuid(),
                };

                var allTests = await _orderService.PopulateTestItemModel();
                order.SaveTestDetails(allTests);

                _orderService.PopulateOrder(model, order);

                order.History.Add(new History
                {
                    Action = "Created",
                    Status = order.Status,
                    ActorId = order.Creator.NormalizedUserName,
                    ActorName = order.Creator.Name,
                    JsonDetails = order.JsonDetails
                });
                _context.Add(order);
                await _context.SaveChangesAsync();
                idForRedirection = order.Id;
            }


            return Json(new { success = true, id = idForRedirection });
        }


        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return NotFound();
            }

            if (order.Status == OrderStatusCodes.Created)
            {
                ErrorMessage = "Must confirm order before viewing details.";
                return RedirectToAction("Index");
            }
            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();

            return View(model);
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.CreatorId != CurrentUserId)
            {
                if (AllowAdminOverride() == false)
                {
                    ErrorMessage = "You don't have access to this order.";
                    return NotFound();
                }
                Message = "Warning, Admin editing order for user.";
            }

            await _orderService.UpdateTestsAndPrices(order);

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();

            return View(model);
        }

        // Used only for confirmation without signature
        [HttpPost]
        [Authorize(Roles = RoleCodes.Admin)]
        public async Task<IActionResult> Confirmation(int id, bool confirm)
        {
            return await ConfirmOrder(id);
        }

        // Interacts with docusign to get the signature url
        [HttpPost]
        public async Task<IActionResult> PendingSignature(int id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (User.IsImpersonating())
            {
                ErrorMessage = "You can't sign orders while impersonating.";
                return RedirectToAction("Index");
            }

            // only the creator can send for signature
            if (order.CreatorId == CurrentUserId)
            {
                var signatureUrl = await _documentSigningService.SendForEmbeddedSigning(id);

                if (string.IsNullOrWhiteSpace(signatureUrl))
                {
                    ErrorMessage = "Unable to get signature url";
                    return RedirectToAction("Index");
                }

                return Redirect(signatureUrl);
            }

            // if they are an admin, they can confirm without signature
            if (User.IsInRole(RoleCodes.Admin))
            {
                return await ConfirmOrder(id);
            }

            // if they are not the creator or an admin, they can't do anything
            ErrorMessage = "You don't have access to this order.";
            return Forbid();
        }

        // Page that docusign redirects to after signing
        public async Task<IActionResult> SignatureCallback(int id)
        {
            // Check if the signature was successful by inspecting the "event" query string parameter
            var eventStatus = Request.Query["event"];

            if (eventStatus != "signing_complete")
            {
                ErrorMessage = "Unable to get signature";
                return RedirectToAction("Index");
            }

            var envelopeId = Request.Query["envelopeId"];

            if (string.IsNullOrWhiteSpace(envelopeId))
            {
                ErrorMessage = "Unable to get signature - envelope information missing";
                return RedirectToAction("Index");
            }

            return await ConfirmOrder(id, envelopeId);
        }

        // Page not visible by user which will be used to generate the signature document content
        public async Task<IActionResult> Document(int id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.CreatorId != CurrentUserId)
            {
                if (!User.IsInRole(RoleCodes.Admin))
                {
                    ErrorMessage = "You don't have access to this order.";
                    return NotFound();
                }
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();

            return View(model);
        }

        public async Task<IActionResult> Confirmed(int id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.CreatorId != CurrentUserId)
            {
                if (!User.IsInRole(RoleCodes.Admin))
                {
                    ErrorMessage = "You don't have access to this order.";
                    return NotFound();
                }
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();

            return View(model);
        }

        public async Task<IActionResult> ViewSignedDocument(int id)
        {
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // make sure the order has a signed envelope
            if (string.IsNullOrWhiteSpace(order.SignedEnvelopeId))
            {
                ErrorMessage = "No signed document found.";
                return RedirectToAction("Index");
            }

            if (order.CreatorId != CurrentUserId)
            {
                if (!User.IsInRole(RoleCodes.Admin))
                {
                    ErrorMessage = "You don't have access to this order.";
                    return NotFound();
                }
            }

            return await _documentSigningService.DownloadEnvelope(order.SignedEnvelopeId);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.Include(a => a.History).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.CreatorId != CurrentUserId)
            {
                ErrorMessage = "You don't have access to this order.";
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Created)
            {
                ErrorMessage = "Can't delete confirmed orders.";
                return RedirectToAction("Index");
            }

            _context.Remove(order);
            await _context.SaveChangesAsync();

            Message = "Order deleted";
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<ClientDetailsLookupModel> LookupClientId(string id)
        {
            var result = await _labworksService.GetClientDetails(id);
            if (result == null)
            {
                return null;
            }
            return result;
        }

        private async Task<IActionResult> ConfirmOrder(int id, string envelopeId = null)
        {
            User adminUser = null;
            var order = await _context.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.CreatorId != CurrentUserId)
            {
                adminUser = await GetAdminUser();
                if (adminUser == null)
                {
                    ErrorMessage = "You don't have access to this order.";
                    return NotFound();
                }
                Message = "Warning, Admin editing order for user.";
            }

            if (order.Status != OrderStatusCodes.Created)
            {
                ErrorMessage = "Already confirmed";
                return RedirectToAction("Index");
            }

            if (order.PaymentType == PaymentTypeCodes.UcDavisAccount)
            {
                var orderDetails = order.GetOrderDetails();
                try
                {
                    if (_aeSettings.UseCoA)
                    {
                        var validateAccount = await _aggieEnterpriseService.IsAccountValid(orderDetails.Payment.Account);
                        if (validateAccount.IsValid)
                        {
                            orderDetails.Payment.AccountName = validateAccount.Description;
                        }
                        else
                        {
                            orderDetails.Payment.AccountName = string.Empty;
                        }
                    }
                    else
                    {
                        orderDetails.Payment.AccountName = await _financialService.GetAccountName(orderDetails.Payment.Account);
                    }
                }
                catch
                {
                    orderDetails.Payment.AccountName = string.Empty;
                }
                order.SaveDetails(orderDetails);
                if (string.IsNullOrWhiteSpace(orderDetails.Payment.AccountName))
                {
                    await _context.SaveChangesAsync();
                    ErrorMessage = "Unable to verify UC Account number. Please edit your order and re-enter the UC account number. Then try again.";
                    return RedirectToAction("Confirmation", new {id = order.Id});
                }
            }
            if (order.PaymentType == PaymentTypeCodes.Other)
            {
                var orderDetails = order.GetOrderDetails();
                if (orderDetails.OtherPaymentInfo.PaymentType.Equals("Agreement", StringComparison.OrdinalIgnoreCase))
                {
                    await _orderMessageService.EnqueueBillingMessage(order, "Anlab Work Order Billing Info -- Agreement"); //TODO: Maybe a different one for an Agreement?
                }
            }

            await _orderService.UpdateTestsAndPrices(order);

            var orderDetailsForClient = order.GetOrderDetails();
            if (!string.IsNullOrWhiteSpace(orderDetailsForClient.ClientInfo.ClientId))
            {
                var result = await LookupClientId(orderDetailsForClient.ClientInfo.ClientId);
                if (result == null)
                {
                    Log.Error($"Error looking up clientId {orderDetailsForClient.ClientInfo.ClientId}");
                }
                else
                {
                    orderDetailsForClient.ClientInfo.Email = result.SubEmail;
                    orderDetailsForClient.ClientInfo.PhoneNumber = result.SubPhone;
                    orderDetailsForClient.ClientInfo.CopyPhone = result.CopyPhone;
                    orderDetailsForClient.ClientInfo.Department = result.Department;
                    order.SaveDetails(orderDetailsForClient);
                }

            }

            _orderService.UpdateAdditionalInfo(order);
            order.Status = OrderStatusCodes.Confirmed;

            if (!string.IsNullOrWhiteSpace(envelopeId))
            {
                order.SignedEnvelopeId = envelopeId;
            }

            order.History.Add(new History
            {
                Action = "Confirmed",
                Status = order.Status,
                ActorId = adminUser != null ? adminUser.NormalizedUserName : order.Creator.NormalizedUserName,
                ActorName = adminUser != null ? adminUser.Name : order.Creator.Name,
                JsonDetails = order.JsonDetails,
                Notes = adminUser != null ? "Admin Override" : string.Empty
            });

            await _orderMessageService.EnqueueCreatedMessage(order);

            await _context.SaveChangesAsync();

            return RedirectToAction("Confirmed", new {id = order.Id});
        }

    }

}

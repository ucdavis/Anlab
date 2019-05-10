using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Models;
using AnlabMvc.Models.FileUploadModels;
using AnlabMvc.Models.Order;
using AnlabMvc.Models.Roles;
using AnlabMvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Anlab.Core.Domain;
using Anlab.Core.Extensions;
using Anlab.Core.Services;
using AnlabMvc.Extensions;
using Serilog;
using AnlabMvc.Helpers;

namespace AnlabMvc.Controllers
{
    [Authorize(Roles = RoleCodes.Admin + "," + RoleCodes.LabUser)]
    public class LabController : ApplicationController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IOrderService _orderService;
        private readonly ILabworksService _labworksService;
        private readonly IOrderMessageService _orderMessageService;
        private readonly IFileStorageService _fileStorageService;
        private readonly ISlothService _slothService;
        private readonly IFinancialService _financialService;

        private const int _maxShownOrders = 1000;

        public LabController(ApplicationDbContext dbContext, IOrderService orderService, ILabworksService labworksService, IOrderMessageService orderMessageService, IFileStorageService fileStorageService, ISlothService slothService, IFinancialService financialService)
        {
            _dbContext = dbContext;
            _orderService = orderService;
            _labworksService = labworksService;
            _orderMessageService = orderMessageService;
            _fileStorageService = fileStorageService;
            _slothService = slothService;
            _financialService = financialService;
        }

        [HttpGet]
        public IActionResult Orders(bool showComplete)
        {
            var ordersQueryable = _dbContext.Orders
                    .Where(a => a.Status != OrderStatusCodes.Created);
            if (!showComplete)
            {
                ordersQueryable = ordersQueryable.Where(a => a.Status != OrderStatusCodes.Complete);

            }

            var orders = ordersQueryable.Select(c => new Order
                {
                    Id = c.Id,
                    ClientId = c.ClientId,
                    Creator = new User { Email = c.Creator.Email },
                    Created = c.Created,
                    Updated = c.Updated,
                    RequestNum = c.RequestNum,
                    Status = c.Status,
                    ShareIdentifier = c.ShareIdentifier,
                    Paid = c.Paid,
                    ClientName = c.ClientName
                })
            .OrderByDescending(a => a.Updated)
            .Take(_maxShownOrders)
            .ToList();

            if (orders.Count >= _maxShownOrders)
            {
                ErrorMessage = $"Warning, maximum orders displayed is {_maxShownOrders}";
            }

            ViewBag.ShowComplete = showComplete;

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id && o.Status != OrderStatusCodes.Created);

            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();
            model.HideLabDetails = false;

            await GetHistories(id, model);

            return View(model);
        }

        private async Task GetHistories(int id, OrderReviewModel model)
        {
            model.History = await _dbContext.History.Where(a => a.OrderId == id).Select(s =>
                new History
                {
                    Action = s.Action,
                    ActionDateTime = s.ActionDateTime,
                    Id = s.Id,
                    Status = s.Status,
                    ActorId = s.ActorId,
                    ActorName = s.ActorName,
                    Notes = s.Notes
                }).OrderBy(o => o.ActionDateTime).ToListAsync(); //Basically filtering out jsonDetails
        }

        [HttpPost]
        public async Task<IActionResult> ClearRequestNumber(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Confirmed)
            {
                ErrorMessage = "You can only clear the WRN on a confirmed order";
                return RedirectToAction("Orders");
            }

            var saveRequestNumber = order.RequestNum;

            order.RequestNum = null;
            var user = _dbContext.Users.Single(a => a.Id == CurrentUserId);
            order.History.Add(new History
            {
                Action = "Cleared Request Number",
                Status = order.Status,
                ActorId = user.NormalizedUserName,
                ActorName = user.Name,
                JsonDetails = order.JsonDetails,
                Notes = $"Request Number: {saveRequestNumber} Cleared",
            });

            Message = "NOTE!!! The order details will remain changed until you add a new request number";

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("AddRequestNumber", new {id});
        }


        public async Task<IActionResult> AddRequestNumber(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Confirmed) 
            {
                ErrorMessage = "You can only receive a confirmed order";
                return RedirectToAction("Orders");
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();
            model.HideLabDetails = false;

            await GetHistories(id, model);

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> AddRequestNumber(int id, bool confirm, string requestNum)
        {
            if (string.IsNullOrWhiteSpace(requestNum))
            {
                ErrorMessage = "A request number is required";
                return RedirectToAction("AddRequestNumber");
            }

            requestNum = requestNum.SafeToUpper();  //Force Uppercase

            var checkReqNum = await _dbContext.Orders.AnyAsync(i => i.Id != id && i.RequestNum == requestNum);
            if (checkReqNum)
            {
                ErrorMessage = "That request number is already in use";
#if !DEBUG
                return RedirectToAction("AddRequestNumber");
#endif
            }

            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Confirmed)
            {
                ErrorMessage = "You can only receive a confirmed order";
                return RedirectToAction("Orders");
            }


            order.RequestNum = requestNum;
            var result = await _orderService.OverwriteOrderFromDb(order);
            if (result.WasError)
            {
                if (result.ErrorMessage != null)
                {
                    ErrorMessage = $"Error. Unable to continue. Error looking up on Labworks: {result.ErrorMessage}";
                }
                else
                {
                    ErrorMessage =
                        string.Format("Error. Unable to continue. The following codes were not found locally: {0}",
                            string.Join(",", result.MissingCodes));
                }

                return RedirectToAction("Orders");
            }

            if (!string.IsNullOrWhiteSpace(order.ClientId) && order.ClientId != result.ClientId)
            {
                ErrorMessage = $"Warning!!! Client Id is changing from {order.ClientId} to {result.ClientId}";
            }

            order = await UpdateOrderFromLabworksResult(order, result);

            var user = _dbContext.Users.Single(a => a.Id == CurrentUserId);
            order.History.Add(new History
            {
                    Action = "Added Request Number",
                    Status = order.Status,
                    ActorId = user.NormalizedUserName,
                    ActorName = user.Name,
                    JsonDetails = order.JsonDetails,
                    Notes = $"Request Number: {order.RequestNum} {ErrorMessage}",
            });

            await _dbContext.SaveChangesAsync();

            Message = "Order updated from work request number";
            return RedirectToAction("Confirmation", new { id = id });

        }

        /// <summary>
        /// This should probably be moved to a service...
        /// </summary>
        /// <param name="order"></param>
        /// <param name="result"></param>
        /// <param name="finalizeModel"></param>
        /// <returns></returns>
        private async Task<Order> UpdateOrderFromLabworksResult(Order order, OverwriteOrderResult result, LabFinalizeModel finalizeModel = null)
        {
            order.ClientId = result.ClientId;
            order.ClientName = "[Not Found]"; //Updated below if we find it
            var orderDetails = order.GetOrderDetails();

            if (!string.IsNullOrWhiteSpace(order.ClientId))
            {

                //TODO: update other info if we pull it: phone numbers
                var clientInfo = await _labworksService.GetClientDetails(order.ClientId);

                if (clientInfo != null)
                {
                    orderDetails.ClientInfo.ClientId = order.ClientId;
                    orderDetails.ClientInfo.Email = clientInfo.SubEmail;
                    orderDetails.ClientInfo.Name = clientInfo.Name;
                    orderDetails.ClientInfo.PhoneNumber = clientInfo.SubPhone;
                    orderDetails.ClientInfo.CopyPhone = clientInfo.CopyPhone;
                    orderDetails.ClientInfo.Department = clientInfo.Department;
                    order.ClientName = clientInfo.Name;
                }

                order.AdditionalEmails = AdditionalEmailsHelper.AddClientInfoEmails(order, orderDetails.ClientInfo);

            }

            if (result.MissingTestsToAdd.Any())
            {
                var allTests = order.GetTestDetails();
                foreach (var missingTest in result.MissingTestsToAdd)
                {
                    allTests.Add(missingTest);
                }
                order.SaveTestDetails(allTests);
            }

            orderDetails.Quantity = result.Quantity;
            orderDetails.SelectedTests = result.SelectedTests;
            orderDetails.Total = orderDetails.SelectedTests.Sum(x => x.Total) + (orderDetails.Payment.IsInternalClient ? orderDetails.InternalProcessingFee : orderDetails.ExternalProcessingFee);
            orderDetails.Total = orderDetails.Total * result.RushMultiplier;
            orderDetails.RushMultiplier = result.RushMultiplier;
            orderDetails.LabworksSampleDisposition = result.LabworksSampleDisposition;

            if (finalizeModel != null)
            {
                orderDetails.LabComments = finalizeModel.LabComments;
                orderDetails.AdjustmentAmount = finalizeModel.AdjustmentAmount;
                if (orderDetails.AdjustmentAmount != 0)
                {
                    orderDetails.AdjustmentComments = finalizeModel.AdjustmentComments;
                }
            }



            order.SaveDetails(orderDetails);
            order.SaveBackedupTestDetails(result.BackedupTests);

            return order;
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }
            if (order.Status != OrderStatusCodes.Confirmed)
            {
                ErrorMessage = "You can only receive a confirmed order";
                return RedirectToAction("Orders");
            }

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();
            model.HideLabDetails = false;

            await GetHistories(id, model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirmation(int id, LabReceiveModel model)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Confirmed)
            {
                ErrorMessage = "You can only receive a confirmed order";
                return RedirectToAction("Orders");
            }

            if(string.IsNullOrWhiteSpace(order.RequestNum))
            {
                ErrorMessage = "You must add a request number first";
                return RedirectToAction("AddRequestNumber", new { id = id });
            }

            var orderDetails = order.GetOrderDetails();

            orderDetails.LabComments = model.LabComments;
            orderDetails.AdjustmentAmount = model.AdjustmentAmount;
            if (orderDetails.AdjustmentAmount != 0)
            {
                orderDetails.AdjustmentComments = model.AdjustmentComments;
            }
            else
            {
                orderDetails.AdjustmentComments = null;               
            }

            order.SaveDetails(orderDetails);

            order.Status = OrderStatusCodes.Received;

            await _orderService.SendOrderToAnlab(order);

            await _orderMessageService.EnqueueReceivedMessage(order, model.BypassEmail);

            var extraMessage = model.BypassEmail ? "Without Email" : "";
            var user = _dbContext.Users.Single(a => a.Id == CurrentUserId);
            order.History.Add(new History
            {
                    Action = "Received",
                    Status = order.Status,
                    ActorId = user.NormalizedUserName,
                    ActorName = user.Name,
                    JsonDetails = order.JsonDetails,
                    Notes = $"Request Number: {order.RequestNum} {extraMessage}",
            });

            await _dbContext.SaveChangesAsync();

            Message = "Order marked as received";
            return RedirectToAction("Orders");

        }

        [HttpPost]
        public async Task<IActionResult> GeneratePartialResultsEmail(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status == OrderStatusCodes.Created || order.Status == OrderStatusCodes.Confirmed)
            {
                ErrorMessage = "Don't generate an email in this status.";
                return RedirectToAction("Orders");
            }

            await _orderMessageService.EnqueuePartialResultsMessage(order);
            var user = _dbContext.Users.Single(a => a.Id == CurrentUserId);
            order.History.Add(new History
            {
                Action = "Partial Results",
                Status = order.Status,
                ActorId = user.NormalizedUserName,
                ActorName = user.Name,
                JsonDetails = order.JsonDetails,
            });

            await _dbContext.SaveChangesAsync();

            Message = "Email Generated";

            return RedirectToAction("MailQueue", "Admin", new {id = order.Id});
        }

        public async Task<IActionResult> Finalize(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Received)
            {
                ErrorMessage = "You can only Complete a Received order";
                return RedirectToAction("Orders");
            }

            var result = await _orderService.OverwriteOrderFromDb(order);
            if (result.WasError)
            {
                if (result.ErrorMessage != null)
                {
                    ErrorMessage = $"Error. Unable to continue. Error looking up on Labworks: {result.ErrorMessage}";
                }
                else
                {
                    ErrorMessage = string.Format("Error. Unable to continue. The following codes were not found locally: {0}", string.Join(",", result.MissingCodes));
                }
                return RedirectToAction("Orders");
            }

            order = await UpdateOrderFromLabworksResult(order, result);

            var model = new OrderReviewModel();
            model.Order = order;
            model.OrderDetails = order.GetOrderDetails();
            model.HideLabDetails = false;

            await GetHistories(id, model);

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Finalize(int id, LabFinalizeModel model)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != OrderStatusCodes.Received)
            {
                ErrorMessage = "You can only Complete a Received order";
                return RedirectToAction("Orders");
            }

            if (order.ResultsFileIdentifier == null && ( model.UploadFile == null || model.UploadFile.Length <= 0))
            {
                ErrorMessage = "You need to upload the results at this time.";
                return RedirectToAction("Finalize", new{id});
            }

            order.Status = OrderStatusCodes.Finalized;
            order.DateFinalized = DateTime.UtcNow;

            var result = await _orderService.OverwriteOrderFromDb(order);
            if (result.WasError)
            {
                if (result.ErrorMessage != null)
                {
                    ErrorMessage = $"Error. Unable to continue. Error looking up on Labworks: {result.ErrorMessage}";
                }
                else
                {
                    ErrorMessage = string.Format("Error. Unable to continue. The following codes were not found locally: {0}", string.Join(",", result.MissingCodes));
                }
                return RedirectToAction("Orders");
            }

            //File Upload
            if (model.UploadFile != null)
            {
                order.ResultsFileIdentifier = await _fileStorageService.UploadFile(model.UploadFile);
            }

            order = await UpdateOrderFromLabworksResult(order, result, model);
            
            var extraMessage = string.Empty;
            if (order.PaymentType == PaymentTypeCodes.UcDavisAccount)
            {
                var slothResult = await _slothService.MoveMoney(order);
                if (slothResult.Success)
                {
                    order.KfsTrackingNumber = slothResult.KfsTrackingNumber;
                    order.SlothTransactionId = slothResult.Id;
                    order.Paid = true;
                    extraMessage = " and UC Davis account marked as paid";
                }
                else
                {
                    ErrorMessage = $"There was a problem processing the payment for this account. {slothResult.Message}";
                    return RedirectToAction("Finalize");
                }
            }
            //Only send email if there wasn't a problem with sloth.
            await _orderMessageService.EnqueueFinalizedMessage(order, model.BypassEmail);

            var extraMailMessage = model.BypassEmail ? "Without Email" : "";
            var user = _dbContext.Users.Single(a => a.Id == CurrentUserId);
            order.History.Add(new History
            {
                    Action = "Finalized",
                    Status = order.Status,
                    ActorId = user.NormalizedUserName,
                    ActorName = user.Name,
                    JsonDetails = order.JsonDetails,
                    Notes = $"Order marked as Finalized{extraMessage} {extraMailMessage}",
            });


            await _dbContext.SaveChangesAsync();

            Message = $"Order marked as Finalized{extraMessage}";
            return RedirectToAction("Orders");

        }

        [Authorize(Roles = RoleCodes.Admin)]
        [HttpGet]
        public async Task<ActionResult> OverrideOrder(int id)
        {
            var order = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var model = new OverrideOrderModel
            {
                OrderReviewModel =
                {
                    Order = order,
                    OrderDetails = order.GetOrderDetails(),
                    HideLabDetails = false
                },
                IsDeleted = order.IsDeleted,
                Paid = order.Paid,
                Status = order.Status,
                Emails = order.AdditionalEmails
            };
            if (order.PaymentType == PaymentTypeCodes.UcDavisAccount ||
                order.PaymentType == PaymentTypeCodes.UcOtherAccount)
            {
                model.Account = model.OrderReviewModel.OrderDetails.Payment.Account;
            }

            await GetHistories(id, model.OrderReviewModel);

            return View(model);
        }

        [Authorize(Roles = RoleCodes.Admin)]
        [HttpPost]
        public async Task<ActionResult> OverrideOrder(int id, OverrideOrderModel model)
        {
            var historyNote = string.Empty;

            var orderToUpdate = await _dbContext.Orders.Include(i => i.Creator).SingleOrDefaultAsync(o => o.Id == id);

            if (orderToUpdate == null)
            {
                return NotFound();
            }

            if (orderToUpdate.Status != model.Status)
            {
                if (!OrderStatusCodes.All.Contains(model.Status))
                {
                    ErrorMessage = $"Unexpected Status Value: {model.Status}";
                    return RedirectToAction("OverrideOrder", new {id});
                }
            }

            if (orderToUpdate.PaymentType == PaymentTypeCodes.UcDavisAccount ||
                orderToUpdate.PaymentType == PaymentTypeCodes.UcOtherAccount)
            {
                var orderDetails = orderToUpdate.GetOrderDetails();
                model.Account = model.Account.SafeToUpper();
                if (orderDetails.Payment.Account != model.Account)
                {
                    model.OrderReviewModel = new OrderReviewModel
                    {
                        Order = orderToUpdate,
                        OrderDetails = orderDetails,
                        HideLabDetails = false
                    };
                    if (string.IsNullOrWhiteSpace(model.Account))
                    {                        
                        model.Account = model.OrderReviewModel.OrderDetails.Payment.Account;
                        ModelState.AddModelError("Account", "Account is required");                        
                    }

                    if (orderToUpdate.PaymentType == PaymentTypeCodes.UcDavisAccount)
                    {
                        try
                        {
                            orderDetails.Payment.AccountName = await _financialService.GetAccountName(model.Account);
                        }
                        catch
                        {
                            orderDetails.Payment.AccountName = string.Empty;
                        }

                        //order.SaveDetails(orderDetails);
                        if (string.IsNullOrWhiteSpace(orderDetails.Payment.AccountName))
                        {
                            ModelState.AddModelError("Account","Unable to verify UC Account number.");
                        }
                    }
                    if (!ModelState.IsValid)
                    {
                        await GetHistories(id, model.OrderReviewModel);
                        ErrorMessage = "Errors Detected. Override failed";
                        return View(model);
                    }
                    historyNote = $"Account Changed Old: {orderDetails.Payment.Account} New: {model.Account}.";

                    orderDetails.Payment.Account = model.Account;
                    orderToUpdate.SaveDetails(orderDetails);
                }
            }

            //TODO: Parse emails to validate
            if (orderToUpdate.AdditionalEmails != model.Emails)
            {
                if (string.IsNullOrWhiteSpace(model.Emails))
                {
                    orderToUpdate.AdditionalEmails = string.Empty;
                }
                else
                {
                    var filteredEmailList = new List<string>();
                    var emailList = model.Emails.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var em in emailList)
                    {
                        var em1 = em.ToLower().Trim();
                        if (em1.IsEmailValid())
                        {
                            if (!filteredEmailList.Contains(em1))
                            {
                                filteredEmailList.Add(em1);
                            }
                        }
                        else
                        {
                            ErrorMessage = $"Invalid email detected: {em1}";
                            return RedirectToAction("OverrideOrder", new { id });
                        }

                    }

                    historyNote = $"Additional emails changed. Old: {orderToUpdate.AdditionalEmails} New: {model.Emails}. {historyNote}";

                    orderToUpdate.AdditionalEmails = string.Join(';', filteredEmailList);
                }
            }

            if (orderToUpdate.Paid != model.Paid)
            {
                historyNote = $"Paid value changed. New: {model.Paid.ToYesNoString()}. {historyNote}";
            }
            if (orderToUpdate.Status != model.Status)
            {
                historyNote = $"Status value changed. Old: {orderToUpdate.Status}. {historyNote}";
            }
            if (model.IsDeleted)
            {
                historyNote = $"Marked as Deleted. {historyNote}";
            }

            if (model.UploadFile != null && model.UploadFile.Length >= 0)
            {
                historyNote = $"New File Uploaded. {historyNote}";
            }

            orderToUpdate.Paid = model.Paid;
            orderToUpdate.Status = model.Status;
            orderToUpdate.IsDeleted = model.IsDeleted;
            if (model.UploadFile != null && model.UploadFile.Length >= 0)
            {
                Log.Information($"Old Results File Identifier {orderToUpdate.ResultsFileIdentifier}");
                orderToUpdate.ResultsFileIdentifier = await _fileStorageService.UploadFile(model.UploadFile);
                Log.Information($"New Results File Identifier {orderToUpdate.ResultsFileIdentifier}");
            }

            var user = _dbContext.Users.Single(a => a.Id == CurrentUserId);
            orderToUpdate.History.Add(new History
            {
                    Action = "Admin Override",
                    Status = orderToUpdate.Status,
                    ActorId = user.NormalizedUserName,
                    ActorName = user.Name,
                    JsonDetails = orderToUpdate.JsonDetails,
                    Notes = historyNote,
            });

            await _dbContext.SaveChangesAsync();
            
            if (model.IsDeleted)
            {
                ErrorMessage = "Order deleted!!!";
                return RedirectToAction("Orders");
            }
            Message = "Order Updated";
            if (model.Status == OrderStatusCodes.Created)
            {
                return RedirectToAction("Confirmation", "Order", new {id});
            }

            return RedirectToAction("Details", new{id});
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Search(string term)
        {
            Order order = null;
            Guid guidSearch = Guid.Empty;
            int intSearch = 0;

            if (Guid.TryParse(term, out guidSearch))
            {
                order = await _dbContext.Orders.SingleOrDefaultAsync(a => a.ShareIdentifier == guidSearch);
            }
            else if(int.TryParse(term, out intSearch))
            {
                order = await _dbContext.Orders.SingleOrDefaultAsync(a => a.Id == intSearch);
            }
            else
            {
                order = await _dbContext.Orders.FirstOrDefaultAsync(a => a.RequestNum == term.SafeToUpper());
            }

            if (order == null)
            {
                ErrorMessage = "Order Not Found";
                return RedirectToAction("Search");
            }

            if (order.Status == OrderStatusCodes.Created)
            {
                if (User.IsInRole(RoleCodes.Admin))
                {
                    return RedirectToAction("Confirmation", "Order", new {id = order.Id});
                }
                else
                {
                    ErrorMessage = "Order is in the Created Status. Need Admin rights to access";
                    return RedirectToAction("Search");
                }
            }

            return RedirectToAction("Details", new {id = order.Id});

        }

        [Authorize(Roles = RoleCodes.Admin)]
        public async Task<ActionResult> JsonDetails(int id)
        {
            var order = await _dbContext.Orders.SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return new JsonResult(order.GetOrderDetails());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Ping()
        {
            var localDbCheck = _dbContext.TestItems.FirstOrDefault(a => a.Public);
            if (localDbCheck == null)
            {
                throw new Exception("Unable to get local db record");
            }

            var code = await _labworksService.TestDbConnection();
            if (code == null)
            {
                throw new Exception("Unable to connect to Labworks DB");
            }

            return Content("pong");
        }

    }
}

using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Models.Roles;
using AnlabMvc.Models.SystemAlertModels;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AnlabMvc.Controllers
{
    [Authorize(Roles = RoleCodes.Admin)]
    public class AlertsController : ApplicationController
    {
        private readonly ApplicationDbContext _dbContext;

        public AlertsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            var alerts = await _dbContext.SystemAlerts.OrderBy(a => a.IsActive).ThenBy(a => a.Updated).ToListAsync();
            return View(alerts);
        }

        // GET: AdminAnalysis/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(SystemAlertEditModel systemAlert)
        {
            ModelState.Clear();
            TryValidateModel(systemAlert);

            if (ModelState.IsValid)
            {
                var alertToCreate = new SystemAlert
                {
                    IsActive    = false,
                    Markdown    = systemAlert.Markdown,
                    Danger      = systemAlert.Danger,
                    Created     = DateTime.UtcNow,
                    Updated     = DateTime.UtcNow,
                    Description = systemAlert.Description,
                };
                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Build();
                alertToCreate.Content = Markdown.ToHtml(alertToCreate.Markdown, pipeline);
                _dbContext.Add(alertToCreate);
                await _dbContext.SaveChangesAsync();
                Message = "Alert Created. Activate it if it looks good.";
                return RedirectToAction("Details", new { id = alertToCreate.Id });
            }
            ErrorMessage = "There was a problem creating the alert.";
            return View(systemAlert);
        }

        public async Task<ActionResult> Details(int id)
        {
            var alert = await _dbContext.SystemAlerts.SingleAsync(x => x.Id == id);
            
            return View(alert);
        }
        
        [HttpPost]
        public async Task<ActionResult> ToggleActive(int id)
        {
            var alert = await _dbContext.SystemAlerts.SingleAsync(x => x.Id == id);
            alert.IsActive = !alert.IsActive;

            await _dbContext.SaveChangesAsync();

            Message = $"Alert has been {(alert.IsActive ? "activated" : "deactivated")}.";

            return RedirectToAction("Details", new { id = alert.Id });
        }

        public async Task<ActionResult> Edit(int id)
        {
            var alert = await _dbContext.SystemAlerts.Select(a => new SystemAlertEditModel { Id = a.Id, Description = a.Description, Markdown = a.Markdown, Danger = a.Danger}).SingleAsync(x => x.Id == id);
            return View(alert);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(SystemAlertEditModel systemAlert)
        {
            ModelState.Clear();
            TryValidateModel(systemAlert);
            if (ModelState.IsValid)
            {
                var alertToUpdate = await _dbContext.SystemAlerts.SingleAsync(x => x.Id == systemAlert.Id);
                alertToUpdate.IsActive    = false; //Force to false when editing.
                alertToUpdate.Markdown    = systemAlert.Markdown;
                alertToUpdate.Danger      = systemAlert.Danger;
                alertToUpdate.Updated     = DateTime.UtcNow;
                alertToUpdate.Description = systemAlert.Description;

                var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Build();
                alertToUpdate.Content = Markdown.ToHtml(alertToUpdate.Markdown, pipeline);

                await _dbContext.SaveChangesAsync();
                Message = "Alert Updated. Activate it if it looks good.";
                return RedirectToAction("Details", new { id = alertToUpdate.Id });
            }
            ErrorMessage = "There was a problem updating the alert.";
            return View(systemAlert);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var alert = await _dbContext.SystemAlerts.SingleAsync(x => x.Id == id);
            return View(alert);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(SystemAlert systemAlert)
        {
            var alertToDelete = await _dbContext.SystemAlerts.SingleAsync(x => x.Id == systemAlert.Id);
            _dbContext.Remove(alertToDelete);
            await _dbContext.SaveChangesAsync();
            Message = "Alert Deleted.";
            return RedirectToAction("Index");
        }
    }
}

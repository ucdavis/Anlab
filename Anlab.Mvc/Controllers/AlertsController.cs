using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Models.Roles;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult Index()
        {
            return View();
        }

        // GET: AdminAnalysis/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminAnalysis/Create
        [HttpPost]
        public async Task<ActionResult> Create(SystemAlert systemAlert)
        {
            ModelState.Clear();
            TryValidateModel(systemAlert);

            if (ModelState.IsValid)
            {
                var alertToCreate = new SystemAlert
                {
                    IsActive    = systemAlert.IsActive,
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
            _dbContext.Update(alert);
            await _dbContext.SaveChangesAsync();

            Message = $"Alert has been {(alert.IsActive ? "activated" : "deactivated")}.";

            return RedirectToAction("Details", new { id = alert.Id });
        }

        public async Task<ActionResult> Edit(int id)
        {
            var alert = await _dbContext.SystemAlerts.SingleAsync(x => x.Id == id);
            return View(alert);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(SystemAlert systemAlert)
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
                _dbContext.Update(alertToUpdate);
                await _dbContext.SaveChangesAsync();
                Message = "Alert Updated. Activate it if it looks good.";
                return RedirectToAction("Details", new { id = alertToUpdate.Id });
            }
            ErrorMessage = "There was a problem updating the alert.";
            return View(systemAlert);
        }

        //TODO: Call above with form on details page.
        //Add an edit page
        //Add an index page with a toggle to activate/deactivate...
        //Update SQL project
    }
}

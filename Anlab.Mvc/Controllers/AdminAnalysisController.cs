using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anlab.Core.Data;
using Anlab.Core.Domain;
using AnlabMvc.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnlabMvc.Controllers
{
    [Authorize(Roles = RoleCodes.Admin)]
    public class AdminAnalysisController : ApplicationController
    {
        private readonly ApplicationDbContext _dbContext;

        public AdminAnalysisController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: AdminAnalysis
        public async Task<ActionResult> Index()
        {
            return View(await _dbContext.AnalysisMethods.ToListAsync());
        }


        // GET: AdminAnalysis/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminAnalysis/Create
        [HttpPost]
        public async Task<ActionResult> Create(AnalysisMethod analysisMethod)
        {
            ModelState.Clear();
            TryValidateModel(analysisMethod);

            if (ModelState.IsValid)
            {
                if (await _dbContext.AnalysisMethods.AnyAsync(a => a.Id == analysisMethod.Id))
                {
                    ErrorMessage = "Id already in use";
                    return View(analysisMethod);
                }
                _dbContext.Add(analysisMethod);
                await _dbContext.SaveChangesAsync();
                Message = "Test created";
                return RedirectToAction("Index");
            }
            return View(analysisMethod);
        }

        // GET: AdminAnalysis/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var model = await _dbContext.AnalysisMethods.SingleOrDefaultAsync(a => a.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: AdminAnalysis/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(int id, AnalysisMethod analysisMethod)
        {
            if (id != analysisMethod.Id)
            {
                return NotFound();
            }
            ModelState.Clear();
            TryValidateModel(analysisMethod);

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(analysisMethod);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _dbContext.AnalysisMethods.AnyAsync(a => a.Id == analysisMethod.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                Message = "Edit saved";
                return RedirectToAction("Index");
            }
            return View(analysisMethod);
        }

        // GET: AdminAnalysis/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminAnalysis/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
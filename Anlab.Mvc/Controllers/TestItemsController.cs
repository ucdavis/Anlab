using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Anlab.Core.Domain;
using Anlab.Core.Data;
using Microsoft.AspNetCore.Authorization;
using AnlabMvc.Models.Roles;


namespace AnlabMvc.Controllers
{
    [Authorize(Roles = RoleCodes.Admin)]
    public class TestItemsController : ApplicationController
    {
        private readonly ApplicationDbContext _context;

        public TestItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TestItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.TestItems.ToListAsync());
        }

        // GET: TestItems/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testItem = await _context.TestItems
                .SingleOrDefaultAsync(m => m.Id == id);
            if (testItem == null)
            {
                return NotFound();
            }

            return View(testItem);
        }

        // GET: TestItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TestItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Analysis,Category,Group,Public,Notes")] TestItem testItem)
        {

            if (ModelState.IsValid)
            {
                testItem.Id = testItem.Id.ToUpper();
                if (TestItemExists(testItem.Id))
                {
                    ErrorMessage = "Code already in use";
                    return View();
                }
                _context.Add(testItem);
                await _context.SaveChangesAsync();
                Message = "Test created";
                return RedirectToAction("Index");
            }
            return View(testItem);
        }

        // GET: TestItems/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testItem = await _context.TestItems.SingleOrDefaultAsync(m => m.Id == id);
            if (testItem == null)
            {
                return NotFound();
            }
            return View(testItem);
        }

        // POST: TestItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Analysis,Category,Group,Public,Notes")] TestItem testItem)
        {
            if (id != testItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(testItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestItemExists(testItem.Id))
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
            return View(testItem);
        }

        // GET: TestItems/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var testItem = await _context.TestItems
                .SingleOrDefaultAsync(m => m.Id == id);
            if (testItem == null)
            {
                return NotFound();
            }

            return View(testItem);
        }

        // POST: TestItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var testItem = await _context.TestItems.SingleOrDefaultAsync(m => m.Id == id);
            _context.TestItems.Remove(testItem);
            await _context.SaveChangesAsync();
            Message = "Test deleted";
            return RedirectToAction("Index");
        }

        private bool TestItemExists(string id)
        {
            return _context.TestItems.Any(e => e.Id == id);
        }
    }
}

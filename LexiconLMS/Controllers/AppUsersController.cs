using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LexiconLMS.Data;
using LexiconLMS.Models.Entities;

namespace LexiconLMS.Controllers
{
    public class AppUsersController : Controller
    {
        private readonly ApplicationDbContext db;

        public AppUsersController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET: AppUsers
        public async Task<IActionResult> Index()
        {
            var model = await db.AppUser.Include(a => a.Course).ToListAsync();

            return View(model);
        }

        // GET: AppUsers/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var model = await db.AppUser
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: AppUsers/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(db.Set<Course>(), "Id", "Id");

            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                db.Add(appUser);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseId"] = new SelectList(db.Set<Course>(), "Id", "Id", appUser.CourseId);

            return View(appUser);
        }

        // GET: AppUsers/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await db.AppUser.FindAsync(id);

            if (appUser == null)
            {
                return NotFound();
            }

            ViewData["CourseId"] = new SelectList(db.Set<Course>(), "Id", "Id", appUser.CourseId);

            return View(appUser);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, AppUser appUser)
        {
            if (id != appUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(appUser);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseId"] = new SelectList(db.Set<Course>(), "Id", "Id", appUser.CourseId);

            return View(appUser);
        }

        // GET: AppUsers/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await db.AppUser
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var appUser = await db.AppUser.FindAsync(id);
            db.AppUser.Remove(appUser);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppUserExists(string id)
        {
            return db.AppUser.Any(e => e.Id == id);
        }
    }
}

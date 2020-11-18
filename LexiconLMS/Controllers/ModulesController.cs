using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LexiconLMS.Data;
using LexiconLMS.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using LexiconLMS.Models.ViewModels;
using AutoMapper;

namespace LexiconLMS.Controllers
{
    public class ModulesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public ModulesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        // GET: Modules
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Modules.Include(c => c.Course);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Modules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await _context.Modules
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (module == null)
            {
                return NotFound();
            }

            return View(module);
        }

        // GET: Modules/Create
        [Authorize(Roles = "Teacher")]
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id");
            return View();
        }

        // POST: Modules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(ModuleActivityCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                //var module = mapper.Map<Module>(viewModel);

                var module = new Module
                {
                    CourseId = viewModel.CourseId,
                    Name = viewModel.ModuleName,
                    Description = viewModel.ModuleDescription,
                    StartTime = viewModel.ModuleStartTime,
                    EndTime = viewModel.ModuleEndTime
                };

                _context.Add(module);

                foreach (var item in viewModel.Activities)
                {
                    var activity = new Activity
                    {
                        Name = item.ActivityName,
                        Description = item.ActivityDescription,
                        StartTime = item.ActivityStartTime,
                        EndTime = item.ActivityEndTime,
                        ActivityTypeId = item.ActivityTypeId,
                        Module = module
                    };

                    _context.Add(activity);
                }

                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // TODO: change so it points to course dashboard
            }
            //ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", module.CourseId); // What does this show? Which course it belongs to?
            return View(viewModel);
        }

        // GET: Modules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await _context.Modules.FindAsync(id);
            if (module == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", module.CourseId);
            return View(module);
        }

        // POST: Modules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartTime,EndTime,CourseId")] Module module)
        {
            if (id != module.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(module);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModuleExists(module.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", module.CourseId);
            return View(module);
        }

        // GET: Modules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var module = await _context.Modules
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (module == null)
            {
                return NotFound();
            }

            return View(module);
        }

        // POST: Modules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var module = await _context.Modules.FindAsync(id);
            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModuleExists(int id)
        {
            return _context.Modules.Any(e => e.Id == id);
        }


        //public void AddActivity(ModuleActivityCreateViewModel viewModel)
        //{
        //    var activity = new ActivityListViewModel
        //    {
        //        ActivityName = viewModel.ActivityName,
        //        ActivityDescription = viewModel.ActivityDescription,
        //        ActivityStartTime = viewModel.ActivityStartTime,
        //        ActivityEndTime = viewModel.ActivityEndTime,
        //        ActivityTypeId = viewModel.ActivityTypeId
        //    };
        //    var model = new ModuleActivityCreateViewModel();
        //    model.Activities.Add(activity);
        //    ModelState.Clear();
        //    //return activity;
        //    return View("Create",model);
        //    //return RedirectToAction(nameof(Create));
        //}

    }
}

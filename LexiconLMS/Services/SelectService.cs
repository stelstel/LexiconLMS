using LexiconLMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Services
{
    public class SelectService : ISelectService
    {

        private ApplicationDbContext db;


        public SelectService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<SelectListItem>> SelectCourses()
        {
            return await db.Courses.Select(n =>
            new SelectListItem()
            {
                Text = n.Name,
                Value = n.Id.ToString()
            }).ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> SelectModules(int? id)
        {
            return await db.Modules
                .Where(n => n.CourseId == id)
                .Select(n =>
            new SelectListItem()
            {
                Text = n.Name,
                Value = n.Id.ToString()
            })
            .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> SelectActivities(int? id)
        {
            return await db.Activities
                .Where(n => n.ModuleId == id)
                .Select(n =>
            new SelectListItem()
            {
                Text = n.Name,
                Value = n.Id.ToString()
            })
            .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> SelectActivityTypes()
        {
            return await db.ActivityTypes.Select(n =>
            new SelectListItem()
            {
                Text = n.Name,
                Value = n.Id.ToString()
            }).ToListAsync();
        }

        // Select 
        public async Task<IEnumerable<SelectListItem>> SelectCourseSetEmptyDefault()
        {
            var selectList = await db.Courses
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
                .ToListAsync();

            selectList.Insert(0, new SelectListItem { Text = "---", Value = string.Empty, Selected = true });
            return selectList;
        }

        public async Task<IEnumerable<SelectListItem>> SelectCourseSetSelected(int? selected)
        {
            var selectList = await SelectCourseSetEmptyDefault();
            if (selected == null)
            {
                return selectList;
            }
            var theSelected = selectList.Where(s => s.Value == selected.ToString()).FirstOrDefault();
            theSelected.Selected = true;
            return selectList;
        }
    }
}

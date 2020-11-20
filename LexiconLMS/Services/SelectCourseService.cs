using LexiconLMS.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Services
{
    public class SelectCourseService : ISelectCourseService
    {
        private readonly ApplicationDbContext db;
        public SelectCourseService(ApplicationDbContext db)
        {
            this.db = db;
        }
        public async Task<IEnumerable<SelectListItem>> GetCoursesAsync()
        {
            var selectList =  await db.Courses
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
                .ToListAsync();

            selectList.Insert(0, new SelectListItem { Text = "---", Value = string.Empty, Selected = true});
            return selectList;
        }

        public async Task<IEnumerable<SelectListItem>> GetCoursesAsync(int? selected)
        {
            var selectList = await GetCoursesAsync();
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

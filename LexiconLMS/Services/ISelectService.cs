using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LexiconLMS.Services
{
    public interface ISelectService
    {
        Task<IEnumerable<SelectListItem>> SelectCourses();
        Task<IEnumerable<SelectListItem>> SelectActivityTypes();
        Task<IEnumerable<SelectListItem>> SelectCourseSetEmptyDefault();
        Task<IEnumerable<SelectListItem>> SelectCourseSetSelected(int? selected);
        Task<IEnumerable<SelectListItem>> SelectModules(int? id);
        Task<IEnumerable<SelectListItem>> SelectActivities(int? id);
    }
}

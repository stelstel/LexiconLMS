using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LexiconLMS.Services
{
    public interface ISelectService
    {
        Task<IEnumerable<SelectListItem>> SelectCourses();
        Task<IEnumerable<SelectListItem>> SelectActivityTypes();
        Task<IEnumerable<SelectListItem>> GetCourse(int? id);
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LexiconLMS.Services
{
    public interface ISelectCourseService
    {
        Task<IEnumerable<SelectListItem>> GetCoursesAsync();
    }
}
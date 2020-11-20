using System;
using System.Collections.Generic;
using LexiconLMS.Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Teacher
{
    public class TeacherHomeViewModel
    {
        public IEnumerable<Course> Courses { get; set; }
    }
}

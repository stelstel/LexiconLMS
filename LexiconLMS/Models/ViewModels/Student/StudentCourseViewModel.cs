using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Student
{
    public class StudentCourseViewModel
    {
        public IEnumerable<AssignmentListViewModel> AssignmentList { get; set; }
    }
}

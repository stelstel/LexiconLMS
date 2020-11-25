using LexiconLMS.Models.Entities;
using LexiconLMS.Models.ViewModels.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Teacher
{
    public class TeacherViewModel
    {
        public Course Course { get; set; }
        public TeacherCurrentViewModel TeacherCurrentViewModel { get; set; }
        public IEnumerable<TeacherAssignmentListViewModel> AssignmentList { get; set; }
        public IEnumerable<TeacherModuleViewModel> ModuleList { get; set; }
        public IEnumerable<ActivityListViewModel> ActivityList { get; set; }
    }
}

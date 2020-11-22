using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Student
{
    public class StudentViewModel
    {
        public IEnumerable<AssignmentListViewModel> AssignmentList { get; set; }
        public IEnumerable<ModuleListViewModel> ModuleList { get; set; }
        public IEnumerable<ActivityListViewModel> ActivityList { get; set; }
        public AppUser AppUser { get; set; }
        public CurrentViewModel CurrentViewModel { get; set; }
    }
}

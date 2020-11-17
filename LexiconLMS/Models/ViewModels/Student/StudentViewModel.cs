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
        public AppUser AppUser { get; set; }
    }
}

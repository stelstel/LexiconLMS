using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Teacher
{
    public class TeacherCurrentViewModel
    {
        public Activity Activity { get; set; }
        public Course Course { get; set; }
        public ICollection<TeacherAssignmentsViewModel> Assignments { get; set; }
        public Module Module { get; set; }
        public string Finished { get; set; }
        public DateTime DueDate { get; set; }
    }
}

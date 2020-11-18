using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Student
{
    public class CurrentViewModel
    {
        public Activity Activity { get; set; }
        public string CourseName { get; set; }
        public ICollection<Activity> Assignments { get; set; }
        public Module Module { get; set; }

    }
}

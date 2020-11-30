using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Student
{
    public class CurrentAssignmentsViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [DisplayName("Is Finished")]
        public bool? IsFinished { get; set; }
        [DisplayName("Due Time")]
        public DateTime DueTime { get; set; }
    }
}

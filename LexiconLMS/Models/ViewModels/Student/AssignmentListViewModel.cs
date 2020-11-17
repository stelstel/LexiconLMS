using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Student
{
    public class AssignmentListViewModel
    {
        public string Name { get; set; }

        [DisplayName("Start Time")]
        public DateTime StartTime { get; set; }

        [DisplayName("End Time")]
        public DateTime EndTime { get; set; }

        [DisplayName("Finished")]
        public bool IsFinished { get; set; }
    }
}

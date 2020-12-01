using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Student
{
    public class AssignmentListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [DisplayName("Start Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime StartTime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]

        [DisplayName("End Time")]
        public DateTime EndTime { get; set; }

        [DisplayName("Finished")]
        public bool? IsFinished { get; set; }
    }
}

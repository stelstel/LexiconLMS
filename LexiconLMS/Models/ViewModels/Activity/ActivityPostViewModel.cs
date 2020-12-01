using System;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Models.ViewModels
{
    public class ActivityPostViewModel
    {
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime ActivityStartTime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime ActivityEndTime { get; set; }
        public int ActivityTypeId { get; set; }
    }
}

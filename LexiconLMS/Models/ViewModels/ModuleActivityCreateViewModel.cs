using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Models.ViewModels
{
    public class ModuleActivityCreateViewModel
    {

        [DisplayName("Course (temp)")]
        public int CourseId { get; set; }
        [DisplayName("Name")]
        public string ModuleName { get; set; }
        [DisplayName("Description")]
        public string ModuleDescription { get; set; }
        [DisplayName("Start Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime ModuleStartTime { get; set; }
        [DisplayName("End Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime ModuleEndTime { get; set; }


        //public List<ActivityPostViewModel> Activities { get; set; }

        [DisplayName("Name")]
        public string ActivityName { get; set; }
        [DisplayName("Description")]
        public string ActivityDescription { get; set; }
        [DisplayName("Start Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime ActivityStartTime { get; set; }
        [DisplayName("End Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime ActivityEndTime { get; set; }
        [DisplayName("Type")]
        public int ActivityTypeId { get; set; }

        //public Module Module { get; set; }






    }
}

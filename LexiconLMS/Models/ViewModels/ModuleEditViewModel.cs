using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace LexiconLMS.Models.ViewModels
{
    public class ModuleEditViewModel
    {

        //public Module Module { get; set; }
        public int CourseId { get; set; }
        public int? ModuleId { get; set; }
        [DisplayName("Name")]
        public string ModuleName { get; set; }
        [DisplayName("Description")]
        
        public string ModuleDescription { get; set; }
        [DisplayName("Start Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
        public DateTime ModuleStartTime { get; set; }
        [DisplayName("End Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
        public DateTime ModuleEndTime { get; set; }



        public List<Entities.Activity> Activities { get; set; }


        [DisplayName("Name")]
        public string ActivityName { get; set; }
        [DisplayName("Description")]
        public string ActivityDescription { get; set; }
        [DisplayName("Start Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
        public DateTime ActivityStartTime { get; set; }
        [DisplayName("End Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-ddTHH:mm}")]
        public DateTime ActivityEndTime { get; set; }
        [DisplayName("Type")]
        public int ActivityTypeId { get; set; }
        public ActivityType ActivityType { get; set; }



    }
}

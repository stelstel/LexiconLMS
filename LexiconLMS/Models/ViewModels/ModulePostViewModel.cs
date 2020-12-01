using System;
using System.ComponentModel.DataAnnotations;


namespace LexiconLMS.Models.ViewModels
{
    public class ModulePostViewModel
    {
        public int CourseId { get; set; }
        public int? ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleDescription { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime ModuleStartTime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime ModuleEndTime { get; set; }
    }
}

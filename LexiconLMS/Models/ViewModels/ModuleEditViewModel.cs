using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels
{
    public class ModuleEditViewModel
    {

        //public Module Module { get; set; }
        public int? ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleDescription { get; set; }
        public DateTime ModuleStartTime { get; set; }
        public DateTime ModuleEndTime { get; set; }



        public List<Activity> Activities { get; set; }


        [DisplayName("Name")]
        public string ActivityName { get; set; }
        [DisplayName("Description")]
        public string ActivityDescription { get; set; }
        [DisplayName("Start Time")]
        public DateTime ActivityStartTime { get; set; }
        [DisplayName("End Time")]
        public DateTime ActivityEndTime { get; set; }
        [DisplayName("Type")]
        public int ActivityTypeId { get; set; }



    }
}

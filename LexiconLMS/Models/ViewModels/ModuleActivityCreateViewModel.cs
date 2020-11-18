using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels
{
    public class ModuleActivityCreateViewModel
    {
        // TODO: change names so it fits both Module and Activity, fix mismatch in controller

        public int CourseId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleDescription { get; set; }
        public DateTime ModuleStartTime { get; set; }
        public DateTime ModuleEndTime { get; set; }


        public List<ActivityListViewModel> Activities { get; set; }

        // Maybe make this into a collection of viewmodels that are passed from a partial view? 
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime ActivityStartTime { get; set; }
        public DateTime ActivityEndTime { get; set; }
        public int ActivityTypeId { get; set; }
        public Module Module { get; set; }






    }
}

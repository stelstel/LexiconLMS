using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels
{
    public class ActivityListViewModel
    {
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime ActivityStartTime { get; set; }
        public DateTime ActivityEndTime { get; set; }
        public int ActivityTypeId { get; set; }
        public Module Module { get; set; }
    }
}

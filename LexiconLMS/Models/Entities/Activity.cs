using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.Entities
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayName("Start Time")]
        public DateTime StartTime { get; set; }
        [DisplayName("Start End")]
        public DateTime EndTime { get; set; }
        [DisplayName("Is Finished")]
        public bool IsFinished { get; set; }

        // Foreign Keys
        public int ModuleId { get; set; }
        public int ActivityTypeId { get; set; }

        // Navigation Properties
        public Module Module { get; set; }
        [DisplayName("Activity Type")]
        public ActivityType ActivityType { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}

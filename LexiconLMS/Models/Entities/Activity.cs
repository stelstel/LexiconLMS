using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.Entities
{
    public class Activity
    {
        public int Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayName("Start Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime StartTime { get; set; }
        [DisplayName("End Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime EndTime { get; set; }

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

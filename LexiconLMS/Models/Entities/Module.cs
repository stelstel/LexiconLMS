using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.Entities
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DisplayName("Start Time")]
        public DateTime StartTime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DisplayName("End Time")]
        public DateTime EndTime { get; set; }
        //public Module CurrentModule { get; set; }

        // Foreign Key
        public int CourseId { get; set; }

        // Navigation Properties
        public Course Course { get; set; }
        public ICollection<Activity> Activities { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.Entities
{
    public class Document
    {
        // TODO: add actual documents

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime UploadTime { get; set; }
        public string FilePath { get; set; }

        // Foreign Keys
        public string AppUserId { get; set; }
        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? ActivityId { get; set; }

        // Navigation Properties
        public AppUser AppUser { get; set; }
        public Course Course { get; set; }
        public Module Module { get; set; }
        public Activity Activity { get; set; }
        
    }
}

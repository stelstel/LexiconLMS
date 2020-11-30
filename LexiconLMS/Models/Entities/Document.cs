using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LexiconLMS.Models.Entities
{
    public class Document
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DisplayName("Upload Time")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime UploadTime { get; set; }
        public bool? IsFinished { get; set; }

        // Foreign Keys
        public string AppUserId { get; set; }
        public int? CourseId { get; set; }
        public int? ModuleId { get; set; }
        public int? ActivityId { get; set; }

        // Navigation Properties
        [DisplayName("Uploader")]
        public AppUser AppUser { get; set; }
        public Course Course { get; set; }
        public Module Module { get; set; }
        public Activity Activity { get; set; }
    }
}

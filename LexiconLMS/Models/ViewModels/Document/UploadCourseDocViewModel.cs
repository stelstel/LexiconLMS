using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Document
{
    public class UploadCourseDocViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime UploadTime => DateTime.Now;

        public string AppUserId { get; set; }
        public int? CourseId { get; set; }

        public AppUser AppUser { get; set; }
        public Course Course { get; set; }
    }
}

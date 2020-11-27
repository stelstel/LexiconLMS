using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Document
{
    public class DownloadCourseDocumentViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime UploadTime { get; set; }
        public string AppUserId { get; set; }
        public int? CourseId { get; set; }
        public string DirectoryPath { get; set; }
        public ICollection<string> fileList { get; set; }

        public AppUser AppUser { get; set; }
        public Course Course { get; set; }
    }
}

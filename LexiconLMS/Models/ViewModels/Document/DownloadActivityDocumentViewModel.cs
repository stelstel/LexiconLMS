using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Document
{
    public class DownloadActivityDocumentViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime UploadTime { get; set; }
        public string AppUserId { get; set; }
        public int? CourseId { get; set; }
        // public string DirectoryPath { get; set; }
        public ICollection<Entities.Document> DocumentList { get; set; }
        // public string CurrentDirectory { get; set; }
        
        public AppUser AppUser { get; set; }
        public Course Course { get; set; }
    }
}

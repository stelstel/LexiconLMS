using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Document
{
    public class UploadActivityDocumentViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ModuleId { get; set; }
        public int ActivityId { get; set; }
        public DateTime UploadTime { get; set; }
        public Activity Activity { get; set; }
        public Course Course { get; set; }
        public Module Module { get; set; }
    }
}

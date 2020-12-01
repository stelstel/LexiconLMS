using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Student
{
    public class StudentUploadViewModel
    {
        public int Id { get; set; }
        public string Notes { get; set; }
        public bool IsFinished { get; set; }
        public DateTime UploadTime { get; set; }
        public int StudentId { get; set; }
        public int CourseId  { get; set; }
        public int ModuleId  { get; set; }
        public int ActivityId  { get; set; }
        public AppUser Student { get; set; }
        public Course Course { get; set; }
        public Module Module { get; set; }
        public Entities.Activity Activity { get; set; }
    }
}

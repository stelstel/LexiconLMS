using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }



        // Foreign Keys
        public string AppUserId { get; set; }
        public int ModuleId { get; set; }
        public int? DocumentId { get; set; }


        // Navigation Properties
        public ICollection<AppUser> AppUsers { get; set; }
        public ICollection<Module> Modules { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}

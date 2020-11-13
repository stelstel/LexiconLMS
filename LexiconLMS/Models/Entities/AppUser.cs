using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        // Foregin Keys
        public int? CourseId { get; set; }



        // Navigation Properties
        public Course Course { get; set; }

        public ICollection<Document> Documents { get; set; }

    }
}

using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels
{
    public class AppUserListViewModel
    {
        public string AppUserId { get; set; }
        [Display(Name = "Name")]
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool IsTeacher { get; set; }
        public Course Course { get; set; }

    }
}

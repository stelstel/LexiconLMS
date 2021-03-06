﻿using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Teacher
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        [Required, StringLength(30, ErrorMessage = "Do not enter more than 30 characters")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required, StringLength(30, ErrorMessage = "Do not enter more than 30 characters")]
        public string LastName { get; set; }

        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Display(Name = "New Password")]
        [DataType(DataType.Password), Compare(nameof(ConfirmPassword))]
        public string? Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password), Compare(nameof(ConfirmPassword))]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Course")]
        public int? CourseId { get; set; }

        public bool IsTeacher { get; set; }

    }
}

﻿using LexiconLMS.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Teacher
{
    public class CreateUserViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Display(Name = "First Name")]
        [Required, StringLength(30, ErrorMessage = "Do not enter more than 30 characters")]       
        public string FirstName { get; set; }
        
        [Display(Name = "Last Name")]
        [Required, StringLength(30, ErrorMessage = "Do not enter more than 30 characters")]       
        public string LastName { get; set; }

        [Required, DataType(DataType.Password), Compare(nameof(ConfirmPassword))]
        public string Password { get; set; }
        
        [Display(Name = "Confirm Password")]
        [Required, DataType(DataType.Password), Compare(nameof(ConfirmPassword))]       
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Teacher")]
        public bool IsTeacher { get; set; } = true;

        [Display(Name = "Course")]
        public int? CourseId { get; set; }

    }
}

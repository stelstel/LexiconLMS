using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Teacher
{
    public class TeacherViewModel
    {
        public Course Course { get; set; }
        public TeacherCurrentViewModel TeacherCurrentViewModel { get; set; }
    }
}

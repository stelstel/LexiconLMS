using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels.Teacher
{
    public class TeacherModuleViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [DisplayName("Start Time")]
        public DateTime StartTime { get; set; }

        [DisplayName("End Time")]
        public DateTime EndTime { get; set; }
    }
}

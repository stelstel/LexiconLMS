using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels
{
    public class ModulePostViewModel
    {
        public int CourseId { get; set; }
        public int? ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleDescription { get; set; }
        public DateTime ModuleStartTime { get; set; }
        public DateTime ModuleEndTime { get; set; }
    }
}

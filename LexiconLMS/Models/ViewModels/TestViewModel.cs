using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels
{
    public class TestViewModel
    {
        public IEnumerable<ActivityListViewModel> Data { get; set; }
        public ModulePostViewModel Module { get; set; }
    }
}

using LexiconLMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.ViewModels
{
    public class ModuleEditViewModel
    {

        public Module Module { get; set; }

        public List<Activity> Activities { get; set; }


    }
}

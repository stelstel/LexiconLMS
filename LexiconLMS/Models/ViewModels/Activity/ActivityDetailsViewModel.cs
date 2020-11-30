using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexiconLMS.Models.Entities;

namespace LexiconLMS.Models.ViewModels.Activity
{
    public class ActivityDetailsViewModel
    {
        public Entities.Activity Activity { get; set; }
        public List<AppUser> Students { get; set; }

    }
}

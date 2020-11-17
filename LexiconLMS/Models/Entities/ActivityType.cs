using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Models.Entities
{
    public class ActivityType
    {
        public int Id { get; set; }
        public string Name { get; set; }        

        // Navigation Properties
        public ICollection<Activity> Activities { get; set; }
    }
}

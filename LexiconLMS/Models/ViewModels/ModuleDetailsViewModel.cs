using LexiconLMS.Models.Entities;
using System.Collections.Generic;


namespace LexiconLMS.Models.ViewModels
{
    public class ModuleDetailsViewModel
    {

        public Module Module { get; set; }
        public List<Entities.Document> Documents { get; set; }
        public Entities.Activity Activity { get; set; }
        public AppUser AppUser { get; set; }
    }
}

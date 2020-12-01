using System;


namespace LexiconLMS.Models.ViewModels
{
    public class ActivityPostViewModel
    {
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime ActivityStartTime { get; set; }
        public DateTime ActivityEndTime { get; set; }
        public int ActivityTypeId { get; set; }
    }
}

using System.Collections.Generic;
using StudentPortal.Models;

namespace StudentPortal.ViewModels.Admin
{
    public class TimeTableDetailsViewModel
    {
        public TimeTable? TimeTable { get; set; }
        public List<string> Weekdays { get; set; } = new();
    }
}

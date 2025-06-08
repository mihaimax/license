using StudentPortal.Models;
using System.Collections.Generic;

namespace StudentPortal.ViewModels.Admin
{
    public class TimeTablesViewModel
    {
        public List<TimeTable> TimeTable { get; set; } = new();
        public List<string> Weekday { get; set; }
    }
}

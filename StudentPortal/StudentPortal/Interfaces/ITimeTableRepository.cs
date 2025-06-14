using StudentPortal.Models;
using StudentPortal.ViewModels.Admin;

namespace StudentPortal.Interfaces
{
    public interface ITimeTableRepository
    {
        Task<IEnumerable<TimeTable>> GetAllTimeTablesAsync();
        Task<TimeTable?> GetTimeTableByPKAsync(string departmentCode,
            int year,
            int semester,
            string subjectCode,
            string weekday,
            TimeOnly startTime,
            TimeOnly endTime);
        Task AddAsync(TimeTable timeTable);
        Task SaveChangesAsync();
        Task<List<TimeTableViewModel>> GetAllViewModelsAsync();
    }
}

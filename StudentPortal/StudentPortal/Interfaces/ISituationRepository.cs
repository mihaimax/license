using StudentPortal.ViewModels.Admin;
using StudentPortal.ViewModels.Student;

namespace StudentPortal.Interfaces
{
    public interface ISituationRepository
    {
        Task<List<SituationViewModel>> GetSituationForStudentAsync(string userName);
    }
}

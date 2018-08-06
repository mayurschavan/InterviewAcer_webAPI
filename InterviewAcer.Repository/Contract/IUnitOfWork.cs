using InterviewAcer.Repository.Implementation;
using System.Threading.Tasks;

namespace InterviewAcer.Repository.Contract
{
    public interface IUnitOfWork
    {
        Task<int> Save();
        InterviewRepository GetInterviewRepository();
        ForgotPasswordRepository GetForgotPasswordRepository();
        StageRepository GetStageRepository();
    }
}

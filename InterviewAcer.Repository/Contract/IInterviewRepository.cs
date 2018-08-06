using InterviewAcer.Common.DTO;
using InterviewAcer.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InterviewAcer.Repository.Contract
{
    interface IInterviewRepository
    {
        Task<List<InterviewDetailsDTO>> GetInterviewDetails(string userName);
        InterviewDetail SaveInterviewDetails(InterviewDetailsDTO interviewDetails, string userName);
    }
}

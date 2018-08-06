using InterviewAcer.Data;
using InterviewAcer.Repository.Contract;
using InterviewAcer.Repository.Implementation;
using System;
using System.Threading.Tasks;

namespace InterviewAcer.Repository.Implementation
{
    public class UnitOfWork: IUnitOfWork, IDisposable
    {
        private InterviewAcerDbContext _context = new InterviewAcerDbContext();
        private InterviewRepository _interviewRepoitory;
        private ForgotPasswordRepository _forgotPasswordRepository;
        private StageRepository _stageRepository;
    

        public InterviewRepository GetInterviewRepository()
        {
            if (this._interviewRepoitory == null)
            {
                this._interviewRepoitory = new InterviewRepository(_context);
            }
            return _interviewRepoitory;
        }

        public ForgotPasswordRepository GetForgotPasswordRepository()
        {
            if (this._forgotPasswordRepository == null)
            {
                this._forgotPasswordRepository = new ForgotPasswordRepository(_context);
            }
            return _forgotPasswordRepository;
        }

        public StageRepository GetStageRepository()
        {
            if(this._stageRepository == null)
            {
                this._stageRepository = new StageRepository(_context);
            }
            return _stageRepository;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

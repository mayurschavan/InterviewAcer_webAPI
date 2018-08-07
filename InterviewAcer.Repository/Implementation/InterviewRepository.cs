using InterviewAcer.Common.DTO;
using InterviewAcer.Data;
using InterviewAcer.Repository.Contract;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace InterviewAcer.Repository.Implementation
{
    public class InterviewRepository : IInterviewRepository
    {
        private InterviewAcerDbContext _dbContext;
        public InterviewRepository(InterviewAcerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<InterviewDetailsDTO>> GetInterviewDetails(string userName)
        {
            List<InterviewDetailsDTO> interviewDetailsList = new List<InterviewDetailsDTO>();
            var interviewDetails = await _dbContext.InterviewDetails.Where(x => x.UserName == userName).ToListAsync();
            foreach (var item in interviewDetails)
            {
                var interviewDetailItem = new InterviewDetailsDTO();
                interviewDetailItem.CompanyName = item.CompanyName;
                interviewDetailItem.Designation = item.Designation;
                interviewDetailItem.HiringIndividualName = item.HiringIndividualName;
                interviewDetailItem.InterviewDate = item.InterviewDate;
                interviewDetailItem.InterviewTypeId = item.InterviewTypeId;
                interviewDetailItem.Tag = item.ColorCode;
                interviewDetailItem.InterviewId = item.InterviewDetailId;
                var stages = _dbContext.Stages.Where(x => x.InterviewTypeId == interviewDetailItem.InterviewTypeId);
                if (stages != null && stages.Any())
                {
                    interviewDetailItem.TotalNumberOfStages = stages.Count();
                    interviewDetailItem.CompletedNumberOfStages = _dbContext.InterviewCompletedStageMappings.Count(x => x.InterviewId == interviewDetailItem.InterviewId);
                    //if (completedStages != null)
                    //    interviewDetailItem.CompletedNumberOfStages = completedStages.Count();
                }
                else
                    interviewDetailItem.TotalNumberOfStages = 0;

                GetInterviewStage_Result stageDetails = _dbContext.GetInterviewStage(item.InterviewDetailId).FirstOrDefault();
                if (stageDetails != null)
                {
                    interviewDetailItem.CurrentStageId = stageDetails.Id;
                    interviewDetailItem.CurrentStageName = stageDetails.StageName;
                }
                interviewDetailsList.Add(interviewDetailItem);
            }
            return interviewDetailsList;
        }

        public InterviewDetail SaveInterviewDetails(InterviewDetailsDTO interviewDetails, string userName)
        {
            var interviewDetailEntity = new InterviewDetail();
            interviewDetailEntity.CompanyName = interviewDetails.CompanyName;
            interviewDetailEntity.Designation = interviewDetails.Designation;
            interviewDetailEntity.HiringIndividualName = interviewDetails.HiringIndividualName;
            interviewDetailEntity.InterviewDate = interviewDetails.InterviewDate;
            interviewDetailEntity.InterviewTypeId = interviewDetails.InterviewTypeId;
            interviewDetailEntity.UserName = userName;
            interviewDetailEntity.ColorCode = interviewDetails.Tag;
            return _dbContext.InterviewDetails.Add(interviewDetailEntity);
        }

        public IQueryable<int> GetCompletedCheckList(int interviewId)
        {
            //List<int> checkListIdList = new List<int>();
            //checkListIdList = await _dbContext.InterviewCheckListMappings.Where(x => x.InterviewId == interviewId).Select(x => x.CheckListId).ToListAsync(); 
            return _dbContext.InterviewCheckListMappings.Where(x => x.InterviewId == interviewId).Select(x => x.CheckListId);
            //return checkListIdList;
        }

        public IQueryable<int> GetCompletedStages(int interviewId)
        {
            //List<int> checkListIdList = new List<int>();
            //checkListIdList = await _dbContext.InterviewCheckListMappings.Where(x => x.InterviewId == interviewId).Select(x => x.CheckListId).ToListAsync(); 
            return _dbContext.InterviewCompletedStageMappings.Where(x => x.InterviewId == interviewId).Select(x => x.StageId);
            //return checkListIdList;
        }

        public bool updateCheckList(int interviewId, int checkListId, bool isChecked)
        {
            bool checkListUpdate = false;
            var checkListItem = _dbContext.GroupCheckLists.FirstOrDefault(x => x.Id == checkListId);
            var interviewItem = _dbContext.InterviewDetails.First(x => x.InterviewDetailId == interviewId);
            if (checkListItem != null && interviewItem != null)
            {
                _dbContext.usp_UpdateCheckList(checkListId, interviewId);
                checkListUpdate = true;
            }
            return checkListUpdate;
        }

        public InterviewCurrentStatusDTO GetInterviewCurrentStatus(int interviewId)
        {
            InterviewCurrentStatusDTO interviewStatus = new InterviewCurrentStatusDTO();
            var status = _dbContext.GetInterviewStage(interviewId).First();
            interviewStatus.CurrentStatusId = status.Id;
            interviewStatus.CurrentStatusName = status.StageName;
            return interviewStatus;
        }

        public void MarkStageAsComplete(int stageId, int interviewId)
        {
            var isCompleted = _dbContext.InterviewCompletedStageMappings.FirstOrDefault(x => x.InterviewId == interviewId && x.StageId == stageId);
            if(isCompleted ==  null)
            {
                _dbContext.InterviewCompletedStageMappings.Add(new InterviewCompletedStageMapping() { InterviewId = interviewId, StageId = stageId });
            }
        }

        public void SaveFeedback(int stageId, int interviewId,string feedback)
        {
          InterviewCompletedStageMapping interviewFeedback= _dbContext.InterviewCompletedStageMappings.FirstOrDefault(x => x.StageId==stageId && x.InterviewId == interviewId);

            if(interviewFeedback != null)
            interviewFeedback.FeedBack = feedback;

        }
    }
}

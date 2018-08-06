using InterviewAcer.Common.DTO;
using InterviewAcer.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewAcer.Repository.Implementation
{

    public class StageRepository
    {
        private InterviewAcerDbContext _dbContext;
        public StageRepository(InterviewAcerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<StageDTO> GetStages(int interviewTypeId)
        {
            List<StageDTO> stagesList = new List<StageDTO>();
            var stages = _dbContext.Stages.Where(x => x.InterviewTypeId == interviewTypeId);
            foreach (var stageItem in stages)
            {
                StageDTO stage = new StageDTO();
                stage.StageId = stageItem.Id;
                stage.Name = stageItem.StageName;
                stage.Sequence = stageItem.Sequence;
                stagesList.Add(stage);
            }
            return stagesList;
        }

        public List<GroupDTO> GetGroups(int stageId)
        {
            List<GroupDTO> groupList = new List<GroupDTO>();
            var groups = _dbContext.StageGroups.Where(x => x.StageId == stageId);
            foreach (var grp in groups)
            {
                GroupDTO grpItem = new GroupDTO();
                grpItem.Name = grp.GroupName;
                grpItem.GroupId = grp.Id;
                grpItem.Sequence = grp.Sequence;
                var groupCheckList = GetCheckList(grpItem.GroupId);
                grpItem.GroupCheckList = new List<CheckListDTO>();
                foreach (var checkList in groupCheckList)
                {
                    CheckListDTO checkListItem = new CheckListDTO();
                    checkListItem.CheckListId = checkList.Id;
                    checkListItem.Name = checkList.Name;
                    checkListItem.Points = checkList.Points;                 
                    grpItem.GroupCheckList.Add(checkListItem);
                }
                groupList.Add(grpItem);
            }
            return groupList;
        }

        public List<CheckListDTO> GetCheckListDTOList(int groupId)
        {
            var CheckListDataList =  GetCheckList(groupId);
            List<CheckListDTO> checkListDTOList = new List<CheckListDTO>();
            foreach(var checkListItem in CheckListDataList)
            {
                var checkListDTOItem = new CheckListDTO()
                {
                    Name = checkListItem.Name,
                    CheckListId = checkListItem.Id,
                    Points = checkListItem.Points
                };
                checkListDTOList.Add(checkListDTOItem);
            }
            return checkListDTOList;
        }

        public IQueryable<GroupCheckList> GetCheckList(int grouId)
        {
            return _dbContext.GroupCheckLists.Where(x => x.GroupId == grouId);
        }

        public List<StageDTO> GetAllStageData(int interviewType, IQueryable<int> completedCheckList, IQueryable<int> completedStagesList)
        {
            int totalStageScore;
            int totalCheckListCount;
            int completedCheckListCount;
            List<StageDTO> stageDetails = new List<StageDTO>();
            var stages = _dbContext.Stages.Where(x => x.InterviewTypeId == interviewType); ;
            foreach (var stage in stages)
            {
                StageDTO stageDetailItem = new StageDTO();
                totalStageScore = 0;
                completedCheckListCount = 0;
                totalCheckListCount = 0;
                stageDetailItem.Name = stage.StageName;
                stageDetailItem.StageId = stage.Id;
                stageDetailItem.Sequence = stage.Sequence;
                stageDetailItem.IsCompleted = completedStagesList.Any(x => x == stageDetailItem.StageId);
                var stageGroups = _dbContext.StageGroups.Where(x => x.StageId == stageDetailItem.StageId);
                stageDetailItem.StageGroups = new List<GroupDTO>();
                foreach (var group in stageGroups)
                {
                    GroupDTO groupItem = new GroupDTO();
                    groupItem.GroupId = group.Id;
                    groupItem.Name = group.GroupName;
                    groupItem.Sequence = group.Sequence;
                    var groupCheckList = GetCheckList(groupItem.GroupId);
                    groupItem.GroupCheckList = new List<CheckListDTO>();
                    foreach (var checkList in groupCheckList)
                    {
                        CheckListDTO checkListItem = new CheckListDTO();
                        checkListItem.CheckListId = checkList.Id;
                        checkListItem.Name = checkList.Name;
                        checkListItem.Points = checkList.Points;
                        checkListItem.IsChecked = completedCheckList.Any(x => x == checkList.Id);
                        if (checkListItem.IsChecked)
                        {
                            totalStageScore = totalStageScore + checkList.Points;
                            completedCheckListCount++;
                        }
                        totalCheckListCount++;
                        groupItem.GroupCheckList.Add(checkListItem);
                    }
                    stageDetailItem.StageGroups.Add(groupItem);
                }
                stageDetailItem.TotalCheckListCount = totalCheckListCount;
                stageDetailItem.CompletedCheckListCount = completedCheckListCount;
                stageDetailItem.StageScore = totalStageScore;
                stageDetails.Add(stageDetailItem);
            }
            return stageDetails;
        }

        public void AddGroup(string groupName, int stageId)
        {
            var maxSequene = _dbContext.StageGroups.Where(x => x.StageId == stageId).Select(x => x.Sequence).Max();
            StageGroup group = new StageGroup();
            group.GroupName = groupName;
            group.StageId = stageId;
            group.Sequence = maxSequene++;
            _dbContext.StageGroups.Add(group);
        }

        public int GetUserTotal(string userId)
        {
            return _dbContext.usp_GetUserTotalScore(userId).FirstOrDefault() ?? 0;
        }

        public bool IsCheckListExists(int checkListId, int groupId)
        {
            return _dbContext.GroupCheckLists.Any(x => x.Id == checkListId && x.GroupId == groupId);
        }

        public bool IsGroupExists(int groupId)
        {
            return _dbContext.GroupCheckLists.Any(x => x.Id == groupId);
        }

        public void AddCheckList(string checkListDescription, int checkListPoints, int GroupId)
        {
            GroupCheckList checkListItem = new GroupCheckList();
            checkListItem.GroupId = GroupId;
            checkListItem.Name = checkListDescription;
            checkListItem.Points = checkListPoints;
            _dbContext.GroupCheckLists.Add(checkListItem);
        }

        public void UpdateCheckList(int checkListId, string checkListDescription, int checkListPoints, int GroupId)
        {
            GroupCheckList checkListItem = _dbContext.GroupCheckLists.FirstOrDefault(x => x.Id == checkListId);
            if(checkListItem != null)
            {
                checkListItem.GroupId = GroupId;
                checkListItem.Name = checkListDescription;
                checkListItem.Points = checkListPoints;
            }
        }

        public void UpdateGroupName(int groupId, string groupName)
        {
            var group = _dbContext.StageGroups.First(x => x.Id == groupId);
            if(group != null)
            {
                group.GroupName = groupName;
            }
        }

    }
}

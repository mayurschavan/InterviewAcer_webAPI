using System.Web.Http;
using InterviewAcer.Repository.Contract;
using InterviewAcer.Repository.Implementation;
using System.Threading.Tasks;
using InterviewAcer.Common.DTO;
using System;
using System.Security.Claims;
using System.Linq;
using InterviewAcer.RequestClasses;
using InterviewAcer.ResponseClasses;
using System.Collections.Generic;

namespace InterviewAcer.Controllers
{
    public class StageController : ApiController
    {
        private IUnitOfWork _unitOfWork { get; set; }
        public StageController()
        {
            _unitOfWork = new UnitOfWork();
        }

        [Authorize(Roles ="Administrator")]
        [Route("api/GetStageDetails")]
        [HttpGet]
        public IHttpActionResult GetStages(int interviewTypeId)
        {
            try
            {
                var stages = _unitOfWork.GetStageRepository().GetStages(interviewTypeId);
                if(stages != null && stages.Any())
                {
                    return Ok(stages);
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Authorize(Roles = "Administrator")]
        [Route("api/GetGroups")]
        [HttpGet]
        public IHttpActionResult GetGroups(int stageId)
        {
            try
            {
                var stages = _unitOfWork.GetStageRepository().GetGroups(stageId);
                if (stages != null && stages.Any())
                {
                    return Ok(stages);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Authorize(Roles = "Administrator")]
        [Route("api/GetCheckList")]
        [HttpGet]
        public IHttpActionResult GetCheckList(int groupId)
        {
            try
            {
                var checkListDTOList = _unitOfWork.GetStageRepository().GetCheckListDTOList(groupId);
                if (checkListDTOList != null && checkListDTOList.Any())
                {
                    return Ok(checkListDTOList);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("api/AddGroup")]
        [Authorize(Roles ="Administrator")]
        [HttpPost]
        public async Task<IHttpActionResult> AddGroup(AddGroup groupDetails)
        {
            try
            {
                _unitOfWork.GetStageRepository().AddGroup(groupDetails.GroupName, groupDetails.StageId);
                await _unitOfWork.Save();
                return Ok();
            }
            catch(Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("api/AddUpdateCheckList")]
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IHttpActionResult> AddUpdateCheckList(List<AddUpdateCheckList> checkListDetails)
        {
            bool someRecordsAreNotValid = true;
            try
            {
                foreach (var checkListItem in checkListDetails)
                {
                    if (_unitOfWork.GetStageRepository().IsGroupExists(checkListItem.GroupId))
                    {
                        if (checkListItem.CheckListId > 0)
                        {
                            bool IsCheckListExists = _unitOfWork.GetStageRepository().IsCheckListExists(checkListItem.CheckListId, checkListItem.GroupId);
                            if (IsCheckListExists)
                            {
                                _unitOfWork.GetStageRepository().UpdateCheckList(checkListItem.CheckListId, checkListItem.CheckListDescription, checkListItem.CheckListScore, checkListItem.GroupId);
                                await _unitOfWork.Save();
                                someRecordsAreNotValid = false;
                            }
                          
                        }
                        else
                        {
                            _unitOfWork.GetStageRepository().AddCheckList(checkListItem.CheckListDescription, checkListItem.CheckListScore, checkListItem.GroupId);
                            await _unitOfWork.Save();
                            someRecordsAreNotValid = false;
                        }
                    }
                }
                if(someRecordsAreNotValid)
                {
                    return NotFound();
                }
                else
                {
                    return Ok();
                }
            }
            catch(Exception e)
            {
                return InternalServerError(e);
            }
        }

        [Route("api/UpdateGroupName")]
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateGroupName(UpdateGroupName updateGroupName)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                _unitOfWork.GetStageRepository().UpdateGroupName(updateGroupName.GroupId, updateGroupName.GroupName);
                await _unitOfWork.Save();
                return Ok();
            }
            catch(Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}
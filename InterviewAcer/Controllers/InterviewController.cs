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

namespace InterviewAcer.Controllers
{
    public class InterviewController : ApiController
    {
        private IUnitOfWork _unitOfWork { get; set; }
        public InterviewController()
        {
            _unitOfWork = new UnitOfWork();
        }


        /// <summary>
        /// Gets the list of interviews added by the logged In User
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/GetInterview")]
        public async Task<IHttpActionResult> GetInterviewDetails()
        {
            try
            {
                var claimsIdentity = RequestContext.Principal.Identity as ClaimsIdentity;
                var userName = claimsIdentity.Claims.Where(x => x.Type == "sub").Select(y => y.Value).SingleOrDefault();
                var interviewDetails = await _unitOfWork.GetInterviewRepository().GetInterviewDetails(userName);
                if (interviewDetails == null || interviewDetails.Count == 0)
                {
                    return NotFound();
                }
                return Ok(interviewDetails);
            }
            catch
            {
                return InternalServerError();
            }
        }



        /// <summary>
        /// Saves the interview details against the logged in user
        /// </summary>
        /// <param name="interviewDetails"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SaveInterview")]
        public async Task<IHttpActionResult> SaveInterviewDetails(InterviewDetailsDTO interviewDetails)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                if (interviewDetails != null)
                {
                    var claimsIdentity = RequestContext.Principal.Identity as ClaimsIdentity;
                    var userName = claimsIdentity.Claims.Where(x => x.Type == "sub").Select(y => y.Value).SingleOrDefault();
                    var savedInterviewDetails = _unitOfWork.GetInterviewRepository().SaveInterviewDetails(interviewDetails, userName);
                    await _unitOfWork.Save();
                    var response = new { interviewId = savedInterviewDetails.InterviewDetailId };
                    return Ok(response);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        /// <summary>
        /// Gets all data for stages of interview type
        /// </summary>
        /// <param name="interviewType"></param>
        /// <param name="interviewId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("api/GetStages")]
        public IHttpActionResult GetStageDetails(int interviewType, int interviewId)
        {
            try
            {
                if (interviewType == 0)
                {
                    return BadRequest();
                }
                var completedCheckList = _unitOfWork.GetInterviewRepository().GetCompletedCheckList(interviewId);
                var completedStageList = _unitOfWork.GetInterviewRepository().GetCompletedStages(interviewId);
                var stages = _unitOfWork.GetStageRepository().GetAllStageData(interviewType, completedCheckList, completedStageList);
                if (stages == null || stages.Count == 0)
                {
                    return NotFound();
                }
                return Ok(stages);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }


        /// <summary>
        /// Gets the checklist which are completed in the interview
        /// </summary>
        /// <param name="interviewId"></param>
        /// <returns></returns>
        //[Authorize]
        //[HttpGet]
        //[Route("api/GetCompletedCheckList")]
        //public async Task<IHttpActionResult> GetCompletedCheckList(int interviewId)
        //{
        //    try
        //    {
        //        if (interviewId == 0)
        //        {
        //            return BadRequest();
        //        }
        //        var CompletedCheckList = await _unitOfWork.GetInterviewRepository().GetCompletedCheckList(interviewId);
        //        if (CompletedCheckList == null || CompletedCheckList.Count == 0)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(CompletedCheckList);
        //    }
        //    catch (Exception e)
        //    {
        //        return InternalServerError(e);
        //    }
        //}

        /// <summary>
        /// Update the checklist for an interview
        /// </summary>
        /// <param name="updateCheckList"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/UpdateCheckList")]
        public async Task<IHttpActionResult> UpdateCheckList(UpdateCheckListRequest updateCheckList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var isUpdateSuccess = _unitOfWork.GetInterviewRepository().updateCheckList(updateCheckList.InterviewId, updateCheckList.CheckListId, updateCheckList.IsChecked);

                if (isUpdateSuccess)
                {
                    await _unitOfWork.Save();
                    var status = _unitOfWork.GetInterviewRepository().GetInterviewCurrentStatus(updateCheckList.InterviewId);
                    return Ok(status);
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

        [Authorize]
        [HttpPost]
        [Route("api/MarkStageAsComplete")]
        public async Task<IHttpActionResult> MarkStageAsComplete(MarkStageAsCompleteRequest markStageComplete)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                _unitOfWork.GetInterviewRepository().MarkStageAsComplete(markStageComplete.StageId, markStageComplete.InterviewId);
                await _unitOfWork.Save();
                var status = _unitOfWork.GetInterviewRepository().GetInterviewCurrentStatus(markStageComplete.InterviewId);
                return Ok(status);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}

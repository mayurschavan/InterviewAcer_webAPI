
CREATE PROCEDURE [dbo].[usp_GetUserTotalScore]
	@UserId NVARCHAR(MAX)
AS
BEGIN

  select SUM(gcl.Points) from GroupCheckList gcl
  INNER JOIN InterviewCheckListMapping iclm on gcl.Id = iclm.CheckListId
  INNER JOIN InterviewDetails id on iclm.InterviewId = id.InterviewDetailId
  INNER JOIN AspNetUsers u on u.UserName = id.UserName
  where u.id = @UserId
END



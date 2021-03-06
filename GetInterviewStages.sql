ALTER PROCEDURE [dbo].[GetInterviewStage] 
	@InterviewId int
AS
BEGIN
	select top 1 S.StageName, S.Id, S.Sequence from Stages s 
	WHERE S.Id not in (select StageId from InterviewCompletedStageMapping where InterviewId = @interviewId)
	AND S.InterviewTypeId = (select InterviewTypeId from InterviewDetails where InterviewDetailId = @interviewId)
	order by Sequence
END
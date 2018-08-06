
CREATE PROCEDURE usp_GetCompletedStages
	@interviewId int
AS
BEGIN
	select s.Id As StageId, COUNT(gcl.Id) CompletedCheckList into #temp from Stages s
	inner join StageGroups sg on s.Id = sg.StageId
	inner join GroupCheckList gcl on sg.Id = gcl.GroupId
	inner join InterviewCheckListMapping iclm on iclm.CheckListId = gcl.Id
	where iclm.InterviewId = @interviewId
	group by s.Id

	select s.Id As StageId, COUNT(gcl.Id) totalchecklist into #temp2  from Stages s
	inner join StageGroups sg on s.Id = sg.StageId
	inner join GroupCheckList gcl on sg.Id = gcl.GroupId
	group by s.Id

	select distinct #temp.StageId from #temp
	inner join #temp2 on #temp.CompletedCheckList = #temp2.totalchecklist

END
GO

using System.Collections.Generic;

namespace InterviewAcer.Common.DTO
{
    public class StageDTO
    {
        public int StageId { get; set; }
        public string Name { get; set; }
        public List<GroupDTO> StageGroups { get; set; }
        public int Sequence { get; set; }
        public int StageScore { get; set; }
        public int CompletedCheckListCount { get; set; }
        public int TotalCheckListCount { get; set; }
        public bool IsCompleted { get; set; }
    }
}

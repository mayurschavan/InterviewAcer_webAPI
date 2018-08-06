using System.Collections.Generic;

namespace InterviewAcer.Common.DTO
{
    public class GroupDTO
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public List<CheckListDTO> GroupCheckList { get; set; }
        public int Sequence { get; set; }
    }
}

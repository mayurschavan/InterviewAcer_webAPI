using System.ComponentModel.DataAnnotations;

namespace InterviewAcer.RequestClasses
{
    public class UpdateCheckListRequest
    {
        [Required]
        public int InterviewId { get; set; }
        [Required]
        public int CheckListId { get; set; }
        [Required]
        public bool IsChecked { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace InterviewAcer.Common.DTO
{
    public class InterviewDetailsDTO
    {
        [Required]
        [MaxLength(50)]
        public string CompanyName { get; set; }
        public int InterviewId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Designation { get; set; }
        [Required]
        public DateTime InterviewDate { get; set; }
        [Required]
        public int InterviewTypeId { get; set; }
        [MaxLength(50)]
        public string HiringIndividualName { get; set; }
        public string CurrentStageName { get; set; }
        public int CurrentStageId { get; set; }
        public string Tag { get; set; }
        public int TotalNumberOfStages { get; set; }
        public int CompletedNumberOfStages { get; set; }
    }
}

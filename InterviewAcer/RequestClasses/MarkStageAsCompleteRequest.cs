using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InterviewAcer.RequestClasses
{
    public class MarkStageAsCompleteRequest
    {
        [Required]
        public int StageId { get; set; }
        [Required]
        public int InterviewId { get; set; }
    }
}
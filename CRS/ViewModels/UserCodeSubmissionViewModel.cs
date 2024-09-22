using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRS.ViewModels
{
    public class UserCodeSubmissionViewModel
    {
        public int SubmissionId { get; set; }
        public string OrgCodeSnippet { get; set; }
        public string CodeSnippet { get; set; }
        public string Language { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
using CRS.Models;
using System.Collections.Generic;

namespace CRS.ViewModels
{
    public class CodeReviewDetailsViewModel
    {
        /*        public CodeSubmission Submission { get; set; }
                public List<CodeSuggestion> Suggestions { get; set; }
                public string OriginalCode { get; set; }
                public string IndentedCode { get; set; }
                public string Language { get; set; }*/
        public string OriginalCode { get; set; }
        public string IndentedCode { get; set; }
        public string Language { get; set; }
        public List<CodeSuggestion> Suggestions { get; set; }
    }
}

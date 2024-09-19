using CRS.Models;
using System.Collections.Generic;

namespace CRS.ViewModels
{
    public class CodeReviewDetailsViewModel
    {
        public CodeSubmission Submission { get; set; }
        public List<CodeSuggestion> Suggestions { get; set; }
    }
}

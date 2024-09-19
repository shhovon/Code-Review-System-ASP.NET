using System;
using System.Web.UI.WebControls;

namespace CRS.Models
{
    public class Suggestion
    {
        public int SuggestionId { get; set; }
        public int ContentId { get; set; }
        public string SuggestionText { get; set; }
        public string SuggestionType { get; set; }
        public DateTime CreatedAt { get; set; }
        public Content Content { get; set; }
    }
}

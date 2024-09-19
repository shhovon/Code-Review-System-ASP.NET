using CRS.Models;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace CRS.ViewModels
{
    public class ContentDetailsViewModel
    {
        public Content Content { get; set; }
        public List<Suggestion> Suggestions { get; set; }
    }
}

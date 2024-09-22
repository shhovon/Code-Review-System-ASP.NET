using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using CRS.Models;
using CRS.ViewModels;

namespace CRS.Controllers
{
    [Authorize]
    public class CodeReviewController : Controller
    {
        private readonly CRSEntities _context;

        public CodeReviewController()
        {
            _context = new CRSEntities();
        }

        public ActionResult SubmitCode()
        {
            ViewBag.Languages = GetProgrammingLanguages();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> SubmitCode(CodeSubmission submission)
        {
            ViewBag.Languages = GetProgrammingLanguages();

            if (ModelState.IsValid)
            {
                try
                {
                    // Save the original user input
                    submission.OrgCodeSnippet = submission.OrgCodeSnippet;

                    // Generate the indented version
                    submission.CodeSnippet = IndentCode(submission.OrgCodeSnippet, submission.Language);

                    submission.CreatedAt = DateTime.Now;
                    _context.CodeSubmissions.Add(submission);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", new { id = submission.SubmissionId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while processing your submission. Please try again.");
                }
            }
            return View(submission);
        }

        public ActionResult Details(int id)
        {
            var submission = _context.CodeSubmissions
                .Include("CodeSuggestions")
                .FirstOrDefault(s => s.SubmissionId == id);

            if (submission == null)
            {
                return HttpNotFound();
            }

            var viewModel = new CodeReviewDetailsViewModel
            {
                OriginalCode = submission.OrgCodeSnippet,
                IndentedCode = IndentCode(submission.CodeSnippet, submission.Language),
                Language = submission.Language,
                Suggestions = submission.CodeSuggestions.ToList()
            };

            return View(viewModel);
        }

        private List<SelectListItem> GetProgrammingLanguages()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "csharp", Text = "C#" },
                new SelectListItem { Value = "java", Text = "Java" },
                new SelectListItem { Value = "python", Text = "Python" },
                new SelectListItem { Value = "javascript", Text = "JavaScript" },
                new SelectListItem { Value = "ruby", Text = "Ruby" },
                new SelectListItem { Value = "go", Text = "Go" },
                new SelectListItem { Value = "swift", Text = "Swift" },
                new SelectListItem { Value = "php", Text = "PHP" },
                new SelectListItem { Value = "cpp", Text = "C++" },
                new SelectListItem { Value = "typescript", Text = "TypeScript" }
            };
        }

        private string IndentCode(string code, string language)
        {
            var lines = SplitIntoLogicalLines(code);
            var indentedLines = new List<string>();
            int indentLevel = 0;
            string indentString = GetIndentString(language);
            bool inMultiLineComment = false;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (string.IsNullOrWhiteSpace(trimmedLine))
                {
                    indentedLines.Add(string.Empty);
                    continue;
                }

                if (IsMultiLineCommentStart(trimmedLine, language))
                    inMultiLineComment = true;

                if (!inMultiLineComment && !IsCommentLine(trimmedLine, language))
                {
                    var indentedLine = new string(' ', indentLevel * indentString.Length);

                    if (trimmedLine.StartsWith("}") || trimmedLine.StartsWith(")"))
                        indentLevel = Math.Max(0, indentLevel - 1);

                    indentedLine += trimmedLine;
                    indentedLines.Add(indentedLine);

                    if (trimmedLine.EndsWith("{") || trimmedLine.EndsWith("("))
                        indentLevel++;
                    else if ((trimmedLine.EndsWith("}") || trimmedLine.EndsWith(")")) && !trimmedLine.StartsWith("}") && !trimmedLine.StartsWith(")"))
                        indentLevel = Math.Max(0, indentLevel - 1);
                }
                else
                {
                    indentedLines.Add(new string(' ', indentLevel * indentString.Length) + trimmedLine);
                }

                if (IsMultiLineCommentEnd(trimmedLine, language))
                    inMultiLineComment = false;
            }

            return string.Join(Environment.NewLine, indentedLines);
        }

        private List<string> SplitIntoLogicalLines(string code)
        {
            var logicalLines = new List<string>();
            var currentLine = "";
            bool inString = false;
            char stringDelimiter = '\0';

            foreach (char c in code)
            {
                currentLine += c;

                if (c == '"' || c == '\'')
                {
                    if (!inString)
                    {
                        inString = true;
                        stringDelimiter = c;
                    }
                    else if (c == stringDelimiter)
                    {
                        inString = false;
                    }
                }

                if (!inString && (c == ';' || c == '{' || c == '}'))
                {
                    logicalLines.Add(currentLine.Trim());
                    currentLine = "";
                }
            }

            if (!string.IsNullOrWhiteSpace(currentLine))
            {
                logicalLines.Add(currentLine.Trim());
            }

            return logicalLines;
        }


        private string GetIndentString(string language)
        {
            switch (language.ToLower())
            {
                case "python":
                case "ruby":
                    return "    "; // 4 spaces
                default:
                    return "  "; // 2 spaces
            }
        }

        private bool IsOpeningBrace(string line, string language)
        {
            switch (language.ToLower())
            {
                case "python":
                    return line.EndsWith(":") && !line.TrimStart().StartsWith("else:") && !line.TrimStart().StartsWith("elif ");
                case "ruby":
                    return line.EndsWith("do") || line.EndsWith("begin") || Regex.IsMatch(line, @"(\s|^)if\s.*$") || Regex.IsMatch(line, @"(\s|^)unless\s.*$") ||
                           Regex.IsMatch(line, @"(\s|^)while\s.*$") || Regex.IsMatch(line, @"(\s|^)for\s.*$") || Regex.IsMatch(line, @"(\s|^)until\s.*$") ||
                           Regex.IsMatch(line, @"(\s|^)def\s.*$") || Regex.IsMatch(line, @"(\s|^)class\s.*$") || Regex.IsMatch(line, @"(\s|^)module\s.*$");
                default:
                    return line.EndsWith("{");
            }
        }

        private bool IsClosingBrace(string line, string language)
        {
            switch (language.ToLower())
            {
                case "python":
                    return false;
                case "ruby":
                    return line.TrimStart().StartsWith("end");
                default:
                    return line.TrimStart().StartsWith("}");
            }
        }

        private bool ShouldIncreaseIndent(string line, string language)
        {
            switch (language.ToLower())
            {
                case "python":
                    return line.EndsWith(":") && !line.TrimStart().StartsWith("else:") && !line.TrimStart().StartsWith("elif ");
                case "ruby":
                    return line.EndsWith("do") || line.EndsWith("begin");
                default:
                    return false;
            }
        }

        private bool ShouldReduceIndent(string line, string language)
        {
            switch (language.ToLower())
            {
                case "python":
                    return line.TrimStart().StartsWith("else:") || line.TrimStart().StartsWith("elif ") || line.TrimStart().StartsWith("except:") || line.TrimStart().StartsWith("finally:");
                case "ruby":
                    return line.TrimStart().StartsWith("else") || line.TrimStart().StartsWith("elsif ") || line.TrimStart().StartsWith("rescue") || line.TrimStart().StartsWith("ensure");
                default:
                    return line.TrimStart().StartsWith("else") || line.TrimStart().StartsWith("else if") || line.TrimStart().StartsWith("elsif ") ||
                           line.TrimStart().StartsWith("catch") || line.TrimStart().StartsWith("finally");
            }
        }

        private bool IsCommentLine(string line, string language)
        {
            switch (language.ToLower())
            {
                case "python":
                    return line.TrimStart().StartsWith("#");
                case "ruby":
                    return line.TrimStart().StartsWith("#");
                case "javascript":
                case "typescript":
                case "java":
                case "csharp":
                case "cpp":
                case "go":
                case "swift":
                case "php":
                    return line.TrimStart().StartsWith("//") || line.TrimStart().StartsWith("/*") || line.TrimStart().StartsWith("*");
                default:
                    return false;
            }
        }

        private bool IsMultiLineCommentStart(string line, string language)
        {
            switch (language.ToLower())
            {
                case "python":
                    return line.TrimStart().StartsWith("'''") || line.TrimStart().StartsWith("\"\"\"");
                case "javascript":
                case "typescript":
                case "java":
                case "csharp":
                case "cpp":
                case "go":
                case "swift":
                case "php":
                    return line.Contains("/*") && !line.Contains("*/");
                default:
                    return false;
            }
        }

        private bool IsMultiLineCommentEnd(string line, string language)
        {
            switch (language.ToLower())
            {
                case "python":
                    return line.TrimEnd().EndsWith("'''") || line.TrimEnd().EndsWith("\"\"\"");
                case "javascript":
                case "typescript":
                case "java":
                case "csharp":
                case "cpp":
                case "go":
                case "swift":
                case "php":
                    return line.Contains("*/");
                default:
                    return false;
            }
        }
    }
}
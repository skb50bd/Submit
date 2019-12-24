using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Submitter.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public IFormFile Input { get; set; }

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!(Input?.Length > 0)) return RedirectToPage("/Error");

            await using var memoryStream = new MemoryStream();
            await Input.CopyToAsync(memoryStream);
            var byteArray = memoryStream.ToArray();

            var fileName = Path.GetFileNameWithoutExtension(Input.FileName)
                         + $"_{DateTime.Now.ToString("yyyyMMddHHmmss")}"
                         + Path.GetExtension(Input.FileName);
            _logger.LogInformation($"Writing File: {fileName}");

            if (!Directory.Exists("Submissions"))
                Directory.CreateDirectory("Submissions");

            System.IO.File.WriteAllBytes(
                Path.Combine("Submissions", fileName),
                byteArray);

            return RedirectToPage("./ThankYou");
        }
    }
}

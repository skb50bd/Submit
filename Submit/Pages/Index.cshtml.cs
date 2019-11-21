using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Submit.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public InputModel Input { get; set; }

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

            if (!(Input.File?.Length > 0)) return null;

            await using var memoryStream = new MemoryStream();
            await Input.File.CopyToAsync(memoryStream);
            var byteArray = memoryStream.ToArray();

            var fileName = Path.GetFileNameWithoutExtension(Input.File.FileName)
                         + $"_{DateTime.Now.ToString("yyyyMMddHHmmss")}"
                         + Path.GetExtension(Input.File.FileName);
            _logger.LogInformation($"Writing File: {fileName}");

            if (!Directory.Exists("Submissions"))
                Directory.CreateDirectory("Submissions");
            System.IO.File.WriteAllBytes(
                Path.Combine("Submissions", fileName),
                byteArray);
            
            return RedirectToPage("./ThankYou");
        } 
    }

    public class InputModel
    {
        public IFormFile File { get; set; }
    }
}

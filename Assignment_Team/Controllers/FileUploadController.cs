using Assignment_Team.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_Team.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileUploadController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost("FileUpload")]
        public async Task<IActionResult> Index(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var filePaths = new List<string>();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    string contentRootPath = _webHostEnvironment.ContentRootPath;
                    string path = Path.Combine(contentRootPath, "UploadedFiles");

                    var filePath = path;
                    filePaths.Add(filePath);
                    using (var stream = new FileStream(Path.Combine("UploadedFiles", formFile.FileName), FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}

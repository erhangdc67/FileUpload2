using Assignment_Team.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment_Team.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetListInfo()
        {
            var Info = new List<FileInfomations>();

            string contentRootPath = _webHostEnvironment.ContentRootPath;
            string path = Path.Combine(contentRootPath, "UploadedFiles");

            var filePath = path;

            foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                FileInfo _fileInfo = new FileInfo(file);

                Info.Add(new FileInfomations { Path = _fileInfo.Name.ToString(),  Name = _fileInfo.Name, Lenght = _fileInfo.Length.ToString(), CreateDate = _fileInfo.CreationTime.ToString() });
            }
            return Json(Info);
        }

        [HttpPost]
        public JsonResult DeleteItem(string file)
        {
            try
            {
                string contentRootPath = _webHostEnvironment.ContentRootPath;
                string path = Path.Combine(contentRootPath, "UploadedFiles");

                if (System.IO.File.Exists(Path.Combine(path, file)))
                {
                    System.IO.File.Delete(Path.Combine(path, file));
                }

                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
           
        }

        [HttpPost]
        public JsonResult ShowItem(string file)
        {
            try
            {
                string contentRootPath = _webHostEnvironment.ContentRootPath;
                string path = Path.Combine(contentRootPath, "UploadedFiles");

                if (System.IO.File.Exists(Path.Combine(path, file)))
                {
                    var currencyDetails = System.IO.File.ReadAllLines(Path.Combine(path, file));
                    return Json("erhan message");
                }
                else
                    return Json(false);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

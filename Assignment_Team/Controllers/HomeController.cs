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
using System.Text.Json;
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

                Info.Add(new FileInfomations { Path = _fileInfo.Name.ToString(), Name = _fileInfo.Name, Lenght = _fileInfo.Length.ToString(), CreateDate = _fileInfo.CreationTime.ToString() });
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
                    currencyDetails = currencyDetails.Skip(1).ToArray();

                    List<ResultModel> resultModels = new List<ResultModel>();
                    List<ResultModelUI> resultUIModels = new List<ResultModelUI>();
                    foreach (var item in currencyDetails)
                    {
                        string[] row = item.Split(",");

                        DateTime fromDate = string.IsNullOrEmpty(row[2]) || !DateTime.TryParse(row[2], out fromDate) ? DateTime.Now : DateTime.Parse(row[2]);
                        DateTime ToDate = string.IsNullOrEmpty(row[3]) || !DateTime.TryParse(row[3], out ToDate) ? DateTime.Now : DateTime.Parse(row[3]);

                        resultModels.Add(new ResultModel()
                        {
                            EmpID = row[0],
                            ProjectID = row[1],
                            FromDate = fromDate,
                            ToDate = ToDate
                        });
                    }


                    var result = resultModels.GroupBy(l => new { l.ProjectID })
                                  .Select(item => new { item.Key.ProjectID, item }).ToList();

                    foreach (var loopItem in result)
                    {
                        var pair = new ResultModelUI();
                        var row = loopItem.item.ToList();
                        pair.EmpID1 = row.FirstOrDefault().EmpID;
                        pair.EmpID2 = row.LastOrDefault().EmpID;
                        pair.ProjectID= row.FirstOrDefault().ProjectID;


                        bool overlap = row.FirstOrDefault().FromDate < row.LastOrDefault().ToDate && row.LastOrDefault().FromDate < row.FirstOrDefault().ToDate;

                        double totalDays = default(double);
                        if (overlap)
                        {
                            var maxDate = row.FirstOrDefault(f => f.FromDate == row.Max(m => m.FromDate).Date).FromDate;
                            var minDate = row.FirstOrDefault(f => f.ToDate == row.Min(m => m.ToDate).Date) != null ? row.FirstOrDefault(f => f.ToDate == row.Min(m => m.ToDate).Date).ToDate : row.FirstOrDefault().ToDate;

                            totalDays = (minDate - maxDate).Days;
                        }

                        pair.Days = totalDays.ToString();
                        resultUIModels.Add(pair);
                    }


                    return Json(resultUIModels);
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ImageMetaReader.Models;
using Directory = MetadataExtractor.Directory;

namespace ImageMetaReader.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(null);
        }

        [HttpPost]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [Route("read-meta", Name = "read-meta")]
        public IActionResult ReadMetaData()
        {
            var files = this.Request.Form.Files;
            IList<Directory> directories = new List<Directory>();
            if (files != null && files.Any())
            {

                
                foreach (var file in files)
                {
                    InPlaceBitmapMetadataWriter metadataWriter = null;
                    BitmapDecoder decoder = null;
                    string metadaValor = "bonjour";
                    Stream saveLocationStream = new FileStream(file.FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    decoder = new JpegBitmapDecoder(saveLocationStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                    metadataWriter = decoder.Frames[0].CreateInPlaceBitmapMetadataWriter();
                    if (metadataWriter.TrySave())
                    {
                        metadataWriter.SetQuery("/xmp/photoshop:Source", metadaValor);
                    }

                    if (metadataWriter.TrySave())
                    {
                        metadataWriter.SetQuery("/app13/{ushort=0}/{ulonglong=61857348781060}/{ushort=1}/{str=Source}", metadaValor);
                    }

                    saveLocationStream.Close();
                    decoder = null;

                

            }
            }
            return View("Index", directories);
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

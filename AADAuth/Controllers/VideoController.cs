using AADAuth.CustomResult;
using AADAuth.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AADAuth.Controllers
{
    public class VideoController : Controller
    {
        // GET: Video
        public ActionResult Index(string path)
        {
            return new VideoResult();
        }
    }
}
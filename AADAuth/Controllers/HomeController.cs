using AADAuth.CustomResult;
using AADAuth.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace AADAuth.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult Index()
        {
            List<VideoFiles> videolist = new List<VideoFiles>();
            string CS = ConfigurationManager.ConnectionStrings["Inventory"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spGetAllVideoFile", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    VideoFiles video = new VideoFiles();
                    video.ID = Convert.ToInt32(dr["ID"]);
                    video.Name = dr["Name"].ToString();
                    video.FileSize = Convert.ToInt32(dr["FileSize"]);
                    video.FilePath = dr["FilePath"].ToString();
                    videolist.Add(video);
                }
            }
            return View(videolist);
        }

        [HttpGet]
        public EmptyResult VideoStream(long Id)
        {
            string videoPath = string.Empty;
            string CS = ConfigurationManager.ConnectionStrings["Inventory"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spGetVideoFileById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                cmd.Parameters.AddWithValue("@Id", Id);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    if (Convert.ToInt32(dr["ID"]) == Id)
                    {
                        videoPath = dr["FilePath"].ToString();
                    }
                }
            }
            
            var videoFilePath = HostingEnvironment.MapPath(videoPath);
            var file = new FileInfo(videoFilePath);
            //Check the file exist,  it will be written into the response
            if (file.Exists)
            {
                var stream = file.OpenRead();
                var bytesinfile = new byte[stream.Length];
                stream.Read(bytesinfile, 0, (int)file.Length);
                ControllerContext.HttpContext.Response.BinaryWrite(bytesinfile);
            }
            return new EmptyResult();

        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase fileupload)
        {
            if (fileupload != null)
            {
                string fileName = Path.GetFileName(fileupload.FileName);
                int fileSize = fileupload.ContentLength;
                int Size = fileSize / 1000;
                fileupload.SaveAs(Server.MapPath("~/VideoFiles/" + fileName));

                string CS = ConfigurationManager.ConnectionStrings["Inventory"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    SqlCommand cmd = new SqlCommand("spAddNewVideoFile", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Parameters.AddWithValue("@Name", fileName);
                    cmd.Parameters.AddWithValue("@FileSize", Size);
                    cmd.Parameters.AddWithValue("FilePath", "~/VideoFiles/" + fileName);
                    cmd.ExecuteNonQuery();
                    con.Close();
                    ViewData["Message"] = "Video Saved successfully";
                }
            }
            return RedirectToAction("Index");
        }
    }
}
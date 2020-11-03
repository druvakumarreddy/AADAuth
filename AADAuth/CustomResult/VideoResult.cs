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

namespace AADAuth.CustomResult
{
    public class VideoResult : ActionResult
    {
        /// <summary>
        /// The below method will respond with the Video file
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            List<string> urls = new List<string>();
            string CS = ConfigurationManager.ConnectionStrings["Inventory"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spGetAllVideoFile", con);
                cmd.CommandType = CommandType.StoredProcedure;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    urls.Add(rdr["FilePath"].ToString());
                }
                con.Close();
            }

            //The File Path
            foreach (string url in urls)
            {
                var videoFilePath = HostingEnvironment.MapPath(url);
                var file = new FileInfo(videoFilePath);
                //Check the file exist,  it will be written into the response
                if (file.Exists)
                {
                    var stream = file.OpenRead();
                    var bytesinfile = new byte[stream.Length];
                    stream.Read(bytesinfile, 0, (int)file.Length);
                    context.HttpContext.Response.BinaryWrite(bytesinfile);
                }
            }
            //var videoFilePath = HostingEnvironment.MapPath(urls[0]);
            //The header information
            //context.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=Win8.mp4");
            //var file = new FileInfo(videoFilePath);
            ////Check the file exist,  it will be written into the response
            //if (file.Exists)
            //{
            //    var stream = file.OpenRead();
            //    var bytesinfile = new byte[stream.Length];
            //    stream.Read(bytesinfile, 0, (int)file.Length);
            //    context.HttpContext.Response.BinaryWrite(bytesinfile);
            //}
        }

    }
}
using EgyptMenu.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace EgyptMenu.Controllers
{

    public class MeetApiController : ApiController
    {
        // GET: api/MeetApi
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/MeetApi/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/MeetApi
        [HttpPost]
        public void Post([FromUri]string Name)
        {
            //var File = model.Image;
            
            var File = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;           
            var ext = new FileInfo(File.FileName).Extension;
            var fullpath = HttpContext.Current.Server.MapPath("~/Content/images/" + Name+ext);            
            File.SaveAs(fullpath);
        }



        // DELETE: api/MeetApi/5
        public void Delete(int id)
        {
        }
    }
}

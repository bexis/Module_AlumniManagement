using BExIS.Dlm.Services.Party;
using BEXIS.Modules.ALM.UI.Model;
using BExIS.Security.Entities.Subjects;
using BExIS.Security.Services.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using Newtonsoft.Json;
using BExIS.Modules.ALM.UI.Helpers;

namespace BEXIS.Modules.ALM.UI.Controllers
{
    public class AlumniController : Controller
    {
        // GET: Alumni
        public ActionResult Index()
        {
            List<AlumniUserModel> model = new List<AlumniUserModel>();

            UserManager userManager = new UserManager();
            List<object> userObjectList = new List<object>();

            using (var partyManager = new PartyManager())
            {
                foreach (User user in userManager.Users)
                {
                    var party = partyManager.GetPartyByUser(user.Id);
                    model.Add(new AlumniUserModel(user,party, true));
                }
            }


            return View("ManageAlumni", model);
        }

        public ActionResult ChangeAlumniStatus(string userName)
        {
            UserManager userManager = new UserManager();
            User user = userManager.Users.Where(u => u.UserName == userName).FirstOrDefault();

            //Check if alumni
            bool isAlumni = AlumniStatus.IsAlumni(user.Id);
            bool status = false;
            if (isAlumni)
                status = AlumniStatus.ChangeToAlumni(user);
            else
                status = AlumniStatus.ChangeToNonAlumni(user);

            return View("ManageAlumni");
        }

        [WebMethod]
        public JsonResult GetAllUsers()
        {
            List<AlumniUserModel> model = new List<AlumniUserModel>();

            UserManager userManager = new UserManager();
            List<object> userObjectList = new List<object>();

            using (var partyManager = new PartyManager())
            {
                foreach (User user in userManager.Users)
                {
                    var party = partyManager.GetPartyByUser(user.Id);
                    userObjectList.Add("[" + JsonConvert.SerializeObject(new AlumniUserModel(user, party, false)) + "]");
                    //userObjectList.Add(new object[] { JsonHelper.JsonSerializer<AlumniUserModel>(new AlumniUserModel(user, party, false)) });
                }
            }

            return Json(userObjectList, JsonRequestBehavior.AllowGet);
        }
        }
}
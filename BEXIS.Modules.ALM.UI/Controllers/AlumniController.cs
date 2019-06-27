using BExIS.Dlm.Services.Party;
using BEXIS.Modules.ALM.UI.Model;
using BExIS.Security.Entities.Subjects;
using BExIS.Security.Services.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BEXIS.Modules.ALM.UI.Controllers
{
    public class AlumniController : Controller
    {
        // GET: Alumni
        public ActionResult Index()
        {
            List<AlumniUserModel> model = new List<AlumniUserModel>();

            UserManager userManager = new UserManager();

            using (var partyManager = new PartyManager())
            {
                foreach (User user in userManager.Users)
                {
                    var party = partyManager.GetPartyByUser(user.Id);
                    model.Add(new AlumniUserModel(user, party, false));
                }
            }

            return View("ManageAlumni", model);
        }
    }
}
using BExIS.Dlm.Entities.Party;
using BExIS.Security.Entities.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEXIS.Modules.ALM.UI.Model
{
    public class AlumniUserModel
    {
        public User User { get; set; }
        public bool IsAlumni { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public AlumniUserModel()
        {
            User = new User();
        }

        public AlumniUserModel(User user, Party party, bool isAlumni)
        {
            User = user;
            IsAlumni = isAlumni;
            StartDate = party.StartDate;
            EndDate = party.EndDate;
            FirstName = party.Name;
            LastName = party.Name;
        }


    }
}
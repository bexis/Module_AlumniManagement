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
        public string UserName { get; set; }
        public bool IsAlumni { get; set; }
        //public DateTime StartDate { get; set; }
        //public DateTime EndDate { get; set; }
        public string Name { get; set; }

        public AlumniUserModel()
        {
            
        }

        public AlumniUserModel(User user, Party party, bool isAlumni)
        {
            UserName = user.UserName;
            IsAlumni = isAlumni;
            
            //StartDate = party.StartDate;
            //EndDate = party.EndDate;
            Name = party.Name;
        }


    }
}
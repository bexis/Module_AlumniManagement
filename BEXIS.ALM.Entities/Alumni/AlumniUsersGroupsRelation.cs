using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEXIS.ALM.Entities.Alumni
{
    public class AlumniUsersGroupsRelation
    {
        public virtual long UserRef { get; set; }
        public virtual long GroupRef { get; set; }
    }
}

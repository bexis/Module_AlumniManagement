using BExIS.Security.Entities.Objects;
using BExIS.Security.Entities.Subjects;
using Vaiona.Entities.Common;

namespace BEXIS.ALM.Entities.Alumni
{
   

    /// <summary>
    ///
    /// </summary>
    public class AlumniEntityPermission : BaseEntity
    {
        public virtual Entity Entity { get; set; }
        public virtual long Key { get; set; }
        public virtual int Rights { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
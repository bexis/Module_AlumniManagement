using BExIS.Security.Entities.Authorization;
using BExIS.Security.Entities.Objects;
using BExIS.Security.Entities.Subjects;
using Vaiona.Entities.Common;

namespace BEXIS.ALM.Entities.Alumni
{
   
    public class AlumniFeaturePermission : BaseEntity
    {
        public virtual Feature Feature { get; set; }
        public virtual PermissionType PermissionType { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
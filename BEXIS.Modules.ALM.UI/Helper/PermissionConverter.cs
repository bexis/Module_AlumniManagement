using BEXIS.ALM.Entities.Alumni;
using BExIS.Security.Entities.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BEXIS.Modules.ALM.UI.Helper
{
    public static class PermissionConverter
    {
        public static AlumniEntityPermission ToAlumniEntityPermission(EntityPermission entityPermission)
        {
            var alumniEntityPermission = new AlumniEntityPermission
            {
                Entity = entityPermission.Entity,
                Key = entityPermission.Key,
                Rights = entityPermission.Rights,
                Subject = entityPermission.Subject
            };

            return alumniEntityPermission;
        }

        public static EntityPermission ToEntityPermission(AlumniEntityPermission alumniEntityPermission)
        {
            var entityPermission = new EntityPermission
            {
                Entity = alumniEntityPermission.Entity,
                Key = alumniEntityPermission.Key,
                Rights = alumniEntityPermission.Rights,
                Subject = alumniEntityPermission.Subject
            };

            return entityPermission;
        }
    }
}
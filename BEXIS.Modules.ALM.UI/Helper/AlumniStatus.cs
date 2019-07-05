using BExIS.Security.Entities.Authorization;
using BExIS.Security.Entities.Subjects;
using BExIS.Security.Services.Authorization;
using BExIS.Security.Services.Subjects;
using BEXIS.ALM.Entities.Alumni;
using BEXIS.ALM.Services.Alumni;
using BEXIS.Modules.ALM.UI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BExIS.Modules.ALM.UI.Helpers
{
    public static class AlumniStatus
    {
        //check if the user hat entries in the alumni module tables
        public static bool IsAlumni(long userId)
        {
            bool isAlumni = false;
            using (var alumniEntityPermissionManager = new AlumniEntityPermissionManager())
            using (var alumniFeaturePermissionManager = new AlumniFeaturePermissionManager())
            {
                if (alumniEntityPermissionManager.AlumniEntityPermissionRepository.Get(a => a.Subject.Id == userId).Count > 0)
                    isAlumni = true;
                if (alumniFeaturePermissionManager.AlumniFeaturePermissionRepository.Get(a => a.Subject.Id == userId).Count > 0)
                    isAlumni = true;

                return isAlumni;
            }
        }

        public static bool ChangeToAlumni(User user)
        {
            bool statuschanged = false;

            using (var alumniEntityPermissionManager = new AlumniEntityPermissionManager())
            using (var alumniFeaturePermissionManager = new AlumniFeaturePermissionManager())
            using (var entityPermissionManager = new EntityPermissionManager())
            using (var featurePermissionManager = new FeaturePermissionManager())
            {
                //transfer all feature permission
                var featurePermissions = featurePermissionManager.FeaturePermissionRepository.Get(a => a.Subject.Id == userId).ToList();
                if (featurePermissions != null)
                {
                    List<AlumniFeaturePermission> aList = new List<AlumniFeaturePermission>();
                    featurePermissions.ForEach(a => aList.Add(PermissionConverter.ToAlumniFeaturePermission(a)));
                    statuschanged = true;
                }

                //transfer entity permisstions
                var entityPermissions = entityPermissionManager.EntityPermissionRepository.Get(a => a.Subject.Id == userId).ToList();

                if (entityPermissions != null)
                {
                    List<AlumniEntityPermission> aList = new List<AlumniEntityPermission>();
                    entityPermissions.ForEach(a => aList.Add(PermissionConverter.ToAlumniEntityPermission(a)));
                    statuschanged = true;
                }
            }

            //add to alumni group
            GroupManager groupManager = new GroupManager();
            var alumniGroup = groupManager.Groups.Where(g => g.Name.ToLower() == "alumni").FirstOrDefault();
            alumniGroup.Users.Add(user);
            groupManager.UpdateAsync(alumniGroup);

            return statuschanged;
        }

        public static bool ChangeToNonAlumni(User user)
        {
            bool statuschanged = false;

            using (var alumniEntityPermissionManager = new AlumniEntityPermissionManager())
            using (var alumniFeaturePermissionManager = new AlumniFeaturePermissionManager())
            using (var entityPermissionManager = new EntityPermissionManager())
            using (var featurePermissionManager = new FeaturePermissionManager())
            {
                //transfer all feature permission
                var alumniFeaturePermissions = alumniFeaturePermissionManager.AlumniFeaturePermissionRepository.Get(a => a.Subject.Id == user.Id).ToList();
                if (alumniFeaturePermissions != null)
                {
                    List<FeaturePermission> aList = new List<FeaturePermission>();
                    alumniFeaturePermissions.ForEach(a => aList.Add(PermissionConverter.ToFeaturePermission(a)));
                    statuschanged = true;
                }

                //transfer entity permisstions
                var alumniEntityPermissions = alumniEntityPermissionManager.AlumniEntityPermissionRepository.Get(a => a.Subject.Id == user.Id).ToList();

                if (alumniEntityPermissions != null)
                {
                    List<EntityPermission> aList = new List<EntityPermission>();
                    alumniEntityPermissions.ForEach(a => aList.Add(PermissionConverter.ToEntityPermission(a)));
                    statuschanged = true;
                }
            }

            //remove from alumni group
            GroupManager groupManager = new GroupManager();
            var alumniGroup = groupManager.Groups.Where(g => g.Name.ToLower() == "alumni").FirstOrDefault();
            alumniGroup.Users.Remove(user);
            groupManager.UpdateAsync(alumniGroup);
            return statuschanged;
        }
    }

}
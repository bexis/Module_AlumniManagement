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
            //entity and feature permissions
            using (var alumniEntityPermissionManager = new AlumniEntityPermissionManager())
            using (var alumniFeaturePermissionManager = new AlumniFeaturePermissionManager())
            using (var entityPermissionManager = new EntityPermissionManager())
            using (var featurePermissionManager = new FeaturePermissionManager())
            {
                //transfer all feature permission
                var featurePermissions = featurePermissionManager.FeaturePermissionRepository.Get(a => a.Subject.Id == user.Id).ToList();
                if (featurePermissions.Count > 0)
                {
                    //featurePermissions.ForEach(a => aList.Add(PermissionConverter.ToAlumniFeaturePermission(a)));

                    //Create for each feature permission a alumni feature permission
                    featurePermissions.ForEach(u => alumniFeaturePermissionManager.Create(user, u.Feature, u.PermissionType));
                    //Remove feature permissions
                    featurePermissions.ForEach(u => featurePermissionManager.Delete(u));

                    statuschanged = true;
                }

                //transfer entity permisstions where uder has access via
                //var entityPermissions = entityPermissionManager.EntityPermissionRepository.Get(a => a.Subject.Id == user.Id).ToList();

                //if (entityPermissions.Count > 0)
                //{
                //    //Create alumni entity permissions
                //    entityPermissions.ForEach(u => alumniEntityPermissionManager.Create(user, u.Entity, u.Key, u.Rights));
                //    //remove
                //    entityPermissions.ForEach(u => entityPermissionManager.Delete(u));


                //    statuschanged = true;
                //}
            }

            //remove all groups and add alumni
            using (var groupManager = new GroupManager())
            using (var alumniUsersGroupsRelationManager = new AlumniUsersGroupsRelationManager())
            {
                //remove all groups from user and add to alumniUsersGroupsRelation
                foreach (var group in user.Groups)
                {
                    alumniUsersGroupsRelationManager.Create(user.Id, group.Id);

                    group.Users.Remove(user);
                    groupManager.UpdateAsync(group);
                }

                //add alumni
                var alumniGroup = groupManager.Groups.Where(g => g.Name.ToLower() == "alumni").FirstOrDefault();
                alumniGroup.Users.Add(user);
                groupManager.UpdateAsync(alumniGroup);

                statuschanged = true;
            }

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
                if (alumniFeaturePermissions .Count > 0)
                {
                    alumniFeaturePermissions.ForEach(u => featurePermissionManager.Create(user, u.Feature, u.PermissionType));
                    //remove
                    alumniFeaturePermissions.ForEach(u => alumniFeaturePermissionManager.Delete(u));

                    statuschanged = true;
                }

                //transfer entity permisstions
                //var alumniEntityPermissions = alumniEntityPermissionManager.AlumniEntityPermissionRepository.Get(a => a.Subject.Id == user.Id).ToList();

                //if (alumniEntityPermissions.Count > 0)
                //{
                //    alumniEntityPermissions.ForEach(u => entityPermissionManager.Create(user, u.Entity, u.Key, u.Rights));
                //    //remove
                //    alumniEntityPermissions.ForEach(u => alumniEntityPermissionManager.Delete(u));

                //    statuschanged = true;
                //}
            }


            //add all group to user again
            using (var groupManager = new GroupManager())
            using (var alumniUsersGroupsRelationManager = new AlumniUsersGroupsRelationManager())
            {
                var relations = alumniUsersGroupsRelationManager.AlumniFeaturePermissions.Where(r => r.UserRef == user.Id).ToList();
                foreach (var r in relations)
                {
                    //add all group to user again
                    var group = groupManager.FindByIdAsync(r.GroupRef).Result;
                    group.Users.Add(user);
                    groupManager.UpdateAsync(group);

                    //delete relation
                    alumniUsersGroupsRelationManager.Delete(r);
                }

                //remove alumni group
                var alumniGroup = groupManager.Groups.Where(g => g.Name.ToLower() == "alumni").FirstOrDefault();
                alumniGroup.Users.Remove(user);
                groupManager.UpdateAsync(alumniGroup);

                statuschanged = true;
            }

            return statuschanged;
        }
            
     }

}
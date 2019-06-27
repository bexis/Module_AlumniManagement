using BEXIS.ALM.Entities.Alumni;
using BExIS.Security.Entities.Authorization;
using BExIS.Security.Entities.Objects;
using BExIS.Security.Entities.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Vaiona.Persistence.Api;

namespace BEXIS.ALM.Services.Alumni
{
    public class AlumniFeaturePermissionManager : IDisposable
    {
        private readonly IUnitOfWork _guow;
        private bool _isDisposed;

        public AlumniFeaturePermissionManager()
        {
            _guow = this.GetIsolatedUnitOfWork();
            AlumniFeaturePermissionRepository = _guow.GetReadOnlyRepository<AlumniFeaturePermission>();
        }

        ~AlumniFeaturePermissionManager()
        {
            Dispose(true);
        }

        public IReadOnlyRepository<AlumniFeaturePermission> AlumniFeaturePermissionRepository { get; }

        public IQueryable<AlumniFeaturePermission> AlumniFeaturePermissions => AlumniFeaturePermissionRepository.Query();

        public void Create(long? subjectId, long featureId, BExIS.Security.Entities.Authorization.PermissionType permissionType)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var featureRepository = uow.GetReadOnlyRepository<Feature>();
                var subjectRepository = uow.GetReadOnlyRepository<Subject>();

                if (Exists(subjectId, featureId, permissionType)) return;

                var alumniFeaturePermission = new AlumniFeaturePermission
                {
                    Feature = featureRepository.Get(featureId),
                    PermissionType = permissionType,
                    Subject = subjectId == null ? null : subjectRepository.Query(s => s.Id == subjectId).FirstOrDefault()
                };

                var alumniFeaturePermissionRepository = uow.GetRepository<AlumniFeaturePermission>();
                alumniFeaturePermissionRepository.Put(alumniFeaturePermission);
                uow.Commit();
            }
        }

        public void Create(Subject subject, Feature feature, PermissionType permissionType = PermissionType.Grant)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var subjectRepository = uow.GetReadOnlyRepository<Subject>();
                var featureRepository = uow.GetReadOnlyRepository<Feature>();

                var alumniFeaturePermission = new AlumniFeaturePermission()
                {
                    Subject = subjectRepository.Get(subject.Id),
                    Feature = featureRepository.Get(feature.Id),
                    PermissionType = permissionType
                };

                var alumniFeaturePermissionRepository = uow.GetRepository<AlumniFeaturePermission>();
                alumniFeaturePermissionRepository.Put(alumniFeaturePermission);
                uow.Commit();
            }
        }

        public void Delete(AlumniFeaturePermission alumniFeaturePermission)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var alumniFeaturePermissionRepository = uow.GetRepository<AlumniFeaturePermission>();
                alumniFeaturePermissionRepository.Delete(alumniFeaturePermission);
                uow.Commit();
            }
        }

        public void Delete(long? subjectId, long featureId)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var alumniFeaturePermission = Find(subjectId, featureId);

                if (alumniFeaturePermission == null) return;

                var alumnifeaturePermissionRepository = uow.GetRepository<AlumniFeaturePermission>();
                alumnifeaturePermissionRepository.Delete(alumniFeaturePermission);
                uow.Commit();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public bool Exists(Subject subject, Feature feature, PermissionType permissionType)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var alumniFeaturePermissionRepository = uow.GetReadOnlyRepository<AlumniFeaturePermission>();

                if (feature == null)
                    return false;

                if (subject == null)
                    return alumniFeaturePermissionRepository.Get(p => p.Subject == null && p.Feature.Id == feature.Id && p.PermissionType == permissionType).Count == 1;

                return alumniFeaturePermissionRepository.Get(p => p.Subject.Id == subject.Id && p.Feature.Id == feature.Id && p.PermissionType == permissionType).Count == 1;
            }
        }

        public bool Exists(long? subjectId, long featureId, PermissionType permissionType)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var alumniFeaturePermissionRepository = uow.GetReadOnlyRepository<AlumniFeaturePermission>();

                if (subjectId == null)
                    return alumniFeaturePermissionRepository.Get(p => p.Subject == null && p.Feature.Id == featureId && p.PermissionType == permissionType).Count == 1;
                return alumniFeaturePermissionRepository.Get(p => p.Subject.Id == subjectId && p.Feature.Id == featureId && p.PermissionType == permissionType).Count == 1;
            }
        }

        //public bool Exists(long? subjectId, long featureId)
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        var featurePermissionRepository = uow.GetReadOnlyRepository<FeaturePermission>();

        //        if (subjectId == null)
        //            return featurePermissionRepository.Get(p => p.Subject == null && p.Feature.Id == featureId).Count == 1;

        //        return featurePermissionRepository.Get(p => p.Subject.Id == subjectId && p.Feature.Id == featureId).Count == 1;
        //    }
        //}

        //public bool Exists(IEnumerable<long> subjectIds, IEnumerable<long> featureIds, PermissionType permissionType)
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        var featurePermissionRepository = uow.GetReadOnlyRepository<FeaturePermission>();
        //        return featurePermissionRepository.Query(p => featureIds.Contains(p.Feature.Id) && subjectIds.Contains(p.Subject.Id) && p.PermissionType == permissionType).Any();
        //    }
        //}

        public AlumniFeaturePermission Find(long? subjectId, long featureId)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var alumniFeaturePermissionRepository = uow.GetReadOnlyRepository<AlumniFeaturePermission>();
                return subjectId == null ? alumniFeaturePermissionRepository.Query(f => f.Subject == null && f.Feature.Id == featureId).FirstOrDefault() : alumniFeaturePermissionRepository.Query(f => f.Feature.Id == featureId && f.Subject.Id == subjectId).FirstOrDefault();
            }
        }

        public AlumniFeaturePermission Find(Subject subject, Feature feature)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var alumniFeaturePermissionRepository = uow.GetReadOnlyRepository<AlumniFeaturePermission>();
                return alumniFeaturePermissionRepository.Query(f => f.Feature.Id == feature.Id && f.Subject.Id == subject.Id).FirstOrDefault();
            }
        }

        public AlumniFeaturePermission FindById(long id)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var alumniFeaturePermissionRepository = uow.GetReadOnlyRepository<AlumniFeaturePermission>();
                return alumniFeaturePermissionRepository.Get(id);
            }
        }

        //public int GetPermissionType(long subjectId, long featureId)
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        var featurePermission = Find(subjectId, featureId);

        //        if (featurePermission != null)
        //        {
        //            return (int)featurePermission.PermissionType;
        //        }

        //        return 2;
        //    }
        //}

        //public bool HasAccess(long? subjectId, long featureId)
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        var featureRepository = uow.GetReadOnlyRepository<Feature>();
        //        var subjectRepository = uow.GetReadOnlyRepository<Subject>();

        //        var feature = featureRepository.Get(featureId);
        //        var subject = subjectId == null ? null : subjectRepository.Query(s => s.Id == subjectId).FirstOrDefault();

        //        // Anonymous
        //        if (subject == null)
        //        {
        //            while (feature != null)
        //            {
        //                if (Exists(null, feature.Id, PermissionType.Grant))
        //                    return true;

        //                feature = feature.Parent;
        //            }

        //            return false;
        //        }

        //        // Non-Anonymous
        //        while (feature != null)
        //        {
        //            if (Exists(null, feature.Id, PermissionType.Grant))
        //                return true;

        //            if (Exists(subject.Id, feature.Id, PermissionType.Deny))
        //                return false;

        //            if (Exists(subject.Id, feature.Id, PermissionType.Grant))
        //                return true;

        //            if (subject is User)
        //            {
        //                var user = subject as User;
        //                var groupIds = user.Groups.Select(g => g.Id).ToList();

        //                if (Exists(groupIds, new[] { feature.Id }, PermissionType.Deny))
        //                {
        //                    return false;
        //                }

        //                if (Exists(groupIds, new[] { feature.Id }, PermissionType.Grant))
        //                {
        //                    return true;
        //                }
        //            }

        //            feature = feature.Parent;
        //        }

        //        return false;
        //    }
        //}

        ////public bool HasAccess(Subject subject, Feature feature)
        ////{
        ////    using (var uow = this.GetUnitOfWork())
        ////    {
        ////        var featureRepository = uow.GetReadOnlyRepository<Feature>();
        ////        var subjectRepository = uow.GetReadOnlyRepository<Subject>();

        ////        // Anonymous
        ////        if (subject == null)
        ////        {
        ////            while (feature != null)
        ////            {
        ////                if (Exists(null, feature.Id, PermissionType.Grant))
        ////                    return true;

        ////                feature = feature.Parent;
        ////            }

        ////            return false;
        ////        }

        ////        // Non-Anonymous
        ////        while (feature != null)
        ////        {
        ////            if (Exists(null, feature.Id, PermissionType.Grant))
        ////                return true;

        ////            if (Exists(subject.Id, feature.Id, PermissionType.Deny))
        ////                return false;

        ////            if (Exists(subject.Id, feature.Id, PermissionType.Grant))
        ////                return true;

        ////            if (subject is User)
        ////            {
        ////                var user = subject as User;
        ////                var groupIds = user.Groups.Select(g => g.Id).ToList();

        ////                if (Exists(groupIds, new[] { feature.Id }, PermissionType.Deny))
        ////                {
        ////                    return false;
        ////                }

        ////                if (Exists(groupIds, new[] { feature.Id }, PermissionType.Grant))
        ////                {
        ////                    return true;
        ////                }
        ////            }

        ////            feature = feature.Parent;
        ////        }

        ////        return false;
        ////    }
        ////}

        //public bool HasAccess<T>(string subjectName, string module, string controller, string action) where T : Subject
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        var operationRepository = uow.GetReadOnlyRepository<Operation>();
        //        var SubjectRepository = uow.GetReadOnlyRepository<Subject>();

        //        var operation = operationRepository.Query(x => x.Module.ToUpperInvariant() == module.ToUpperInvariant() && x.Controller.ToUpperInvariant() == controller.ToUpperInvariant() && x.Action.ToUpperInvariant() == action.ToUpperInvariant()).FirstOrDefault();
        //        var feature = operation?.Feature;
        //        var subject = SubjectRepository.Query(s => s.Name.ToUpperInvariant() == subjectName.ToUpperInvariant() && s is T).FirstOrDefault();
        //        if (feature != null && subject != null)
        //            return HasAccess(subject.Id, feature.Id);

        //        return false;
        //    }
        //}

        public void Update(AlumniFeaturePermission entity)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var repo = uow.GetRepository<AlumniFeaturePermission>();
                repo.Merge(entity);
                var merged = repo.Get(entity.Id);
                repo.Put(merged);
                uow.Commit();
            }
        }

        protected void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (_guow != null)
                        _guow.Dispose();
                    _isDisposed = true;
                }
            }
        }
    }
}
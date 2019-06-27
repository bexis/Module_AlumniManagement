using BEXIS.ALM.Entities.Alumni;
using BExIS.Dlm.Entities.Party;
using BExIS.Security.Entities.Authorization;
using BExIS.Security.Entities.Objects;
using BExIS.Security.Entities.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Vaiona.Persistence.Api;

namespace BEXIS.ALM.Services.Alumni
{
    // Sven
    // UoW -> Done
    public class AlumniEntityPermissionManager : IDisposable
    {
        private readonly IUnitOfWork _guow;
        private bool _isDisposed;

        public AlumniEntityPermissionManager()
        {
            _guow = this.GetIsolatedUnitOfWork();
            AlumniEntityPermissionRepository = _guow.GetReadOnlyRepository<AlumniEntityPermission>();
        }

        ~AlumniEntityPermissionManager()
        {
            Dispose(true);
        }

        public IReadOnlyRepository<AlumniEntityPermission> AlumniEntityPermissionRepository { get; }
        public IQueryable<AlumniEntityPermission> EntityPermissions => AlumniEntityPermissionRepository.Query();

        public void Create(AlumniEntityPermission alumniEntityPermission)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var entityPermissionRepository = uow.GetRepository<AlumniEntityPermission>();
                entityPermissionRepository.Put(alumniEntityPermission);
                uow.Commit();
            }
        }

        public void Create(Subject subject, Entity entity, long key, int rights)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var alumniEntityPermission = new AlumniEntityPermission()
                {
                    Subject = subject,
                    Entity = entity,
                    Key = key,
                    Rights = rights
                };

                var alumniEntityPermissionRepository = uow.GetRepository<AlumniEntityPermission>();
                alumniEntityPermissionRepository.Put(alumniEntityPermission);
                uow.Commit();
            }
        }
     
        public void Delete(AlumniEntityPermission alumniEntityPermission)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var anlumniEntityPermissionRepository = uow.GetRepository<AlumniEntityPermission>();
                anlumniEntityPermissionRepository.Delete(alumniEntityPermission);
                uow.Commit();
            }
        }

        

        public void Delete(long alumniEntityPermissionId)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var alumniEntityPermissionRepository = uow.GetRepository<AlumniEntityPermission>();
                alumniEntityPermissionRepository.Delete(alumniEntityPermissionRepository.Get(alumniEntityPermissionId));
                uow.Commit();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public bool Exists(Subject subject, Entity entity, long key)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var alumniEntityPermissionRepository = uow.GetRepository<AlumniEntityPermission>();

                if (entity == null)
                    return false;

                if (subject == null)
                    return alumniEntityPermissionRepository.Get(p => p.Subject == null && p.Entity.Id == entity.Id && p.Key == key).Count == 1;

                return alumniEntityPermissionRepository.Get(p => p.Subject.Id == subject.Id && p.Entity.Id == entity.Id && p.Key == key).Count == 1;
            }
        }

        public AlumniEntityPermission Find(long? subjectId, long entityId, long instanceId)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var alumniEntityPermissionRepository = uow.GetRepository<AlumniEntityPermission>();
                return subjectId == null ? alumniEntityPermissionRepository.Get(p => p.Subject == null && p.Entity.Id == entityId && p.Key == instanceId).FirstOrDefault() : alumniEntityPermissionRepository.Get(p => p.Subject.Id == subjectId && p.Entity.Id == entityId && p.Key == instanceId).FirstOrDefault();
            }
        }

        public AlumniEntityPermission FindById(long alumniEntityPermissionId)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var alumniEntityPermissionRepository = uow.GetRepository<AlumniEntityPermission>();
                return alumniEntityPermissionRepository.Get(alumniEntityPermissionId);
            }
        }

        //public int GetEffectiveRights(long? subjectId, long entityId, long key)
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        var subjectRepository = uow.GetReadOnlyRepository<Subject>();
        //        var entityRepository = uow.GetReadOnlyRepository<Entity>();
        //        var entityPermissionRepository = uow.GetReadOnlyRepository<EntityPermission>();

        //        var subject = subjectId == null ? null : subjectRepository.Query(s => s.Id == subjectId).FirstOrDefault();

        //        if (entityRepository.Get(entityId) == null)
        //            return 0;

        //        var rights = new List<int>();

        //        rights.Add(entityPermissionRepository.Get(m => m.Subject == null && m.Entity.Id == entityId && m.Key == key).FirstOrDefault()?.Rights ?? 0);

        //        if (subject is User)
        //        {
        //            var partyUserRepository = uow.GetReadOnlyRepository<PartyUser>();

        //            var partyUser = partyUserRepository.Query(m => m.UserId == subject.Id).FirstOrDefault();

        //            if (partyUser != null)
        //            {
        //                var partyRepository = uow.GetReadOnlyRepository<Party>();
        //                var userParty = partyRepository.Get(partyUser.PartyId);

        //                var entityParty = partyRepository.Query(m => m.PartyType.Title == entityRepository.Get(entityId).Name && m.Name == key.ToString()).FirstOrDefault();

        //                if (userParty != null && entityParty != null)
        //                {
        //                    var partyRelationshipRepository = uow.GetReadOnlyRepository<PartyRelationship>();
        //                    var partyRelationships = partyRelationshipRepository.Query(m => m.SourceParty.Id == userParty.Id && m.TargetParty.Id == entityParty.Id);

        //                    rights.AddRange(partyRelationships.Select(m => m.Permission));
        //                }
        //            }

        //            var user = subject as User;
        //            var subjectIds = new List<long>() { user.Id };
        //            subjectIds.AddRange(user.Groups.Select(g => g.Id).ToList());
        //            rights.AddRange(entityPermissionRepository.Get(m => subjectIds.Contains(m.Subject.Id) && m.Entity.Id == entityId && m.Key == key).Select(e => e.Rights).ToList());
        //        }

        //        if (subject is Group)
        //        {
        //            rights.Add(entityPermissionRepository.Get(m => m.Subject.Id == subject.Id && m.Entity.Id == entityId && m.Key == key).FirstOrDefault()?.Rights ?? 0);
        //        }

        //        return rights.Aggregate(0, (left, right) => left | right);
        //    }
        //}

        //public IEnumerable<long> GetKeys<T>(string name, string v, Type type, RightType delete)
        //{
        //    throw new NotImplementedException();
        //}

        //// Entity Null
        //// User Null
        //public List<long> GetKeys(string userName, string entityName, Type entityType, RightType rightType)
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        if (entityType == null)
        //            return new List<long>();

        //        if (string.IsNullOrEmpty(entityName))
        //            return new List<long>();

        //        var entityRepository = uow.GetReadOnlyRepository<Entity>();

        //        var entity = entityRepository.Query(e => e.Name.ToUpperInvariant() == entityName.ToUpperInvariant() && e.EntityType == entityType).FirstOrDefault();
        //        if (entity == null)
        //            return new List<long>();

        //        var entityPermissionRepository = uow.GetReadOnlyRepository<EntityPermission>();
        //        var userRepository = uow.GetReadOnlyRepository<User>();

        //        var user = userRepository.Query(s => s.Name.ToUpperInvariant() == userName.ToUpperInvariant()).FirstOrDefault();
        //        if (user == null)
        //            return entityPermissionRepository
        //                .Query(e => e.Subject == null && e.Entity.Id == entity.Id).AsEnumerable()
        //                .Where(e => (e.Rights & (int)rightType) > 0)
        //                .Select(e => e.Key)
        //                .ToList();

        //        var subjectIds = new List<long>() { user.Id };
        //        subjectIds.AddRange(user.Groups.Select(g => g.Id).ToList());

        //        return
        //            entityPermissionRepository
        //                .Query(e => (subjectIds.Contains(e.Subject.Id) || e.Subject == null) && e.Entity.Id == entity.Id).AsEnumerable()
        //                .Where(e => (e.Rights & (int)rightType) > 0)
        //                .Select(e => e.Key)
        //                .ToList();
        //    }
        //}

        //public List<long> GetKeys(long? subjectId, long entityId, RightType rightType)
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        var entityPermissionRepository = uow.GetReadOnlyRepository<EntityPermission>();

        //        if (subjectId == null)
        //            return entityPermissionRepository.Query(e =>
        //                e.Subject == null &&
        //                e.Entity.Id == entityId &&
        //                (e.Rights & (int)rightType) > 0
        //                )
        //            .Select(e => e.Key)
        //            .ToList();

        //        return entityPermissionRepository.Query(e =>
        //            e.Subject.Id == subjectId &&
        //            e.Entity.Id == entityId &&
        //            (e.Rights & (int)rightType) > 0
        //            )
        //        .Select(e => e.Key)
        //        .ToList();
        //    }
        //}

        //public int GetRights(Subject subject, Entity entity, long key)
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        var entityPermissionRepository = uow.GetReadOnlyRepository<EntityPermission>();

        //        if (subject == null)
        //        {
        //            return entityPermissionRepository.Get(m => m.Subject == null && m.Entity.Id == entity.Id && m.Key == key).FirstOrDefault()?.Rights ?? 0;
        //        }
        //        if (entity == null)
        //            return 0;
        //        return entityPermissionRepository.Get(m => m.Subject.Id == subject.Id && m.Entity.Id == entity.Id && m.Key == key).FirstOrDefault()?.Rights ?? 0;
        //    }
        //}

        //public int GetRights(long? subjectId, long entityId, long key)
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        var subjectRepository = uow.GetReadOnlyRepository<Subject>();
        //        var entityRepository = uow.GetReadOnlyRepository<Entity>();

        //        var subject = subjectId == null ? null : subjectRepository.Query(s => s.Id == subjectId).FirstOrDefault();
        //        var entity = entityRepository.Get(entityId);
        //        return GetRights(subject, entity, key);
        //    }
        //}

        //public bool HasEffectiveRight(Subject subject, Entity entity, long key, RightType rightType)
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        var entityPermissionRepository = uow.GetReadOnlyRepository<EntityPermission>();

        //        if (subject == null)
        //        {
        //            return (entityPermissionRepository.Get(m => m.Subject == null && m.Entity.Id == entity.Id && m.Key == key).FirstOrDefault()?.Rights & (int)rightType) > 0;
        //        }
        //        if (entity == null)
        //            return false;

        //        return (GetEffectiveRights(subject.Id, entity.Id, key) & (int)rightType) > 0;
        //    }
        //}

        //public bool HasEffectiveRight(string username, string entityName, Type entityType, long key, RightType rightType)
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        var userRepository = uow.GetReadOnlyRepository<User>();
        //        var entityRepository = uow.GetReadOnlyRepository<Entity>();

        //        var user = userRepository.Query(s => s.Name.ToUpperInvariant() == username.ToUpperInvariant()).FirstOrDefault();
        //        var entity = entityRepository.Query(e => e.Name.ToUpperInvariant() == entityName.ToUpperInvariant() && e.EntityType == entityType).FirstOrDefault();

        //        return entity != null && HasEffectiveRight(user?.Id, entity.Id, key, rightType);
        //    }
        //}

        //public bool HasEffectiveRight(long? subjectId, long entityId, long key, RightType rightType)
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        return (GetEffectiveRights(subjectId, entityId, key) & (int)rightType) > 0;
        //    }
        //}

        //public bool HasRight<T>(string subjectName, string entityName, Type entityType, long key, RightType rightType) where T : Subject
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        var subjectRepository = uow.GetReadOnlyRepository<Subject>();
        //        var entityRepository = uow.GetReadOnlyRepository<Entity>();

        //        var subject = subjectRepository.Query(s => s.Name.ToUpperInvariant() == subjectName.ToUpperInvariant() && s is T).FirstOrDefault();
        //        var entity = entityRepository.Query(e => e.Name.ToUpperInvariant() == entityName.ToUpperInvariant() && e.EntityType == entityType).FirstOrDefault();

        //        return (GetRights(subject, entity, key) & (int)rightType) > 0;
        //    }
        //}

        //public bool HasRight(long? subjectId, long? entityId, long key, RightType rightType)
        //{
        //    using (var uow = this.GetUnitOfWork())
        //    {
        //        var entityPermissionRepository = uow.GetRepository<EntityPermission>();

        //        if (entityId == null)
        //            return false;

        //        return subjectId == null ? (entityPermissionRepository.Get(p => p.Subject == null && p.Entity.Id == entityId && p.Key == key).FirstOrDefault()?.Rights & (int)rightType) > 0 : (entityPermissionRepository.Get(p => (p.Subject.Id == subjectId || p.Subject == null) && p.Entity.Id == entityId && p.Key == key).FirstOrDefault()?.Rights & (int)rightType) > 0;
        //    }
        //}

        public void Update(AlumniEntityPermission entity)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var repo = uow.GetRepository<AlumniEntityPermission>();
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
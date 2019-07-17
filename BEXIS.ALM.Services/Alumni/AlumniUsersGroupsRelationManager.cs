using BExIS.Security.Entities.Subjects;
using BEXIS.ALM.Entities.Alumni;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vaiona.Persistence.Api;

namespace BEXIS.ALM.Services.Alumni
{
    public class AlumniUsersGroupsRelationManager : IDisposable
    {
        private readonly IUnitOfWork _guow;
        private bool _isDisposed;

        public IReadOnlyRepository<AlumniUsersGroupsRelation> AlumniUsersGroupsRelationRepository { get; }

        public IQueryable<AlumniUsersGroupsRelation> AlumniFeaturePermissions => AlumniUsersGroupsRelationRepository.Query();

        public AlumniUsersGroupsRelationManager()
        {
            _guow = this.GetIsolatedUnitOfWork();
            AlumniUsersGroupsRelationRepository = _guow.GetReadOnlyRepository<AlumniUsersGroupsRelation>();
        }

        ~AlumniUsersGroupsRelationManager()
        {
            Dispose(true);
        }


        public void Create(long userRefId, long groupRefId)
        {
            using (var uow = this.GetUnitOfWork())
            {
                
                var alumniUsersGroupsRelation = new AlumniUsersGroupsRelation()
                {
                    UserRef = userRefId,
                    GroupRef = groupRefId
                };

                var alumniUsersGroupsRelationRepository = uow.GetRepository<AlumniUsersGroupsRelation>();
                alumniUsersGroupsRelationRepository.Put(alumniUsersGroupsRelation);
                uow.Commit();
            }
        }

        public void Delete(AlumniUsersGroupsRelation alumniUsersGroupsRelation)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var alumniUsersGroupsRelationRepository = uow.GetRepository<AlumniUsersGroupsRelation>();
                alumniUsersGroupsRelationRepository.Delete(alumniUsersGroupsRelation);
                uow.Commit();
            }
        }


        public void Dispose()
        {
            Dispose(true);
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

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

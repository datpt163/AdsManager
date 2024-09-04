using FBAdsManager.Common.Database.Data;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Abstractions;
using System.Data;
using System.Reflection.Emit;
using System.Security;

namespace FBAdsManager.Common.Database.Repository
{
    public interface IUnitOfWork
    {
        public IRepository<Branch> Branchs { get; }
        public IRepository<Employee> Employees { get; }
        public IRepository<Group> Groups { get; }
        public IRepository<Organization> Organizations { get; }
        public IRepository<Pm> Pms { get; }
        public IRepository<Role> Roles { get; }
        public IRepository<User> Users { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        Task<int> SaveChangesAsync();

        Task Dispose(bool disposing);
    }
}

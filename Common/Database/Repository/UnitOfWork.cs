using FBAdsManager.Common.Database.Data;
using FBAdsManager.Common.Database.DbContexts;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection.Emit;
using System.Security;

namespace FBAdsManager.Common.Database.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbAdsmanagerContext _context;
        private bool disposed = false;
        public UnitOfWork(DbAdsmanagerContext context)
        {
            _context = context;
        }
        public IRepository<Branch> branchs = null!;
        public IRepository<Employee> employees = null!;
        public IRepository<Group> groups = null!;
        public IRepository<Organization> organizations = null!;
        public IRepository<Role> roles = null!;
        public IRepository<User> users = null!;
        public IRepository<Insight> insights = null!;
        public IRepository<Ads> adses = null!;
        public IRepository<Adset> adsets = null!;
        public IRepository<Campaign> campaigns = null!;
        public IRepository<AdsAccount> adsAccounts = null!;

        public IRepository<Branch> Branchs => branchs ?? new Repository<Branch>(_context);
        public IRepository<Employee> Employees => employees ?? new Repository<Employee>(_context);
        public IRepository<Group> Groups => groups ?? new Repository<Group>(_context);
        public IRepository<Organization> Organizations => organizations ?? new Repository<Organization>(_context);
        public IRepository<Role> Roles => roles ?? new Repository<Role>(_context);
        public IRepository<User> Users => users ?? new Repository<User>(_context);
        public IRepository<Insight> Insights => insights ?? new Repository<Insight>(_context);
        public IRepository<Ads> Adses => adses ?? new Repository<Ads>(_context);
        public IRepository<Adset> Adsets => adsets ?? new Repository<Adset>(_context);
        public IRepository<Campaign> Campaigns => campaigns ?? new Repository<Campaign>(_context);
        public IRepository<AdsAccount> AdsAccounts => adsAccounts ?? new Repository<AdsAccount>(_context);



        public int SaveChanges()
        {
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        //public Task BulkSaveChangesAsync()
        //{
        //    return _context.BulkSaveChangesAsync();
        //}

        public IRepository<T> AsyncRepository<T>() where T : class
        {
            return new Repository<T>(_context);
        }

        public async Task Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    await _context.DisposeAsync();
                }
            }
            disposed = true;
        }
    }
}

using NoteBook.DataService.IConfiguration;
using NoteBook.DataService.IRepository;
using NoteBook.DataService.Repository;


namespace NoteBook.DataService.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _dbContext;

        public IUserRepository Users { get; private set; }

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;

            Users = new UserRepository(dbContext);
        }
        public async Task CompleteAsync()
        {
           await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
           _dbContext.Dispose();
        }
    }
}

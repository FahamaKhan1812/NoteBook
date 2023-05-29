using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NoteBook.DataService.Data;
using NoteBook.DataService.IRepository;
using NoteBook.Entities.DbSet;


namespace NoteBook.DataService.Repository
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _dbSet
                    .Where(x => x.Status == 1)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new List<User>();
            }
        }
    }
}

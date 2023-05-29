using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.DataService.IRepository
{
    // All CRUD operations

    public interface IGenericRepository<T> where T : class 
    {
        // Get all entities
        Task<IEnumerable<T>> GetAllAsync();
        
        // Get all entity By Id
        Task<T> GetByIdAsync(Guid id);

        // Create new Entity
        Task<bool> Add(T entity);
        
        /// <summary>
        ///  Delete an existing entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> Delete(Guid id, string  userId);
        
        // Edit/Update
        Task<bool> Upsert(T entity);
    }
}

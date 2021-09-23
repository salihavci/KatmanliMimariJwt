using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KatmanliMimariJwt.Core.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity:class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        //Where içerisindeki parametre bir delegate'dir TEntity verip Bool döndürecek
        IQueryable<TEntity> Where(Expression<Func<TEntity,bool>> predicate);
        Task AddAsync(TEntity entity);
        void Remove(TEntity entity);
        TEntity Update(TEntity entity);
    }
}

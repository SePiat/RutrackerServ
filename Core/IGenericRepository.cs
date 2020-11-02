using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        T GetById(object id);
        Task<T> GetByIdAsync(object id);
        void Add(T obj);
        void AddRange(IEnumerable<T> objects);
        Task AddAsync(T obj);
        Task AddRangeAsync(IEnumerable<T> objects);
        void Update(T obj);
        void Delete(object id);
        void Remove(IEnumerable<T> range);
        List<T> ToList();
        Task<List<T>> ToListAsync();
        IEnumerable<T> Where1(Func<T, bool> predicate);
        IEnumerable<T> Where(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        IEnumerable<T> Skip(int count);
        T FirstOrDefault(IEnumerable<T> obj);
        T FirstOrDefault_(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        IQueryable<T> AsQueryable();
        IEnumerable<T> OrderBy(Func<T, string> predicate);
        IEnumerable<T> OrderByDescending(Func<T, string> predicate);
        IEnumerable<T> Take(int count);
        bool Contains(T obj);
        Task<bool> ContainsAsync(T obj);
        int Count();
        Task<int> CountAsync();
        IQueryable<T> Include(string predicate);

    }

}

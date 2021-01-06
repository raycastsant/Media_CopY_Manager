using System.Collections.Generic;

namespace MCP.db
{
    public interface IRepository<T> //where T : IEntity
    {
        List<T> List { get; }
        T Add(T entity);
        void Delete(int Id);
        void Update(T entity);
        T FindById(int Id);
    }
}

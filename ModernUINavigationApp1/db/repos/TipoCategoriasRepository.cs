using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;

namespace MCP.db
{
    public class TipoCategoriasRepository : IRepository<tipo_categorias>
    {
        public TipoCategoriasRepository()
        {
        }

        public List<tipo_categorias> List
        {
            get
            {
                return DBManager.Context.tipo_categorias.OrderBy(e => e.nombre).ToList();
            }
        }
        public tipo_categorias Add(tipo_categorias entity)
        {
            tipo_categorias cp = DBManager.Context.tipo_categorias.Add(entity);
            DBManager.Context.SaveChanges();

            return cp;
        }
        public void Delete(int Id)
        {
            DBManager.Context.tipo_categorias.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public tipo_categorias FindById(int id)
        {
            tipo_categorias result = (from r in DBManager.Context.tipo_categorias
                          where r.id == id
                          select r).FirstOrDefault();
            return result;
        }

        public void Update(tipo_categorias entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }
    }
}

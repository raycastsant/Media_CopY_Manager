using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;

namespace MCP.db
{
    public class TrabajadoresRepository : IRepository<trabajadore>
    {
        public TrabajadoresRepository()
        {
        }

        public List<trabajadore> List
        {
            get
            {
                return DBManager.Context.trabajadores.OrderBy(c => c.nombre_apell).ToList();
            }
        }
        public trabajadore Add(trabajadore entity)
        {
            trabajadore c = DBManager.Context.trabajadores.Add(entity);
            DBManager.Context.SaveChanges();

            return c;
        }
        public void Delete(int Id)
        {
            DBManager.Context.trabajadores.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public trabajadore FindById(int id)
        {
            trabajadore result = (from r in DBManager.Context.trabajadores
                                  where r.id == id
                                  select r).FirstOrDefault();
            return result;
        }

        public trabajadore FindByName(string name, int notid)
        {
            trabajadore result = (from r in DBManager.Context.trabajadores
                                  where r.nombre_apell == name && r.id != notid
                                  select r).FirstOrDefault();
            return result;
        }

        public void Update(trabajadore entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }
    }
}

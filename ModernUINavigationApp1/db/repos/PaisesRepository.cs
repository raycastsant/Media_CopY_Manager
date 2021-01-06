using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;

namespace MCP.db
{
    public class PaisesRepository : IRepository<pais>
    {
        //private media_managerEntities _context;

        public PaisesRepository()
        {
           // _context = new media_managerEntities();
        }

        public List<pais> List
        {
            get
            {
                return DBManager.Context.paises.OrderBy(e => e.nombre).ToList();
            }
        }
        public pais Add(pais entity)
        {
            pais p = DBManager.Context.paises.Add(entity);
            DBManager.Context.SaveChanges();

            return p;
        }
        public void Delete(int Id)
        {
            DBManager.Context.paises.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public pais FindById(int id)
        {
            pais result = (from r in DBManager.Context.paises
                           where r.id == id
                          select r).FirstOrDefault();
            return result;
        }

        public pais FindByName(string name, int notid)
        {
            pais result = (from r in DBManager.Context.paises
                                   where r.nombre == name && r.id != notid
                                   select r).FirstOrDefault();
            return result;
        }

        public void Update(pais entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }
    }
}

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;

namespace MCP.db
{
    public class GenerosRepository : IRepository<genero>
    {
        //private media_managerEntities _context;

        public GenerosRepository()
        {
            //_context = new media_managerEntities();
        }

        public List<genero> List
        {
            get
            {
                return DBManager.Context.generos.OrderBy(e => e.nombre).ToList();
            }
        }
        public genero Add(genero entity)
        {
            genero g = DBManager.Context.generos.Add(entity);
            DBManager.Context.SaveChanges();

            return g;
        }
        public void Delete(int Id)
        {
            DBManager.Context.generos.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public genero FindById(int id)
        {
            genero result = (from r in DBManager.Context.generos
                             where r.id == id
                          select r).FirstOrDefault();
            return result;
        }

        public genero FindByName(string name, int notid)
        {
            genero result = (from r in DBManager.Context.generos
                           where r.nombre == name && r.id != notid
                           select r).FirstOrDefault();
            return result;
        }

        public void Update(genero entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }
    }
}

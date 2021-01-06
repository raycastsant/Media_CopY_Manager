using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;

namespace MCP.db
{
    public class PuntoCopiasRepository : IRepository<copia_puntos>
    {
        //private media_managerEntities _context;

        public PuntoCopiasRepository()
        {
           // _context = new media_managerEntities();
        }

        public List<copia_puntos> List
        {
            get
            {
                return DBManager.Context.copia_puntos.
                    Where(e => e.inactivo==false).
                    OrderBy(e => e.nombre).
                    ToList();
            }
        }

        public List<copia_puntos> ListForUser(usuario user)
        {
            user.copia_puntos.OrderBy(e => e.nombre).ToList();
            return DBManager.Context.copia_puntos.OrderBy(e => e.nombre).ToList();
        }
        public copia_puntos Add(copia_puntos entity)
        {
            copia_puntos cp = DBManager.Context.copia_puntos.Add(entity);
            DBManager.Context.SaveChanges();

            return cp;
        }
        public void Delete(int Id)
        {
            DBManager.Context.copia_puntos.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public copia_puntos FindById(int id)
        {
            copia_puntos result = (from r in DBManager.Context.copia_puntos
                          where r.id == id
                          select r).FirstOrDefault();
            return result;
        }

        public copia_puntos FindByName(string name, int notid)
        {
            copia_puntos result = (from r in DBManager.Context.copia_puntos
                              where r.nombre == name && r.id != notid
                              select r).FirstOrDefault();
            return result;
        }
        public void Update(copia_puntos entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }
    }
}

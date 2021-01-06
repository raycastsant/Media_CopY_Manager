using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;

namespace MCP.db
{
    public class TipoPagosRepository : IRepository<tipos_pago>
    {
        public TipoPagosRepository()
        {
        }

        public List<tipos_pago> List
        {
            get
            {
                return DBManager.Context.tipos_pago.OrderBy(e => e.nombre).ToList();
            }
        }
        public tipos_pago Add(tipos_pago entity)
        {
            tipos_pago cp = DBManager.Context.tipos_pago.Add(entity);
            DBManager.Context.SaveChanges();

            return cp;
        }
        public void Delete(int Id)
        {
            DBManager.Context.tipos_pago.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public tipos_pago FindById(int id)
        {
            tipos_pago result = (from r in DBManager.Context.tipos_pago
                          where r.id == id
                          select r).FirstOrDefault();
            return result;
        }

        public tipos_pago FindByCode(string cod)
        {
            tipos_pago result = (from r in DBManager.Context.tipos_pago
                                 where r.codigo == cod
                                 select r).FirstOrDefault();
            return result;
        }

        public void Update(tipos_pago entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }
    }
}

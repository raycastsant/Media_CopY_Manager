using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;

namespace MCP.db
{
    public class CoeficientePagosRepository : IRepository<coeficientes_pago>
    {
        public CoeficientePagosRepository()
        {
        }

        public List<coeficientes_pago> List
        {
            get
            {
                return DBManager.Context.coeficientes_pago.Where(c => c.tipos_pago.codigo == "TA").ToList();
            }
        }

        public coeficientes_pago Add(coeficientes_pago c)
        {
            coeficientes_pago r = DBManager.Context.coeficientes_pago.Add(c);
            DBManager.Context.SaveChanges();

            return r;
        }

        public void Delete(int Id)
        {
            DBManager.Context.coeficientes_pago.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public coeficientes_pago FindById(int id)
        {
            coeficientes_pago result = (from r in DBManager.Context.coeficientes_pago
                                        where r.id == id
                                        select r).FirstOrDefault();
            return result;
        }

        public coeficientes_pago FindCosto(int tipoCategId, int tipoPagoId)
        {
            coeficientes_pago result = (from r in DBManager.Context.coeficientes_pago
                                        where r.tipo_categoria_id == tipoCategId && r.tipo_pago_id == tipoPagoId
                                        select r).FirstOrDefault();
            return result;
        }

        public void Update(coeficientes_pago entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }
    }
}

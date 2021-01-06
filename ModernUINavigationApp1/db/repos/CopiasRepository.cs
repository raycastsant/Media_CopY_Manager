using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;

namespace MCP.db
{
    public class CopiasRepository : IRepository<copia>
    {
        public CopiasRepository()
        {
            //_context = new media_managerEntities();
        }

        public List<copia> List
        {
            get
            {
                return DBManager.Context.copias.OrderBy(c => c.codigo).ToList();
            }
        }

        public List<registro_copias> ListRegistros(int copyId)
        {
            return DBManager.Context.registro_copias.Where(c => c.copia_id == copyId).ToList();
        }

        public copia Add(copia entity)
        {
            try {
                copia c = DBManager.Context.copias.Add(entity);
                DBManager.Context.SaveChanges();

                if(c != null)
                {
                    preferencia pref = this.getSeriePref();
                    pref.valor_int++;

                    DBManager.Context.Entry(pref).State = EntityState.Modified;
                    DBManager.Context.SaveChanges();
                }

                return c;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public void Delete(int Id)
        {
            DBManager.Context.copias.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public copia FindById(int id)
        {
            copia result = (from r in DBManager.Context.copias
                          where r.id == id
                          select r).FirstOrDefault();

            return result;
        }

        public void Update(copia entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }

        private preferencia getSeriePref()
        {
            preferencia pref = (from p in DBManager.Context.preferencias
                                where p.nombre == "copia_serie"
                                select p).FirstOrDefault();

            return pref;
        }

        public int NextSerie()
        {
            preferencia pref = this.getSeriePref();

            if (pref != null && pref.valor_int != null)
                return (int)pref.valor_int;
            else
                return -1;
        }
    }
}

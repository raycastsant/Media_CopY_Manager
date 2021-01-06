using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;
using MySql.Data.MySqlClient;
using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace MCP.db
{
    public class RegistroCopiasRepository : IRepository<registro_copias>
    {
        public RegistroCopiasRepository()
        {
            
        }
        public List<registro_copias> List
        {
            get
            {
                return DBManager.Context.registro_copias.OrderByDescending(e => e.fecha).ToList();
            }
        }
        public registro_copias Add(registro_copias entity)
        {
           registro_copias g = DBManager.Context.registro_copias.Add(entity);
           DBManager.Context.SaveChangesAsync();

            return g;
        }

        public void Insert(registro_copias rc, MySqlConnection conn)
        {
            try
            {
                MySqlCommand sql = new MySqlCommand();
                sql.Connection = conn;

                sql.CommandText = "insert into registro_copias(copia_id, fecha, archivo_url, destino_url, peso, " +
                    "titulo, nombre_categoria, media_file_id) values(" +
                                rc.copia_id + ", '" +
                                rc.fecha.ToString("yyyy-MM-dd HH:mm:ss") + "', '" +
                                rc.archivo_url.Replace("\\", "\\\\") + "', '" +
                                rc.destino_url.Replace("\\", "\\\\") + "', " +
                                rc.peso + ", '" +
                                rc.titulo + "', '" +
                                rc.nombre_categoria + "', " +
                                rc.media_file_id + ")";
                sql.ExecuteNonQuery();
            }
            catch(MySql.Data.MySqlClient.MySqlException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Delete(int Id)
        {
            DBManager.Context.registro_copias.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public registro_copias FindById(int id)
        {
            registro_copias result = (from r in DBManager.Context.registro_copias
                             where r.id == id
                          select r).FirstOrDefault();
            return result;
        }

        public List<string> ListDistinctCategValues()
        {
            return (from r in DBManager.Context.registro_copias
                    select r.nombre_categoria).Distinct().ToList();
        }

        public List<registro_copias> Find(DateTime fdesde, DateTime fhasta, int puntoCopiaId = -1, string categoria = "", int userId = -1)
        {
            var query = (from r in DBManager.Context.registro_copias
                         select r);

            if (fdesde != null)
                query = query.Where(r => r.fecha.CompareTo(fdesde) >= 0);

            if (fdesde != null)
                query = query.Where(r => r.fecha.CompareTo(fhasta) <= 0);

            if (puntoCopiaId > 0)
                query = query.Where(r => r.copia.punto_copia_id == puntoCopiaId);

            if (categoria != null && !string.IsNullOrEmpty(categoria))
                query = query.Where(r => r.nombre_categoria == categoria);

            if (userId > 0)
                query = query.Where(r => r.copia.user_id == userId);

            return query.OrderByDescending(r => r.fecha).ToList();
        }

        public Task<List<registro_copias>> FindAsync(DateTime fdesde, DateTime fhasta, int puntoCopiaId = -1, string categoria = "", int userId = -1)
        {
            var query = (from r in DBManager.Context.registro_copias
                         select r);

            if (fdesde != null)
                query = query.Where(r => r.fecha.CompareTo(fdesde) >= 0);

            if (fdesde != null)
                query = query.Where(r => r.fecha.CompareTo(fhasta) <= 0);

            if (puntoCopiaId > 0)
                query = query.Where(r => r.copia.punto_copia_id == puntoCopiaId);

            if (categoria != null && !string.IsNullOrEmpty(categoria))
                query = query.Where(r => r.nombre_categoria == categoria);

            if (userId > 0)
                query = query.Where(r => r.copia.user_id == userId);

            return query.OrderByDescending(r => r.fecha).ToListAsync();
        }

        public void Update(registro_copias entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;

namespace MCP.db
{
    public class MediaGenerosRepository : IRepository<media_generos>
    {
       // private media_managerEntities _context;

        public MediaGenerosRepository()
        {
           // _context = new media_managerEntities();
        }

        public List<media_generos> List
        {
            get
            {
                return DBManager.Context.media_generos.OrderBy(e => e.genero.nombre).ToList();
            }
        }
        public media_generos Add(media_generos entity)
        {
            media_generos m = DBManager.Context.media_generos.Add(entity);
            DBManager.Context.SaveChanges();

            return m;
        }
        public void RefreshList(List<media_generos> list, int mediaId)
        {
            string sql = "delete from media_generos where media_id=@media_id";
            object[] parameters = new object[1];
            parameters[0] = mediaId;
            DBManager.Context.media_generos.SqlQuery(sql, mediaId);

            foreach (media_generos mg in list)
            {
                DBManager.Context.media_generos.Add(mg);
            }
            
            DBManager.Context.SaveChanges();
        }

        public void Delete(int Id)
        {
            DBManager.Context.media_generos.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public media_generos FindById(int id)
        {
            media_generos result = (from mf in DBManager.Context.media_generos
                                  where mf.id == id
                          select mf).FirstOrDefault();
            return result;
        }

        public void Update(media_generos entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();

            Console.WriteLine(DBManager.Context.Entry(entity).State);
        }
    }
}

using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

namespace MCP.db
{
    public class CategoriasRepository : IRepository<categoria>
    {
        //private media_managerEntities _context;

        public CategoriasRepository()
        {
            //_context = new media_managerEntities();
        }

        public List<categoria> List
        {
            get
            {
                return DBManager.Context.categorias.OrderBy(c => c.categoria1).ToList();
            }
        }
        public Task< List<categoria> > ListAsync
        {
            get
            {
                return DBManager.Context.categorias.OrderBy(c => c.categoria1).ToListAsync();
            }
        }

        public categoria Add(categoria entity)
        {
            categoria c = DBManager.Context.categorias.Add(entity);
            DBManager.Context.SaveChanges();

            return c;
        }
        public void Delete(int Id)
        {
            DBManager.Context.categorias.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public categoria FindById(int id)
        {
            categoria result = (from r in DBManager.Context.categorias
                          where r.id == id
                          select r).FirstOrDefault();
            return result;
        }

        public categoria FindByName(string name, int notid)
        {
            categoria result = (from r in DBManager.Context.categorias
                                  where r.categoria1 == name && r.id != notid
                                  select r).FirstOrDefault();
            return result;
        }

        public void Update(categoria entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }

       /* public List<media_files> ListFirstMedias(int categId)
        {
            if(categId > 0)
            {
                categoria c = FindById(categId);
                if (c != null)
                    return ListFirstMedias(c.media_files);
            }

            return new List<media_files>();
        }*/

        public List<media_files> ListFirstMedias(ICollection<media_files> collection, bool foldersOnly)
        {
            IEnumerable<media_files> query;

            if (foldersOnly)
            {
                query = from mf in collection
                        where (mf.parent_id <= 0 && mf.is_folder == true)
                        orderby mf.titulo
                        select mf;
            }
            else
            {
                query = from mf in collection
                        where (mf.parent_id <= 0)
                        orderby mf.is_folder descending, mf.titulo
                        select mf;
            }
            

            return query.ToList();
        }
    }
}

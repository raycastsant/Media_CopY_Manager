using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MCP.db
{
    public class MediaFilesRepository : IRepository<media_files>
    {
        public MediaFilesRepository()
        {
        }

        public List<media_files> List
        {
            get
            {
                return DBManager.Context.media_files.OrderBy(e => e.titulo).ToList();
            }
        }
        public media_files Add(media_files entity)
        {
            /*  if (i == 40)
                  i = 40;*/

           // i++;
            //Console.WriteLine(i + "  :  tit- "+entity.titulo.Length+"   url-"+entity.file_url.Length);

            media_files m = DBManager.Context.media_files.Add(entity);
            DBManager.Context.SaveChanges();

            return m;
        }

        public media_files Add_WithoutSaveContext(media_files entity)
        {
            media_files m = DBManager.Context.media_files.Add(entity);
            return m;
        }

        public void Delete(int Id)
        {
            DBManager.Context.media_files.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public void DeleteEntity(media_files mf)
        {
           // DbContextTransaction transaction = DBManager.Context.Database.BeginTransaction();
            try
            {
                if (mf.is_folder)
                    deleteChildrens(mf.categoria_id, mf.id);

                DBManager.Context.media_files.Remove(mf);

               // transaction.Commit();
                DBManager.Context.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBoxButton btn = MessageBoxButton.OK;
               // transaction.Rollback();
                ModernDialog.ShowMessage("Ocurrió un error al realizar la operación." + "\n" + e.Message, "Error", btn);
            }

           // DBManager.Context.SaveChanges();
        }

        private void deleteChildrens(int categId, int parentId)
        {
            List<media_files> list = FindByCategoria(categId, parentId, false);
            foreach(media_files mf in list)
            {
                if(mf.is_folder)
                    deleteChildrens(categId, mf.id);

                DBManager.Context.media_files.Remove(mf);
            }
        }

        public media_files FindById(int id)
        {
            media_files result = (from mf in DBManager.Context.media_files
                                  where mf.id == id
                          select mf).FirstOrDefault();

            return result;
        }

        public async Task<media_files> FindByIdAsync(int id)
        {
            return await (from mf in DBManager.Context.media_files
                                  where mf.id == id
                                  select mf).FirstOrDefaultAsync();

            //return result;
        }

        public bool Exists(string fileURL)
        {
            media_files result = (from mf in DBManager.Context.media_files
                                  where mf.file_url == fileURL
                                  select mf).FirstOrDefault();

            return result != null;
        }

        public media_files FindByUrl(string fileURL)
        {
            media_files result = (from mf in DBManager.Context.media_files
                                  where mf.file_url == fileURL
                                  select mf).FirstOrDefault();

            return result;
        }

        public List<media_files> FindByCategoria(int categId, int parentId, bool foldersOnly)
        {
            IEnumerable<media_files> query = from mf in DBManager.Context.media_files
                                             where (mf.parent_id == parentId && mf.categoria_id == categId)
                                             orderby mf.is_folder descending, mf.titulo
                                             select mf;

            if(foldersOnly)
                query = query.Where(mf => mf.is_folder == true);

            return query.ToList();
        }

        public async Task<List<media_files>> FindByCategoriaAsync(int categId, int parentId, bool foldersOnly)
        {
            IQueryable<media_files> query = from mf in DBManager.Context.media_files
                                             where (mf.parent_id == parentId && mf.categoria_id == categId)
                                             orderby mf.is_folder descending, mf.titulo
                                             select mf;
            if (foldersOnly)
                query = query.Where(mf => mf.is_folder == true);

            return await query.ToListAsync();
        }

        public List<int?> ListMediaYears()
        {
            var query = (from mf in DBManager.Context.media_files orderby mf.anno ascending select mf.anno).Distinct().ToList();
           // List<int?> list = DBManager.Context.media_files.Select(mf=>mf.anno).Distinct().OrderBy.ToList();
            //List<int?> list = DBManager.Context.media_files.Select(mf => mf.anno).Distinct().ToList();
           
            return query.ToList();
        }

       /* public List<media_files> AplyFilter(int categId, string filter)
        {
            string f = filter.ToLower();
            IEnumerable<media_files> query = (from mf in DBManager.Context.media_files
                                             join mg in DBManager.Context.media_generos on mf.id equals mg.media_id 
                                             into join1 from mg in join1.DefaultIfEmpty() //LEFT JOIN
                                             join gen in DBManager.Context.generos on mg.genero_id equals gen.id 
                                             into join11 from GEN in join11.DefaultIfEmpty()
                                             join mp in DBManager.Context.media_paises on mf.id equals mp.media_fi_id 
                                             into join2 from mp in join2.DefaultIfEmpty()  //LEFT JOIN
                                             join pais in DBManager.Context.paises on mp.pais_id equals pais.id 
                                             into join22 from PAIS in join22.DefaultIfEmpty()
                                             where (    mf.titulo.ToLower().Contains(f) ||
                                                        mf.premios.ToLower().Contains(f) ||
                                                        mf.anno.ToString().Contains(f) ||
                                                        mf.sinopsis.ToLower().Contains(f) ||
                                                        mf.reparto.ToLower().Contains(f) ||
                                                        mf.duracion.ToString().Contains(f) ||
                                                        mf.director.ToLower().Contains(f) ||
                                                        mf.productora.ToLower().Contains(f)  ||
                                                        GEN.nombre.ToLower().Contains(f) ||
                                                        PAIS.nombre.ToLower().Contains(f)
                                                    ) && (mf.categoria_id==categId)
                                             orderby mf.titulo
                                             select mf).Distinct();

            return query.ToList();
        }*/

        public async Task<List<media_files>> AplyFilterAsync(int categId, string filter, int year, int generoId)
        {
            string f = filter.ToLower();

            var query = (from mf in DBManager.Context.media_files
                       //  join mg in DBManager.Context.media_generos on mf.id equals mg.media_id
                       //  into join1
                        // from mg in join1.DefaultIfEmpty() //LEFT JOIN
                         //join gen in DBManager.Context.generos on mg.genero_id equals gen.id
                        // into join11
                        // from GEN in join11.DefaultIfEmpty()
                         join mp in DBManager.Context.media_paises on mf.id equals mp.media_fi_id
                         into join2
                         from mp in join2.DefaultIfEmpty()  //LEFT JOIN
                         join pais in DBManager.Context.paises on mp.pais_id equals pais.id
                         into join22
                         from PAIS in join22.DefaultIfEmpty()
                         where (mf.titulo.ToLower().Contains(f) ||
                                 mf.premios.ToLower().Contains(f) ||
                                 mf.anno.ToString().Contains(f) ||
                                 mf.sinopsis.ToLower().Contains(f) ||
                                 mf.reparto.ToLower().Contains(f) ||
                                // mf.duracion.ToString().Contains(f) ||
                                 mf.director.ToLower().Contains(f) ||
                                 mf.productora.ToLower().Contains(f) ||
                                // GEN.nombre.ToLower().Contains(f) ||
                                 PAIS.nombre.ToLower().Contains(f)
                             )
                         orderby mf.titulo
                         select mf);

           /* if (generoId > 0)
            {
                query = (from mf in DBManager.Context.media_files
                         join mg in DBManager.Context.media_generos on mf.id equals mg.media_id
                         into join1
                         from mg in join1.DefaultIfEmpty() //LEFT JOIN
                         join gen in DBManager.Context.generos on mg.genero_id equals gen.id
                         into join11
                         from GEN in join11.DefaultIfEmpty()
                         join mp in DBManager.Context.media_paises on mf.id equals mp.media_fi_id
                         into join2
                         from mp in join2.DefaultIfEmpty()  //LEFT JOIN
                         join pais in DBManager.Context.paises on mp.pais_id equals pais.id
                         into join22
                         from PAIS in join22.DefaultIfEmpty()
                         where (mf.titulo.ToLower().Contains(f) ||
                                 mf.premios.ToLower().Contains(f) ||
                                 mf.anno.ToString().Contains(f) ||
                                 mf.sinopsis.ToLower().Contains(f) ||
                                 mf.reparto.ToLower().Contains(f) ||
                                 mf.duracion.ToString().Contains(f) ||
                                 mf.director.ToLower().Contains(f) ||
                                 mf.productora.ToLower().Contains(f) ||
                               //  GEN.nombre.ToLower().Contains(f) ||
                                 PAIS.nombre.ToLower().Contains(f)
                             ) && (GEN.id == generoId)
                         orderby mf.titulo
                         select mf);
            }*/

            if(categId > 0)
                query = query.Where(m => m.categoria_id == categId);

            if (year > 0)
                query = query.Where(m => m.anno == year);

            if (generoId > 0)
                query = query.Where(m => m.media_generos.Any(g => g.genero_id == generoId));

            query = query.Distinct().OrderByDescending(m => m.is_folder).ThenBy(m => m.titulo);

            /* var query = (from mf in DBManager.Context.media_files
                             join mg in DBManager.Context.media_generos on mf.id equals mg.media_id
                             into join1
                             from mg in join1.DefaultIfEmpty() //LEFT JOIN
                             join gen in DBManager.Context.generos on mg.genero_id equals gen.id
                             into join11
                             from GEN in join11.DefaultIfEmpty()
                             join mp in DBManager.Context.media_paises on mf.id equals mp.media_fi_id
                             into join2
                             from mp in join2.DefaultIfEmpty()  //LEFT JOIN
                             join pais in DBManager.Context.paises on mp.pais_id equals pais.id
                             into join22
                             from PAIS in join22.DefaultIfEmpty()
                             where (mf.titulo.ToLower().Contains(f) ||
                                     mf.premios.ToLower().Contains(f) ||
                                     mf.anno.ToString().Contains(f) ||
                                     mf.sinopsis.ToLower().Contains(f) ||
                                     mf.reparto.ToLower().Contains(f) ||
                                     mf.duracion.ToString().Contains(f) ||
                                     mf.director.ToLower().Contains(f) ||
                                     mf.productora.ToLower().Contains(f) ||
                                     GEN.nombre.ToLower().Contains(f) ||
                                     PAIS.nombre.ToLower().Contains(f)
                                 ) && (mf.categoria_id == categId)
                             orderby mf.titulo
                             select mf).
                                         Distinct().
                                         OrderByDescending(m => m.is_folder).ThenBy(m => m.titulo);*/

            return await query.ToListAsync();
        }

      /*  public async Task<List<media_files>> AplyFilterAsync(string filter)
        {
            string f = filter.ToLower();
            var query = (from mf in DBManager.Context.media_files
                         join mg in DBManager.Context.media_generos on mf.id equals mg.media_id
                         into join1
                         from mg in join1.DefaultIfEmpty() //LEFT JOIN
                         join gen in DBManager.Context.generos on mg.genero_id equals gen.id
                         into join11
                         from GEN in join11.DefaultIfEmpty()
                         join mp in DBManager.Context.media_paises on mf.id equals mp.media_fi_id
                         into join2
                         from mp in join2.DefaultIfEmpty()  //LEFT JOIN
                         join pais in DBManager.Context.paises on mp.pais_id equals pais.id
                         into join22
                         from PAIS in join22.DefaultIfEmpty()
                         where (mf.titulo.ToLower().Contains(f) ||
                                    mf.premios.ToLower().Contains(f) ||
                                    mf.anno.ToString().Contains(f) ||
                                    mf.sinopsis.ToLower().Contains(f) ||
                                    mf.reparto.ToLower().Contains(f) ||
                                    mf.duracion.ToString().Contains(f) ||
                                    mf.director.ToLower().Contains(f) ||
                                    mf.productora.ToLower().Contains(f) ||
                                    GEN.nombre.ToLower().Contains(f) ||
                                    PAIS.nombre.ToLower().Contains(f)
                                )
                         orderby mf.titulo
                         select mf).
                                     Distinct().
                                     OrderByDescending(m => m.is_folder).ThenBy(m => m.categoria.categoria1).ThenBy(m => m.titulo);

            return await query.ToListAsync();
        }*/

        public void Update(media_files entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }

        public void Update(media_files entity, List<media_generos> generosList, List<media_paises> paisesList, bool useTransaction)
        {
            /* var mgList = entity.media_generos.ToArray();
             foreach (media_generos mg in mgList)
             {
                 entity.media_generos.Remove(mg);
             }*/

            /* string sql = "delete from media_generos where media_id=" + entity.id;
             object[] parameters = new object[0];
             DBManager.Context.Database.ExecuteSqlCommand(sql, parameters);*/
            if (useTransaction)
            {
                DbContextTransaction transaction = DBManager.Context.Database.BeginTransaction();
                try
                {
                    UpdateEntity(entity, generosList, paisesList);

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    MessageBoxButton btn = MessageBoxButton.OK;
                    transaction.Rollback();
                    ModernDialog.ShowMessage("Ocurrió un error al realizar la operación." + "\n" + e.Message, "Error", btn);
                }
            }
            else
            {
                UpdateEntity(entity, generosList, paisesList);
            }
            
          //  DBManager.Context.Entry(entity).State = EntityState.Modified;
            //DBManager.Context.Entry(entity).Reload();
            //DBManager.Context.Entry(entity).State = EntityState.Detached;
        }

        private void UpdateEntity(media_files entity, List<media_generos> generosList, List<media_paises> paisesList)
        {
            entity.media_generos.ToList().ForEach(e => DBManager.Context.Entry(e).State = EntityState.Deleted);
            entity.media_paises.ToList().ForEach(e => DBManager.Context.Entry(e).State = EntityState.Deleted);

            foreach (media_generos mg in generosList)
            {
                entity.media_generos.Add(DBManager.Context.media_generos.Add(mg));
            }

            foreach (media_paises mg in paisesList)
            {
                entity.media_paises.Add(DBManager.Context.media_paises.Add(mg));
            }

            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }
    }
}

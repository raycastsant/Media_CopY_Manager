using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

namespace MCP.db
{
    public class UsuariosRepository : IRepository<usuario>
    {
        public UsuariosRepository()
        {
        }

        public List<usuario> List
        {
            get
            {
                return DBManager.Context.usuarios.
                    Where(e => e.eliminado == false).
                    OrderBy(e => e.nombre).
                    ToList();
            }
        }

        public List<usuario> ListUnused
        {
            get
            {
                return DBManager.Context.usuarios.
                    Where(e => e.eliminado == false && e.trabajadores.Count() == 0 && !e.is_admin).
                    OrderBy(e => e.nombre).
                    ToList();
            }
        }

        public usuario Add(usuario user)
        {
            usuario u = DBManager.Context.usuarios.Add(user);
            DBManager.Context.SaveChanges();

            return u;
        }
        public void Delete(int Id)
        {
            DBManager.Context.usuarios.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public usuario FindById(int id)
        {
            usuario result = (from r in DBManager.Context.usuarios
                          where r.id == id
                          select r).FirstOrDefault();
            return result;
        }

        public usuario FindByName(string name)
        {
            usuario result = (from r in DBManager.Context.usuarios
                              where r.nombre == name
                              select r).FirstOrDefault();
            return result;
        }

        public usuario FindByName(string name, int notid)
        {
            usuario result = (from r in DBManager.Context.usuarios
                              where r.nombre == name && r.id != notid
                              select r).FirstOrDefault();
            return result;
        }

        public Task<usuario> LoginUser(string name, string pass)
        {
            /* MD5 md5 = MD5.Create();
             byte[] encoded = new UTF8Encoding().GetBytes(pass);
             byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encoded);
             string encodedPass = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();*/

            string encodedPass = AppMAnager.getMD5(pass);

            var result = (from r in DBManager.Context.usuarios
                              where r.nombre == name && r.pass == encodedPass
                              select r).FirstOrDefaultAsync();
            return result;
        }

        public void Update(usuario entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }
    }
}

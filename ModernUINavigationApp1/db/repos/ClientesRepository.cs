using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCP.db.repos
{
    public class ClientesRepository : IRepository<cliente>
    {

        public ClientesRepository()
        {

        }

        public List<cliente> List
        {
            get
            {
                return DBManager.Context.clientes.OrderBy(c => c.id_cliente).ToList();
            }
        }

        public cliente Add(cliente entity)
        {
            cliente cliente = DBManager.Context.clientes.Add(entity);
            DBManager.Context.SaveChanges();

            return cliente;
        }

        public void Delete(int Id)
        {
            DBManager.Context.clientes.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public cliente FindById(int Id)
        {
            cliente cliente = (from c in DBManager.Context.clientes
                               where c.id_cliente == Id
                               select c).FirstOrDefault();
            return cliente;

        }

        public void Update(cliente entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }

        public cliente FindByPhone(string phone)
        {
            cliente cliente = (from c in DBManager.Context.clientes
                               where c.telefono == phone
                               select c).FirstOrDefault();
            return cliente;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCP.db.repos
{
    public class UsbRepository : IRepository<usb>
    {
        public UsbRepository()
        {

        }

        public List<usb> List
        {
            get
            {
                return DBManager.Context.usbs.OrderBy(c => c.id_usb).ToList();
            }
        }
      

        public usb Add(usb entity)
        {
            usb usb = DBManager.Context.usbs.Add(entity);
            DBManager.Context.SaveChanges();

            return usb;
        }

        public void Delete(int Id)
        {
            DBManager.Context.usbs.Remove(FindById(Id));
            DBManager.Context.SaveChanges();
        }

        public usb FindById(int Id)
        {
            usb usb = (from c in DBManager.Context.usbs
                               where c.id_usb == Id
                               select c).FirstOrDefault();
            return usb;
        }
        public usb FindBySerial(string serial)
        {
            usb usb = (from c in DBManager.Context.usbs
                       where c.numero_serie == serial
                       select c).FirstOrDefault();
            return usb;
        }



        public void Update(usb entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }
    }
}

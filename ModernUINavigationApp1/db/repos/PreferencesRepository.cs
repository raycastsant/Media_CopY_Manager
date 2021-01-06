using System.Data;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

namespace MCP.db
{
    public class PreferencesRepository
    {
        public static string CONTENT_CHANGED_KEY = "CONTENT_CHANGED";

        public PreferencesRepository()
        {
        }

        public async Task<preferencia> FindContentChanged()
        {
            preferencia result = await (from r in DBManager.Context.preferencias
                                  where r.nombre == CONTENT_CHANGED_KEY
                                  select r).FirstOrDefaultAsync();
            return result;
        }

        public void SetContentUnchangedRegistry()
        {
            preferencia pref = (from r in DBManager.Context.preferencias
                                where r.nombre == CONTENT_CHANGED_KEY
                                select r).FirstOrDefault();
            if (pref != null)
            {
                pref.valor_int = 0;
                Update(pref);
            }
        }

        public void Update(preferencia entity)
        {
            DBManager.Context.Entry(entity).State = EntityState.Modified;
            DBManager.Context.SaveChanges();
        }
    }
}

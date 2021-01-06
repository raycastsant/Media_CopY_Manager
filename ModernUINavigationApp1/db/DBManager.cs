using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MCP.db
{
    public class DBManager
    {
        //private static int Copys_In_Progress = 0;
        //private static DbContextTransaction transaction = null;

        public static media_managerEntities Context;// = new media_managerEntities();

        public static MediaFilesRepository MediaFilesRepo = new MediaFilesRepository();
        public static MediaGenerosRepository MediaGenerosRepo = new MediaGenerosRepository();
        public static CategoriasRepository CategoriasRepo = new CategoriasRepository();
        public static TipoCategoriasRepository TipoCategoriasRepo = new TipoCategoriasRepository();
        public static RegistroCopiasRepository RegistroCopiasRepo = new RegistroCopiasRepository();
        public static UsuariosRepository UsuariosRepo = new UsuariosRepository();
        public static PuntoCopiasRepository PuntoCopyRepo = new PuntoCopiasRepository();
        public static CoeficientePagosRepository CoeficientesRepo = new CoeficientePagosRepository();
        public static TrabajadoresRepository TrabajadoresRepo = new TrabajadoresRepository();
        public static CopiasRepository CopiasRepo = new CopiasRepository();
        public static TipoPagosRepository TipoPagosRepo = new TipoPagosRepository();
        public static PreferencesRepository PreferenciasRepo = new PreferencesRepository();

        public static bool SavingState = false;

        public static void Reset_Context()
        {
            //Context.Dispose();
            Context = new media_managerEntities();
        }

        /*  private static DbContextTransaction GetTransaction()
          {
             if(transaction == null)
                 transaction = Context.Database.BeginTransaction();

              return transaction;
          }

         public static void RollBackTransaction()
         {
             if (transaction != null)
             {
                 transaction.Rollback();
                 transaction = null;
                 Copys_In_Progress = 0;
             }
         }

          public static void StartCopyProgress()
          {
              GetTransaction();
              Copys_In_Progress++;
          }

          public static void EndCopyProgress()
          {
             Copys_In_Progress--;
             if(Copys_In_Progress == 0)
             {
                 if(transaction != null)
                     transaction.Commit();
             }
          }*/
    }
}

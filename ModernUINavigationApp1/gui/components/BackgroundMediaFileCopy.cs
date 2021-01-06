using Backup2;
using MCP.db;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MCP.gui.components
{
    class BackgroundMediaFileCopy : ICopyController
    {
        // private ProgressBar pBar;
        // private TextBlock labelState;
        // private TextBlock labelPercentage;
        // private TextBlock labelFilePercent;
        private string destinationPath;
        private ProgressInfo pinfo;
        private List<MediaFile_Basic_Info> mf_list;
        //private Dictionary<string, registro_copias> registro_copias_list; 
        private int dbCopyId;

        /// <summary>
        /// Bit flags that are used in UpdateUI method to synchronize the UI.
        /// </summary>
        [Flags]
        enum SyncronizationOperations : short
        {
            SetText = 0x1,
            SetStatus = 0x2,
            SetProgress = 0x4,
            CheckBox = 0x8,
            UpdateList = 0x10
        }

        #region Private Data Member
        private BackupCreator copyier;
       // private bool completed;
        #endregion

        public BackgroundMediaFileCopy(int copyId)
        {
            this.dbCopyId = copyId;

            copyier = new BackupCreator(this.dbCopyId);
            copyier.CopyFlags ^= CopyFileFlags.FileFailIfExists;  //Sobreescribir

            //copyier.CopyCanceledEvent += new EventHandler(copyier_CopyCanceled);
            copyier.CopyCompletedEvent += new EventHandler<BackupEventArgs>(copyier_CopyCompleted);
            copyier.CopyProgressEvent += new EventHandler<BackupEventArgs>(copyier_CopyProgressEvent);
            copyier.FileCopyStartEvent += new EventHandler<FileEventArgs>(copyier_FileCopyStart);
            copyier.FileCopyProgressEvent += new EventHandler<FileProgressEventArgs>(copyier_FileCopyProgressEvent);
            copyier.StatusChangedEvent += new EventHandler<StatusChangedEventArgs>(copyier_StatusChangedEvent);
            copyier.CountCompletedEvent += new EventHandler<BackupEventArgs>(copyier_CountCompleted);
            //copyier.FileCopyUnsucessfulEvent += new EventHandler<FileCopyUnsuccessfulEventArgs>(copyier_FileCopyUnsucessfull);
        }

        public void StartCopyWorker(List<MediaFile_Basic_Info> mf_list, string destinationPath, ProgressInfo pinfo)
        {
            if (Directory.Exists(destinationPath))
            {
                this.destinationPath = destinationPath;

                if (pinfo != null)
                {
                    this.pinfo = pinfo;

                    //this.pBar = pBar;
                    this.pinfo._ProgressBar.Maximum = 100; // this.pBar.Maximum = 100;
                    this.pinfo._ProgressBar.Value = 0;   //this.pBar.Value = 0;
                    this.pinfo._lPercentage.Text = "0%";
                }

               /* if (labelState != null)
                {
                    this.labelState = labelState;
                }

                if (labelPercentage != null)
                {
                    this.labelPercentage = labelPercentage;
                    this.labelPercentage.Text = "0%";
                }

                if (labelFilePercent != null)
                {
                    this.labelFilePercent = labelFilePercent;
                }*/

                this.mf_list = mf_list;
                
                RegisterFiles(this.mf_list);
                copyier.Destination = destinationPath;

                new Thread(() => {

                    copyier.StartCopy();
                    //copyier.Paused = false;

                }).Start();
            }
        }

        private void RegisterFiles(List<MediaFile_Basic_Info> list) 
        {
            foreach (MediaFile_Basic_Info mf in list)
            {
                copyier.Sources.Add(mf);
            }
        }

        private void copyier_FileCopyStart(object sender, FileEventArgs e)
        {
            this.pinfo._lProgressState.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                (Action)(() =>
                {
                    this.pinfo._lProgressState.Text = "Copiando: " + e.DestinationFilePath;
                }));
        }

        private void copyier_CopyProgressEvent(object sender, BackupEventArgs e)
        {
            this.pinfo._ProgressBar.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                (Action)(() =>
                {
                    this.pinfo._ProgressBar.Value = (int)e.Progress;
                }));

            this.pinfo._lPercentage.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                (Action)(() =>
                {
                    this.pinfo._lPercentage.Text = ((int)e.Progress).ToString()+"%";
                }));
        }

        private void copyier_CountCompleted(object sender, BackupEventArgs e)
        {
            if (copyier.CurrentStatus != CopyStatus.Copying)
                copyier.Paused = false;
        }

        private void copyier_StatusChangedEvent(object sender, StatusChangedEventArgs e)
        {
            Console.WriteLine("copyier_StatusChangedEvent "+e.Status+"  --- > "+copyier.CurrentStatus);
        }

        private void copyier_FileCopyProgressEvent(object sender, FileProgressEventArgs e)
        {
            this.pinfo._lFilePercent.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                (Action)(() =>
                {
                    this.pinfo._lFilePercent.Text = "("+((int)e.Progress).ToString() + "%)";
                }));
        }

        private void copyier_CopyCompleted(object sender, EventArgs e)
        {
            AppMAnager.RUNNING_COPYS_COUNT--;

            if (copyier.UncopiedFiles.Count == 0)
            {
                this.pinfo._lProgressState.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        (Action)(() =>
                        {
                            this.pinfo._lProgressState.Text = "Finalizando...";
                        }));
                try
                {
                    string connstring = DBManager.Context.Database.Connection.ConnectionString;
                    //string connstring = "server=localhost;userid=root;password=;database=media_manager";
                    MySqlConnection conn = new MySqlConnection(connstring);
                    conn.Open();

                    //registro_copias_list = new Dictionary<string, registro_copias>();
                   // RegisterDbCopys(mf_list, destinationPath);

                    foreach (registro_copias RC in copyier.RCList)
                    {
                        DBManager.RegistroCopiasRepo.Insert(RC, conn);
                    }

                    this.pinfo.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           this.pinfo.SetCompleted(this.dbCopyId);
                       }));
                    
                    conn.Close();

                    this.pinfo._lProgressState.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                        (Action)(() =>
                        {
                            this.pinfo._lProgressState.Text = "Operación completada correctamente";
                        }));
                }
                catch (MySqlException exc)
                {
                    Console.WriteLine(exc.Message);

                    this.pinfo._lProgressState.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           this.pinfo._lProgressState.Text = "Operación completada. Ocurrieron errores!!!";
                       }));
                }
            }
            else
            {
                this.pinfo._lProgressState.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                       (Action)(() =>
                       {
                           this.pinfo._lProgressState.Text = "Operación no completada. Faltaron archivos por copiar";
                       }));
            }
        }

        public void Pause()
        {
            copyier.Paused = true;
        }

        public void Continue()
        {
            copyier.Paused = false;
        }

        public void Quit()
        {
            if(copyier.CurrentStatus != CopyStatus.CopyCompleted)
                copyier.AbortCopy();
        }
    }
}


using MCP.db;
using MCP.gui.components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Backup2
{
    public class BackupCreator : IDisposable 
    {
        #region Enums

        private enum CopyProgressResult : uint
        {
            ProgressContinue = 0,
            ProgressCancel = 1,
            ProgressStop = 2,
            ProgressQuiet = 3
        }

        private enum CopyProgressCallbackReason : uint
        {
            CallbackChunkedFinished = 0x00000000,
            CallbackStreamSwitch = 0x00000001
        }
        #endregion

        #region Delegates

        /// <summary>
        /// CopyProgressRoutine gives the progress of the file currently being copied
        /// </summary>
        /// <param name="TotalFileSize">Total size of the file in bytes</param>
        /// <param name="TotalBytesTransferred">The size in bytes that has been copied</param>
        /// <param name="StreamSize">Total Size in bytes of the current stream that is being copied</param>
        /// <param name="StreamBytesTransferred">The size in bytes of the current stream that has been copied</param>
        /// <param name="dwStreamNumber"></param>
        /// <param name="dwCallbackReason"></param>
        /// <param name="hSourceFile">Pointer to the Source File</param>
        /// <param name="hDestinationFile">Pointer to the Destination File</param>
        /// <param name="lpData"></param>
        /// <returns>Returns the progress status of the file being copied</returns>
        private delegate CopyProgressResult CopyProgressRoutine(Int64 TotalFileSize, Int64 TotalBytesTransferred, Int64 StreamSize,
            Int64 StreamBytesTransferred, UInt32 StreamNumber, CopyProgressCallbackReason CallbackReason, IntPtr SourceFile,
            IntPtr DestinationFile, IntPtr Data);
        #endregion

        #region Events
        public event EventHandler CountStartedEvent;
        public event EventHandler<BackupEventArgs> CountCompletedEvent;
        //public event EventHandler CountCanceledEvent;
        //public event EventHandler<PausedResumedEventArgs> CountPausedEvent;

        public event EventHandler CopyStartedEvent;
        public event EventHandler<BackupEventArgs> CopyCompletedEvent;
        public event EventHandler CopyCanceledEvent;
        public event EventHandler<PausedResumedEventArgs> CopyPausedEvent;
        public event EventHandler<BackupEventArgs> CopyProgressEvent;

        public event EventHandler<FileEventArgs> FileCopyStartEvent;
        public event EventHandler<FileEventArgs> FileCopyCompletedEvent;
        public event EventHandler<FileProgressEventArgs> FileCopyProgressEvent;
        public event EventHandler<FileCopyUnsuccessfulEventArgs> FileCopyUnsucessfulEvent;

        public event EventHandler<StatusChangedEventArgs> StatusChangedEvent;
        //public event EventHandler<BackupEventArgs> SourceChangedEvent;
        #endregion

        #region Private Data Members
        /// <summary>
        /// Contains the list of file that are to be copied.
        /// </summary>
        private Dictionary<string, string> files;
        /// <summary>
        /// Contains the list of files that were not successfully copied.
        /// </summary>
        private List<UncopiedFilesStructure> unCopiedFiles;
        /// <summary>
        /// Contains the list of sources, i.e., the source paths that are to be copied
        /// </summary>
        //private List<string> sources;
        private List<MediaFile_Basic_Info> sources;
        private List<registro_copias> RC_LIST;
        private int dbCopyId;
        /// <summary>
        /// The destination where the files are to be copied
        /// </summary>
        private string destination;
        /// <summary>
        /// The parant directory of the sources. 
        /// </summary>
        //private string sourceDirectory;
        /// <summary>
        /// Thread for copies the files
        /// </summary>
        private Thread copyThread;
        /// <summary>
        /// controls the working of copyThread
        /// </summary>
        private ManualResetEvent resetEvent;
        /// <summary>
        /// Indicates that the copying, if in progress, must be canceled
        /// </summary>
        private Boolean cancelCopy;
        /// <summary>
        /// Represents the total bytes that is to be copied.
        /// </summary>
        private double totalSize;
        /// <summary>
        /// Represents the total bytes that has been copied
        /// </summary>
        private double copiedSize;
        /// <summary>
        /// Sotres the number of bytes that were copied prior to the the current iteration of CopyProgressRoutineHandler
        /// </summary>
        private double previousCopiedSize;
        /// <summary>
        /// Indicates weather the copy is paused or not.
        /// </summary>
        private bool paused;
        /// <summary>
        /// skips the the current file
        /// </summary>
        private bool skip;
        #endregion

        #region Properties

        #region Property Paused
        /// <summary>
        /// Wrapper for paused
        /// </summary>
        public bool Paused
        {
            get { return paused; }
            set
            {
                paused = value;

                if (copyThread != null)
                {
                    if (paused)
                    {
                        resetEvent.Reset();
                    }
                    else
                    {
                        resetEvent.Set();
                    }
                }

                RaiseCopyPausedEvent(paused);
            }
        }
        #endregion

        #region Property CurrentStatus
        /// <summary>
        /// Gets Current status of the Backup
        /// </summary>
        public CopyStatus CurrentStatus { get; private set; }
        #endregion

        #region Property CurrentFile
        public string CurrentFile { get; private set; }
        #endregion

        #region Property UncopiedFiles
        public ICollection<UncopiedFilesStructure> UncopiedFiles
        {
            get
            {
                if (this.CurrentStatus != CopyStatus.CopyCompleted)
                    throw new InvalidOperationException("operation is not complete.");

                return unCopiedFiles;
            }
        }
        #endregion

        #region Property CopyFlags
        public CopyFileFlags CopyFlags { get; set; }
        #endregion

        #region Property TotalFiles
        public long TotalFiles
        {
            get { return files.Count; }            
        }
        #endregion

        #region Property TotalSize
        public double TotalSize { get { return totalSize; } }
        #endregion

        /*  public ICollection<string> Sources
          {
              get
              {
                  if (this.CurrentStatus == CopyStatus.Copying || this.CurrentStatus == CopyStatus.Counting)
                      return new ReadOnlyCollection<string>(this.sources);
                  return this.sources;
              }
          }*/

        public ICollection<MediaFile_Basic_Info> Sources
        {
            get
            {
                if (this.CurrentStatus == CopyStatus.Copying || this.CurrentStatus == CopyStatus.Counting)
                    return new ReadOnlyCollection<MediaFile_Basic_Info>(this.sources);
                return this.sources;
            }
        }

        public ICollection<registro_copias> RCList
        {
            get
            {
                if (this.CurrentStatus == CopyStatus.Copying || this.CurrentStatus == CopyStatus.Counting)
                    return new ReadOnlyCollection<registro_copias>(this.RC_LIST);
                return this.RC_LIST;
            }
        }

        public string Destination
        {
            get { return destination; }
            set
            {
                if (this.CurrentStatus == CopyStatus.Copying || this.CurrentStatus == CopyStatus.Counting)
                    throw new InvalidOperationException("Cannot Change Destination while under process.");

                if (value[value.Length - 1] != Path.DirectorySeparatorChar)
                    value = value + Path.DirectorySeparatorChar;

                destination = value;
            }
        }

        #endregion

        #region Public Fucntions

        public BackupCreator(int dbCopyId)
        {
            this.dbCopyId = dbCopyId;
            Init();
        }

        /// <summary>
        /// Initialises the Backup
        /// </summary>
        /// <param name="source">The source that is to be copied</param>
        /// <param name="destination">The destination where the source is to be copied</param>
        /*public BackupCreator(string source, string destination)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException("source");
            }
            if (string.IsNullOrEmpty(destination))
            {
                throw new ArgumentNullException("destination");
            }

            Init();

            this.Sources.Add(source);
            this.Destination = destination;
        }*/

        /// <summary>
        /// Initialise the basic settings of Backup
        /// </summary>
        public void Init()
        {
            files = new Dictionary<string, string>();   //files = new Dictionary<string, double>();
            unCopiedFiles = new List<UncopiedFilesStructure>();
            sources = new List<MediaFile_Basic_Info>();
            RC_LIST = new List<registro_copias>();

            this.CopyFlags = CopyFileFlags.FileFailIfExists;
            this.resetEvent = new ManualResetEvent(false);

            this.CopyPausedEvent += new EventHandler<PausedResumedEventArgs>(BackupCreator_CopyPausedEvent);
            this.CountCompletedEvent += new EventHandler<BackupEventArgs>(BackupCreator_CountCompletedEvent);
            
        }
        /// <summary>
        /// Start the Counting of files specified by sources
        /// </summary>
        public void StartCount()
        {
            if (this.CurrentStatus != CopyStatus.None)
                return;

            if (Sources.Count == 0)
                throw new InvalidOperationException("No Source available");

            if (string.IsNullOrEmpty(this.Destination))
                throw new InvalidOperationException("No Destination available");

            ThreadPool.QueueUserWorkItem(new WaitCallback(StartCountFiles));            
        }
        /// <summary>
        /// Start copying of the files listed in files
        /// </summary>
        public void StartCopy()
        {
            if (this.CurrentStatus != CopyStatus.CountCompleted)
                StartCount();

            //System.Threading.AutoResetEvent a = new AutoResetEvent(true);
            copyThread = new Thread(new ThreadStart(CopyFiles));
            copyThread.Name = "Copy Files";
            copyThread.IsBackground = false;
            copyThread.Start();
            copyThread.Join(1);
            Thread.Sleep(1);
        }
        /// <summary>
        /// Cancel the copy progress
        /// </summary>
        public void AbortCopy()
        {
            this.skip = true;
            this.cancelCopy = true;
            if (this.copyThread != null && this.copyThread.ThreadState == ThreadState.Suspended)
            {                
                resetEvent.Set();
            }
        }
        /// <summary>
        /// If some files were not copied, try copying them again.
        /// </summary>
        public void RetryCopy()
        {
            if (CurrentStatus != CopyStatus.CopyCompleted)
                throw new InvalidOperationException("No files to copy");

            this.files.Clear();
            totalSize = 0;
            copiedSize = 0;
            this.CurrentStatus = CopyStatus.Counting;
            this.RaiseStatusChangedEvent();
            foreach (var item in unCopiedFiles)
            {
                this.files.Add(item.Path, item.Size.ToString());
                totalSize += item.Size;
            }
            unCopiedFiles.Clear();
            this.CurrentFile = "";
            this.CurrentStatus = CopyStatus.CountCompleted;
            this.RaiseStatusChangedEvent();
            StartCopy();
        }
        /// <summary>
        /// Skip the currently copying file and resume from the netxt file.
        /// </summary>
        public void SkipCurrentFile()
        {
            this.skip = true;
        }

        #endregion

        #region Private Functions
        /// <summary>
        /// CopyProgressRoutineHandler gives the progress of the file currently being copied
        /// </summary>
        /// <param name="TotalFileSize">Total size of the file in bytes</param>
        /// <param name="TotalBytesTransferred">The size in bytes that has been copied</param>
        /// <param name="StreamSize">Total Size in bytes of the current stream that is being copied</param>
        /// <param name="StreamBytesTransferred">The size in bytes that has been copied</param>
        /// <param name="dwStreamNumber"></param>
        /// <param name="dwCallbackReason"></param>
        /// <param name="hSourceFile">Pointer to the Source File</param>
        /// <param name="hDestinationFile">Pointer to the Destination File</param>
        /// <param name="lpData"></param>
        /// <returns>Returns the progress status of the file being copied</returns>
        private CopyProgressResult CopyProgressRoutineHandler(Int64 TotalFileSize, Int64 TotalBytesTransferred, Int64 StreamSize,
            Int64 StreamBytesTransferred, UInt32 StreamNumber, CopyProgressCallbackReason CallbackReason, IntPtr SourceFile,
            IntPtr DestinationFile, IntPtr Data)
        {
            resetEvent.WaitOne();
            switch (CallbackReason)
            {
                case CopyProgressCallbackReason.CallbackChunkedFinished:
                    this.copiedSize += TotalBytesTransferred - previousCopiedSize;
                    RaiseFileCopyProgressEvent(this.CurrentFile, TotalBytesTransferred, TotalFileSize);
                    this.RaiseCopyProgressEvent();
                    previousCopiedSize = TotalBytesTransferred;
                    break;
                case CopyProgressCallbackReason.CallbackStreamSwitch:
                    break;
                default:
                    break;
            }

            return (skip) ? CopyProgressResult.ProgressCancel : CopyProgressResult.ProgressContinue;
        }

        private void registerFileCopy(string archivo_url, string destino_url, MediaFile_Basic_Info mf)
        {
            registro_copias RC = new registro_copias();
            if (destino_url.IndexOf("\\\\") >= 0)
                destino_url = destino_url.Replace("\\\\", "\\");

            RC.copia_id = this.dbCopyId;
            RC.archivo_url = archivo_url;
            RC.destino_url = destino_url;
            RC.fecha = DateTime.Now;
            RC.media_file_id = mf.id;
            RC.nombre_categoria = mf.nombre_categoria;
            RC.titulo = mf.titulo;
            RC.peso = new FileInfo(archivo_url).Length;

            RC_LIST.Add(RC);
        }

        private void registerRCFile(string url, MediaFile_Basic_Info item, string targetDir)
        {
            FileInfo file = new FileInfo(url);
            string targetFile = Path.Combine(targetDir, file.Name);

            files.Add(url, targetFile);   //files.Add(url, file.Length);
            this.totalSize += file.Length;

            // string namePart = url.Substring(0, url.LastIndexOf(Path.DirectorySeparatorChar)).Substring(sourceDirectory.Length - 1);
            // string destinationDirectory = Destination + namePart + Path.DirectorySeparatorChar;

            registerFileCopy(url, targetFile, item);
        }

        private void StartCountFiles(object state)
        {
            this.CurrentStatus = CopyStatus.Counting;
            RaiseStatusChangedEvent();
            RaiseCountStartedEvent();

            //            sourceDirectory = this.sources[0].file_url.Substring(0, this.sources[0].file_url.LastIndexOf(Path.DirectorySeparatorChar));  //this.sources[0].Substring(0, this.sources[0].LastIndexOf(Path.DirectorySeparatorChar));

            CountFiles(this.sources, Destination);

           /* foreach (MediaFile_Basic_Info item in this.Sources)
            {
                if (File.Exists(item.file_url))
                {
                    registerRCFile(item.file_url, item, targetDir);

                    if (File.Exists(item.str_file))
                        registerRCFile(item.str_file, item, targetDir);

                    if (File.Exists(item.fichero_portada))
                        registerRCFile(item.fichero_portada, item, targetDir);

                    if (File.Exists(item.fichero_trailer))
                        registerRCFile(item.fichero_trailer, item, targetDir);
                }
                else if (Directory.Exists(item.file_url))
                {
                   files.Add(item.file_url, sourceDirectory); // files.Add(item.file_url, 0);
                   CountFiles(new DirectoryInfo(item.file_url).GetFileSystemInfos(), item, sourceDirectory);
                }
            }*/

            /* foreach (string item in this.Sources)
             {
                 if (File.Exists(item))
                 {
                     FileInfo file = new FileInfo(item);
                     files.Add(item, file.Length);
                     this.totalSize += file.Length;
                 }
                 else if (Directory.Exists(item))
                 {
                     files.Add(item, 0);
                     CountFiles(new DirectoryInfo(item).GetFileSystemInfos());
                 }
             }*/

            // if (sourceDirectory[sourceDirectory.Length - 1] != Path.DirectorySeparatorChar)
            //   sourceDirectory = sourceDirectory + Path.DirectorySeparatorChar;

            this.CurrentStatus = CopyStatus.CountCompleted;
            RaiseCountCompletedEvent();
            RaiseStatusChangedEvent();
        }

        private void CountFiles(List<MediaFile_Basic_Info> list, string targetDir)
        {
            string newpath;
            foreach (MediaFile_Basic_Info item in list)
            {
                if (!item.is_folder)
                {
                    if (File.Exists(item.file_url))
                    {
                        registerRCFile(item.file_url, item, targetDir);

                        if (File.Exists(item.str_file))
                            registerRCFile(item.str_file, item, targetDir);

                        if (File.Exists(item.fichero_portada))
                            registerRCFile(item.fichero_portada, item, targetDir);

                        if (File.Exists(item.fichero_trailer))
                            registerRCFile(item.fichero_trailer, item, targetDir);
                    }
                }
                else
                {
                    newpath = Path.Combine(targetDir, item.titulo);

                    if (!Directory.Exists(newpath))
                        Directory.CreateDirectory(newpath);

                    CountFiles(item.Childrens, newpath);
                }
            }
        }

        /*private void CountFiles(FileSystemInfo[] fileSystemInfo, MediaFile_Basic_Info mf, string sourceDirectory)
        {
            if (fileSystemInfo == null)
            {
                return;
            }

            string destinationDirectory;
            string namePart;
            foreach (FileSystemInfo item in fileSystemInfo)
            {
                if (item is FileInfo && (File.GetAttributes(item.FullName) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint)
                {
                    FileInfo file = item as FileInfo;
                    files.Add(file.FullName, sourceDirectory);  // files.Add(file.FullName, file.Length);
                    this.totalSize += file.Length;

                    namePart = file.FullName.Substring(0, file.FullName.LastIndexOf(Path.DirectorySeparatorChar)).Substring(sourceDirectory.Length+1);
                    destinationDirectory = Destination + namePart + Path.DirectorySeparatorChar;
                    registerFileCopy(file.FullName, destinationDirectory + Path.GetFileName(file.FullName), mf);
                }
                else if (item is DirectoryInfo)
                {
                    files.Add(item.FullName, sourceDirectory);   //files.Add(item.FullName, 0);
                    CountFiles((item as DirectoryInfo).GetFileSystemInfos(), mf, sourceDirectory);
                }
            }
        }*/

        private void CopyFiles()
        {
            if (this.CurrentStatus == CopyStatus.Counting)
            { 
                resetEvent.Reset();    
            }

            resetEvent.WaitOne();

            this.CurrentStatus = CopyStatus.Copying;
            RaiseCopyStartedEvent();
            RaiseStatusChangedEvent();

            foreach (KeyValuePair<string, string> item in files)
            {
                //resetEvent.WaitOne(1);
            Retry:
                bool success = false;
                Boolean b = false;
                previousCopiedSize = 0;
                skip = false;

               /* sourceDirectory = item.Value;
                spart1 = item.Key.Substring(0, item.Key.LastIndexOf(Path.DirectorySeparatorChar));
                spart2 = spart1.Substring(sourceDirectory.Length - 1);
                destinationDirectory = Destination + spart2;
                if (Directory.Exists(item.Key))
                {                    
                    if ((Directory.CreateDirectory(Destination + item.Key.Substring(sourceDirectory.Length - 1)) == null))
                        unCopiedFiles.Add(new UncopiedFilesStructure(item.Key, new FileInfo(item.Key).Length, UnsafeNativeMethods.GetHResult((uint)Marshal.GetLastWin32Error()).ToString()));    //  unCopiedFiles.Add(new UncopiedFilesStructure(item.Key, item.Value, UnsafeNativeMethods.GetHResult((uint)Marshal.GetLastWin32Error()).ToString()));
                    
                    continue;
                }            */    

                if (cancelCopy)
                {
                    RaiseCopyCanceledEvent();
                    copyThread.Abort();
                }


                /*if (destinationDirectory[destinationDirectory.Length - 1] != Path.DirectorySeparatorChar)
                    destinationDirectory += Path.DirectorySeparatorChar;*/

                this.CurrentFile = item.Key;
                RaiseFileCopyStartEvent(item.Key, item.Value, new FileInfo(item.Key).Length);  // RaiseFileCopyStartEvent(item.Key, Path.Combine(destinationDirectory, Path.GetFileName(item.Key)), new FileInfo(item.Key).Length);

                //  unsafe
                //{
                success = UnsafeNativeMethods.CopyFileEx(item.Key,
                           item.Value, 
                           new CopyProgressRoutine(CopyProgressRoutineHandler), IntPtr.Zero,
                           ref b, this.CopyFlags);

                /* success = UnsafeNativeMethods.CopyFileEx(item.Key,
                     destinationDirectory + Path.GetFileName(item.Key),
                     new CopyProgressRoutine(CopyProgressRoutineHandler), IntPtr.Zero,
                     ref b, this.CopyFlags);*/
                // }
                if (!success)
                {
                    string hres = "Error native";  //UnsafeNativeMethods.HResultToString(UnsafeNativeMethods.GetHResult((uint)Marshal.GetLastWin32Error()));

                    if (skip) { unCopiedFiles.Add(new UncopiedFilesStructure(item.Key, new FileInfo(item.Key).Length, "Skipped: " + hres)); continue; }
                    //if (skip) { unCopiedFiles.Add(new UncopiedFilesStructure(item.Key, item.Value, "Skipped: " + hres)); continue; }

                    switch (RaiseFileCopyUnsuccessfulEvent(item.Key, item.Value, new FileInfo(item.Key).Length, hres))    //switch (RaiseFileCopyUnsuccessfulEvent(item.Key, destinationDirectory + Path.GetFileName(item.Key), new FileInfo(item.Key).Length, hres))
                    {
                        case FileHandleProcedure.Skip:
                        case FileHandleProcedure.Cancel:
                            unCopiedFiles.Add(new UncopiedFilesStructure(item.Key, new FileInfo(item.Key).Length, hres)); break;
                        case FileHandleProcedure.Retry: goto Retry;
                        case FileHandleProcedure.CancelAll: this.cancelCopy = true; break;
                        default: break;
                    }
                }
                else
                {
                    RaiseFileCopyCompletedEvent(item.Key, item.Value, new FileInfo(item.Key).Length);  // RaiseFileCopyCompletedEvent(item.Key, destinationDirectory + Path.GetFileName(item.Key), new FileInfo(item.Key).Length);
                }
            }

            this.CurrentStatus = CopyStatus.CopyCompleted;
            RaiseStatusChangedEvent();
            RaiseCopyCompletedEvent();
        }

        #region EventHandlers
        private void BackupCreator_CountCompletedEvent(object sender, BackupEventArgs e)
        {
            if (copyThread != null && copyThread.ThreadState == ThreadState.WaitSleepJoin)
            {
                //copyThread.Interrupt();                
                resetEvent.Set();
            }
        }

        private void BackupCreator_CopyPausedEvent(object sender, PausedResumedEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Paused)
            {
                this.CurrentStatus = CopyStatus.CopyPaused;
            }
            else
            {
                this.CurrentStatus = CopyStatus.Copying;
            }
            this.RaiseStatusChangedEvent();
        }
        #endregion

        #region RaiseEvents
        private void RaiseStatusChangedEvent()
        {
            if (StatusChangedEvent != null)
            {
                StatusChangedEvent(this, new StatusChangedEventArgs(this.CurrentStatus));
            }
        }
        private void RaiseFileCopyStartEvent(string source, string destination, double fileSize)
        {
            if (FileCopyStartEvent != null)
            {
                FileCopyStartEvent(this, new FileEventArgs(source, destination, fileSize));
            }
        }
        private void RaiseFileCopyProgressEvent(string file, double copied, double total)
        {
            if (FileCopyProgressEvent != null)
            {
                FileCopyProgressEvent(this, new FileProgressEventArgs(file, copied, total));
            }
        }
        private void RaiseFileCopyCompletedEvent(string file, string filePath, double fileSize)
        {
            if (FileCopyCompletedEvent != null)
            {
                FileCopyCompletedEvent(this, new FileEventArgs(file, filePath, fileSize));
            }
        }

        private void RaiseCountStartedEvent()
        {
            if (CountStartedEvent != null)
            {
                CountStartedEvent(this, new EventArgs());
            }
        }
        
        private void RaiseCountCompletedEvent()
        {
            if (CountCompletedEvent != null)
            {
                CountCompletedEvent(this, new BackupEventArgs(this.copiedSize, this.totalSize));
            }
        }
        
        private void RaiseCopyStartedEvent()
        {
            if (CopyStartedEvent != null)
            {
                CopyStartedEvent(this, new EventArgs());
            }
        }
        private void RaiseCopyCanceledEvent()
        {
            if (CopyCanceledEvent != null)
            {
                CopyCanceledEvent(this, new EventArgs());
            }
        }
        private void RaiseCopyCompletedEvent()
        {
            if (CopyCompletedEvent != null)
            {
                CopyCompletedEvent(this, new BackupEventArgs(this.copiedSize, this.totalSize));
            }
        }
        private void RaiseCopyPausedEvent(bool value)
        {
            if (CopyPausedEvent != null)
            {
                CopyPausedEvent(this, new PausedResumedEventArgs(value));
            }
        }

        private void RaiseCopyProgressEvent()
        {
            if (CopyProgressEvent != null)
            {
                CopyProgressEvent(this, new BackupEventArgs(this.copiedSize, this.totalSize));
            }
        }

        private FileHandleProcedure RaiseFileCopyUnsuccessfulEvent(string source, string destination, double fileSize, string tag)
        {
            if (FileCopyUnsucessfulEvent != null)
            {
                FileCopyUnsuccessfulEventArgs file = new FileCopyUnsuccessfulEventArgs(source, destination, fileSize, tag);
                FileCopyUnsucessfulEvent(this, file);
                return file.Handle;
            }
            return FileHandleProcedure.None;
        }
        #endregion

        #region IDisposable Members

        //private void Dispose(bool disposing)
        //{
        //    if (disposing && unCopiedFiles.Count > 0)
        //    {
        //        foreach (var item in unCopiedFiles)
        //        {
        //            string destination = Destination + item.Path.Substring(0, item.Path.LastIndexOf(Path.DirectorySeparatorChar));
        //            if (File.Exists(destination))
        //            {
        //                File.Delete(destination);
        //            }
        //            else if (Directory.Exists(destination))
        //            {
        //                Directory.Delete(destination);
        //            }
        //        }
        //    }
        //}

        //public void Dispose()
        //{
        //    //throw new NotImplementedException();
        //    Dispose(true);
        //}

        #endregion

        #endregion

        private static class UnsafeNativeMethods
        {
            const Int32 FACILITY_WIN32 = 7;

            //
            // HRESULT_FROM_WIN32(x) used to be a macro, however we now run it as an inline function
            // to prevent double evaluation of 'x'. If you still need the macro, you can use __HRESULT_FROM_WIN32(x)
            //           

            //#define __HRESULT_FROM_WIN32(x) ((HRESULT)(x) <= 0 ? ((HRESULT)(x)) : ((HRESULT) (((x) & 0x0000FFFF) | (FACILITY_WIN32 << 16) | 0x80000000)))

            //#if ndef __midl
            //FORCEINLINE HRESULT HRESULT_FROM_WIN32(unsigned long x) { 
            //return (HRESULT)(x) <= 0 ? (HRESULT)(x) : (HRESULT) (((x) & 0x0000FFFF) | (FACILITY_WIN32 << 16) | 0x80000000);
            //}
            //#else
            //#define HRESULT_FROM_WIN32(x) __HRESULT_FROM_WIN32(x)
            //#endif

            /// <summary>
            /// Makes HRESULT value from x
            /// </summary>
            /// <param name="x">value to be converted to HRESULT</param>
            /// <returns>Corresponding HRESULT</returns>
            public static uint GetHResult(UInt32 x)
            {
                return (uint)(x) <= 0 ? (uint)(x) : (uint)(((x) & 0x0000FFFF) | (FACILITY_WIN32 << 16) | 0x80000000);
            }
            /// <summary>
            /// Converts the HRESULT value to string.
            /// </summary>
            /// <param name="result">value to be converted</param>
            /// <returns>Correspondin string to HRESULT</returns>
           /* public static string HResultToString(uint result)
            {
                System.IO.StreamReader str = new StreamReader(Properties.Resources.ErrorCodeSource);
                LumenWorks.Framework.IO.Csv.CsvReader csv = new CsvReader(str, true);

                csv.SkipEmptyLines = true;
                csv.SupportsMultiline = true;

                foreach (var item in csv)
                {
                    if (uint.Parse(item[0], System.Globalization.CultureInfo.InvariantCulture) == (uint)result)
                    {
                        return item[2];
                    }
                }
                return "INVALID HRESULT";
            }*/

            /// <summary>
            /// Shell Method to copy a file.
            /// </summary>
            /// <param name="lpExistingFileName">The Source File with Path</param>
            /// <param name="lpNewFileName">The Destination File with Path</param>
            /// <param name="lpProgressRoutine">The progress routine that handles the progress of the file being copied</param>
            /// <param name="lpData"></param>
            /// <param name="pbCancel"></param>
            /// <param name="dwCopyFlags"></param>
            /// <returns>true if the file was successfully copied, false otherwise</returns>
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CopyFileEx(string ExistingFileName, string NewFileName,
                CopyProgressRoutine ProgressRoutine, IntPtr Data,
                ref Boolean Cancel, CopyFileFlags CopyFlags);
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                resetEvent.Close();
                copyThread = null;

                files.Clear();
                files = null;
                unCopiedFiles.Clear();
                unCopiedFiles = null;
                sources.Clear();
                sources = null;

                resetEvent = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

#region Notes
/*
 * sourceDirectory->Assuming that all the sources reside under one parant directory.
 * */
#endregion
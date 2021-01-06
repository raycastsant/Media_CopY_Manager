using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Backup2
{
    public class FileEventArgs : EventArgs
    {
        public FileEventArgs(string source, string destination, double fileSize)
        {
            //throw new System.NotImplementedException();
            this.SourceFilePath = source;
            this.DestinationFilePath = destination;
            this.FileSize = fileSize;
        }

        public string SourceFilePath { get; private set; }


        #region Property FileSize
        public double FileSize { get; private set; }
        #endregion


        #region Property DestinationFilePath
        public string DestinationFilePath { get; private set; }
        #endregion


    }

    public class BackupEventArgs : EventArgs
    {
        public double CopiedSize { get; private set; }

        public double TotalSize { get; private set; }

        #region Property Progress
        public double Progress { get; private set; }
        #endregion

        public BackupEventArgs(double copiedSize, double totalSize)
        {
            this.CopiedSize = copiedSize;
            this.TotalSize = totalSize;

            this.Progress = this.CopiedSize / this.TotalSize * 100;
            if (this.Progress.CompareTo(double.NaN)==0)
            {
                this.Progress = 100;
            }
        }
    }

    public class FileProgressEventArgs : EventArgs
    {

        #region Property CopiedSize
        public double CopiedSize { get; private set; }
        #endregion


        #region Property TotalSize
        public double TotalSize { get; private set; }
        #endregion


        #region Property Progress
        public double Progress { get; private set; }
        #endregion


        #region Property File
        public string File { get; private set; }
        #endregion

        public FileProgressEventArgs(string file, double copiedSize, double totalSize)
        {
            this.CopiedSize = copiedSize;
            this.TotalSize = totalSize;
            this.File = file;
            this.Progress = copiedSize / totalSize * 100;
            if (this.Progress.CompareTo(double.NaN) == 0)
            {
                this.Progress = 100;
            }
        }
    }

    public class PausedResumedEventArgs : EventArgs
    {

        #region Property Paused
        public bool Paused { get; private set; }
        #endregion

        public PausedResumedEventArgs(bool paused)
        {
            this.Paused = paused;
        }
    }

    public class StatusChangedEventArgs : EventArgs
    {

        #region Property Status
        public CopyStatus Status { get; private set; }
        #endregion

        public StatusChangedEventArgs(CopyStatus status)
        {
            this.Status = status;
        }
    }

    public class FileCopyUnsuccessfulEventArgs : FileEventArgs
    {

        #region Property Handled
        public FileHandleProcedure Handle { get; set; }
        #endregion

        #region Property Tag
        public string Tag { get; private set; }
        #endregion

        public FileCopyUnsuccessfulEventArgs(string source, string destination, double fileSize, string tag) :
            base(source, destination, fileSize)
        {
            Handle = FileHandleProcedure.None;
            Tag = tag;
        }             
    }
}


namespace Backup2
{
    public struct UncopiedFilesStructure
    {
        /// <summary>
        /// The path of the source file.
        /// </summary>
        private string _path;
        /// <summary>
        /// Any additional information. Generally used to store the reason for the unsuccessfull copy of the file.
        /// </summary>
        private string _tag;
        /// <summary>
        /// The size of the file in bytes
        /// </summary>
        private double _size;

        #region Property Path
        public string Path { get { return _path; } }
        #endregion

        #region Property Tag
        public string Tag { get { return _tag; } }
        #endregion

        #region Property Size
        public double Size { get { return _size; } }
        #endregion

        /// <summary>
        /// Information about the unsuccessfull copy of the file.
        /// </summary>
        /// <param name="path">Path of the File</param>
        /// <param name="size">Size of the file in bytes</param>
        /// <param name="tag">Reason for failure</param>
        public UncopiedFilesStructure(string path, double size, string tag)
        {
            _path = path;
            _size = size;
            _tag = tag;
        }

        /// <summary>
        /// The returned string is formatted as Path: (Size)bytes\tTag.
        /// </summary>
        /// <returns>Returns a string that has all the information of the file.</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}bytes\t{2}", Path, Size, Tag);
        }
    }

    public enum CopyStatus
    {
        None,
        Counting,
        CountingPaused,
        CountCompleted,
        Copying,
        CopyPaused,
        CopyCompleted,
    }

    [System.Flags]
    public enum CopyFileFlags : int
    {
        FileFailIfExists = 0x00000001,                  //If the file already exists do not overwrite it.
        FileRestartable = 0x00000002,                     //
        FileOpenSourceForWrite = 0x00000004,           //
        FileAllowDecryptedDestination = 0x00000008      //
    }

    public enum FileHandleProcedure
    {
        None,
        Skip,
        Retry,
        Cancel,
        CancelAll
    }  
}
/*
	Copyright © Bryan Apellanes 2015  
*/

using System.Text;
using System.Diagnostics;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects
{
    /// <summary>
    /// Abstract base class for data that can be safely read and written across multiple processes
    /// using a file-based locking mechanism. Uses separate write, read, and lock files to ensure
    /// atomic updates.
    /// </summary>
    public abstract class MultiProcessData : RawData
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MultiProcessData"/> for the specified data object.
        /// </summary>
        /// <param name="data">The data object to manage across processes.</param>
        /// <param name="encoding">The text encoding to use, or null for default.</param>
        protected MultiProcessData(object data, Encoding encoding = null!) : base()
        {         
            Args.ThrowIfNull(data, nameof(data));
            LockTimeout = 150;
            AcquireLockRetryInterval = 50;
            DataType = data.GetType();
        }

        /// <summary>
        /// Gets or sets the encoder/decoder used to serialize and deserialize data for file storage.
        /// </summary>
        protected abstract IObjectEncoderDecoder ObjectEncoder
        {
            get;
            set;
        }

        /// <summary>
        /// Writes the specified data to the data file using file-based locking to ensure cross-process safety.
        /// Returns false if the lock could not be acquired within the timeout period.
        /// </summary>
        /// <param name="data">The data object to write.</param>
        /// <returns><c>true</c> if the data was written successfully; <c>false</c> if the lock could not be acquired.</returns>
        public virtual bool Write(object data)
        {
            if(AcquireLock(LockTimeout))
            {
                // if the message file doesn't exist write to it
                string writeTo = DataFile;
                if (File.Exists(DataFile))
                {
                    //  else write to the WriteFile
                    writeTo = WriteFile;
                }

                IObjectEncoding encoding = ObjectEncoder.Encode(data);
                
                File.WriteAllBytes(writeTo, encoding.Value);

                // if WriteFile exists move it on top of MessageFile
                if (File.Exists(WriteFile))
                {
                    File.Delete(DataFile);
                    File.Move(WriteFile, DataFile);
                }

                // copy MessageFile to ReadFile
                File.Copy(DataFile, ReadFile, true);
                File.Move(LockFile, TempLockFile);
                File.Delete(TempLockFile);
                return true;
            }

            return false;
        }

        /// <summary>
        /// The number of milliseconds to wait to 
        /// try and acquire a lock
        /// </summary>
        public int LockTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the acquire lock retry interval, the amount of time in milliseconds to sleep
        /// between attempts to acquire a lock.
        /// </summary>
        /// <value>
        /// The acquire lock retry interval.
        /// </value>
        public int AcquireLockRetryInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the data being managed.
        /// </summary>
        public Type DataType
        {
            get;
            set;
        }

        string _rootDirectory = null!;
        readonly object _rootDirectoryLock = new object();
        /// <summary>
        /// Gets or sets the root directory for the data, lock, read, and write files.
        /// Defaults to a subdirectory named after the data type under the process data folder.
        /// </summary>
        public string RootDirectory
        {
            get
            {
                return _rootDirectoryLock.DoubleCheckLock(ref _rootDirectory, () => Path.Combine(RuntimeSettings.ProcessDataFolder, DataType.Name));
            }
            set
            {
                _rootDirectory = Path.Combine(value, DataType.Name);
            } 
        }

        /// <summary>
        /// Occurs when an exception is thrown while attempting to acquire a file lock.
        /// </summary>
        public event EventHandler AcquireLockException = null!;

        /// <summary>
        /// Raises the <see cref="AcquireLockException"/> event and records the exception message.
        /// </summary>
        /// <param name="ex">The exception that occurred during lock acquisition.</param>
        protected void OnAcquireLockException(Exception ex)
        {
            if (AcquireLockException != null)
            {
                LastExceptionMessage = "PID={0}:{1}".Format(Process.GetCurrentProcess().Id, ex.Message);
                AcquireLockException(this, new EventArgs());
            }
        }

        /// <summary>
        /// Occurs when this instance is waiting for another process to release the file lock.
        /// </summary>
        public event EventHandler WaitingForLock = null!;

        /// <summary>
        /// Raises the <see cref="WaitingForLock"/> event.
        /// </summary>
        protected void OnWaitingForLock()
        {
            WaitingForLock?.Invoke(this, new EventArgs());
        }
                
        /// <summary>
        /// Gets or sets the message from the most recent exception encountered during lock acquisition.
        /// </summary>
        public string LastExceptionMessage { get; set; } = null!;

        /// <summary>
        /// Gets the process id of the process who has 
        /// the lock
        /// </summary>
        public string CurrentLockerId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the machine name of the process that currently holds the lock.
        /// </summary>
        public string CurrentLockerMachineName { get; set; } = null!;

        protected string LockFile => Path.Combine(RootDirectory, "{0}.lock".Format(HashHexString));

        protected string TempLockFile => $"{LockFile}.tmp";

        protected internal string WriteFile => Path.Combine(RootDirectory, "{0}.write".Format(HashHexString));

        protected internal string ReadFile => Path.Combine(RootDirectory, "{0}.read".Format(HashHexString));

        protected internal string DataFile => Path.Combine(RootDirectory, HashHexString);

        private void EnsureRoot()
        {
            if (!Directory.Exists(RootDirectory))
            {
                Directory.CreateDirectory(RootDirectory);
            }
        }

        static readonly object _lock = new object();
        private bool AcquireLock(int timeoutInMilliseconds)
        {
            try
            {
                lock (_lock)
                {
                    EnsureRoot();
                    MultiProcessDataLockInfo lockInfo = new MultiProcessDataLockInfo();
                    bool timeoutExpired = Exec.TakesTooLong(() =>
                    {
                        bool logged = false;
                        while (File.Exists(LockFile))
                        {
                            if (!logged)
                            {
                                logged = true;
                                MultiProcessDataLockInfo currentLockInfo =
                                    ObjectEncoder.Decode<MultiProcessDataLockInfo>(File.ReadAllBytes(LockFile));
                                CurrentLockerId = currentLockInfo?.ProcessId.ToString()!;
                                CurrentLockerMachineName = currentLockInfo?.MachineName!;
                                OnWaitingForLock();
                            }

                            Thread.Sleep(AcquireLockRetryInterval);
                        }
                        return LockFile;
                    }, (lockFile) =>
                    {
                        IObjectEncoding encoding = ObjectEncoder.Encode(lockInfo);
                        
                        File.WriteAllBytes(lockFile, encoding.Value);
                        
                        return lockFile;
                    }, TimeSpan.FromMilliseconds(timeoutInMilliseconds));

                    return !timeoutExpired;
                }
            }
            catch (Exception ex)
            {
                OnAcquireLockException(ex);
                return false;
            }
        }

    }
}

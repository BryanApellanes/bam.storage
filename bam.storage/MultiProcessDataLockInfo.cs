/*
	Copyright © Bryan Apellanes 2015  
*/

using System.Diagnostics;

namespace Bam
{
	/// <summary>
	/// Contains information about the process holding a multi-process data lock,
	/// including the process ID and machine name. Automatically populated with the current process info on construction.
	/// </summary>
	[Serializable]
    public class MultiProcessDataLockInfo
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MultiProcessDataLockInfo"/> with the current process ID and machine name.
        /// </summary>
        public MultiProcessDataLockInfo()
        {
            this.ProcessId = Process.GetCurrentProcess().Id;
            this.MachineName = Environment.MachineName;
        }

        /// <summary>
        /// Gets or sets the process ID of the lock holder.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the machine name of the lock holder.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Determines whether the specified object represents the same lock holder (same process ID and machine name).
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns><c>true</c> if the lock info matches; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is MultiProcessDataLockInfo lockInfo)
            {
                return lockInfo.ProcessId == ProcessId && MachineName.Equals(MachineName);
            }
            return false;
        }
        /// <summary>
        /// Returns a hash code based on the machine name and process ID combination.
        /// </summary>
        /// <returns>A hash code for the current lock info.</returns>
        public override int GetHashCode()
        {
            return $"{MachineName}:{ProcessId}".GetHashCode();
        }
    }
}

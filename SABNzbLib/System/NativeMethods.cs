using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nzb.System
{
    internal static class NativeMethods
    {
        /// <summary>
        /// Retrieves the full path of a known folder identified by the folder's KnownFolderID.
        /// </summary>
        /// <param name="rfid">A KnownFolderID that identifies the folder.</param>
        /// <param name="dwFlags">Flags that specify special retrieval options. This value can be 0; otherwise, one or
        /// more of the KnownFolderFlag values.</param>
        /// <param name="hToken">An access token that represents a particular user. If this parameter is NULL, which is
        /// the most common usage, the function requests the known folder for the current user. Assigning a value of -1
        /// indicates the Default User. The default user profile is duplicated when any new user account is created.
        /// Note that access to the Default User folders requires administrator privileges.</param>
        /// <param name="ppszPath">When this method returns, contains the address of a string that specifies the path of
        /// the known folder. The returned path does not include a trailing backslash.</param>
        /// <returns>Returns S_OK if successful, or an error value otherwise.</returns>
        [DllImport("Shell32.dll")]
        internal static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)]Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr ppszPath);

    }
}

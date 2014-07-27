using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;

namespace MightyHID
{
    public class HIDDev : IDisposable
    {
        
        /* device handle */
        private IntPtr handle;
        /* stream */
        private FileStream _fileStream;

        /* stream */
        public FileStream fileStream 
        {
            get { return _fileStream; }
            internal set { _fileStream = value; }
        }

        /* dispose */
        public void Dispose()
        {
            /* deal with file stream */
            if (_fileStream != null) {
                /* close stream */
                _fileStream.Close();
                /* get rid of object */
                _fileStream = null;
            }

            /* close handle */
            Native.CloseHandle(handle);
        }

        /* open hid device */
        public bool Open(string path)
        {
            /* safe file handle */
            SafeFileHandle shandle;

            /* opens hid device file */
            handle = Native.CreateFile(path, 
                Native.GENERIC_READ | Native.GENERIC_WRITE,
                Native.FILE_SHARE_READ | Native.FILE_SHARE_WRITE,
                IntPtr.Zero, Native.OPEN_EXISTING, Native.FILE_FLAG_OVERLAPPED,
                IntPtr.Zero);

            /* whops */
            if (handle == Native.INVALID_HANDLE_VALUE) {
                return false;
            }

            /* build up safe file handle */
            shandle = new SafeFileHandle(handle, false);

            /* prepare stream - async */
            _fileStream = new FileStream(shandle, FileAccess.ReadWrite, 
                32, true);

            /* report status */
            return true;
        }

        /* close hid device */
        public void Close()
        {
            /* deal with file stream */
            if (_fileStream != null) {
                /* close stream */
                _fileStream.Close();
                /* get rid of object */
                _fileStream = null;
            }

            /* close handle */
            Native.CloseHandle(handle);
        }

        /* write record */
        public void Write(byte[] data)
        {
            /* write some bytes */
            _fileStream.Write(data, 0, data.Length);
            /* flush! */
            _fileStream.Flush();
        }

        /* read record */
        public void Read(byte[] data)
        {
            /* get number of bytes */
            int n = 0, bytes = data.Length;

            /* read buffer */
            while (n != bytes) {
                /* read data */
                int rc = _fileStream.Read(data, n, bytes - n);
                /* update pointers */
                n += rc;
            }
        }
    }
}

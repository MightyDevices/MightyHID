using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace MightyHID
{
    public class HIDBrowse
    {
        /* browse all HID class devices */
        public static List<HIDInfo> Browse()
        {
            /* hid device class guid */
            Guid gHid;
            /* list of device information */
            List<HIDInfo> info = new List<HIDInfo>();

            /* obtain hid guid */
            Native.HidD_GetHidGuid(out gHid);
            /* get list of present hid devices */
            var hInfoSet = Native.SetupDiGetClassDevs(ref gHid, null, IntPtr.Zero,
                Native.DIGCF_DEVICEINTERFACE | Native.DIGCF_PRESENT);

            /* allocate mem for interface descriptor */
            var iface = new Native.DeviceInterfaceData();
            /* set size field */
            iface.Size = Marshal.SizeOf(iface);
            /* interface index */
            uint index = 0;

            /* iterate through all interfaces */
            while (Native.SetupDiEnumDeviceInterfaces(hInfoSet, 0, ref gHid, 
                index, ref iface)) {

                /* vid and pid */
                short vid, pid;

                /* get device path */
                var path = GetPath(hInfoSet, ref iface);
                
                /* open device */
                var handle = Open(path);
                /* device is opened? */
                if (handle != Native.INVALID_HANDLE_VALUE) {
                    /* get device manufacturer string */
                    var man = GetManufacturer(handle);
                    /* get product string */
                    var prod = GetProduct(handle);
                    /* get serial number */
                    var serial = GetSerialNumber(handle);
                    /* get vid and pid */
                    GetVidPid(handle, out vid, out pid);

                    /* build up a new element */
                    HIDInfo i = new HIDInfo(prod, serial, man, path, vid, pid);
                    /* add to list */
                    info.Add(i);

                    /* close */
                    Close(handle);
                }

                /* next, please */
                index++;
            }

            /* clean up */
            if (Native.SetupDiDestroyDeviceInfoList(hInfoSet) == false) {
                /* fail! */
                throw new Win32Exception();
            }

            /* return list */
            return info;
        }

        /* open device */
        private static IntPtr Open(string path)
        {
            /* opens hid device file */
            return Native.CreateFile(path,
                Native.GENERIC_READ | Native.GENERIC_WRITE,
                Native.FILE_SHARE_READ | Native.FILE_SHARE_WRITE,
                IntPtr.Zero, Native.OPEN_EXISTING, Native.FILE_FLAG_OVERLAPPED,
                IntPtr.Zero);
        }

        /* close device */
        private static void Close(IntPtr handle)
        {
            /* try to close handle */
            if (Native.CloseHandle(handle) == false) {
                /* fail! */
                throw new Win32Exception();
            }
        }

        /* get device path */
        private static string GetPath(IntPtr hInfoSet, 
            ref Native.DeviceInterfaceData iface)
        {
            /* detailed interface information */
            var detIface = new Native.DeviceInterfaceDetailData();
            /* required size */
            uint reqSize = (uint)Marshal.SizeOf(detIface);

            /* set size. The cbSize member always contains the size of the 
             * fixed part of the data structure, not a size reflecting the 
             * variable-length string at the end. */
            /* now stay with me and look at that x64/x86 maddness! */
            detIface.Size = Marshal.SizeOf(typeof(IntPtr)) == 8 ? 8 : 5;

            /* get device path */
            bool status = Native.SetupDiGetDeviceInterfaceDetail(hInfoSet,
                ref iface, ref detIface, reqSize, ref reqSize, IntPtr.Zero);

            /* whops */
            if (!status) {
                /* fail! */
                throw new Win32Exception();
            }

            /* return device path */
            return detIface.DevicePath;
        }

        /* get device manufacturer string */
        private static string GetManufacturer(IntPtr handle)
        {
            /* buffer */
            var s = new StringBuilder(256);
            /* returned string */
            string rc = String.Empty;

            /* get string */
            if (Native.HidD_GetManufacturerString(handle, s, s.Capacity)) {
                rc = s.ToString();
            }

            /* report string */
            return rc;
        }

        /* get device product string */
        private static string GetProduct(IntPtr handle)
        {
            /* buffer */
            var s = new StringBuilder(256);
            /* returned string */
            string rc = String.Empty;

            /* get string */
            if (Native.HidD_GetProductString(handle, s, s.Capacity)) {
                rc = s.ToString();
            }

            /* report string */
            return rc;
        }

        /* get device product string */
        private static string GetSerialNumber(IntPtr handle)
        {
            /* buffer */
            var s = new StringBuilder(256);
            /* returned string */
            string rc = String.Empty;

            /* get string */
            if (Native.HidD_GetSerialNumberString(handle, s, s.Capacity)) {
                rc = s.ToString();
            }

            /* report string */
            return rc;
        }

        /* get vid and pid */
        private static void GetVidPid(IntPtr handle, out short Vid, out short Pid)
        {
            /* attributes structure */
            var attr = new Native.HiddAttributtes();
            /* set size */
            attr.Size = Marshal.SizeOf(attr);

            /* get attributes */
            if (Native.HidD_GetAttributes(handle, ref attr) == false) {
                /* fail! */
                throw new Win32Exception();
            }

            /* update vid and pid */
            Vid = attr.VendorID; Pid = attr.ProductID;
        }
    }
}

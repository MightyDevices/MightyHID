using System;
using System.Collections.Generic;

namespace MightyHID
{
    public class HIDInfo
    {
        /* device path */
        public string Path { get; private set; }
        /* vendor ID */
        public short Vid { get; private set; }
        /* product id */
        public short Pid { get; private set; }
        /* usb product string */
        public string Product { get; private set; }
        /* usb manufacturer string */
        public string Manufacturer { get; private set; }
        /* usb serial number string */
        public string SerialNumber { get; private set; }

        /* constructor */
        public HIDInfo(string product, string serial, string manufacturer, 
            string path, short vid, short pid)
        {
            /* copy information */
            Product = product;
            SerialNumber = serial;
            Manufacturer = manufacturer;
            Path = path;
            Vid = vid;
            Pid = pid;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MightyHID;

namespace MightyHIDTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /* hello, world! */
            Console.WriteLine("List of USB HID devices:");
            /* browse for hid devices */
            var devs = HIDBrowse.Browse();
            /* display VID and PID for every device found */
            foreach (var dev in devs) {
                Console.WriteLine("VID = " + dev.Vid.ToString("X4") +
                    " PID = " + dev.Pid.ToString("X4") + 
                    " Product: " + dev.Product);
            }
            
            /* try to connect to first device */
            if (devs.Count > 0) {
                /* new device */
                HIDDev dev = new HIDDev(); 
                /* connect */
                dev.Open(devs[0]);
                /* an example of hid report, report id is always located 
                 * at the beginning of every report. Here it was set to 0x01.
                 * adjust this report so it does meet your hid device reports */
                byte[] report = new byte[32]; report[0] = 0x01;
                /* send report */
                dev.Write(report);
                /* get response */
                dev.Read(report);
            }

            /* bye bye! */
            Console.Read();
        }
    }
}

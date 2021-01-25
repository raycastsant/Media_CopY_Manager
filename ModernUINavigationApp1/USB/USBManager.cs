using MCP.db;
using MCP.gui.Pages.administracion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MCP.USB
{
    class USBManager
    {
        private static bool seeking_device = false;
        private static USBSerialNumber usb_serial;
        private static List<string> connected_devices = new List<string>();   //serial number string

        //----------------- USB Management --------------------------------------------------
        /*  public static void FetchConnectedDrivers()
          {
              DriveInfo[] allDrives = DriveInfo.GetDrives();

              foreach (DriveInfo d in allDrives)
              {
                  if (d.IsReady == true && d.DriveType == DriveType.Removable)
                  {
                      Console.WriteLine("Drive {0}", d.Name);
                      Console.WriteLine("  Drive type: {0}", d.DriveType);

                      Console.WriteLine("  Volume label: {0}", d.VolumeLabel);
                      Console.WriteLine("  File system: {0}", d.DriveFormat);
                      Console.WriteLine(
                          "  Available space to current user:{0, 15} bytes",
                          d.AvailableFreeSpace);

                      Console.WriteLine(
                          "  Total available space:          {0, 15} bytes",
                          d.TotalFreeSpace);

                      Console.WriteLine(
                          "  Total size of drive:            {0, 15} bytes ",
                          d.TotalSize);
                  }
              }
          }*/

        public static void StartUsbDeviceWatcher(bool session)
        {
            usb_serial = new USBSerialNumber();
            if(AppMAnager.usbTimer == null || session)
            {
                AppMAnager.usbTimer = new DispatcherTimer();
                AppMAnager.usbTimer.Tick += new EventHandler(watchForInsertedDevice);
                AppMAnager.usbTimer.Interval = new TimeSpan(0, 0, 3);
                AppMAnager.usbTimer.Start();
            }
            else
            {
                AppMAnager.usbTimer.Stop();
            }
            
        }

      

        private static void watchForInsertedDevice(object sender, EventArgs e)
        {
            if (!seeking_device)
            {
                seeking_device = true;

                string letter;
                string serial;
                float capacity = 0;
                List<string> usb_list = new List<string>();
                foreach (DriveInfo di in DriveInfo.GetDrives())
                {
                    if (di.DriveType == DriveType.Fixed || di.DriveType == DriveType.Removable)
                    {
                        
                        letter = di.RootDirectory.ToString().Substring(0, 2);
                        capacity = (float)(di.TotalSize*Math.Pow(10,-9));
                        
                        serial = usb_serial.getSerialNumberFromDriveLetter(letter);

                        if (!string.IsNullOrEmpty(serial))
                        {
                            usb_list.Add(serial);
                        }
                    }
                }

                foreach (string sn in usb_list)
                {
                    if (!connected_devices.Contains(sn))
                    {
                        connected_devices.Add(sn);
                        CheckIfExistUsb(capacity, sn);
                        //FireNewDeviceConnected(sn);
                    }
                }

                //Search for removed devices
                List<string> to_remove = new List<string>();
                bool device_removed = false;
                foreach (string cdv in connected_devices)
                {
                    if (!usb_list.Contains(cdv))
                    {
                        device_removed = true;
                        to_remove.Add(cdv);
                    }
                }

                if (device_removed)
                {
                    foreach (string dev in to_remove)
                    {
                        connected_devices.Remove(dev);
                        Debug.WriteLine("Removed device: " + dev);
                    }
                }

                seeking_device = false;
            }
        }

        private static void CheckIfExistUsb(float capacity, string sn)
        {
            bool exist = FireNewDeviceConnected(sn);
            if (!exist)
            {

               
                usb usb = new usb
                {
                    numero_serie = sn,
                    capacidad = capacity,
                    //id_cliente = 0,
                    marca = ""
                    //cliente = DBManager.ClienteRepo.FindById(1)
                };
                Window window = new Window
                {
                    Title = "Test",
                    Content = new PClientes(usb)
                    
                };

                window.WindowState = WindowState.Maximized;
           
                window.ShowDialog();
                
                //DBManager.UsbRepo.Add(usb);
            }
        }

        private static Boolean FireNewDeviceConnected(string serial)
        {
            bool exist = true;
            Debug.WriteLine("Connected device: " + serial);
            usb usb = DBManager.UsbRepo.FindBySerial(serial);
            if(usb == null)
            {
                MessageBox.Show("El dispositvo no pertenece a ningún cliente. Debe añadir el cliente al sistema");
                exist = false;
            }
            else
            {
                cliente cliente = DBManager.ClienteRepo.FindById(usb.id_cliente);
                MessageBox.Show("Dispositvo perteneciente a "+cliente.nombre_cliente + " "+cliente.apellidos_cliente);
            }
            return exist;
        }

        /* private static string GetDeviceSerial(string driveletter)
         {
             string serial = "";
             /* ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
              foreach(ManagementObject mob in searcher.Get())
              {
                  Debug.WriteLine(mob["SerialNumber"]+"    "+mob["Signature"]);
              }*/

        /*  ManagementClass mclass = new ManagementClass("Win32_DiskDrive");
          ManagementObjectCollection mcol = mclass.GetInstances();
          foreach(ManagementObject mob in mcol)
          {
              string g = Convert.ToString(mob["VolumeSerialNumber"]);
              serial += g;
          }*/

        /*var index = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDiskToPartition").Get().Cast<System.Management.ManagementObject>();
        var disks = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive").Get().Cast<ManagementObject>();

        try
        {
            var drive = (from i in index where i["Dependent"].ToString().Contains(driveletter.ToUpper()) select i).FirstOrDefault();
           string asc = drive["Antecedent"].ToString();
           var key = drive["Antecedent"].ToString().Split('#')[1].Split(',')[0];

            var disk = (from d in disks
                        where
                            d["Name"].ToString() == "\\\\.\\PHYSICALDRIVE" + key &&
                            d["InterfaceType"].ToString() == "USB"
                        select d).FirstOrDefault();
            serial = disk["PNPDeviceID"].ToString().Split('\\').Last();
        }
        catch
        {
            //drive not found!!
        }*/

        // Must be 2 characters long.
        // Function expects "C:" or "D:" etc...
        /*  if (driveletter.Length != 2)
              return "";

          try
          {
              using (var partitions = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" + driveletter +
                                               "'} WHERE ResultClass=Win32_DiskPartition"))
              {
                  foreach (var partition in partitions.Get())
                  {
                      using (var drives = new ManagementObjectSearcher("ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" +
                                                           partition["DeviceID"] +
                                                           "'} WHERE ResultClass=Win32_DiskDrive"))
                      {
                          foreach (var drive in drives.Get())
                          {
                              return (string)drive["DeviceID"];
                          }
                      }
                  }
              }
          }
          catch
          {
              return "<unknown>";
          }

          // Not Found
          return "<unknown>";

       //   return serial;
      }

      public static void Usb_Disk_Inserted(string serial)
      {
          Debug.WriteLine(serial);
      }*/
    }
}

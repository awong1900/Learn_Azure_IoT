using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System.Diagnostics;
using System.Threading.Tasks;



namespace PiCreateDeviceKey
{
    public sealed partial class MainPage : Page
    {

        static RegistryManager registryManager;
        static string connectionString = "HostName=ten-test.azure-devices.cn;SharedAccessKeyName=iothubowner;SharedAccessKey=F/R9W8eUk/7gYEnNHO8sX7HA0ZMbnVNsrEcsViIVrrA=";

        public MainPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("Add Device...");
            
            GetDeviceAsync().Wait();
        }



        private static async Task GetDeviceAsync()
        {
            string deviceId = "myFirstDevice";
            Device device;
            Debug.WriteLine("Get Device...");
            try
            {
                // device = await registryManager.AddDeviceAsync(new Device(deviceId));
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
                device = await registryManager.GetDeviceAsync(deviceId);
                Debug.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);
            }
            catch (DeviceAlreadyExistsException)
            {
                throw;
                // device = await registryManager.GetDeviceAsync(deviceId);
            }
            // Debug.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);

        }
    }
}

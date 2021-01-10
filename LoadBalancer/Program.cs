using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace LoadBalancer
{
    class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:10000/LoadBalancerService";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;

            ServiceHost host = new ServiceHost(typeof(LoadBalancerService));
            host.AddServiceEndpoint(typeof(ILoadBalancer), binding, address);

            host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
            host.Description.Behaviors.Add(new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });

            host.Open();
            Console.WriteLine();
            Console.WriteLine("LoadBalancerService is open!");
            Console.WriteLine("Press <enter> to stop service...");
            Console.ReadLine();
            host.Close();

        }
    }
}

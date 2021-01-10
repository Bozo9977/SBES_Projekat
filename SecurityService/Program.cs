using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SecurityManager;
using System.IdentityModel.Policy;

namespace SecurityService
{
	public class Program
	{
		static void Main(string[] args)
		{
			NetTcpBinding binding = new NetTcpBinding();
			string address = "net.tcp://localhost:9999/SecurityService";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;

            binding.MaxReceivedMessageSize = 5000000;
            binding.MaxBufferSize = 5000000;
            binding.MaxBufferPoolSize = 5000000;
            

            ServiceHost host = new ServiceHost(typeof(SecurityService));
			host.AddServiceEndpoint(typeof(ISecurityService), binding, address);

			host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
			host.Description.Behaviors.Add(new ServiceDebugBehavior(){IncludeExceptionDetailInFaults = true });

            host.Authorization.ServiceAuthorizationManager = new CustomServiceAuthorizationManager();

            host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
            policies.Add(new CustomAuthorizationPolicy());
            host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();
            

			host.Open();
            
			Console.WriteLine("SecurityService service is started.");
			Console.WriteLine("Press <enter> to stop service...");

			Console.ReadLine();
			host.Close();
		}
	}
}

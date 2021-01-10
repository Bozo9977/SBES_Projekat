using SecurityManager;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Linq;
using System.Net.Security;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
	public class Program
	{
		static void Main(string[] args)
		{
            Console.ReadLine();

            Thread thread = null;
            

            NetTcpBinding binding = new NetTcpBinding();
			string address = "net.tcp://localhost:9999/SecurityService";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;

            binding.MaxReceivedMessageSize = 5000000;
            binding.MaxBufferSize = 5000000;
            binding.MaxBufferPoolSize = 5000000;
            

            InstanceContext instanceContext = new InstanceContext(new NewsletterService());

            
            using (ClientProxy proxy = new ClientProxy(binding, address, instanceContext))
            {
                bool isSuperReader = false;


                foreach (IdentityReference group in WindowsIdentity.GetCurrent().Groups)
                {
                    SecurityIdentifier sid = (SecurityIdentifier)group.Translate(typeof(SecurityIdentifier));
                    var name = sid.Translate(typeof(NTAccount));

                    if ((name.ToString()).Contains("SuperReaders"))
                        isSuperReader = true;
                }

                if (!isSuperReader)
                {
                    thread = new Thread(new ThreadStart(proxy.GenerateEvent));
                    thread.Start();
                }
                while(isSuperReader)
                {
                    Console.Title = "I am a SuperReader!";
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.SetWindowSize((int)(Console.LargestWindowWidth * 0.7), (int)(Console.LargestWindowHeight * 0.7));
                    Console.Clear();
                    Console.WriteLine("1. Newsletter");
                    int choice;
                    Int32.TryParse(Console.ReadLine(), out choice);

                    while(choice != 1)
                    {
                        Console.WriteLine("Wrong imput!");
                        Int32.TryParse(Console.ReadLine(), out choice);
                    }

                    string checkPermission;
                    if (!((checkPermission = proxy.Newsletter())).Contains("don't"))
                    {
  
                        Console.Clear();
                        Console.WriteLine("Currently in database:");
                        Console.WriteLine(checkPermission);
                    }
                    else
                    {
                        Console.WriteLine(checkPermission);
                    }
                }

                while (!isSuperReader)
                {
                    
                    Console.WriteLine("Choose an option:\n");
                    Console.WriteLine("1. Read");
                    Console.WriteLine("2. Read all");
                    Console.WriteLine("3. Modify");
                    Console.WriteLine("4. Delete");
                    Console.WriteLine("q. Exit");
                    Console.Write("Choice:");

                    string operationChoice = Console.ReadLine();
                    if (operationChoice == "q" || operationChoice == "Q")
                        break;

                    int number = 0;
                    Int32.TryParse(operationChoice, out number);
                    Console.WriteLine();

                    if (number == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("Wrong choice please try again!");
                        continue;
                    }


                    string retVal;
                    switch (number)
                    {
                        case 1:
                            Console.WriteLine(proxy.Read());
                            break;
                        case 2:
                            Console.WriteLine(proxy.ReadAll());
                            break;
                        case 3:
                            
                            if ((retVal = proxy.HasUpdatePermission()).Contains("don't"))
                            {
                                Console.Clear();
                                Console.WriteLine(retVal);
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine(retVal);
                                Console.Write("Enter id that you want to modify: ");
                                string id = Console.ReadLine();
                                Console.WriteLine();
                                int choice = 0;
                                bool wrongImput = true;
                                while (wrongImput)
                                {
                                    Console.Write("Enter number that signifies event with which you want to replace this event(1-10): ");
                                    Int32.TryParse(Console.ReadLine(), out choice);
                                    if (choice >= 1 && choice <= 10)
                                        wrongImput = false;
                                }
                                proxy.Modify(id, choice);
                            }
                            break;
                        case 4:
                            if ((retVal = proxy.HasUpdatePermission()).Contains("don't"))
                            {
                                Console.Clear();
                                Console.WriteLine(retVal);
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine(retVal);
                                Console.Write("Enter id that you want to delete: ");
                                string id = Console.ReadLine();
                                proxy.Delete(id);
                            }

                            break;
                        default:
                            Console.WriteLine("Not an option!");
                            break;
                    }
                }
                if(thread != null)
                    thread.Abort();

                proxy.Close();
                
			}

            
            return;
		}
	}
}

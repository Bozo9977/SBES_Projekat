using Datebase;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecurityService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class SecurityService : ISecurityService
	{


        ILoadBalancer proxy;

        INewsletter serviceCallback = null;
        public delegate void DBUpdateHandler(object sender, string eventDescription);
        public static event DBUpdateHandler UpdateEvent;
        DBUpdateHandler DBEventHandler = null;

        EventLog eventLog = new EventLog();
        

        public SecurityService()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            string address = "net.tcp://localhost:10000/LoadBalancerService";
            proxy = new LoadBalancerProxy(binding, address);

            if(!EventLog.SourceExists("SBESProjectLog"))
            {
                EventLog.CreateEventSource("SBESProjectLog", "SBESLog");
            }
        }


        public string Read()
        {
            string retValue;
            eventLog.Source = "SBESProjectLog";

            if(Thread.CurrentPrincipal.IsInRole("Read"))
            {                
                
                retValue = DatabaseAccess.ReadFromDatabase();
                Console.WriteLine($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called Read method SUCCESSFULLY.");
                eventLog.WriteEntry($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called Read method SUCCESSFULLY.", EventLogEntryType.SuccessAudit);
            }
            else
            {
                retValue = "You don't have Reader permission!";
                Console.WriteLine($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called Read method UNSUCCESSFULLY.");
                eventLog.WriteEntry($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called Read method UNSUCCESSFULLY.",EventLogEntryType.FailureAudit);
            }
            return retValue;
        }

        
        public string ReadAll()
        {
            string retValue;
            eventLog.Source = "SBESProjectLog";
            if (Thread.CurrentPrincipal.IsInRole("ReadAll"))
            {
                retValue = DatabaseAccess.ReadAllFromDatabase();
                Console.WriteLine($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called ReadAll method SUCCESSFULLY.");
                eventLog.WriteEntry($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called ReadAll method SUCCESSFULLY.", EventLogEntryType.SuccessAudit);
            }
            else
            {
                retValue = "You don't have Administrate permission!";
                Console.WriteLine($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called ReadAll method UNSUCCESSFULLY.");
                eventLog.WriteEntry($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called ReadAll method UNSUCCESSFULLY.",EventLogEntryType.FailureAudit);
            }

            return retValue;
        }

        public void GenerateEvent()
        {
            eventLog.Source = "SBESProjectLog";

            if (DatabaseAccess.GenerateEvent())
            {
                if(UpdateEvent!=null)
                UpdateEvent(this, "NEW EVENT GENERATED!\n" + DatabaseAccess.ReadAllFromDatabase());
                Console.WriteLine($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called GenerateEvent method SUCCESSFULLY.");
                eventLog.WriteEntry($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called GenerateEvent method SUCCESSFULLY.",EventLogEntryType.SuccessAudit);
            }
            else
            {
                Console.WriteLine($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called GenerateEvent method UNSUCCESSFULLY.");
                eventLog.WriteEntry($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called GenerateEvent method UNSUCCESSFULLY.",EventLogEntryType.FailureAudit);
            }

        }

        public string HasUpdatePermission()
        {
            string retVal;
            if (Thread.CurrentPrincipal.IsInRole("Modify") || Thread.CurrentPrincipal.IsInRole("Delete"))
            {
                retVal = DatabaseAccess.ReadFromDatabase();
            }
            else
            {
                retVal = "You don't have required permission to update database!";
            }
            return retVal;
        }

        
        public void Modify(string id, int newVersion)
        {
            eventLog.Source = "SBESProjectLog";
            try
            {
                var sid = ((WindowsIdentity)Thread.CurrentPrincipal.Identity).User;
                string eventSid = newVersion.ToString() + "_" + sid.ToString();
                if (proxy.Modify(id, eventSid))
                {
                    UpdateEvent(this, $"MODIFIED DATABASE ENTRY [ID={id}\n" + DatabaseAccess.ReadAllFromDatabase());
                    Console.WriteLine($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called Modify method SUCCESSFULLY.");
                    eventLog.WriteEntry($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called Modify method SUCCESSFULLY.",EventLogEntryType.SuccessAudit);
                }
                else
                {
                    Console.WriteLine($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called Modify method UNSUCCESSFULLY.");
                    eventLog.WriteEntry($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called Modify method UNSUCCESSFULLY.",EventLogEntryType.FailureAudit);
                }
                    
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: "+ e.Message);
                eventLog.WriteEntry("Modification of Database Entry failed due to reasons unknown!", EventLogEntryType.Error);
            }
        }

        public void Delete(string id)
        {
            eventLog.Source = "SBESProjectLog";
            try
            {
                string sid = ((WindowsIdentity)Thread.CurrentPrincipal.Identity).User.ToString();
                if (proxy.Delete(id, sid))
                {
                    UpdateEvent(this, $"DELETED DATABASE ENTRY [ID={id}]\n" + DatabaseAccess.ReadAllFromDatabase());
                    Console.WriteLine($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called Delete method SUCCESSFULLY.");
                    eventLog.WriteEntry($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called Delete method SUCCESSFULLY.",EventLogEntryType.SuccessAudit);
                }
                else
                {
                    Console.WriteLine($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called Delete method UNSUCCESSFULLY.");
                    eventLog.WriteEntry($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} called Delete method UNSUCCESSFULLY.",EventLogEntryType.FailureAudit);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                eventLog.WriteEntry("Deletion of Database Entry failed due to reasons unknown!", EventLogEntryType.Error);
            }
        }


       

        public string Newsletter()
        {
            eventLog.Source = "SBESProjectLog";
            string retVal;
            if (Thread.CurrentPrincipal.IsInRole("SuperRead"))
            {
                try
                {
                    retVal = DatabaseAccess.ReadAllFromDatabase();
                    serviceCallback = OperationContext.Current.GetCallbackChannel<INewsletter>();
                    DBEventHandler = new DBUpdateHandler(InvokeCallback);
                    UpdateEvent += DBEventHandler;
                    eventLog.WriteEntry($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} identified as SuperReader SUCCESSFULLY", EventLogEntryType.Information);
                }catch(Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                    retVal = "";
                    eventLog.WriteEntry("Newsletter identification of SuperReader FAILED due to reasons unknown!", EventLogEntryType.Error);
                }
            }
            else
            {
                retVal = "You don't have required permission!";
                eventLog.WriteEntry($"{((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name} ISN'T identified as SuperReader SUCCESSFULLY", EventLogEntryType.FailureAudit);
            }
            return retVal;
        }
        
        public void InvokeCallback(object sender, string eventDescription)
        {
            try
            {
                serviceCallback.SendUpdates(eventDescription);
            }
            catch (Exception e) { }
        }


    }
}

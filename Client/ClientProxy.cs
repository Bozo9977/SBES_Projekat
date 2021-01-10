using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    public class ClientProxy : DuplexChannelFactory<ISecurityService>, ISecurityService, IDisposable
    {
        ISecurityService factory;

        public ClientProxy(NetTcpBinding binding, string address, InstanceContext instanceContext) : base(instanceContext, binding, address)
        {
            factory = this.CreateChannel();
        }

        public string Read()
        {
            string value;
            try
            {
                value = factory.Read();
            }catch(Exception e)
            {
                Console.WriteLine("Error: "+e.Message);
                value = null;
            }
            return value;
        }

        public string ReadAll()
        {
            string value;
            try
            {
                value = factory.ReadAll();
            }
            catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message + "\n" + e.StackTrace);
                value = null;
            }
            return value;
        }

        public void GenerateEvent()
        {
            bool running = true;

            while(running)
            {

                try
                {
                    factory.GenerateEvent();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message + "\n" +e.StackTrace);
                    running = false;
                }

                Thread.Sleep(10000);
            }
           
        }

        public string HasUpdatePermission()
        {
            string retVal;
            try
            {
                retVal = factory.HasUpdatePermission();
            }catch(Exception e)
            {
                Console.WriteLine("Error: "+e.Message);
                retVal = null;
            }
            return retVal;
        }


        public void Modify(string id, int newVersion)
        {
            try
            {
                factory.Modify(id, newVersion);
            }catch(Exception e)
            {
                Console.WriteLine("Error: "+e.Message);
            }
        }

        public void Delete(string id)
        {
            try
            {
                factory.Delete(id);
            }catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }

        public string Newsletter()
        {
            string retVal;

            try
            {
                retVal = factory.Newsletter();
            }catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                retVal = "";
            }

            return retVal;
        }



    }
}

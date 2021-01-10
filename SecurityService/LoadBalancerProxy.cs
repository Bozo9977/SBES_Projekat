using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SecurityService
{
    public class LoadBalancerProxy : ChannelFactory<ILoadBalancer>, IDisposable, ILoadBalancer
    {
        ILoadBalancer factory;

        public LoadBalancerProxy(NetTcpBinding binding, string address): base(binding, address)
        {
            
            factory = this.CreateChannel();
        }

        public bool Delete(string id, string sid)
        {
            bool success = false;

            try
            {
                success = factory.Delete(id, sid);
            }catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }

            return success;
        }

        public bool Modify(string id, string eventSid)
        {
            bool success = false;
            try
            {
                success = factory.Modify(id, eventSid);
            }catch(Exception e)
            {
                Console.WriteLine("Error: "+e.Message);
            }

            return success;
        }

        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }
            this.Close();
        }
    }
}

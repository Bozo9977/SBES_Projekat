using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    [ServiceContract]
    public interface ILoadBalancer
    {
        [OperationContract]
        bool Modify(string id, string newEvent);

        [OperationContract]
        bool Delete(string id, string sid);
    }
}

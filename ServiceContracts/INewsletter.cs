using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    
    public interface INewsletter
    {
        [OperationContract(IsOneWay = true)]
        void SendUpdates(string updatedDatabase);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
	[ServiceContract(SessionMode = SessionMode.Required,
      CallbackContract = typeof(INewsletter))]
	public interface ISecurityService
	{
		[OperationContract(IsOneWay = false)]
		string Read();
        [OperationContract(IsOneWay = false)]
        string ReadAll();
        [OperationContract(IsOneWay = false)]
        void GenerateEvent();
        [OperationContract(IsOneWay = false)]
        string HasUpdatePermission();
        [OperationContract(IsOneWay = false)]
        void Modify(string id, int newVersion);
        [OperationContract(IsOneWay = false)]
        void Delete(string id);

        [OperationContract(IsOneWay = false, IsInitiating = true)]
        string Newsletter();
	}
}

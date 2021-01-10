using Datebase;
using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoadBalancer
{
    public class Parameter
    {
        public ModifyType type;
        public string id;
        public string newEvent;

        public Parameter(ModifyType mType, string databaseID, string newVersion)
        {
            type = mType;
            id = databaseID;
            newEvent = newVersion;
        }

        public override string ToString()
        {
            return type.ToString() + " " + id + " " + newEvent;
        }
    }
        
    

    public class LoadBalancerService : ILoadBalancer
    {
        const int numberOfThreads = 4;
        public static Thread[] threads = new Thread[numberOfThreads];
        public static bool[] isBusy = new bool[numberOfThreads];
        public static Parameter[] parameters = new Parameter[numberOfThreads];
        public static bool[] activeThreads = new bool[numberOfThreads];

        public LoadBalancerService()
        {
            for(int i = 0; i<numberOfThreads; i++)
            {
                int current = i;
                threads[i] = new Thread(() => Worker(current));
                isBusy[i] = true;
                activeThreads[i] = false;

                threads[i].Start();
                isBusy[i] = false;
            }
        }

      
        public bool Modify(string id, string newEvent)
        {
            bool canUpdate = DatabaseAccess.HasPermissionToUpdate(id, newEvent.Split('_')[1]);

            if(canUpdate)
                ChooseWorker(ModifyType.MODIFY, id, newEvent);

            Thread.Sleep(1500);
            return canUpdate;
        }

        public bool Delete(string id, string sid)
        {
            bool canUpdate = DatabaseAccess.HasPermissionToUpdate(id, sid);

            if (canUpdate)
                ChooseWorker(ModifyType.DELETE, id, "");

            Thread.Sleep(1500);
            return canUpdate;
        }

        public void ChooseWorker(ModifyType type, string id, string newEvent)
        {
            bool workerAvailable = false;
            
            Parameter param = new Parameter(type, id, newEvent);

            while (!workerAvailable)
            {

                int counter = 0;

                foreach (var item in isBusy)
                {
                    if (item)
                        counter++;
                    else
                        break;
                }

                if (counter < numberOfThreads)
                {
                    workerAvailable = true;
                    parameters[counter] = param;
                    activeThreads[counter] = true;
                }
                else
                {
                    Thread.Sleep(300);
                }
            }
        }



        public void Worker(int workerID)
        {
            while (true)
            {
                if (activeThreads[workerID])
                {
                    isBusy[workerID] = true;
                    if(parameters[workerID].type == ModifyType.MODIFY)
                    {
                        if (DatabaseAccess.Modify(parameters[workerID].id, parameters[workerID].newEvent))
                            Console.WriteLine("UPDATE SUCCESS!!!");
                        else
                            Console.WriteLine("UPDATE FAILURE!!! -- probably wrong id");
                        
                    }else if(parameters[workerID].type == ModifyType.DELETE)
                    {
                        if (DatabaseAccess.Delete(parameters[workerID].id))
                            Console.WriteLine("UPDATE SUCCESS!!!");
                        else
                            Console.WriteLine("UPDATE FAILURE!!! -- probably wrong id");
                    }
                    isBusy[workerID] = false;
                    activeThreads[workerID] = false;
                }
                Thread.Sleep(1000);
            }
        }

        
    }
}

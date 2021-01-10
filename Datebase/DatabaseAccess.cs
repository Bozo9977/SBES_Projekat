using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Datebase
{
    public static class DatabaseAccess
    {
        private static readonly object x = new object();

        public static object RoleConfigFile { get; private set; }

        

        public static string ReadFromDatabase()
        {
            string retValue;
            try
            {
                lock (x)
                {
                    string sid = ((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).User.ToString();
                    string[] lines = File.ReadAllLines("../../../Datebase/bin/Debug/Database.txt");

                    List<string> toPrint = lines.ToList().FindAll(x => (x.Split(';')[1]).Split('=')[1] == sid);
                    if (toPrint.Count != 0)
                        retValue = String.Join("\n", toPrint.ToArray());
                    else
                    {
                        retValue = "You don't have any events in database!";
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine("Error: "+e.Message);
                retValue = "Error in ReadFromDatabase!";
            }
            
            return retValue;
        }

        public static string ReadAllFromDatabase()
        {
            string retValue;
            try
            {
                lock (x)
                {
                    string[] lines = File.ReadAllLines("../../../Datebase/bin/Debug/Database.txt");

                    if (lines.Count() != 0)
                        retValue = String.Join("\n", lines);
                    else
                    {
                        retValue = "Database is currently empty!";
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                retValue = "Error!";
            }
            
            return retValue;
        }

        public static bool GenerateEvent()
        {

            Random random = new Random();
            int randValue = random.Next(1, 11);
            string randEvent = "EVENT_" + randValue;
            bool success = false;

            try
            {
                lock (x)
                {
                    string eventRandom = (string)Events.ResourceManager.GetObject(randEvent);


                    string write;
                    if (!File.Exists("../../../Datebase/bin/Debug/Database.txt"))
                    {

                        int id = 0;
                        write = "ID=" + id.ToString() + ";SID=" + ((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).User + ";Name=" + ((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name + ";TimeStamp=" + DateTime.Now + ";Event=" + eventRandom + "\n";

                    }
                    else
                    {
                        int id;

                        string[] lines = File.ReadAllLines("../../../Datebase/bin/Debug/Database.txt");
                        id = int.Parse(((lines[lines.Length - 1]).Split(';')[0]).Split('=')[1]);
                        id++;
                        write = "ID=" + id.ToString() + ";SID=" + ((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).User + ";Name=" + ((WindowsIdentity)(Thread.CurrentPrincipal.Identity)).Name + ";TimeStamp=" + DateTime.Now + ";Event=" + eventRandom + "\n";


                    }

                    File.AppendAllText("../../../Datebase/bin/Debug/Database.txt", write);
                    success = true;
                }
            }catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                success = false;
            }
            return success;
        }

        public static bool Modify(string id, string eventNum)
        {
            bool retVal = false;

            try
            {
                lock (x)
                {
                    string[] linesArray = File.ReadAllLines("../../../Datebase/bin/Debug/Database.txt");
                    List<string> lines = linesArray.ToList();

                    string sid = eventNum.Split('_')[1];
                    

                    List<string> toPrint = new List<string>();

                    foreach (var line in lines)
                    {
                        if (((line.Split(';')[0]).Split('=')[1] == id) && ((line.Split(';')[1]).Split('=')[1] == sid))
                        {
                            retVal = true;
                            string[] elementsOfLine = line.Split(';');
                            elementsOfLine[3] = "TimeStamp=" + DateTime.Now;
                            elementsOfLine[4] = "Event=" + (string)Events.ResourceManager.GetObject("EVENT_" + eventNum.Split('_')[0]);
                            toPrint.Add(String.Join(";", elementsOfLine));
                        }
                        else
                        {
                            toPrint.Add(line);
                        }
                    }

                    File.WriteAllLines("../../../Datebase/bin/Debug/Database.txt", toPrint.ToArray());
                }
            }catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                retVal = false;
            }
            
            return retVal;
        }
        
        public static bool Delete(string id)
        {
            bool retVal = false;

            try
            {
                lock (x)
                {
                    string[] lines = File.ReadAllLines("../../../Datebase/bin/Debug/Database.txt");
                    List<string> toPrint = new List<string>();

                    foreach (var line in lines)
                    {
                        if ((line.Split(';')[0]).Split('=')[1] == id)
                        {
                            retVal = true;
                            continue;
                        }
                        else
                            toPrint.Add(line);
                    }

                    File.WriteAllLines("../../../Datebase/bin/Debug/Database.txt", toPrint.ToArray());
                }
            }catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                retVal = false;
            }
            
            
            return retVal;
        }


        public static bool HasPermissionToUpdate(string id, string sid)
        {
            bool retVal = false;
            try
            {
                lock (x)
                {
                    string[] lines = File.ReadAllLines("../../../Datebase/bin/Debug/Database.txt");

                    foreach (string line in lines)
                    {
                        if (line.Split(';')[0].Split('=')[1] == id && line.Split(';')[1].Split('=')[1] == sid)
                            retVal = true;
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return retVal;
        }

    }
}

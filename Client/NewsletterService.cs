using ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class NewsletterService : INewsletter
    {
        public void SendUpdates(string updatedDatabase)
        {
            Console.Clear();
            Console.WriteLine(updatedDatabase);
        }
    }
}

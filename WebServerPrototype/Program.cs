using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ChatPrograms
{
   
    class BusicWaitHandle
    {
        static void Main()
        {
            WebServer server = new WebServer(@"http://localhost:51111/myapp/");
            try
            {
                server.Start();

                Console.ReadLine();
            }
            finally
            {
                server.Stop();
            }
        }
    }

}

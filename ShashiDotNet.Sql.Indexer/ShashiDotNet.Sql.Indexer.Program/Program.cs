using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShashiDotNet.Sql.Indexer.Program
{
    class Program
    {
        static void Main(string[] args)
        {
            int numThreads = 100;
            string dbName = "MicrosoftDynamicsAX";

            // Test1.Test();
            ThreadData.RebuildPct = 90;
            ThreadData.ReorgPct = 80;

            if(args != null && args.Count() >= 3)
            {
                dbName = args[0];

                double rebuildPct, reOrdPct;
                if (Double.TryParse(args[1], out rebuildPct) == false)
                    rebuildPct = 100;
                else
                    ThreadData.RebuildPct = rebuildPct;

                if (Double.TryParse(args[2], out reOrdPct) == false)
                    reOrdPct = 100;
                else
                    ThreadData.ReorgPct = reOrdPct;

                if (int.TryParse(args[3], out numThreads) == false)
                    numThreads = 40;
                //ThreadData.RebuildPct = (double)args[1];
                //ThreadData.ReorgPct = (double)args[2];
            }

            // Start the ThreadStaticAttribute first
            TableReader.StartThreads(numThreads);

            // Put the data in the Tables
            GetTables(dbName);
            ThreadData.TablesAddedComplete(); // sets that there are no more tables to be added;

            // Wait for the threads to complete
            Console.WriteLine("Wait for threads to compelte");
            while (true)
            {
                if (ThreadData.IsComplete())
                {
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(2000);
                }
            }
            /*
            while (true)
            {
                var threadsUpdates = ThreadData.updateOrReadThreadsFinished();
                if(threadsUpdates == 10)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Waiting for threads");
                    System.Threading.Thread.Sleep(1500);
                }
            }
             * */
        }

        private static void GetTables(string dbName)
        {
            Server srv = new Server();
            var dbAx = srv.Databases.OfType<Database>().Where(db => db.Name.Equals(dbName)).FirstOrDefault();
            Console.WriteLine(dbAx.Name);

            var tables = dbAx.Tables.OfType<Table>();//.Take(100);
            foreach (var tab in tables)
            {
                // Console.WriteLine(tab.Name);
                // IndexTable(dbName, tab.Name);
                ThreadData.AddTableToIndex(dbName, tab.Name);
            }
            Console.WriteLine("Tables added to queue");
        }
    }
}

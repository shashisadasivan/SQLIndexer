using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShashiDotNet.Sql.Indexer.Program
{
    class TableReader
    {
        public static void StartThreads(int threads)
        {
            for (int i = 0; i < threads; i++)
            {
                new Task(IndexTable).Start();
                Console.WriteLine("Starting thread: " + i.ToString());
            }
        }

        public static void IndexTable()
        {
            Console.WriteLine("Starting Thread");
            // This is a job that polls the Tables to index
            while (true)
            {
                // Check if job needs to quit
                if (ThreadData.IsComplete())
                {
                    Console.WriteLine("Quitting thread.....complete");
                    return;
                }
                var tableData = ThreadData.GetTable();
                if (tableData != null)
                {
                    // Console.WriteLine("Indexing table: " + tableData.TableName);
                    // Index all in this table
                    var tableIndexer = new TableIndexer(tableData.DBName, tableData.TableName, 90.0, 80.0);
                    tableIndexer.ReIndex();
                    ThreadData.TableUpdated(true); // MArks this table as complete 
                }
            }
            /*
            var guid = Guid.NewGuid().ToString();
            for (int i = 0; i < 10; i++)
            {
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Test : " + guid);
            }

            ThreadData.updateOrReadThreadsFinished(true);
             */
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Data;

namespace ShashiDotNet.Sql.Indexer.Program
{
    class Test1
    {
        public static void Test()
        {
            // Connect();

            GetTables("MicrosoftDynamicsAX");
        }

        public static void Connect()
        {
            Console.WriteLine("Test");
            Server srv = new Server();
            Console.WriteLine(srv.Information.Version);

            foreach (Database db in srv.Databases)
            {
                Console.WriteLine(db.Name);
            }


        }
        private static void GetTables(string dbName)
        {
            Server srv = new Server();
            var dbAx = srv.Databases.OfType<Database>().Where(db => db.Name.Equals(dbName)).FirstOrDefault();
            Console.WriteLine(dbAx.Name);

            var tables = dbAx.Tables.OfType<Table>().Take(15);
            foreach (var tab in tables)
            {
                Console.WriteLine(tab.Name);
                IndexTable(dbName, tab.Name);

                
            }
        }

        private static void IndexTable(string dbName, string tableName)
        {
            Server srv = new Server();
            var dbAx = srv.Databases.OfType<Database>().Where(db => db.Name.Equals(dbName)).FirstOrDefault();
            Console.WriteLine(dbAx.Name);

            var table = dbAx.Tables.OfType<Table>()
                            .Where(t => t.Name.Equals(tableName))
                            .FirstOrDefault();
            
            var indxs = table.Indexes.OfType<Index>().ToList();
            foreach (var idx in indxs)
            {
                Console.WriteLine("--" + idx.Name);
                // var idxProps = idx.Properties.OfType<Property>().ToList();
                var fragsDataTable = idx.EnumFragmentation();//FragmentationOption.Detailed);
                if (fragsDataTable.Rows.Count > 0)
                {
                    double frag = 0;
                    Double.TryParse(fragsDataTable.Rows[0]["AverageFragmentation"].ToString(), out frag);
                    if (frag > 0.0)
                    {
                        Console.WriteLine("Frag%: " + frag.ToString());
                    }
                }
            }
        }


    }
}

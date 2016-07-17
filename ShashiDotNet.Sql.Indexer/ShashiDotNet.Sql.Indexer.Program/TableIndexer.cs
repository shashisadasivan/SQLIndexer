using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShashiDotNet.Sql.Indexer.Program
{
    public class TableIndexer
    {
        public string DBName { get; set; }
        public string TableName { get; set; }
        public double RebuildPct { get; set; }
        public double ReOrganizePct { get; set; }
        
        public TableIndexer(
            string dbName,
            string tableName,
            double rebuildPct,
            double reOrganizePct)
        {
            this.DBName = dbName;
            this.TableName = tableName;
            this.RebuildPct = rebuildPct;
            this.ReOrganizePct = reOrganizePct;
        }

        public void ReIndex()
        {

            Server srv = new Server();
            var dbAx = srv.Databases.OfType<Database>().Where(db => db.Name.Equals(this.DBName)).FirstOrDefault();
            // Console.WriteLine(dbAx.Name);

            var table = dbAx.Tables.OfType<Table>()
                            .Where(t => t.Name.Equals(this.TableName))
                            .FirstOrDefault();

            var indxs = table.Indexes.OfType<Index>().ToList();
            foreach (var idx in indxs)
            {
                // Console.WriteLine("--" + idx.Name);
                // var idxProps = idx.Properties.OfType<Property>().ToList();
                var fragsDataTable = idx.EnumFragmentation();//FragmentationOption.Detailed);
                if (fragsDataTable.Rows.Count > 0)
                {
                    double frag = 0;
                    Double.TryParse(fragsDataTable.Rows[0]["AverageFragmentation"].ToString(), out frag);
                    if (frag > this.RebuildPct)
                    {
                        Console.WriteLine("ReBuild " + table.Name  + "Frag%: " + frag.ToString());
                        System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
                        st.Start();
                        idx.Rebuild();
                        st.Start();
                        Console.WriteLine("Rebuild complete: " + st.Elapsed.TotalSeconds.ToString());
                    }
                    else if (frag > this.ReOrganizePct)
                    {
                        Console.WriteLine("ReOrganize " + table.Name + " Frag%: " + frag.ToString());
                        
                        System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
                        st.Start();
                        idx.Reorganize();
                        st.Start();
                        Console.WriteLine("ReOrganize complete: " + st.Elapsed.TotalSeconds.ToString());
                    }
                    else
                    {
                        // Console.WriteLine("Index is ok: " + frag.ToString() + "%");
                    }
                }
                else
                {
                    Console.WriteLine("No information Found");
                }
            }
        }
        
    }
}

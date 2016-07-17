using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ShashiDotNet.Sql.Indexer.Program
{
    public class ThreadData
    {
        public static Double RebuildPct = 95.0;
        public static Double ReorgPct = 94.0;

        private static int threadsFinished = 0;
        public static readonly object locker = new object();
        public static int updateOrReadThreadsFinished(bool update = false)
        {
            if (update)
            {
                Console.WriteLine("updateing threads finished");
                lock (locker)
                {
                    threadsFinished++;
                }
            }
            return threadsFinished;
        }
        private static bool TablesFoundComplete = false;
        private static int TablesAdded = 0;
        private static int TablesUpdated = 0;

        public static readonly object TablesAddedLocker = new object();
        public static readonly object TablesUpdatedLocker = new object();


        public static ConcurrentBag<TableData> tables = new ConcurrentBag<TableData>();
        public ThreadData()
        {
            
        }

        public static void AddTableToIndex(string dbName, string tableName)
        {
            lock (TablesAddedLocker)
            {
                tables.Add(new TableData() { DBName = dbName, TableName = tableName});
                TablesAdded++;
            }
        }
        public static void TablesAddedComplete()
        {
            lock (TablesAddedLocker)
            {
                TablesFoundComplete = true;
            }
        }

        public static int TableUpdated(bool update = false)
        {
            if (update)
            {
                lock (TablesUpdatedLocker)
                {
                    TablesUpdated++;
                }
            }
            return TablesUpdated;
        }

        private static object TableGetLocker = new object();
        public static TableData GetTable()
        {
            TableData tableData = null;
            lock (TableGetLocker)
            {
                tables.TryTake(out tableData);
            }
            return tableData;
        }

        public static bool IsComplete()
        {
            if (TablesFoundComplete == true
                && TablesAdded == TablesUpdated)
            {
                return true;
            }
            return false;
        }
        
    }

    public class TableData
    {
        public string DBName { get; set; }
        public string TableName { get; set; }
        
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace ParallelFileDeleter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2 && Directory.Exists(args[0]) && Directory.Exists(args[1]))
            {
                Globals.inputDirectory = args[0];
                Globals.logDirectory = args[1];
                Globals.logFile = Globals.logDirectory + "\\log.txt";
            }
            else
            {
                Console.WriteLine("Usage = ParallelFileDeleter.exe[Directory Containing Queue.csv][Directory to save log]");
                return;
            }

            var inputDirectory = Globals.inputDirectory;
            var logDirectory = Globals.logDirectory;
            var logFile = Globals.logFile;

            //Kick off the pull from logqueue task:
            ThreadPool.QueueUserWorkItem(o => PullFromLogQueue(logFile));

            //Queue the work items
            var concurrentQueue = QueueWorkItems(inputDirectory);

            //Process each item concurrently...
            Parallel.ForEach(concurrentQueue, new ParallelOptions { MaxDegreeOfParallelism = System.Environment.ProcessorCount }, q =>
            {
                string currentItem;

                if (concurrentQueue.TryDequeue(out currentItem))
                {
                    try
                    {
                        File.Delete(currentItem);
                        Console.WriteLine("Deleted" + currentItem);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        LogQueue.blockingcollection.Add("Error deleting : " + currentItem + "\r\n" + e.Message);
                    }
                }
            }
            );

        }

        #region Functions
        /// <summary>
        /// Queues work items into a ConcurrentQueue Of Strings
        /// </summary>
        /// <param name="inputDirectory"></param>
        /// <returns>A ConcurrentQueue of strings</returns>
        static ConcurrentQueue<string> QueueWorkItems(string inputDirectory)
        {
            if (Directory.Exists(inputDirectory))
            {
                Console.WriteLine("Input Directory Exists:{0}", inputDirectory);
                var csvFileNameArray = Directory.EnumerateFiles(inputDirectory, "*.csv");
                ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

                foreach (var csvFile in csvFileNameArray)
                {
                    string[] lines = File.ReadAllLines(csvFile);
                    foreach (string line in lines)
                    {
                        queue.Enqueue(line);
                    }

                }
                return queue;
            }
            else
            {
                throw new InvalidOperationException("Directory Doesn't Exist");
            }
        }
        /// <summary>
        /// Pulls a message from a blocking collection and writes to a file
        /// </summary>
        /// <param name="logFile">The log file to log to</param>
        static void PullFromLogQueue(string logFile)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (string value in LogQueue.blockingcollection.GetConsumingEnumerable())
                {
                    Console.WriteLine(value);

                    using (StreamWriter swg = new StreamWriter(logFile, true))
                    {
                        swg.WriteLine(value);
                        swg.Dispose();
                    }
                }
            });
        }
        #endregion


    }
}

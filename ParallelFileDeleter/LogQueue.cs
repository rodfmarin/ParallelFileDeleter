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
    public static class LogQueue
    {
        public static BlockingCollection<string> blockingcollection = new BlockingCollection<string>();
    }
}

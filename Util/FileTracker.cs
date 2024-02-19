using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MCMonitor
{
    internal static class FileTracker
    { 
        static List<string> pendingDelete = new List<string>();
        
        public static void DeleteAfter(string path, int deleteAfter = 10)
        {
            lock (pendingDelete)
                 pendingDelete.Add(path);

            Task.Delay(deleteAfter * 1000).ContinueWith(t => {
                try { File.Delete(path); } catch { }
                lock (pendingDelete)
                    pendingDelete.Remove(path);
            });
        }

        public static void Purge()
        {
            lock (pendingDelete)
            {
                foreach (var path in pendingDelete)
                    try { File.Delete(path); } catch { }
                pendingDelete.Clear();
            }
        }
    }
}

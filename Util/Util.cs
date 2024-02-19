using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MCMonitor
{
    public static class Util
    {
        [DllImport("ole32")]
        private static extern int CLSIDFromProgIDEx([MarshalAs(UnmanagedType.LPWStr)] string lpszProgID, out Guid lpclsid);

        [DllImport("oleaut32")]
        private static extern int GetActiveObject([MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, IntPtr pvReserved, [MarshalAs(UnmanagedType.IUnknown)] out object ppunk);


        public static object GetActiveObject(string progId, bool throwOnError = false)
        {
            if (progId == null)
                throw new ArgumentNullException(nameof(progId));

            var hr = CLSIDFromProgIDEx(progId, out var clsid);
            if (hr < 0)
            {
                if (throwOnError)
                    Marshal.ThrowExceptionForHR(hr);

                return null;
            }

            hr = GetActiveObject(clsid, IntPtr.Zero, out var obj);
            if (hr < 0)
            {
                if (throwOnError)
                    Marshal.ThrowExceptionForHR(hr);

                return null;
            }
            return obj;
        }


        public static bool IsDirectoryWritable(string dirPath)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
                { }
                return true;
            }
            catch { }
            return false;
        }

        internal static Stream getEmbeddedResourceStream(string resourcePath)
        {
            Stream stream = null;
            resourcePath = resourcePath.Replace('\\', '.').Replace('/', '.');
            try
            {
                stream = findResourceStream(Assembly.GetEntryAssembly(), resourcePath);
                if (stream == null) stream = findResourceStream(Assembly.GetExecutingAssembly(), resourcePath);
                if (stream == null) return null;

                return stream;
            }
            catch { }
            return null;
        }
        
        internal static bool ExtractResource(string resource, string fileName, bool overwrite = true)
        {
            Stream resStream = getEmbeddedResourceStream(resource);
            if (resStream == null)
                return false;
            try
            {
                FileInfo fi = new FileInfo(fileName);
                if (overwrite || !fi.Exists || fi.Length != resStream.Length)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(fileName)));
                    using (StreamWriter sw = new StreamWriter(fileName))
                        resStream.CopyTo(sw.BaseStream);
                }
                else
                {
                    //Console.WriteLine($"Resource '{resource}' is up to date ('{fileName}')");
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (resStream != null) resStream.Close();
            }
        }

        private static Stream findResourceStream(Assembly assembly, string resourcePath)
        {
            try
            {
                Stream stream = assembly.GetManifestResourceStream(resourcePath);
                if (stream == null)
                {
                    string res = assembly.GetManifestResourceNames().Where(r => r.EndsWith(resourcePath)).FirstOrDefault();
                    if (res != null)
                        stream = assembly.GetManifestResourceStream(res);
                }
                return stream;
            }
            catch { }
            return null;
        }
    }
}


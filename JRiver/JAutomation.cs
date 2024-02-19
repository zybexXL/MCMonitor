using MCMonitor;
using MediaCenter;
using System;
using System.Runtime.InteropServices;

namespace JRiver
{
    // talks to JRiver - opens JRiver as a background app if needed (OLE automation)
    // gets list of playlists, list of fields
    // reads and writes Fields from Files (movies)
    public class JAutomation: IDisposable
    {
        private MCAutomation jr;

        public bool Connected { get; set; }
        public string MCVersion { get; set; }
        public string Library { get; set; }
        public int APIlevel { get; set; }
        public string LibraryPath { get; set; }
        public bool ReadOnly { get { return false; } set { } }

        public event EventHandler<JAutomationEvent> MCEvent;


        public JAutomation()
        {
        }

        ~JAutomation()
        {
            Disconnect();
        }

        public void Dispose()
        {
            Disconnect();
        }

        public bool Connect(bool AllowNewMCInstance = false)
        {
            Connected = CheckConnection();
            if (Connected)
                return true;

            Connected = ConnectExistingInstance();
            if (!Connected && AllowNewMCInstance)
                Connected = ConnectNewInstance();
           
            if (Connected)
                GetLibraryInfo();
            else
                Disconnect();   // release COM object

            RegisterEventHandler(Connected);
            return Connected;
        }

        private bool ConnectExistingInstance()
        {
            //Logger.Log("Connecting to JRiver");
            try
            {
                // connect to existing instance
                //Logger.Log("Connect: getting existing MediaCenter instance");
                jr = (MCAutomation)Util.GetActiveObject("MediaJukebox Application");
                return CheckConnection();
            }
            catch { } // Logger.Log("JRiverAPI.Connect() - MediaCenter is probably not running"); }
            return false;
        }

        private bool ConnectNewInstance()
        {
            try
            {
                Logger.Log("Connect: creating new MediaCenter instance");
                jr = new MCAutomation();
                if (CheckConnection())
                    return true;

                Logger.Log("Connect via MCAutomation object failed!");
            }
            catch (Exception ex) { Logger.Log(ex, "JRiverAPI.Connect() - failed to create new instance"); }
            return false;
        }

        private void RegisterEventHandler(bool register)
        {
            if (jr == null) return;

            // unsubscribe
            jr.FireMJEvent -= JAutomation_FireMJEvent;

            // [re]subscribe
            if (register)
                jr.FireMJEvent += JAutomation_FireMJEvent;
        }

        private void JAutomation_FireMJEvent(string bstrType, string bstrParam1, string bstrParam2)
        {
            if (MCEvent == null) return;

            var ev = new JAutomationEvent(bstrType, bstrParam1, bstrParam2);
            try { MCEvent?.Invoke(this, ev); } catch { }
        }

        public void Disconnect()
        {
            try
            {
                Connected = false;
                RegisterEventHandler(false);
                if (jr != null)
                    Marshal.FinalReleaseComObject(jr);
                jr = null;
            }
            catch { }
        }

        // internal MCWS request
        public string WebRequest(string request, string postdata = "")
        {
            try
            {
                return jr?.ProcessWebService(request, postdata ?? "");
            }
            catch (Exception ex) { Logger.Log(ex, "WebRequest"); }
            return null;
        }

        bool CheckConnection()
        {
            try
            {
                //Logger.Log("Checking connection");
                if (jr == null) return false;
                MCVersion = jr.GetVersion().Version;    // force an API call

                return true;
            }
            catch { } // (Exception ex) { Logger.Log(ex, "JRiverAPI.CheckConnection()"); }
            return false;
        }

        private void GetLibraryInfo()
        {
            try
            {
                MCVersion = jr.GetVersion().Version;
                APIlevel = jr.IVersion;
                string path = null;
                string library = null;
                jr.GetLibrary(ref library, ref path);
                Library = library;
                LibraryPath = path;

                Logger.Log($"MediaCenter version {MCVersion}, APILevel={APIlevel}");
                Logger.Log($"MediaCenter library is '{Library}', path={path}");
            }
            catch { }
        }
    }
}

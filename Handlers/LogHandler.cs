using System.IO;
using System.Text.RegularExpressions;

namespace MCMonitor
{
    internal class LogHandler : BaseHandler
    {
        string template = null;
        string outputpath;

        public LogHandler(Config config) : base(config)
        {
            outputpath = config[ConfigProperty.LogPath];
            if (string.IsNullOrEmpty(outputpath))
                isEnabled = false;
            else
            {
                string folder = Path.GetDirectoryName(Path.GetFullPath(outputpath));
                if (!Util.IsDirectoryWritable(folder))
                {
                    Logger.Log($"Non-writtable folder, JSON output disabled: {folder}");
                    isEnabled = false;
                }
            }

            if (isEnabled)
            {
                template = config[ConfigProperty.LogLine];
                if (string.IsNullOrEmpty(template))
                    template = "$time,$sequence,$type,$arg1,$arg2,$state,$currkey,$nextkey";

                NeedsPlaybackInfo = Regex.IsMatch(template, @"\$(state|currkey|nextkey|json)|\$i\[", RegexOptions.IgnoreCase);
                NeedsDetails = Regex.IsMatch(template, @"\$json|\$\$?\[", RegexOptions.IgnoreCase);
            }
        }

        public override bool Process(MCEventInfo e)
        {
            if (!isEnabled) return false;

            try 
            {
                string line = FillTemplate(template, e);
                File.AppendAllText(outputpath, line + "\r\n");
                return true;
            } 
            catch { }
            return false;
        }
    }
}

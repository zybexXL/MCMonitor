using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MCMonitor
{
    internal class JsonHandler : BaseHandler
    {
        List<MCEventInfo> MCEvents = new List<MCEventInfo>();

        string outputpath;
        int jsoncount;

        public JsonHandler(Config config) : base(config)
        {
            outputpath = config[ConfigProperty.JsonPath];
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
                NeedsPlaybackInfo = true;
                NeedsDetails = config[ConfigProperty.JsonDetails] == "1";
                if (!int.TryParse(config[ConfigProperty.JsonCount], out jsoncount) || jsoncount < 1)
                    jsoncount = 1;
                if (jsoncount > 100)
                    jsoncount = 100;
            }
        }

        public override bool Process(MCEventInfo e)
        {
            if (!isEnabled) return false;

            MCEvents.Insert(0, e);
            if (MCEvents.Count > jsoncount)
                MCEvents.RemoveAt(jsoncount);

            try {
                string json = JsonSerializer.Serialize(MCEvents, jsonOptions);
                File.WriteAllText(outputpath, json);
                return true;
            } 
            catch { }
            return false;
        }
    }
}

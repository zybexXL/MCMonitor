using System;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MCMonitor
{
    internal abstract class BaseHandler
    {
        public bool NeedsPlaybackInfo { get; set; }
        public bool NeedsDetails { get; set; }

        public Config config { get; protected set; }

        public virtual bool isEnabled { get; protected set; } = true;

        private int DeleteDelay;
        protected JsonSerializerOptions jsonOptions = new JsonSerializerOptions() { WriteIndented = true };


        public BaseHandler(Config config)
        { 
            this.config = config;
            if (!int.TryParse(config[ConfigProperty.DeleteDelay], out DeleteDelay))
                DeleteDelay = 10;

            jsonOptions.Converters.Add(new JsonDateConverter());
        }

        abstract public bool Process(MCEventInfo e);

        protected string FillTemplate(string template, MCEventInfo e)
        {
            if (template.Contains("$json", StringComparison.InvariantCultureIgnoreCase))
            {
                
                string path = CreateTempJson(e, DeleteDelay) ?? "";
                if (string.IsNullOrEmpty(path)) path = "\"\"";
                template = template.Replace("$json", path, StringComparison.InvariantCultureIgnoreCase);
            }
            
            template = template.Replace("$time", DateTime.Now.ToString("s"), StringComparison.InvariantCultureIgnoreCase);
            template = template.Replace("$sequence", e.EventCounter.ToString(), StringComparison.InvariantCultureIgnoreCase);
            template = template.Replace("$source", e.Source.ToString(), StringComparison.InvariantCultureIgnoreCase);
            template = template.Replace("$type", e.Type ?? "", StringComparison.InvariantCultureIgnoreCase);
            template = template.Replace("$arg1", e.Arg1 ?? "", StringComparison.InvariantCultureIgnoreCase);
            template = template.Replace("$arg2", e.Arg2 ?? "", StringComparison.InvariantCultureIgnoreCase);
            template = template.Replace("$state", GetPlayingState(e.getPlaybackInfo("State")), StringComparison.InvariantCultureIgnoreCase);
            template = template.Replace("$currKey", e.getPlaybackInfo("FileKey"), StringComparison.InvariantCultureIgnoreCase);
            template = template.Replace("$nextKey", e.getPlaybackInfo("NextFileKey"), StringComparison.InvariantCultureIgnoreCase);

            // handle $[] vars
            template = Regex.Replace(template, @"(\$[\$i]?)\[(.+?)\]", m => GetVariableValue(m, e));

            return template;
        }

        protected string GetPlayingState(string state)
        {
            switch (state)
            {
                case "0": return "STOPPED";
                case "1": return "PAUSED";
                case "2": return "PLAYING";
                case "3": return "WAITING";
            }
            return state;
        }

        protected string GetVariableValue(Match match, MCEventInfo e)
        {
            string field = match.Groups[2].Value;
            string value = null;
            switch (match.Groups[1].Value)
            {
                case "$": value = e.getCurrentFileInfo(field); break;
                case "$$": value = e.getNextFileInfo(field); break;
                case "$i": value = e.getPlaybackInfo(field); break;
            }
            if (string.IsNullOrEmpty(value))
                value = "\"\"";
            if (value.Contains(" "))
                value = $"\"{value}\"";
            return value;
        }

        private string CreateTempJson(MCEventInfo e, int deleteAfter)
        {
            try
            {
                lock (config)
                {
                    int i = 1;
                    string path = Path.Combine(Path.GetTempPath(), $"MCMonitor.{e.EventCounter}.json");
                    while (File.Exists(path))
                        path = Path.Combine(Path.GetTempPath(), $"MCMonitor.{e.EventCounter}.{i++}.json");
                    
                    string json = JsonSerializer.Serialize(e, jsonOptions);
                    File.WriteAllText(path, json);

                    if (deleteAfter > 0)
                        FileTracker.DeleteAfter(path, deleteAfter);
                    return path;
                }
            }
            catch { }
            return "";
        }
    }
}

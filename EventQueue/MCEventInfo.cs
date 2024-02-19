using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MCMonitor
{
    public enum EventSource { MCMonitor, MCEvent, Other }

    public class MCEventInfo
    {
        public int EventCounter { get; set; }
        public DateTime Timestamp {  get; set; }
        public EventSource Source { get; set; }
        public string Type {  get; set; }
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string> PlaybackInfo { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string> CurrentFile { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string> NextFile { get; set; }


        public string getPlaybackInfo(string property)
        {
            return PlaybackInfo == null ? null : PlaybackInfo.TryGetValue(property, out string value) ? value : null;
        }

        public string getCurrentFileInfo(string property)
        {
            return CurrentFile == null ? null : CurrentFile.TryGetValue(property, out string value) ? value : null;
        }

        public string getNextFileInfo(string property)
        {
            return NextFile == null ? null : NextFile.TryGetValue(property, out string value) ? value : null;
        }

        public override string ToString()
        {
            return $"{EventCounter}:{Source}:{Type}:{Arg1}:{Arg2}";
        }
    }
}

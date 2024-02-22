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
        public string State => GetPlayingState();

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? InfoTimestamp { get; set; }

        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SafeDictionary<string, string> PlaybackInfo { get; set; }
        
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SafeDictionary<string, string> CurrentFile { get; set; }
        
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SafeDictionary<string, string> NextFile { get; set; }

        protected string GetPlayingState()
        {
            string state = PlaybackInfo?["State"];
            switch (state)
            {
                case null: return "UNKNOWN";
                case "0": return "STOPPED";
                case "1": return "PAUSED";
                case "2": return "PLAYING";
                case "3": return "WAITING";
            }
            return state;
        }

        public override string ToString()
        {
            return $"{EventCounter}:{Source}:{Type}:{Arg1}:{Arg2}:{State}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MCMonitor
{
    public enum ConfigProperty
    {
        Debug, Filter, Fields, LogPath, LogLine, JsonPath, JsonCount, JsonDetails,
        WebsocketPort, WebsocketDetails, Execute, ExecuteMaxWait, ExecuteMinWait,
        DeleteDelay, TrayNotification
    };

    public class Config
    {
        public const string ConfigFilename = "MCMonitor.ini";
        public static string ConfigPath => Path.Combine(Program.ExecutableFolder, ConfigFilename);

        public Dictionary<ConfigProperty, string> Properties { get; private set; } = new Dictionary<ConfigProperty, string>();
        public List<string> ExecuteList { get; private set; } = new List<string>();

        public bool Debug => this[ConfigProperty.Debug] == "1";
        public bool ShowNotifications => this[ConfigProperty.TrayNotification] == "1";
        public bool isDefaultConfig { get; private set; } = false;


        public string this[ConfigProperty property] { 
            get { return Properties.TryGetValue(property, out string value) ? value : null; } }


        public static bool WriteDefaultConfig()
        {
            return Util.ExtractResource("config.ini", ConfigPath, true);
        }
        
        public static Config Load(bool createDefaultConfigIfNeeded = true)
        {
            try
            {
                Config config = new Config();
                if (!File.Exists(ConfigPath))
                    if (createDefaultConfigIfNeeded)
                        config.isDefaultConfig = WriteDefaultConfig();
                    else
                        return null;

                string ini = File.ReadAllText(ConfigPath);
                var pairs = Regex.Matches(ini, @"(^\s*#)|^([^#=]+?)=(.*)$", RegexOptions.Multiline);
                foreach (Match pair in pairs)
                {
                    if (!string.IsNullOrEmpty(pair.Groups[1].Value)) continue;  // comment line

                    string key = pair.Groups[2].Value.Trim().Replace(".","");
                    string value = pair.Groups[3].Value.Trim();
                    if (!Enum.TryParse<ConfigProperty>(key, true, out ConfigProperty prop))
                        Logger.Log($"Unknown key in config file: {key}");
                    else
                    {
                        if (prop == ConfigProperty.Execute)
                            config.ExecuteList.Add(value);
                        else
                            config.Properties[prop] = value;
                    }
                }
                return config;
            }
            catch (Exception ex) { Logger.Log(ex, $"Exception parsing config.ini"); }
            return null;    // parse failed, or failed to create default config file
        }
    }
}

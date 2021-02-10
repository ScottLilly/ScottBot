using System;
using System.Collections.Generic;
using System.Linq;

namespace ScottBot.Models
{
    public class TwitchBotSettings
    {
        private readonly Dictionary<string, string> _values =
            new Dictionary<string, string>();

        public string ChannelName => _values["Twitch:ChannelName"];
        public string BotName => _values["BotName"];
        public bool HandleAlerts => Convert.ToBoolean(_values["Twitch:HandleAlerts"] ?? "false");
        public string SpeechKey => _values["Speech:Key"];
        public string SpeechRegion => _values["Speech:Region"];
        public string Token => _values["Twitch:Token"];

        public List<ChatMessage> ChatMessages { get; }

        public TwitchBotSettings(IEnumerable<KeyValuePair<string, string>> configuration)
        {
            ChatMessages = new List<ChatMessage>();
            
            foreach((string key, string value) in configuration.Where(c => c.Value != null))
            {
                if(key.StartsWith("Twitch:ChatCommands"))
                {
                    int lastColonIndex = key.LastIndexOf(":");
                    string keywords = key.Substring(lastColonIndex + 1);
                    
                    ChatMessages.Add(new ChatMessage {Keywords = keywords, Message = value});
                }
                else
                {
                    _values.TryAdd(key, value);
                }
            }
        }
    }
}
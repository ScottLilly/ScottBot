using System.Collections.Generic;
using System.Linq;

namespace ScottBot.Models
{
    public class BotSettings
    {
        private readonly Dictionary<string, string> _values =
            new Dictionary<string, string>();

        public string BotName => _values["BotName"];
        public string SpeechKey => _values["Speech:Key"];
        public string SpeechRegion => _values["Speech:Region"];
        public string TwitchChannelName => _values["Twitch:ChannelName"];
        public string TwitchToken => _values["Twitch:Token"];

        public List<ChatMessage> TwitchChatMessages { get; }

        public BotSettings(IEnumerable<KeyValuePair<string, string>> configuration)
        {
            TwitchChatMessages = new List<ChatMessage>();
            
            foreach((string key, string value) in configuration.Where(c => c.Value != null))
            {
                if(key.StartsWith("Twitch:ChatCommands"))
                {
                    int lastColonIndex = key.LastIndexOf(":");
                    string keywords = key.Substring(lastColonIndex + 1);
                    
                    TwitchChatMessages.Add(new ChatMessage {Keywords = keywords, Message = value});
                }
                else
                {
                    _values.TryAdd(key, value);
                }
            }
        }
    }
}
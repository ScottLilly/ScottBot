using System.Collections.Generic;
using System.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace ScottBot.Models
{
    public class TwitchBot
    {
        private readonly string _channelName;
        private readonly List<ChatMessage> _chatMessages;
        private readonly ConnectionCredentials _credentials;
        private readonly TwitchClient _client;
        
        public TwitchBot(string channelName, string token, List<ChatMessage> chatMessages)
        {
            _channelName = channelName;
            _chatMessages = chatMessages;
            
            _credentials = new ConnectionCredentials(channelName, token);
            _client = new TwitchClient();
        }
        
        public void Connect()
        {
            _client.Initialize(_credentials, _channelName);
            _client.Connect();
        }

        public void SendTwitchChatMessage(string spokenText)
        {
            foreach(ChatMessage chatMessage in 
                _chatMessages.Where(cm => spokenText.IncludesTheWords(cm.Keywords.Split(" "))))
            {
                _client.SendMessage(_channelName, chatMessage.Message);
            }
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }
    }
}
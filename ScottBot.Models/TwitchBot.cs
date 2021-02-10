using System;
using System.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;

namespace ScottBot.Models
{
    public class TwitchBot
    {
        private readonly TimeSpan TEN_MINUTES = new TimeSpan(0, 10, 0);

        private readonly TwitchBotSettings _twitchBotSettings;

        private readonly TwitchClient _client = new TwitchClient();
        private readonly ConnectionCredentials _credentials;
        
        public TwitchBot(TwitchBotSettings twitchBotSettings)
        {
            _twitchBotSettings = twitchBotSettings;
            
            _credentials = 
                new ConnectionCredentials(string.IsNullOrWhiteSpace(_twitchBotSettings.BotName) ? twitchBotSettings.ChannelName : _twitchBotSettings.BotName, _twitchBotSettings.Token, disableUsernameCheck:true);
            
            _client.OnDisconnected += Client_OnDisconnected;

            _client.OnMessageReceived += Client_OnChatMessageReceived;

            if(_twitchBotSettings.HandleAlerts)
            {
                _client.OnBeingHosted += Client_OnBeingHosted;
                _client.OnCommunitySubscription += Client_OnCommunitySubscription;
                _client.OnGiftedSubscription += Client_OnGiftedSubscription;
                _client.OnNewSubscriber += Client_OnNewSubscriber;
                _client.OnRaidNotification += Client_OnRaidNotification;
                _client.OnReSubscriber += Client_OnReSubscriber;
            }
        }

        public void Connect()
        {
            _client.Initialize(_credentials, _twitchBotSettings.ChannelName);
            _client.Connect();
        }

        public void HandleTwitchCommand(string spokenText)
        {
            if(spokenText.IncludesTheWords("start", "follower", "only") ||
               spokenText.IncludesTheWords("start", "followers", "only"))
            {
                _client.FollowersOnlyOn(_twitchBotSettings.ChannelName, TEN_MINUTES);
            }
            else if(spokenText.IncludesTheWords("stop", "follower", "only") ||
                    spokenText.IncludesTheWords("stop", "followers", "only"))
            {
                _client.FollowersOnlyOff(_twitchBotSettings.ChannelName);
            }
            else if(spokenText.IncludesTheWords("start", "sub", "only") ||
                    spokenText.IncludesTheWords("start", "subs", "only") ||
                    spokenText.IncludesTheWords("start", "subscriber", "only") ||
                    spokenText.IncludesTheWords("start", "subscribers", "only"))
            {
                _client.SubscribersOnlyOn(_twitchBotSettings.ChannelName);
            }
            else if(spokenText.IncludesTheWords("stop", "sub", "only") ||
                    spokenText.IncludesTheWords("stop", "subs", "only") ||
                    spokenText.IncludesTheWords("stop", "subscriber", "only") ||
                    spokenText.IncludesTheWords("stop", "subscribers", "only"))
            {
                _client.SubscribersOnlyOff(_twitchBotSettings.ChannelName);
            }
            else if(spokenText.IncludesTheWords("start", "slow", "mode"))
            {
                _client.SlowModeOn(_twitchBotSettings.ChannelName, TEN_MINUTES);
            }
            else if(spokenText.IncludesTheWords("stop", "slow", "mode"))
            {
                _client.SlowModeOff(_twitchBotSettings.ChannelName);
            }
            else if(spokenText.IncludesTheWords("clear", "chat"))
            {
                _client.ClearChat(_twitchBotSettings.ChannelName);
            }
            else
            {
                HandleCustomCommands(spokenText);
            }
        }

        public void Disconnect()
        {
            _client.Disconnect();
        }

        private void HandleCustomCommands(string spokenText)
        {
            foreach(ChatMessage chatMessage in
                _twitchBotSettings.ChatMessages
                                  .Where(cm => spokenText.IncludesTheWords(cm.Keywords.Split(" "))))
            {
                SendChatMessage(chatMessage.Message);
            }
        }

        private void SendChatMessage(string message)
        {
            _client.SendMessage(_twitchBotSettings.ChannelName, message);
        }

        private void Client_OnBeingHosted(object? sender, OnBeingHostedArgs e)
        {
            SendChatMessage($"Thank you for hosting {e.BeingHostedNotification.HostedByChannel}!");
        }

        private void Client_OnChatMessageReceived(object? sender, OnMessageReceivedArgs e)
        {
            //TODO: Parse message and respond, if possible
        }

        private void Client_OnCommunitySubscription(object? sender, OnCommunitySubscriptionArgs e)
        {
            // TODO: Thank you message
        }

        private void Client_OnDisconnected(object? sender, OnDisconnectedEventArgs e)
        {
            Connect();
        }

        private void Client_OnGiftedSubscription(object? sender, OnGiftedSubscriptionArgs e)
        {
            SendChatMessage($"Welcome to the channel {e.GiftedSubscription.DisplayName}!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            SendChatMessage(e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime ?
                                $"{e.Subscriber.DisplayName}, thank you for subscribing with Prime!" :
                                $"{e.Subscriber.DisplayName}, thank you for subscribing!");
        }

        private void Client_OnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            SendChatMessage($"{e.RaidNotification.DisplayName}, thank you for raiding!");
        }

        private void Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            SendChatMessage(e.ReSubscriber.SubscriptionPlan == SubscriptionPlan.Prime ?
                                $"{e.ReSubscriber.DisplayName}, thank you for re-subscribing with Prime!" :
                                $"{e.ReSubscriber.DisplayName}, thank you for re-subscribing!");
        }
    }
}
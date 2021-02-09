using System;
using System.Collections.Generic;
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
        
        private readonly string _channelName;
        private readonly List<ChatMessage> _chatMessages;
        private readonly string _botName;
        private readonly ConnectionCredentials _credentials;
        private readonly TwitchClient _client;
        
        public TwitchBot(string channelName, string token, List<ChatMessage> chatMessages, 
                         string botName = "")
        {
            _channelName = channelName;
            _chatMessages = chatMessages;
            _botName = string.IsNullOrWhiteSpace(botName) ? channelName : botName;
            
            _credentials = new ConnectionCredentials(botName, token, disableUsernameCheck:true);
            _client = new TwitchClient();
            
            _client.OnMessageReceived += Client_OnChatMessageReceived;
            _client.OnBeingHosted += Client_OnBeingHosted;
            _client.OnDisconnected += Client_OnDisconnected;
            _client.OnCommunitySubscription += Client_OnCommunitySubscription;
            _client.OnGiftedSubscription += Client_OnGiftedSubscription;
            _client.OnNewSubscriber += Client_OnNewSubscriber;
            _client.OnRaidNotification += Client_OnRaidNotification;
            _client.OnReSubscriber += Client_OnReSubscriber;
        }

        public void Connect()
        {
            _client.Initialize(_credentials, _channelName);
            _client.Connect();
        }

        public void HandleTwitchCommand(string spokenText)
        {
            if(spokenText.IncludesTheWords("follower", "only", "off") ||
               spokenText.IncludesTheWords("followers", "only", "off"))
            {
                _client.FollowersOnlyOff(_channelName);
            }
            else if(spokenText.IncludesTheWords("follower", "only", "on") ||
               spokenText.IncludesTheWords("followers", "only", "on"))
            {
                _client.FollowersOnlyOn(_channelName, TEN_MINUTES);
            }
            else if(spokenText.IncludesTheWords("subs", "only", "off") ||
                    spokenText.IncludesTheWords("subscriber", "only", "off") ||
                    spokenText.IncludesTheWords("subscribers", "only", "off"))
            {
                _client.SubscribersOnlyOff(_channelName);
            }
            else if(spokenText.IncludesTheWords("subs", "only") ||
                    spokenText.IncludesTheWords("subscriber", "only") ||
                    spokenText.IncludesTheWords("subscribers", "only"))
            {
                _client.SubscribersOnlyOn(_channelName);
            }
            else if(spokenText.IncludesTheWords("slow", "on"))
            {
                _client.SlowModeOn(_channelName, TEN_MINUTES);
            }
            else if(spokenText.IncludesTheWords("slow", "off"))
            {
                _client.SlowModeOff(_channelName);
            }
            else if(spokenText.IncludesTheWords("clear", "chat"))
            {
                SendChatMessage(@"/clear");
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
                _chatMessages.Where(cm => spokenText.IncludesTheWords(cm.Keywords.Split(" "))))
            {
                SendChatMessage(chatMessage.Message);
            }
        }

        private void SendChatMessage(string message)
        {
            _client.SendMessage(_channelName, message);
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
            //if(e.GiftedSubscription.IsAnonymous)
            //{
            //    SendChatMessage($"Thanks for gifting a subscription to {e.GiftedSubscription.DisplayName}");
            //}
            //else
            //{
            //    SendChatMessage($"{e.GiftedSubscription.} Thanks for gifting a subscription to {e.GiftedSubscription.DisplayName}");
            //}
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
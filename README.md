# ScottBot

<table>
  <tbody>
	<tr>
	  <td>This project is inactive and has been archived to read-only mode. Please contact me if you want it reopened.</td>
	</tr>
  </tbody>
</table>

This is a speech-recognition app intended to provide many of the capabilities found in commercial apps, without handing over so much of my personal information to the companies that are more-than-willing to abuse it.

It does use [Azure speech-to-text](https://azure.microsoft.com/en-us/services/cognitive-services/speech-to-text/), which sends whatever your microphone hears to Azure. However, this app does not store any data - as you can see by reviewing the code.


## Requirements
1. An [Azure](https://azure.microsoft.com) account
2. An Azure "Cognitive Services" key


## Running the program
Run the console program. It will listen for the bot name (currently "ScottBot", but configurable in the appsettings.json), the word "Twitch", and the matching "ChatCommand" (also configurable in appsettings.json). The program will paste the text into your Twitch chat.

For example, if I say "ScottBot Twitch GitHub", the program will put "My GitHub repos: https://github.com/ScottLilly https://github.com/CodingWithScott" into the Twitch chat for my channel.


## Preparation
1. Create a User Secrets file for the WPF project, using the format below. This will store all the secret keys you want to keep out of source control.
2. Change the BotName and Twitch ChatCommands values to whatever values you want to use.

user secrets values to include
```javascript
{
  "Speech": {
    "Key": "",
    "Region": ""
  },
  "Twitch": {
    "ChannelName": "",
    "Token": ""
  } 
}
```

**NOTES**
- Speech Key and Region are from your Azure Coginitive service "Keys and Endpoint"
- ChannelName is your Twitch user/channel name, e.g. "CodingWithScott"
- For your Twitch token, check https://dev.twitch.tv/docs/authentication and https://twitchapps.com/tmi/

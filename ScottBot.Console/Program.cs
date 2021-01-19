using System.Threading;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ScottBot.Models;

namespace ScottBot.Console
{
    public class Program
    {
        private static BotSettings s_botSettings;

        private static TwitchBot s_twitchBot;
        
        private static AudioConfig s_audioConfig;
        private static SpeechRecognizer s_speechRecognizer;
        private static SpeechSynthesizer s_speechSynthesizer;
            
        private static readonly TaskCompletionSource<int> s_stopRecognition = 
            new TaskCompletionSource<int>();
        
        private static async Task Main(string[] args)
        {
            CreateHostBuilder(args).Build();
            
            Initialize();

            s_twitchBot = new TwitchBot(s_botSettings.TwitchChannelName, 
                                        s_botSettings.TwitchToken,
                                        s_botSettings.TwitchChatMessages);
            s_twitchBot.Connect();

            PhraseListGrammar phraseList = 
                PhraseListGrammar.FromRecognizer(s_speechRecognizer);
            phraseList.AddPhrase(s_botSettings.BotName);
            phraseList.AddPhrase("RPG");
            phraseList.AddPhrase("GitHub");
            phraseList.AddPhrase("Discord");
            
            s_speechRecognizer.Recognized += OnSpeechRecognizedAsync;
            s_speechRecognizer.Canceled += SpeechRecognizerOnCanceled;
            s_speechRecognizer.SessionStopped += SpeechRecognizerOnSessionStopped;
            
            System.Console.WriteLine("Speak into your microphone.");
            
            await s_speechRecognizer.StartContinuousRecognitionAsync();
            
            Task.WaitAny(s_stopRecognition.Task);

            DisposeAll();
        }
        
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();

                    IHostEnvironment env = hostingContext.HostingEnvironment;

                    configuration
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                        .AddUserSecrets<Program>();

                    s_botSettings = new BotSettings(configuration.Build().AsEnumerable());
                });
        
        private static void Initialize()
        {
            SpeechConfig speechConfig = 
                SpeechConfig.FromSubscription(s_botSettings.SpeechKey, s_botSettings.SpeechRegion);
            
            s_audioConfig = AudioConfig.FromDefaultMicrophoneInput();
            s_speechRecognizer = new SpeechRecognizer(speechConfig, s_audioConfig);
            s_speechSynthesizer = new SpeechSynthesizer(speechConfig, s_audioConfig);
        }

        private static async void OnSpeechRecognizedAsync(object sender, SpeechRecognitionEventArgs e)
        {
            if (e.Result.Reason == ResultReason.NoMatch)
            {
                System.Console.WriteLine("NO MATCH: Speech could not be recognized.");
                return;
            }

            string spokenText = e.Result.Text;
                    
            System.Console.WriteLine($"RECOGNIZED: Text={spokenText}");

            // Ignore everything that doesn't include the bot's name
            if(!spokenText.IncludesTheWords(s_botSettings.BotName))
            {
                return;
            }

            spokenText = spokenText.RemoveText(s_botSettings.BotName);
            
            // Process the request
            if(spokenText.IncludesTheWords("start") ||
               spokenText.IncludesTheWords("wake"))
            {
                await SpeakAsync("What can I do for you?");
            }
            else if(spokenText.IncludesTheWords("stop") ||
                    spokenText.IncludesTheWords("sleep") ||
                    spokenText.IncludesTheWords("shut", "down"))
            {
                await SpeakAsync("Shutting down");
                s_stopRecognition.TrySetCanceled(new CancellationToken(true));
            }
            else if(spokenText.IncludesTheWords("Twitch"))
            {
                s_twitchBot.SendTwitchChatMessage(spokenText);
            }
        }

        private static void SpeechRecognizerOnCanceled(object sender, SpeechRecognitionCanceledEventArgs e)
        {
            System.Console.WriteLine($"CANCELED: Reason={e.Reason}");

            if (e.Reason == CancellationReason.Error)
            {
                System.Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                System.Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                System.Console.WriteLine($"CANCELED: Did you update the subscription info?");
            }

            s_stopRecognition.TrySetResult(0);
        }

        private static void SpeechRecognizerOnSessionStopped(object sender, SessionEventArgs e)
        {
            System.Console.WriteLine("Session stopped event.");
            s_stopRecognition.TrySetResult(0);
        }

        private static async Task SpeakAsync(string text)
        {
            await s_speechSynthesizer.SpeakTextAsync(text);
        }

        private static void DisposeAll()
        {
            s_twitchBot.Disconnect();
            s_speechSynthesizer.Dispose();
            s_speechRecognizer.Dispose();
            s_audioConfig.Dispose();
        }
    }
}
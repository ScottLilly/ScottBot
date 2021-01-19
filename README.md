# ScottBot
This is a speech-recognition app intended to provide many of the capabilities found in commercial apps, without handing over so much of my personal information to the companies that are more-than-willing to abuse it.

It does use [Azure speech-to-text](https://azure.microsoft.com/en-us/services/cognitive-services/speech-to-text/), which sends whatever your microphone hears to Azure. However, this app does not store any data - as you can see by reviewing the code.

## Requirements
1. An [Azure](https://azure.microsoft.com) account
2. An Azure "Cognitive Services" key


## Preparation
1. Create a User Secrets file for the WPF project. This will store all the secret keys you want to keep out of source control.
    1. Add your key and region information

As we add more capacilities to the app, this is where we'll add all other keys

**NOTE:** I'll add the exact format for the JSON soon.

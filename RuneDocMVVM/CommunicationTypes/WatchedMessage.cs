namespace RuneDocMVVM.CommunicationTypes;

// This will have more than just a string.
// Needs message type, alert type, detection type, etc.
public class WatchedMessage
{
    public string Message { get; set; }
    public bool TextToSpeech { get; set; }
    public string TextToSpeechMessage { get; set; }

    public static WatchedMessage Create(string message)
    {
        WatchedMessage watchedMessage = new WatchedMessage { Message = message, TextToSpeech = false};
        return watchedMessage;
    }
}
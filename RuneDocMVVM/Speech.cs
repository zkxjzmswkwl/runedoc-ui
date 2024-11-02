using System.Speech.Synthesis;

namespace RuneDocMVVM;

public class Speech
{
    public static void Say(string text)
    {
         SpeechSynthesizer synth = new SpeechSynthesizer();
         synth.SetOutputToDefaultAudioDevice();
         synth.Speak(text);       
    }
}
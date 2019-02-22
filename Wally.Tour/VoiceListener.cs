using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;

namespace Wally.Tour {
    public class VoiceListener {
        private readonly SpeechRecognitionEngine _speechRecognitionEngine;
        public VoiceListener(SpeechRecognitionEngine speechRecognitionEngine) {
            _speechRecognitionEngine = speechRecognitionEngine;
        }

        public void Listen(IEnumerable<string> choices, Action<string> onSpeechParsed)
        {
            //_speechRecognitionEngine.SpeechRecognized += (sender, args) => { onSpeechParsed(args.Result.Text); };
            //commented out to remove listening
            _speechRecognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder(new Choices(choices.ToArray()))));
            _speechRecognitionEngine.SetInputToDefaultAudioDevice();
            _speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
        }
    }
}
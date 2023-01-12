// REWRITE ARTIFACT
//using System;

//namespace Commodore.GameLogic.Sound
//{
//    public class Voice
//    {
//        private int _fadeCounter;
//        private readonly Synthesizer _synthesizer;

//        public VoiceState State { get; private set; }
//        public bool IsAlive { get; private set; }
//        public float Time { get; private set; }
//        public float FadeMultiplier { get; private set; }

//        public float Frequency { get; set; }
//        public Func<float, float, float> Oscillator { get; set; } = Generators.Noise;

//        public Voice(Synthesizer synthesizer)
//        {
//            _synthesizer = synthesizer;
//        }

//        public void Begin(float frequency = -1f, Func<float, float, float> oscillator = null)
//        {
//            Time = 0f;
//            FadeMultiplier = 0f;
//            _fadeCounter = 0;

//            if (oscillator != null)
//                Oscillator = oscillator;

//            if (frequency >= 0)
//                Frequency = frequency;


//            if (_synthesizer.FadeInDuration == 0)
//                State = VoiceState.Sustain;
//            else
//                State = VoiceState.Attack;

//            IsAlive = true;
//        }

//        public void End()
//        {
//            if (_synthesizer.FadeOutDuration == 0)
//                IsAlive = false;
//            else
//            {
//                _fadeCounter = (int)((1f - FadeMultiplier) * _synthesizer.FadeOutDuration);
//                State = VoiceState.Release;
//            }
//        }

//        public void Process(float[,] workingBuffer)
//        {
//            if (IsAlive)
//            {
//                var samplesPerBuffer = workingBuffer.GetLength(1);
//                for (var i = 0; i < samplesPerBuffer; i++)
//                {
//                    if (State == VoiceState.Attack)
//                    {
//                        FadeMultiplier = (float)_fadeCounter / _synthesizer.FadeInDuration;

//                        ++_fadeCounter;

//                        if (_fadeCounter >= _synthesizer.FadeInDuration)
//                            State = VoiceState.Sustain;
//                    }
//                    else if (State == VoiceState.Release)
//                    {
//                        FadeMultiplier = 1f - (float)_fadeCounter / _synthesizer.FadeOutDuration;

//                        ++_fadeCounter;
//                        if (_fadeCounter >= _synthesizer.FadeOutDuration)
//                        {
//                            IsAlive = false;
//                            return;
//                        }
//                    }
//                    else
//                    {
//                        FadeMultiplier = 1f;
//                    }

//                    var sample = Oscillator(Frequency, Time);

//                    workingBuffer[0, i] += sample * 0.2f * FadeMultiplier;
//                    Time += 1f / _synthesizer.SampleRate;
//                }
//            }
//        }
//    }
//}
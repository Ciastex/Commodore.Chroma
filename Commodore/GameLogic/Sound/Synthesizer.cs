// REWRITE ARTIFACT
//using Commodore.Engine;
//using Commodore.GameLogic.Core;
//using System;
//using System.Linq;

//namespace Commodore.GameLogic.Sound
//{
//    public class Synthesizer : EngineObject
//    {
//        public const int BytesPerSample = 2;

//        private readonly DynamicSoundEffectInstance _dynamicSound;
//        private readonly byte[] _xnaBuffer;
//        private readonly float[,] _workingBuffer;

//        private readonly Voice[] _voicePool;

//        public int SampleRate { get; } = 44100;
//        public int SamplesPerBuffer { get; } = 2000;
//        public int VoiceCount { get; } = 8;
//        public int FadeInDuration { get; set; } = 200;
//        public int FadeOutDuration { get; set; } = 200;

//        public Synthesizer()
//        {
//            _dynamicSound = new DynamicSoundEffectInstance(SampleRate, AudioChannels.Mono);
//            _dynamicSound.Play();

//            _xnaBuffer = new byte[SamplesPerBuffer * BytesPerSample];
//            _workingBuffer = new float[1, SamplesPerBuffer];

//            _voicePool = new Voice[VoiceCount];
//            for (var i = 0; i < VoiceCount; i++)
//            {
//                _voicePool[i] = new Voice(this);
//                _voicePool[i].Begin();
//            }
//        }

//        public void Update(float deltaTime)
//        {
//            for (var i = 0; i < VoiceCount; i++)
//            {
//                var frequency = GetVoiceFrequency(i);
//                var generator = GetNoteGenerator(i);

//                if (frequency == 0)
//                {
//                    _voicePool[i].End();
//                    continue;
//                }
//                else
//                {
//                    if (!_voicePool[i].IsAlive)
//                        _voicePool[i].Begin();
//                }

//                switch (generator)
//                {
//                    case 0: _voicePool[i].Oscillator = Generators.Noise; break;
//                    case 1: _voicePool[i].Oscillator = Generators.SawTooth; break;
//                    case 2: _voicePool[i].Oscillator = Generators.Sine; break;
//                    case 3: _voicePool[i].Oscillator = Generators.Square; break;
//                    case 4: _voicePool[i].Oscillator = Generators.Triangle; break;
//                    default: _voicePool[i].End(); continue;
//                }

//                _voicePool[i].Frequency = frequency;
//            }

//            while (_dynamicSound.PendingBufferCount < 2)
//                SubmitBuffer();
//        }

//        private ushort GetVoiceFrequency(int noteIndex)
//        {
//            return (ushort)Kernel.Instance.Memory.Peek16(SystemConstants.SystemMemoryPlane, SystemMemoryAddresses.Voice1Frequency + (noteIndex * 3));
//        }

//        private byte GetNoteGenerator(int noteIndex)
//        {
//            return Kernel.Instance.Memory.Peek8(SystemConstants.SystemMemoryPlane, SystemMemoryAddresses.Voice1Frequency + (noteIndex * 3) + 2);
//        }

//        private void SubmitBuffer()
//        {
//            ClearWorkingBuffer();
//            FillWorkingBuffer();

//            Utilities.WriteToXnaBuffer(_workingBuffer, _xnaBuffer);
//            _dynamicSound.SubmitBuffer(_xnaBuffer);
//        }

//        private void ClearWorkingBuffer() => Array.Clear(_workingBuffer, 0, _workingBuffer.GetLength(0) * SamplesPerBuffer);

//        private void FillWorkingBuffer()
//        {
//            for (var i = _voicePool.Count() - 1; i >= 0; --i)
//            {
//                var voice = _voicePool[i];
//                voice.Process(_workingBuffer);
//            }
//        }
//    }
//}
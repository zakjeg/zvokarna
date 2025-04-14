using NAudio.Wave;
using System;
using System.Linq;

namespace xamlPianoRoll.Audio
{
    class CachedSoundSampleProvider : ISampleProvider
    {
        private readonly CachedSound cachedSound;
        private long position;
        private readonly object positionLock = new object();

        public event Action OnPlaybackFinished;

        public CachedSoundSampleProvider(CachedSound cachedSound)
        {
            if (cachedSound == null)
                throw new ArgumentNullException(nameof(cachedSound));

            if (cachedSound.AudioData == null || cachedSound.AudioData.Length == 0)
                throw new InvalidOperationException("The cached sound has no audio data.");

            this.cachedSound = cachedSound;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            lock (positionLock)
            {
                var availableSamples = cachedSound.AudioData.Length - position;
                var samplesToCopy = Math.Min(availableSamples, count);
                Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
                position += samplesToCopy;

                if (position >= cachedSound.AudioData.Length)
                {
                    position = 0;  
                    OnPlaybackFinished?.Invoke();
                }

                return (int)samplesToCopy;
            }
        }

        public WaveFormat WaveFormat => cachedSound.WaveFormat;
    }
}


//using NAudio.Wave;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace xamlPianoRoll.Audio
//{
//	class CachedSoundSampleProvider : ISampleProvider
//	{
//		private readonly CachedSound cachedSound;
//		private long position;

//		public CachedSoundSampleProvider(CachedSound cachedSound)
//		{
//			this.cachedSound = cachedSound;
//		}

//		public int Read(float[] buffer, int offset, int count)
//		{
//			var availableSamples = cachedSound.AudioData.Length - position;
//			var samplesToCopy = Math.Min(availableSamples, count);
//			Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
//			position += samplesToCopy;
//			return (int)samplesToCopy;
//		}

//		public WaveFormat WaveFormat { get { return cachedSound.WaveFormat; } }
//	}
//}

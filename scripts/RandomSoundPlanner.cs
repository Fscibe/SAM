using UnityEngine;

namespace Sam
{
    /// <summary>
    /// Compute random timings for sounds.
    /// Only ONE sound will play at a time.
    /// </summary>
    public class RandomSoundPlanner
    {
        /// <summary>
        /// Sound timing data.
        /// </summary>
        public struct Result
        {
            public int id;
            public float startTime;
            public float pitch;
        }

        /// <summary>
        /// Maximum number of sounds planned.
        /// </summary>
        public const int MAX_COUNT = 16;

        /// <summary>
        /// Cached array for random values. Used by Compute() method.
        /// </summary>
        private static float[] _randomCache = new float[MAX_COUNT];

        /// <summary>
        /// Cached array of sound duration + minDelay for each sound event. Used by Compute() method.
        /// </summary>
        private static float[] _durationsBeforeNextEvent = new float[MAX_COUNT];

        /// <summary>
        /// Compute random timings.
        /// Returns the number of timings found. It CAN be lesser than the wanted count if there is not enough totalDuration.
        /// </summary>
        public static int Compute(int count, float minDelay, float minPitch, float maxPitch, float totalDuration, float[] soundDurations, RandomPool pickStrategy, Result[] result)
        {
            // early outs
            if (soundDurations.Length == 0)
                return 0;

            // cache remaining time left to plan sound events
            float remainingDuration = totalDuration;

            // randomly pick sounds
            int i = 0;
            for (; i < count; ++i)
            {
                int soundIndex = pickStrategy.GetValue();
                float pitch = Random.Range(minPitch, maxPitch);
                float durationBeforeNextEvent = soundDurations[soundIndex] / pitch + minDelay;

                // if not enough time left, stop
                if (remainingDuration < durationBeforeNextEvent)
                {
                    break;
                }
                remainingDuration -= durationBeforeNextEvent;

                // save results
                result[i].id = soundIndex;
                result[i].pitch = pitch;
                _durationsBeforeNextEvent[i] = durationBeforeNextEvent;
            }

            // now compute random duration between each sound event
            // by taking 'i-1' random times and sorting them so
            // the difference between 2 values will be the a random
            // duration between consecutive sound events. 
            for (int j = 0; j < i; ++j)
            {
                _randomCache[j] = Random.Range(0.0f, remainingDuration);
            }
            System.Array.Sort(_randomCache, 0, i);

            // final results
            float previousValue = 0.0f;
            float currentValue;
            float currentEndTime = 0.0f;
            for (int j = 0; j < i; ++j)
            {
                currentValue = _randomCache[j];
                float delta = currentValue - previousValue;
                result[j].startTime = delta + currentEndTime;
                currentEndTime += delta + _durationsBeforeNextEvent[j];
                previousValue = currentValue;
            }

            return i;
        }
    }
}

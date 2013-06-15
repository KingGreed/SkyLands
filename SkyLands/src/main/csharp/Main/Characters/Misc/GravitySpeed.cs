using System;
using Mogre;

namespace Game.CharacSystem
{
    public static class GravitySpeed
    {
        public const float SPEED_T0 = -500;
        public const float SPEED_TMAX = -4000;
        private const float T_MAX = 1.8f;  // Time until the character reach its max speed fall
        private const float B = T_MAX * (SPEED_TMAX - 1) / (SPEED_T0 - SPEED_TMAX);
        private const float A = SPEED_T0 * B;

        private static Timer mTimeOfFall = new Timer();

        public static float GetSpeed()
        {
            float sec = mTimeOfFall.Milliseconds / 1000f;
            if (sec >= T_MAX) { return SPEED_TMAX; }
            return (sec + A) / (sec + B);
        }

        public static float GetTimeSinceFall() { return mTimeOfFall.Milliseconds; }

        public static void Reset() { mTimeOfFall.Reset(); }
    }
}

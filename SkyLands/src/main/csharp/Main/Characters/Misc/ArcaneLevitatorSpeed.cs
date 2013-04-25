using System;
using Mogre;

namespace Game.CharacSystem
{
    public static class ArcaneLevitatorSpeed
    {
        private const float T_MAX = 1.5f;       // Time until the character reach the max levitation speed
        private const float SPEED_MAX = 300;    // Speed reach at T_MAX time
        private const float K = T_MAX * T_MAX / SPEED_MAX;

        private static Timer mTimeOfLevitation = new Timer();

        public static float GetSpeed()
        {
            float sec = (float)mTimeOfLevitation.Milliseconds / 1000f;
            float speed = sec * sec / K;
            if (sec >= T_MAX) { speed = SPEED_MAX; }
            return speed;
        }

        public static void StartLevitation() { mTimeOfLevitation.Reset(); }
    }
}

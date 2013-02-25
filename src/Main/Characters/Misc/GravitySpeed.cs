using System;
using Mogre;

namespace Game.CharacSystem
{
    public class GravitySpeed
    {
        private const float SPEED_T0 = -450;
        private const float SPEED_TMAX = -2000;
        private const float T_MAX = 5;  // Time until the character reach its max speed fall
        private const float B = T_MAX * (SPEED_TMAX - 1) / (SPEED_T0 - SPEED_TMAX);
        private const float A = SPEED_T0 * B;

        private Timer mTimeOfFall;

        public GravitySpeed()
        {
            this.mTimeOfFall = new Timer();
        }

        public float GetSpeed()
        {
            float sec = ((float)mTimeOfFall.Milliseconds) / 1000f;
            if (sec >= T_MAX) { return SPEED_TMAX; }
            else              { return (sec + A) / (sec + B); }
        }

        public void Reset() { mTimeOfFall.Reset(); }
    }
}

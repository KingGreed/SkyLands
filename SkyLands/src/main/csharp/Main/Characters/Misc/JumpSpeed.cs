using Mogre;

namespace Game.CharacSystem
{
    public static class JumpSpeed
    {
        private const float JUMP_T0 = 550;    // Also the C coef of the 2nd order equation
        private const float T1 = 0.15f;
        private const float JUMP_T1 = 250;  // Speed jump at T1 time
        private const float T_MAX = 0.5f;  // Time until the character reach its max jump speed
        private const float A = (((JUMP_T0 - JUMP_T1) / T1) * T_MAX - JUMP_T0) / (T_MAX * (T_MAX - T1));
        private const float B = (JUMP_T1 - JUMP_T0 - A * T1 * T1) / T1;

        private static readonly Timer timeOfJump = new Timer();

        public static bool IsJumping { get { return GetSec() < T_MAX; } }

        public static float GetSpeed()
        {
            float speed = 0;
            float sec = GetSec();
            if (sec < T_MAX) { speed = A * sec * sec + B * sec + JUMP_T0; }
            return speed;
        }

        public static void Jump() { timeOfJump.Reset(); }

        private static float GetSec(){ return (float)timeOfJump.Milliseconds / 1000f; }
    }
}

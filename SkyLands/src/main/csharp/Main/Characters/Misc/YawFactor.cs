using Mogre;

namespace Game.CharacSystem
{
    public static class YawFactor
    {
        private const float DEGREE_X = 60;
        private const float FACTOR_X = 0.7f;
        private const float A = (DEGREE_X - 180 * FACTOR_X) / (180 * 180 * DEGREE_X - 180 * DEGREE_X * DEGREE_X);
        private const float B = (FACTOR_X - A * DEGREE_X * DEGREE_X) / DEGREE_X;

        public static float GetFactor(Degree diffYaw)    // return between -1 and 1
        {
            float diffYawFloat = diffYaw.ValueAngleUnits;
            while (diffYawFloat > 180)   { diffYawFloat -= 360; }
            while (diffYawFloat <= -180) { diffYawFloat += 360; }
            int sign = System.Math.Sign(diffYawFloat);
            diffYawFloat *= sign;    // Take the abs value

            float factor = A * diffYawFloat * diffYawFloat + B * diffYawFloat; // Here factor is between 0 and 1
            return (factor > 1 ? 1 : factor) * sign;
        }
    }
}

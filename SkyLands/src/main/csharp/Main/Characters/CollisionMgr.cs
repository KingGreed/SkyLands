using Game.World.Blocks;
using Mogre;

using Game.World;
using API.Generic;

namespace Game.CharacSystem
{
    public class CollisionMgr
    {
        private const float COL_SIDE_MARGE = 0.61f;
        private const int NBR_HIT_POINTS = 15;   // Nbr of points for collisions there are at the same height

        private readonly API.Geo.World    mWorld;
        private readonly VanillaCharacter mCharac;
        private readonly Degree[]         mHitDegrees;
        private readonly float            mHitRadius;
        private readonly int              mNbrHitStage;

        public bool WasHorizontalCollision      { get; private set; }
        public bool HasHorizontalCollisionEnded { get; private set; }

        public CollisionMgr(API.Geo.World world, VanillaCharacter charac)
        {
            this.mWorld = world;
            this.mCharac = charac;
            this.mNbrHitStage = 2 + (int)charac.Size.y / Cst.CUBE_SIDE;

            this.mHitRadius = charac.Size.x / 2 * COL_SIDE_MARGE;
            this.mHitDegrees = new Degree[NBR_HIT_POINTS];
            Degree delta = 360 / this.mHitDegrees.Length;
            for (int i = 0; i < this.mHitDegrees.Length; i++)
                this.mHitDegrees[i] = i * delta;
        }

        public Vector3[] GetHitPoints()
        {
            Vector3[] hitPoints = new Vector3[this.mHitDegrees.Length * this.mNbrHitStage];

            for (int i = 0; i < this.mHitDegrees.Length; i++)
            {
                Degree deg = this.mHitDegrees[i] - this.mCharac.GetYaw();

                for (int j = 0; j < this.mNbrHitStage; j++)
                {
                    int index = i + NBR_HIT_POINTS * j;
                    hitPoints[index] = this.mCharac.FeetPosition;
                    hitPoints[index].x += this.mHitRadius * Math.Cos(deg);
                    hitPoints[index].z += this.mHitRadius * Math.Sin(deg);
                    hitPoints[index].y += j < this.mNbrHitStage - 1 ? j * Cst.CUBE_SIDE : this.mCharac.Size.y;
                }
            }

            return hitPoints;
        }

        public Vector3 ComputeCollision(Vector3 absTranslation)  // Takes the wanted translation and returns the possible one
        {
            bool wasHCollision = this.WasHorizontalCollision;
            this.WasHorizontalCollision = false;
            absTranslation.x = MathHelper.clamp(absTranslation.x, -Cst.CUBE_SIDE, Cst.CUBE_SIDE);
            absTranslation.y = MathHelper.clamp(absTranslation.y, -Cst.CUBE_SIDE, Cst.CUBE_SIDE);
            absTranslation.z = MathHelper.clamp(absTranslation.z, -Cst.CUBE_SIDE, Cst.CUBE_SIDE);

            Vector3[] hitPoints = this.GetHitPoints();
            Vector3 actTranslation = absTranslation;
            Vector3[] axes = new Vector3[] { Vector3.UNIT_X, Vector3.UNIT_Y, Vector3.UNIT_Z };

            foreach (Vector3 axis in axes)
            {
                Vector3 delta = absTranslation * axis;
                if (delta.IsZeroLength) { continue; }   // Ignore the axis if there is no translation on it

                Vector3[] actHitPoints = this.AddToAll(hitPoints, delta); // The hitPoints after the wanted translation
                Vector3[] hitPointsToTest = this.GetPointsToTest(actHitPoints, absTranslation, axis);
                for(int i = 0; i < hitPointsToTest.Length; i++)
                {
                    Vector3 relHitPoint = MainWorld.AbsToRelative(hitPointsToTest[i]);
                    if (this.mWorld.HasPointCollision(hitPointsToTest[i]))
                    {
                        /* Compute the possible translation */
                        float val = 0;
                        if (axis == Vector3.UNIT_Y)
                        {
                            float blockHeight = Cst.CUBE_SIDE;
                            if (mWorld.getIsland().getBlock(relHitPoint, false) is SnowEighthBlock) { blockHeight /= 8; }
                            if (this.GetNonNullValue(absTranslation * axis) < 0)
                                val = (relHitPoint.y * Cst.CUBE_SIDE + blockHeight) - this.mCharac.FeetPosition.y;
                        }
                        else { this.WasHorizontalCollision = true; }

                        actTranslation = (actTranslation * (Vector3.UNIT_SCALE - axis)) + val * axis;    // Update the actTranslation
                        break;
                    }
                }
            }

            this.HasHorizontalCollisionEnded = wasHCollision && !this.WasHorizontalCollision;
            return actTranslation;
        }

        private float GetNonNullValue(Vector3 v)
        {
            return v.x != 0 ? v.x : (v.y != 0 ? v.y : v.z);
        }

        private Vector3[] AddToAll(Vector3[] array, Vector3 add)
        {
            Vector3[] res = new Vector3[array.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = array[i] + add;
            return res;
        }

        private Vector3[] GetPointsToTest(Vector3[] hitPoints, Vector3 absTranslation, Vector3 axis)
        {
            if (axis == Vector3.UNIT_Y)
            {
                Vector3[] pointsToTest = new Vector3[NBR_HIT_POINTS];
                int[] indexes = absTranslation.y < 0 ? this.CreateIntArray(0, pointsToTest.Length) :
                                                       this.CreateIntArray(NBR_HIT_POINTS * (mNbrHitStage - 1), pointsToTest.Length);

                for (int i = 0; i < pointsToTest.Length; i++) { pointsToTest[i] = hitPoints[indexes[i]]; }
                return pointsToTest;
            }

            return hitPoints;
        }

        private int[] CreateIntArray(int begin, int length)
        {
            int[] res = new int[length];
            for (int i = begin; i < begin + length; i++) { res[i - begin] = i; }
            return res;
        }
    }
}

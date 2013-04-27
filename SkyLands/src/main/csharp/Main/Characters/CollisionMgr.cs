using System.Collections.Generic;
using Mogre;

using Game.World;
using API.Generic;

namespace Game.CharacSystem
{
    public class CollisionMgr
    {
        private const float COL_SIDE_MARGE = 0.61f;
        private const int NBR_HIT_POINTS = 15;   // Nbr of points for collisions there are at the same height

        private MainWorld        mWorld;
        private VanillaCharacter mCharac;
        private Degree[]         mHitDegrees;
        private float            mHitRadius;
        private SceneNode[]      mPoints;    // mPoints is used to show the cube of collision
        private int              mNbrHitStage;

        public CollisionMgr(SceneManager sceneMgr, MainWorld world, VanillaCharacter charac)
        {
            this.mWorld = world;
            this.mCharac = charac;
            this.mNbrHitStage = 2 + (int)charac.Height / Cst.CUBE_SIDE;

            this.mHitRadius = VanillaCharacter.CHARAC_SIZE.x / 2 * COL_SIDE_MARGE;
            this.mHitDegrees = new Degree[NBR_HIT_POINTS];
            this.mPoints = new SceneNode[this.mHitDegrees.Length * this.mNbrHitStage];
            Degree delta = 360 / this.mHitDegrees.Length;
            for (int i = 0; i < this.mHitDegrees.Length; i++)
            {
                this.mHitDegrees[i] = i * delta;

                for (int j = 0; j < this.mNbrHitStage; j++)
                {
                    Entity cube = sceneMgr.CreateEntity("cube.mesh");
                    int index = i + j * NBR_HIT_POINTS;
                    this.mPoints[index] = sceneMgr.RootSceneNode.CreateChildSceneNode();
                    this.mPoints[index].AttachObject(cube);
                    this.mPoints[index].Scale(0.01f * Vector3.UNIT_SCALE);
                    this.mPoints[index].SetVisible(false);
                }
            }
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
                    hitPoints[index].y += j < this.mNbrHitStage - 1 ? j * Cst.CUBE_SIDE : this.mCharac.Height;
                }
            }

            return hitPoints;
        }

        public void DrawPoints()
        {
            if (this.mCharac.Info.IsPlayer)
            {
                if ((this.mCharac as VanillaPlayer).Input.WasKeyPressed(MOIS.KeyCode.KC_F4))
                    foreach (SceneNode node in this.mPoints) { node.FlipVisibility(); }
                Vector3[] points = this.GetHitPoints();
                for (int i = 0; i < this.mPoints.Length; i++)
                    this.mPoints[i].Position = points[i];
            }
        }


        public Vector3 ComputeCollision(Vector3 absTranslation)  // Takes the wanted translation and returns the possible one
        {
            /* Temp */
            absTranslation.x = MathHelper.clamp<float>(absTranslation.x, -50, 50);
            absTranslation.y = MathHelper.clamp<float>(absTranslation.y, -50, 50);
            absTranslation.z = MathHelper.clamp<float>(absTranslation.z, -50, 50);

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
                    if (this.mWorld.HasPointCollision(ref hitPointsToTest[i], this.mCharac.Info.IslandLoc))
                    {
                        /* Compute the possible translation */
                        float val = 0;
                        if (axis == Vector3.UNIT_Y)
                        {
                            if (this.GetNonNullValue(absTranslation * axis) < 0)
                            { val = (hitPointsToTest[i].y + 1) * Cst.CUBE_SIDE - this.mCharac.FeetPosition.y; }
                            /*else
                            {
                                val = charac.FeetPosition.y + charac.Height - temp[i].y * Cst.CUBE_SIDE;
                                if (absTranslation.y > 0) { absTranslation.y = 0; }
                            }*/
                        }

                        actHitPoints = this.AddToAll(hitPoints, val * axis);   // Cancel the translation of the actual axis
                        actTranslation = (actTranslation * (Vector3.UNIT_SCALE - axis)) + val * axis;    // Update the actTranslation

                        //if (axis == Vector3.UNIT_Y) { break; }
                        break;
                    }
                }
            }

            return actTranslation;
        }

        private float GetNonNullValue(Vector3 v)
        {
            if (v.x != 0)      { return v.x; }
            else if (v.y != 0) { return v.y; }
            else               { return v.z; }
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
            /*if (axis == Vector3.UNIT_Y)
            {
                if (absTranslation.y < 0) { return this.GetIntArray(0, NBR_HIT_POINTS); }
                else                      { return this.GetIntArray(NBR_HIT_POINTS * (mNbrHitStage - 1), NBR_HIT_POINTS * mNbrHitStage); }
            }
            else                          { return this.GetIntArray(0, NBR_HIT_POINTS * mNbrHitStage); }*/
            if (axis == Vector3.UNIT_Y)
            {
                Vector3[] pointsToTest = new Vector3[NBR_HIT_POINTS];
                int[] indexes;
                if (absTranslation.y < 0) { indexes = this.CreateIntArray(0, pointsToTest.Length); }
                else { indexes = this.CreateIntArray(NBR_HIT_POINTS * (mNbrHitStage - 1), pointsToTest.Length); }

                for (int i = 0; i < pointsToTest.Length; i++) { pointsToTest[i] = hitPoints[indexes[i]]; }
                return pointsToTest;
            }
            else
            {
                /*Vector3 newPos = this.mCharac.FeetPosition + absTranslation;
                float distMax = this.Distance(hitPoints[0], newPos);
                pointsToTest = new Vector3[NBR_HIT_POINTS / 2 * this.mNbrHitStage];
                int actIndex = 0;
                for (int i = 0; i < NBR_HIT_POINTS; i++)    // Only run through the below points
                {
                    float dist = this.Distance(hitPoints[i], newPos);
                    if (dist <= distMax)
                    {
                        for (int j = 0; j < this.mNbrHitStage; j++) // Add the point to test for all stages
                        {
                            pointsToTest[actIndex + NBR_HIT_POINTS / 2 * j] = hitPoints[i];
                            pointsToTest[actIndex + NBR_HIT_POINTS / 2 * j].y += j < this.mNbrHitStage - 1 ? j * Cst.CUBE_SIDE : this.mCharac.Height;
                        }
                        actIndex++;
                    }
                }*/
                return hitPoints;
            }
        }

        private int[] CreateIntArray(int begin, int length)
        {
            int[] res = new int[length];
            for (int i = begin; i < begin + length; i++) { res[i - begin] = i; }
            return res;
        }

        /*private float Distance (Vector3 v1, Vector3 v2)  // Squared distance without considering the y axis
        {
            return (v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z);
        }*/

        //private void 
    }
}

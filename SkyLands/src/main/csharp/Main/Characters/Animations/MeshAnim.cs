using System;

using Mogre;

namespace Game.Animation
{
    public abstract class MeshAnim
    {
        protected AnimationMgr mAnimMgr;
        private readonly Vector3 mMeshSize;
        private readonly int mFeetDiff;
        private Vector3 mMoveForwardDir = Vector3.UNIT_Z;
        private Quaternion mInitialOrientation = Quaternion.IDENTITY;

        public AnimationMgr AnimMgr { get { return this.mAnimMgr; } }
        public Vector3 MeshSize { get { return this.mMeshSize; } }
        public Vector3 MoveForwardDir { get { return this.mMoveForwardDir; } protected set { this.mMoveForwardDir = value; } }
        public Quaternion InitialOrientation { get { return this.mInitialOrientation; } protected set { this.mInitialOrientation = value; } }
        public int FeetDiff { get { return this.mFeetDiff; } }

        protected MeshAnim(AnimationMgr animMgr, Vector3 meshSize, int feetDiff)
        {
            this.mAnimMgr = animMgr;
            this.mMeshSize = meshSize;
            this.mFeetDiff = feetDiff;
        }

        public abstract void Idle(bool on = true);
        public abstract void Walk(bool on = true, int timeFactor = 1);
        public abstract void StartJump();
        public abstract void JumpLoop();
        public abstract bool IsAbleTo(string anim);
        public virtual void ToFreeCamMode() {}

        public static string[] GetString<T>(params T[] anims) where T : struct, IConvertible
        {
            string[] result = new string[anims.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = Enum.GetName(typeof(T), anims[i]);

            return result;
        }

        public void Update(float frameTime)
        {
            this.mAnimMgr.Update(frameTime);
        }

        public int Zdir(Vector3 direction)
        {
            if (this.mMoveForwardDir == Vector3.UNIT_X)
                return (int)Mogre.Math.Sign(direction.x);
            else
                return (int)Mogre.Math.Sign(direction.z);
        }
    }
}

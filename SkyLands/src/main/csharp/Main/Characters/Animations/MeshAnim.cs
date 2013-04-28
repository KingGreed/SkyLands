using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.Animation
{
    public abstract class MeshAnim
    {
        protected AnimationMgr mAnimMgr;
        private Vector3 mMeshSize;
        private int mFeetDiff;

        public AnimationMgr AnimMgr { get { return this.mAnimMgr; } }
        public Vector3 MeshSize { get { return this.mMeshSize; } }
        public int FeetDiff { get { return this.mFeetDiff; } }

        public MeshAnim(AnimationMgr animMgr, Vector3 meshSize, int feetDiff)
        {
            this.mAnimMgr = animMgr;
            this.mMeshSize = meshSize;
            this.mFeetDiff = feetDiff;
        }

        public abstract void Idle(bool on = true);
        public abstract void Walk(bool on = true, int timeFactor = 1);
        public abstract void StartJump();
        public abstract bool IsAbleTo(string anim);

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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.Animation
{
    class Sinbad : MeshAnim
    {
        public static readonly Vector3 SINBAD_SIZE = new Vector3(65, 99, 65);
        public static int FEET_DIFF = 8;
        public enum AnimName : byte { IdleBase, IdleTop, RunBase, RunTop, JumpStart, JumpLoop, JumpEnd, Dance }

        public Sinbad(Entity ent) : base(new AnimationMgr(ent.AllAnimationStates, Enum.GetNames(typeof(AnimName)),
            new string[] { Enum.GetName(typeof(AnimName), AnimName.JumpStart), Enum.GetName(typeof(AnimName), AnimName.JumpEnd) }), SINBAD_SIZE, FEET_DIFF)
        {

        }

        public override void Idle(bool on = true)
        {
            if (on) { this.mAnimMgr.SetAnims(MeshAnim.GetString(AnimName.IdleBase, AnimName.IdleTop)); }
            else    { this.mAnimMgr.DeleteAnims(MeshAnim.GetString(AnimName.IdleBase, AnimName.IdleTop)); }
        }

        public override void Walk(bool on = true, int timeFactor = 1)
        {
            if (on) { this.mAnimMgr.SetAnims(timeFactor, MeshAnim.GetString(AnimName.RunBase, AnimName.RunTop)); }
            else    { this.mAnimMgr.DeleteAnims(MeshAnim.GetString(AnimName.RunBase, AnimName.RunTop)); }
        }

        public override void StartJump()
        {
            this.mAnimMgr.SetAnims(MeshAnim.GetString(AnimName.JumpStart, AnimName.JumpLoop));
        }

        public void JumpLoop()
        {
            this.mAnimMgr.SetAnims(MeshAnim.GetString(AnimName.JumpLoop));
        }

        public void EndJump()
        {
            this.mAnimMgr.SetAnims(MeshAnim.GetString(AnimName.JumpEnd));
        }

        public override bool IsAbleTo(string anim)
        {
            List<string> anims = new List<string>(Enum.GetNames(typeof(AnimName)));
            for (int i = 0; i < anims.Count; i++)
                anims[i] = anims[i].ToLower();

            return anims.Contains(anim.ToLower());
        }

        public void SwitchToDebugMode()
        {
            this.mAnimMgr.DeleteAllExcept<string[]>(MeshAnim.GetString(AnimName.IdleBase, AnimName.IdleTop, AnimName.JumpStart, AnimName.JumpLoop, AnimName.JumpEnd, AnimName.Dance));
        }
    }
}

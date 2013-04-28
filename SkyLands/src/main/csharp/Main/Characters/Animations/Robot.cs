using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mogre;

namespace Game.Animation
{
    class Robot : MeshAnim
    {
        public static readonly Vector3 ROBOT_SIZE = new Vector3(70, 130, 85);
        public static int FEET_DIFF = -65;
        public enum AnimName : byte { Idle, Walk, Slump, Shoot, Die }

        public Robot(Entity ent) : base(new AnimationMgr(ent.AllAnimationStates, Enum.GetNames(typeof(AnimName)),
                new string[] { Enum.GetName(typeof(AnimName), AnimName.Slump), Enum.GetName(typeof(AnimName), AnimName.Shoot), Enum.GetName(typeof(AnimName), AnimName.Die) }), ROBOT_SIZE, FEET_DIFF)
        {

        }

        public override void Idle(bool on = true)
        {
            if (on) { this.mAnimMgr.SetAnims(MeshAnim.GetString(AnimName.Idle)); }
            else { this.mAnimMgr.DeleteAnims(MeshAnim.GetString(AnimName.Idle)); }
        }

        public override void Walk(bool on = true, int timeFactor = 1)
        {
            if (on) { this.mAnimMgr.SetAnims(timeFactor, MeshAnim.GetString(AnimName.Walk)); }
            else    { this.mAnimMgr.DeleteAnims(MeshAnim.GetString(AnimName.Walk)); }
        }

        public override void StartJump()
        {
            this.mAnimMgr.SetAnims(MeshAnim.GetString(AnimName.Slump));
        }

        public void Shoot()
        {
            this.mAnimMgr.AddAnims(MeshAnim.GetString(AnimName.Shoot));
        }

        public void Die()
        {
            this.mAnimMgr.SetAnims(new Anim(Enum.GetName(typeof(AnimName), AnimName.Die), 0.85f, true));
        }

        public override bool IsAbleTo(string anim)
        {
            List<string> anims = new List<string>(Enum.GetNames(typeof(AnimName)));
            for (int i = 0; i < anims.Count; i++)
                anims[i] = anims[i].ToLower();

            return anims.Contains(anim.ToLower());
        }
    }
}

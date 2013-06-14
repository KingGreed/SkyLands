using System;
using System.Collections.Generic;
using System.Linq;
using Mogre;

using Game.Input;

namespace Game.Animation
{
    public class Sinbad : MeshAnim
    {
        public static readonly Vector3 SINBAD_SIZE = new Vector3(65, 99, 65);
        public const int FEET_DIFF = 8;

        private enum AnimName : byte { IdleBase, IdleTop, RunBase, RunTop, JumpStart, JumpLoop, JumpEnd, Dance }
        private struct Emote
        {
            private readonly AnimName mAnim;
            private readonly UserAction mAction;

            public AnimName Anim { get { return this.mAnim; } }
            public UserAction Action { get { return this.mAction; } }

            public Emote(UserAction action, AnimName anim) { this.mAction = action; this.mAnim = anim; }
        }

        private readonly Emote[] mEmotes;
        private readonly AnimName[] mEmotesNames;

        public Sinbad(Entity ent)
            : base(new AnimationMgr(ent.AllAnimationStates, Enum.GetNames(typeof(AnimName)),
            new string[] { Enum.GetName(typeof(AnimName), AnimName.JumpStart), Enum.GetName(typeof(AnimName), AnimName.JumpEnd) }),
            SINBAD_SIZE, FEET_DIFF)
        {
            this.mEmotes = new Emote[]
            {
                new Emote(UserAction.Dance, AnimName.Dance)
            };
            this.mEmotesNames = new AnimName[this.mEmotes.Length];
            for (int i = 0; i < this.mEmotes.Length; i++)
                this.mEmotesNames[i] = this.mEmotes[i].Anim;
        }

        public override void Idle(bool on = true)
        {
            if (on) { this.mAnimMgr.SetAnims(GetString(AnimName.IdleBase, AnimName.IdleTop)); }
            else    { this.mAnimMgr.DeleteAnims(GetString(AnimName.IdleBase, AnimName.IdleTop)); }
        }

        public override void Walk(bool on = true, int timeFactor = 1)
        {
            if (on) { this.mAnimMgr.SetAnims(timeFactor, GetString(AnimName.RunBase, AnimName.RunTop)); }
            else    { this.mAnimMgr.DeleteAnims(GetString(AnimName.RunBase, AnimName.RunTop)); }
        }

        public override void StartJump()
        {
            this.mAnimMgr.SetAnims(GetString(AnimName.JumpStart, AnimName.JumpLoop));
        }

        public override void JumpLoop()
        {
            this.mAnimMgr.SetAnims(GetString(AnimName.JumpLoop));
        }

        public void EndJump()
        {
            this.mAnimMgr.SetAnims(GetString(AnimName.JumpEnd));
        }

        public override bool IsAbleTo(string anim)
        {
            List<string> anims = new List<string>(Enum.GetNames(typeof(AnimName)));
            for (int i = 0; i < anims.Count; i++)
                anims[i] = anims[i].ToLower();

            return anims.Contains(anim.ToLower());
        }

        public void UpdateEmotes(Controller controller)
        {
            if (this.AnimMgr.AreAnimationsPlaying(GetString(AnimName.JumpStart, AnimName.JumpLoop, AnimName.JumpEnd,
                                                            AnimName.RunBase, AnimName.RunTop))) { return; }
            foreach (Emote emote in this.mEmotes.Where(emote => controller.HasActionOccured(emote.Action)))
            {
                string[] names = GetString(emote.Anim);
                if (!this.AnimMgr.AreAnimationsPlaying(names))
                    this.AnimMgr.SetAnims(names);
                else
                    this.AnimMgr.DeleteAnims(names);
            }
        }

        public override void ToFreeCamMode()
        {
            this.mAnimMgr.DeleteAllExcept<string[]>(GetString(AnimName.IdleBase,
                                                                       AnimName.IdleTop,
                                                                       AnimName.JumpStart,
                                                                       AnimName.JumpLoop,
                                                                       AnimName.JumpEnd,
                                                                       AnimName.Dance));
        }
    }
}

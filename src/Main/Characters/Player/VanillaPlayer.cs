using System;
using System.Collections.Generic;
using Mogre;

using Game.World;
using Game.Animation;
using Game.World.Display;
using Game.IGConsole;

namespace Game.CharacSystem
{
    public class VanillaPlayer : VanillaCharacter
    {
        private struct Emote
        {
            private AnimName mAnim;
            private MOIS.KeyCode mKey;

            public AnimName Anim        { get { return this.mAnim; } }
            public MOIS.KeyCode Key { get { return this.mKey; } }

            public Emote(MOIS.KeyCode key, AnimName anim) { this.mKey = key; this.mAnim = anim; }
        }

        private static float YAW_SENSIVITY = 1;
        private static float PITCH_SENSIVITY = 0.15f;

        private MoisManager      mInput;
        private Emote[]          mEmotes;
        private AnimName[]       mEmotesNames;
        private float            mYawCamValue;
        private float            mPitchCamValue;
        private bool             mIsFirstView;
        private bool             mIsDebugMode;
        private MainPlayerCamera mCam;

        public MoisManager Input         { get { return this.mInput; } }
        public float       YawCamValue   { get { return this.mYawCamValue; } }
        public float       PitchCamValue { get { return this.mPitchCamValue; } }
        public bool        IsFirstView   { get { return this.mIsFirstView; } }
        public bool        IsDebugMode
        { 
            get { return this.mIsDebugMode; }
            set
            {
                this.mIsDebugMode = value;
                this.mNode.SetVisible(this.mIsDebugMode);
            }
        }

        public VanillaPlayer(CharacMgr characMgr, string meshName, CharacterInfo info, MoisManager input) : base(characMgr, meshName, info)
        {
            this.mInput = input;
            this.mIsFirstView = true;

            this.mEmotes = new Emote[]
            {
                new Emote(MOIS.KeyCode.KC_1, AnimName.Dance)
            };
            this.mEmotesNames = new AnimName[this.mEmotes.Length];
            for (int i = 0; i < this.mEmotes.Length; i++)
                this.mEmotesNames[i] = this.mEmotes[i].Anim;
        }

        public void AttachCamera(MainPlayerCamera cam)
        { 
            this.mCam = cam;
        }

        public new void Update(float frameTime)
        {
            bool isNowMoving = !this.mIsDebugMode || this.mInput.IsCtrltDown;
            if (this.mMovementInfo.IsAllowedToMoved && !isNowMoving)
                this.mAnimMgr.DeleteAllExcept<AnimName[]>(this.mEmotesNames, this.mIdleAnims, this.mJumpAnims);
            this.mMovementInfo.IsAllowedToMoved = isNowMoving;

            if (this.mMovementInfo.IsAllowedToMoved)
            {
                float yawValue = -this.mInput.MouseMoveX * YAW_SENSIVITY;
                float pitchValue = -this.mInput.MouseMoveY * PITCH_SENSIVITY;

                if (this.mIsFirstView) { this.FirstPersonUpdate(yawValue, pitchValue); }
                else { this.ThirdPersonUpdate(yawValue, pitchValue); }

                if (this.mInput.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Left))  { this.OnLClick(); }
                if (this.mInput.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Right)) { this.OnRClick(); }

                /* Update emotes animations */
                if (!this.mAnimMgr.AreAnimationsPlaying(AnimName.JumpStart, AnimName.JumpLoop, AnimName.JumpEnd, AnimName.RunBase, AnimName.RunTop))
                {
                    foreach (Emote emote in this.mEmotes)
                    {
                        if (this.mInput.WasKeyPressed(emote.Key))
                        {
                            if (!this.mAnimMgr.AreAnimationsPlaying(emote.Anim)) { this.mAnimMgr.SetAnims(emote.Anim); }
                            else                                                 { this.mAnimMgr.DeleteAnims(emote.Anim); }
                        }
                    }
                }
            }
        }

        private void FirstPersonUpdate(float yawValue, float pitchValue)
        {
            Vector3 moveDirection = new Vector3();
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_W) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_UP))    { moveDirection.z = 1; }
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_S) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_DOWN))  { moveDirection.z = -1; }
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_A) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_LEFT))  { moveDirection.x = 1; }
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_D) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_RIGHT)) { moveDirection.x = -1; }
            if (this.mInput.WasKeyPressed(MOIS.KeyCode.KC_SPACE))                                         { moveDirection.y = 1; }
            this.mMovementInfo.MoveDirection = moveDirection;

            this.mMovementInfo.YawValue = yawValue;

            this.mYawCamValue = 0;
            this.mPitchCamValue = pitchValue;
        }

        private void ThirdPersonUpdate(float yawValue, float pitchValue) { }

        private bool GetBlockPos(out Vector3 relBlockPos, out API.Geo.Cuboid.Block b, out CubeFace face)
        {
            float distMax = 200;
            float distance = 0;
            relBlockPos = Vector3.ZERO;
            b = null;
            face = CubeFace.frontFace;

            Ray ray = this.mCam.Camera.GetCameraToViewportRay(0.5f, 0.5f);
            do
            {
                distance += 40;
                relBlockPos = ray.GetPoint(distance) / MainWorld.CUBE_SIDE;
                relBlockPos.x = Mogre.Math.IFloor(relBlockPos.x);
                relBlockPos.y = Mogre.Math.IFloor(relBlockPos.y);
                relBlockPos.z = Mogre.Math.IFloor(relBlockPos.z);
                relBlockPos.z++;
                b = this.mCharacMgr.World.getIslandAt(this.mCharInfo.IslandLoc).getBlock(relBlockPos, false);
            } while (b is Game.World.Blocks.AirBlock && distance <= distMax);

            if (distance > distMax)
            {
                relBlockPos = Vector3.ZERO;
                b = null;
                return false;
            }
            else
            {
                /* Compute the exact position */
                Vector3 absPosBlock = relBlockPos * MainWorld.CUBE_SIDE;
                int index = -1;
                float minDist = -1;
                Vector3[] points = Game.World.Generator.VanillaMultiBlock.blockPointCoords;
                for (int i = 0; i < points.Length; i += 4)
                {
                    Mogre.Plane p = new Plane(points[i] + absPosBlock, points[i + 1] + absPosBlock, points[i + 2] + absPosBlock); // Need the 3 first points

                    Vector3 v1 = p.normal * ray.Origin;
                    Vector3 v2 = p.normal * ray.Direction;
                    float dist = (-v1.x - v1.y - v1.z - p.d) / (v2.x + v2.y + v2.z);    // distance between the origin of the ray and the plan

                    if (dist > 0)
                    {
                        Vector3 actPoint = ray.GetPoint(dist);

                        if ((minDist < 0 || dist < minDist) && MathHelper.isInBlock(absPosBlock, actPoint, MainWorld.CUBE_SIDE))
                        {
                            minDist = dist;
                            index = i / 4;
                        }
                    }
                }

                face = (CubeFace)index;

                return true;
            }
        }

        private void OnLClick()
        {
            Vector3 relBlockPos;
            API.Geo.Cuboid.Block b;
            CubeFace f;
            if (!this.GetBlockPos(out relBlockPos, out b, out f)) { return; }

            string material = b.getName();
            this.mCharacMgr.World.getIslandAt(this.mCharInfo.IslandLoc).removeFromScene(relBlockPos);  // Delete block

            if (material != "Air")
                this.mCharacMgr.StateMgr.WriteOnConsole("Deleted : " + material);
        }

        private void OnRClick()
        {
            /*Vector3 relBlockPos;
            API.Geo.Cuboid.Block b;
            CubeFace face;
            if (!this.GetBlockPos(out relBlockPos, out b, out face)) { return; }*/
            float distance = 200;
            Vector3 relBlockPos = this.mCam.Camera.GetCameraToViewportRay(0.5f, 0.5f).GetPoint(distance);

            relBlockPos /= MainWorld.CUBE_SIDE;
            relBlockPos.x = Mogre.Math.IFloor(relBlockPos.x);
            relBlockPos.y = Mogre.Math.IFloor(relBlockPos.y);
            relBlockPos.z = Mogre.Math.IFloor(relBlockPos.z) + 1;

            API.Geo.Cuboid.Block b = this.mCharacMgr.World.getIslandAt(this.mCharInfo.IslandLoc).getBlock(relBlockPos, false);

            if (b is Game.World.Blocks.ConstructionBlock) 
            {
                CharacterInfo iaInfo = new CharacterInfo("NPC_" + Guid.NewGuid().ToString(), false);
                iaInfo.SpawnPoint = this.mCharacMgr.World.getSpawnPoint(); ;
                this.mCharacMgr.AddCharacter(iaInfo);

                //this.mCharacMgr.GetCharacter(this.mCharacMgr.getNumberOfCharacter() - 1).moveTo(relBlockPos * MainWorld.CHUNK_SIDE);
                
            }
            else
            {
                /*if      (face == CubeFace.underFace) { relBlockPos.y--; }
                else if (face == CubeFace.upperFace) { relBlockPos.y++; }
                else if (face == CubeFace.leftFace) { relBlockPos.x--; }
                else if (face == CubeFace.rightFace) { relBlockPos.x++; }
                else if (face == CubeFace.backFace) { relBlockPos.z--; }
                else  /*(face == CubeFace.frontFace)* { relBlockPos.z++; }*/
                
                string material = "";
                if (this.mInput.IsOneKeyEventTrue(this.mInput.IsKeyDown, MOIS.KeyCode.KC_1, MOIS.KeyCode.KC_NUMPAD1))
                    material = "Grass";
                else if (this.mInput.IsOneKeyEventTrue(this.mInput.IsKeyDown, MOIS.KeyCode.KC_2, MOIS.KeyCode.KC_NUMPAD2))
                    material = "Dirt";
                else if (this.mInput.IsOneKeyEventTrue(this.mInput.IsKeyDown, MOIS.KeyCode.KC_3, MOIS.KeyCode.KC_NUMPAD3))
                    material = "Stone";
                else if (this.mInput.IsOneKeyEventTrue(this.mInput.IsKeyDown, MOIS.KeyCode.KC_4, MOIS.KeyCode.KC_NUMPAD4))
                    material = "Wood";
                else if (this.mInput.IsOneKeyEventTrue(this.mInput.IsKeyDown, MOIS.KeyCode.KC_5, MOIS.KeyCode.KC_NUMPAD5))
                    material = "Leaves";
                else if (this.mInput.IsOneKeyEventTrue(this.mInput.IsKeyDown, MOIS.KeyCode.KC_6, MOIS.KeyCode.KC_NUMPAD6))
                    material = "Sand";
                else if (this.mInput.IsOneKeyEventTrue(this.mInput.IsKeyDown, MOIS.KeyCode.KC_7, MOIS.KeyCode.KC_NUMPAD7))
                    material = "Construction";

                if (material != "")
                {
                    this.mCharacMgr.World.getIslandAt(this.mCharInfo.IslandLoc).addBlockToScene(relBlockPos, material);
                    //this.mCharacMgr.StateMgr.WriteOnConsole("Face : " + Enum.GetName(typeof(CubeFace), face));
                    this.mCharacMgr.StateMgr.WriteOnConsole("Added : " + material);
                }
            }
        }
    }
}

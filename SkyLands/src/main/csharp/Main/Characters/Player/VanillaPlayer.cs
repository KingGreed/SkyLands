using System;
using System.Collections.Generic;
using Mogre;

using Game.World;
using Game.Animation;
using Game.Shoot;
using Game.GUICreator;

using API.Geo.Cuboid;
using API.Generic;

using Game.Buildings;

namespace Game.CharacSystem
{
    public class VanillaPlayer : VanillaCharacter
    {
        private struct Emote
        {
            private Sinbad.AnimName mAnim;
            private MOIS.KeyCode mKey;

            public Sinbad.AnimName Anim { get { return this.mAnim; } }
            public MOIS.KeyCode Key { get { return this.mKey; } }

            public Emote(MOIS.KeyCode key, Sinbad.AnimName anim) { this.mKey = key; this.mAnim = anim; }
        }

        public static float DEFAULT_PLAYER_LIFE = 340;
        private static float YAW_SENSIVITY = 0.4f;
        private static float PITCH_SENSIVITY = 0.15f;

        private MoisManager      mInput;
        private Emote[]          mEmotes;
        private Sinbad.AnimName[] mEmotesNames;
        private float            mYawCamValue;
        private float            mPitchCamValue;
        private bool             mIsFirstView;
        private bool             mIsDebugMode;
        private ShootCube        mShootCube;
        private MainPlayerCamera mCam;
        private HUD              mHud;

        public MoisManager Input         { get { return this.mInput; } }
        public float       YawCamValue   { get { return this.mYawCamValue; } }
        public float       PitchCamValue { get { return this.mPitchCamValue; } }
        public bool        IsFirstView   { get { return this.mIsFirstView; } }
        public Camera      Camera        { get { return this.mCam.Camera; } }
        public Degree      Pitch         { get { return this.mCam.Pitch; } }
        public HUD         HUD           { get { return this.mHud; } }
        public bool IsMainPlayer { get { return this.mCam != null; } }
        public bool IsDebugMode
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

            SceneManager sceneMgr = characMgr.SceneMgr;
            Mogre.Entity ent = sceneMgr.CreateEntity("CharacterEnt_" + this.mCharInfo.Id, meshName);
            ent.Skeleton.BlendMode = SkeletonAnimationBlendMode.ANIMBLEND_CUMULATIVE;
            Mogre.Entity swordL = sceneMgr.CreateEntity("Sword.mesh");
            ent.AttachObjectToBone("Sheath.L", swordL);
            Mogre.Entity swordR = sceneMgr.CreateEntity("Sword.mesh");
            ent.AttachObjectToBone("Sheath.R", swordR);
            this.mMesh = new Sinbad(ent);

            this.mNode.AttachObject(ent);
            this.mNode.Scale(this.mMesh.MeshSize / ent.BoundingBox.Size);

            this.mCollisionMgr = new CollisionMgr(characMgr.SceneMgr, this.mCharacMgr.World, this);
            this.FeetPosition = this.mCharInfo.SpawnPoint;
            this.mShootCube = new ShootCube(this.mCharacMgr.SceneMgr, this, this.mCharacMgr.BulletMgr, "fireball");

            this.mEmotes = new Emote[]
            {
                new Emote(MOIS.KeyCode.KC_1, Sinbad.AnimName.Dance)
            };
            this.mEmotesNames = new Sinbad.AnimName[this.mEmotes.Length];
            for (int i = 0; i < this.mEmotes.Length; i++)
                this.mEmotesNames[i] = this.mEmotes[i].Anim;
        }

        public void MainPlayer(MainPlayerCamera cam, HUD hud)
        {
            this.mCam = cam;
            this.mHud = hud;
            this.mHud.UpdateLife(this.mCharInfo.Life, VanillaPlayer.DEFAULT_PLAYER_LIFE);
        }

        public new void Update(float frameTime)
        {
            bool isNowMoving = !this.mIsDebugMode || this.mInput.IsCtrltDown;
            if (this.mMovementInfo.IsAllowedToMove && !isNowMoving)
                (this.mMesh as Sinbad).SwitchToDebugMode();
            this.mMovementInfo.IsAllowedToMove = isNowMoving;

            if (this.mMovementInfo.IsAllowedToMove)
            {
                float yawValue = -this.mInput.MouseMoveX * YAW_SENSIVITY;
                float pitchValue = -this.mInput.MouseMoveY * PITCH_SENSIVITY;

                if (this.mIsFirstView) { this.FirstPersonUpdate(yawValue, pitchValue); }
                else { this.ThirdPersonUpdate(yawValue, pitchValue); }

                if (this.mInput.IsShiftDown)
                {
                    if (this.mInput.IsMouseButtonDown(MOIS.MouseButtonID.MB_Left)) { this.mShootCube.Grow(frameTime, this.mInput.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Left)); }
                    if (this.mInput.WasMouseButtonReleased(MOIS.MouseButtonID.MB_Left)) { this.mShootCube.Burst(); }
                }
                else
                {
                    bool leftClick = this.mInput.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Left);
                    bool rightClick = this.mInput.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Right);
                    if (!this.mIsDebugMode && leftClick || rightClick)
                    {
                        Vector3 relBlockPos;
                        API.Geo.Cuboid.Block b;
                        CubeFace f;
                        if (!this.GetBlockPos(out relBlockPos, out b, out f)) { return; }
                        
                        if (leftClick)
                        {
                            this.mCharacMgr.World.onLeftClick(relBlockPos);
                            this.AddBlock();
                        }
                        if (rightClick)
                        {
                            this.mCharacMgr.World.onRightClick(relBlockPos);
                            //this.DelBlock();
                        }
                    }
                }
                if (this.mInput.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Middle)) { this.setIsPushedByArcaneLevitator(!this.mMovementInfo.IsPushedByArcaneLevitator); }

                /* Update emotes animations */
                if (!this.mMesh.AnimMgr.AreAnimationsPlaying(MeshAnim.GetString<Sinbad.AnimName>(Sinbad.AnimName.JumpStart, Sinbad.AnimName.JumpLoop, Sinbad.AnimName.JumpEnd, Sinbad.AnimName.RunBase, Sinbad.AnimName.RunTop)))
                {
                    foreach (Emote emote in this.mEmotes)
                    {
                        if (this.mInput.WasKeyPressed(emote.Key))
                        {
                            if (!this.mMesh.AnimMgr.AreAnimationsPlaying(MeshAnim.GetString<Sinbad.AnimName>(emote.Anim))) { this.mMesh.AnimMgr.SetAnims(MeshAnim.GetString<Sinbad.AnimName>(emote.Anim)); }
                            else { this.mMesh.AnimMgr.DeleteAnims(MeshAnim.GetString<Sinbad.AnimName>(emote.Anim)); }
                        }
                    }
                }
            }
        }

        private void FirstPersonUpdate(float yawValue, float pitchValue)
        {
            Vector3 moveDirection = new Vector3();
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_W) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_UP)) { moveDirection.z = 1; }
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_S) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_DOWN)) { moveDirection.z = -1; }
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_A) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_LEFT)) { moveDirection.x = 1; }
            if (this.mInput.IsKeyDown(MOIS.KeyCode.KC_D) || this.mInput.IsKeyDown(MOIS.KeyCode.KC_RIGHT)) { moveDirection.x = -1; }
            if (this.mInput.WasKeyPressed(MOIS.KeyCode.KC_SPACE)) { moveDirection.y = 1; }
            this.mMovementInfo.MoveDirection = moveDirection;

            this.mMovementInfo.YawValue = yawValue;

            this.mYawCamValue = 0;
            this.mPitchCamValue = pitchValue;
        }

        private void ThirdPersonUpdate(float yawValue, float pitchValue) { }

        private bool GetBlockPos(out Vector3 relBlockPos, out Block b, out CubeFace face)
        {
            float distMax = 250;
            float distance = 0;
            relBlockPos = Vector3.ZERO;
            b = null;
            face = CubeFace.frontFace;

            Ray ray = this.mCam.Camera.GetCameraToViewportRay(0.5f, 0.5f);
            do
            {
                distance += 40;
                relBlockPos = MainWorld.AbsToRelative(ray.GetPoint(distance));
                b = this.mCharacMgr.World.getIsland().getBlock(relBlockPos, false);
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
                Vector3 absPosBlock = relBlockPos * Cst.CUBE_SIDE;
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

                        if ((minDist < 0 || dist < minDist) && MathHelper.isInBlock(absPosBlock, actPoint, Cst.CUBE_SIDE))
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

        /* Test of ray for vertical collision */
        /*private void OnMClick()
        {
            Vector3 origin = this.FeetPosition + this.Height * Vector3.UNIT_Y;
            origin.y += 200;
            Ray ray = new Ray(origin, Vector3.NEGATIVE_UNIT_Y);
            RaySceneQuery raySQuery = this.mCharacMgr.SceneMgr.CreateRayQuery(ray);
            raySQuery.SetSortByDistance(true, 15);

            RaySceneQueryResult raySQResult = raySQuery.Execute();
            for (Int16 i = 0; i < raySQResult.Count; i++)
                Console.WriteLine("distance : " + raySQResult[i].distance + "\t" + raySQResult[i].movable.MovableType + "\t" + raySQResult[i].movable.Name);
            Console.WriteLine();

            StaticRectangle.DrawLine(this.mCharacMgr.SceneMgr, origin, ray.GetPoint(raySQResult[raySQResult.Count - 1].distance));
        }*/

        /*private void DelBlock()
        {
            Vector3 relBlockPos;
            API.Geo.Cuboid.Block b;
            CubeFace f;
            if (!this.GetBlockPos(out relBlockPos, out b, out f)) { return; }

            string material = b.getName();
            this.mCharacMgr.World.onDeletion(relBlockPos, this.mCharInfo.IslandLoc);
            //this.mCharacMgr.World.getIslandAt(this.mCharInfo.IslandLoc).removeFromScene(relBlockPos);  // Delete block

            if (material != "Air")
                this.mCharacMgr.StateMgr.WriteOnConsole("Deleted : " + material);
        }*/

        private void AddBlock()
        {
            /*Vector3 relBlockPos;
            API.Geo.Cuboid.Block b;
            CubeFace face;
            if (!this.GetBlockPos(out relBlockPos, out b, out face)) { return; }*/
            Island island = this.mCharacMgr.World.getIsland();
            Block block = null;

            float distance = 10;
            Vector3 curRelBlockPos = Vector3.ZERO, prevRelBlockPos = Vector3.ZERO;
            Ray ray = this.mCam.Camera.GetCameraToViewportRay(0.5f, 0.5f);
            while (distance <= 250 && (block is Game.World.Blocks.AirBlock || block == null))
            {
                prevRelBlockPos = curRelBlockPos;
                curRelBlockPos = MainWorld.AbsToRelative(ray.GetPoint(distance));
                block = island.getBlock(curRelBlockPos, false);
                distance += Cst.CUBE_SIDE;
            }

            Vector3 playerHeadBlockPos = this.BlockPosition + Vector3.UNIT_Y;
            if (block is Game.World.Blocks.ConstructionBlock)
            {
                CharacterInfo iaInfo = new CharacterInfo("NPC_" + Guid.NewGuid().ToString(), false);
                iaInfo.SpawnPoint = this.mCharacMgr.World.getSpawnPoint();
                this.mCharacMgr.AddCharacter(iaInfo);

                this.mCharacMgr.GetCharacterByListPos(this.mCharacMgr.GetNumberOfCharacter() - 1).MoveTo(curRelBlockPos * Cst.CHUNK_SIDE);
                new Building(island, curRelBlockPos);
            }
            else if (!(block is Game.World.Blocks.AirBlock) && prevRelBlockPos != playerHeadBlockPos && prevRelBlockPos != this.BlockPosition)
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
                    island.addBlockToScene(prevRelBlockPos, material);
                    this.mCharacMgr.World.onCreation(prevRelBlockPos);
                    //this.mCharacMgr.StateMgr.WriteOnConsole("Face : " + Enum.GetName(typeof(CubeFace), face));
                    this.mCharacMgr.StateMgr.WriteOnConsole("Added : " + material);
                }
            }
        }
    }
}

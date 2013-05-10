using System;
using API.Generic;
using API.Geo.Cuboid;
using Mogre;

using Game.CharacSystem;
using Game.World;
using Game.World.Blocks;
using Game.GUICreator;

namespace Game.States
{
    public abstract class MainState : State
    {
        protected MainWorld mWorld;
        private SceneNode   mSelectedEntity;

        public CameraMgr CameraMgr { get; protected set; }
        public CharacMgr CharacMgr { get; protected set; }
        public MainGUI   MainGUI { get; protected set; }

        protected MainState(StateManager stateMgr, string name) : base(stateMgr, name) { this.mSelectedEntity = null; }

        protected override void Startup()
        {
            this.CameraMgr = new CameraMgr(this.mStateMgr);

            this.mWorld = new MainWorld(this.mStateMgr);
            if (!this.mStateMgr.GameInfo.Load) { this.mWorld.populate(); }
            this.mWorld.setSafeSpawnPoint();
            this.mWorld.display();

            this.AfterWorldCreation();

            this.Show();
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(" => Game loop begin");
        }

        protected virtual void AfterWorldCreation() {}

        public override void Show()
        {
            this.mStateMgr.HideGUIs();
            this.mStateMgr.MiyagiMgr.CursorVisibility = false;
            if (!this.CameraMgr.IsFreeCamMode) { this.MainGUI.Show(); }
            this.CameraMgr.IsAllowedToMoveCam = true;
        }

        public override void Update(float frameTime)
        {
            this.mWorld.Update(frameTime);

            if (this.mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE))
                this.mStateMgr.RequestStatePush(typeof(IGMenuState));

            this.CameraMgr.Update(frameTime);
            this.CharacMgr.Update(frameTime);

            /* Entity selection */
            if (this.CameraMgr.IsFreeCamMode && this.mStateMgr.Input.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Left))
            {
                if (this.mSelectedEntity == null)
                {
                    RaySceneQuery raySceneQuery = this.CharacMgr.SceneMgr.CreateRayQuery(new Ray());
                    raySceneQuery.Ray = this.mStateMgr.Camera.GetCameraToViewportRay(0.5f, 0.5f);

                    RaySceneQueryResult.Enumerator itr = (RaySceneQueryResult.Enumerator) raySceneQuery.Execute().GetEnumerator();

                    while (itr.MoveNext())
                    {
                        if (itr.Current.movable.Name.Contains("CharacterEnt_"))
                        {
                            this.mSelectedEntity = ((SceneNode)itr.Current.movable.ParentNode);
                            this.mSelectedEntity.ShowBoundingBox = true;
                        }
                    }
                }
                else
                {
                    int characId = Convert.ToInt32(this.mSelectedEntity.Name.Substring(14));
                    this.mSelectedEntity.ShowBoundingBox = false;
                    this.CharacMgr.GetCharacterById(characId).MoveTo(this.CharacMgr.World.getSpawnPoint() + Vector3.UNIT_Y);
                    this.mSelectedEntity = null;
                }
            }

            /* Cube addition and suppression */
            if (!this.mStateMgr.Input.IsShiftDown) { this.MainGUI.MoveSelector(this.mStateMgr.Input.MouseMoveZ); }

            bool leftClick = this.mStateMgr.Input.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Left);
            bool rightClick = this.mStateMgr.Input.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Right);
            bool allowWorldEdition = !(this.mStateMgr.GameInfo.IsInEditorMode ^ this.CameraMgr.IsFreeCamMode);
            if (allowWorldEdition && (leftClick || rightClick))
            {
                Vector3 relBlockPos;
                Block block;
                CubeFace face;

                if (!this.GetBlockPos(out relBlockPos, out block, out face)) { return; }
                if (leftClick && this.mWorld.onLeftClick(relBlockPos)) { this.AddBlock(relBlockPos, block, face); }
                if (rightClick && this.mWorld.onRightClick(relBlockPos)) { this.DeleteBlock(relBlockPos); }
            }
        }

        public void EnableMovement(bool enable)
        {
            this.CameraMgr.IsAllowedToMoveCam = enable;
            if(this.CharacMgr.MainPlayer != null) { this.CharacMgr.MainPlayer.MovementInfo.IsAllowedToMove = enable; }
        }

        protected override void Shutdown()
        {
            Mogre.LogManager.Singleton.DefaultLog.LogMessage(" => Game loop end");
            this.mStateMgr.SceneMgr.DestroyAllParticleSystems();
            this.MainGUI.Dispose();
            this.CharacMgr.Dispose();
            this.mWorld.Shutdown();
        }

        public abstract void Save();

        private bool GetBlockPos(out Vector3 relBlockPos, out Block block, out CubeFace face)
        {
            const float distMax = 250;
            float distance = 0;
            face = CubeFace.frontFace;

            Ray ray = this.mStateMgr.Camera.GetCameraToViewportRay(0.5f, 0.5f);
            do
            {
                distance += 40;
                relBlockPos = MainWorld.AbsToRelative(ray.GetPoint(distance));
                block = this.mWorld.getIsland().getBlock(relBlockPos, false);
            } while (block is AirBlock && distance <= distMax);

            if (distance > distMax)
            {
                relBlockPos = Vector3.ZERO;
                block = null;
                return false;
            }

            /* Compute the exact position */
            Vector3 absPosBlock = relBlockPos * Cst.CUBE_SIDE;
            int index = -1;
            float minDist = -1;
            Vector3[] points = World.Generator.VanillaMultiBlock.blockPointCoords;
            for (int i = 0; i < points.Length; i += 4)
            {
                Plane p = new Plane(points[i] + absPosBlock, points[i + 1] + absPosBlock, points[i + 2] + absPosBlock); // Need the 3 first points

                Vector3 v1 = p.normal * ray.Origin;
                Vector3 v2 = p.normal * ray.Direction;
                float dist = (-v1.x - v1.y - v1.z - p.d) / (v2.x + v2.y + v2.z);    // distance between the origin of the ray and the plan

                if (!(dist > 0)) { continue; }
                Vector3 actPoint = ray.GetPoint(dist);

                if ((!(minDist < 0) && !(dist < minDist)) || !MathHelper.isInBlock(absPosBlock, actPoint, Cst.CUBE_SIDE))
                    continue;
                minDist = dist;
                index = i / 4;
            }

            face = (CubeFace)index;

            return true;
        }

        private void DeleteBlock(Vector3 relBlockPos)
        {
            this.mWorld.onDeletion(relBlockPos);
            this.mWorld.getIsland().removeFromScene(relBlockPos);
        }

        private void AddBlock(Vector3 relBlockPos, Block block, CubeFace face)
        {
            /*Vector3 relBlockPos;
            API.Geo.Cuboid.Block b;
            CubeFace face;
            if (!this.GetBlockPos(out relBlockPos, out b, out face)) { return; }*/
            Island island = this.CharacMgr.World.getIsland();
            Block b = null;

            float distance = 10;
            Vector3 curRelBlockPos = Vector3.ZERO, prevRelBlockPos = Vector3.ZERO;
            Ray ray = this.mStateMgr.Camera.GetCameraToViewportRay(0.5f, 0.5f);
            while (distance <= 250 && (b is AirBlock || b == null))
            {
                prevRelBlockPos = curRelBlockPos;
                curRelBlockPos = MainWorld.AbsToRelative(ray.GetPoint(distance));
                b = island.getBlock(curRelBlockPos, false);
                distance += Cst.CUBE_SIDE;
            }

            Vector3 camPos = MainWorld.AbsToRelative(this.mStateMgr.Camera.Position);
            if (b is AirBlock || b is ConstructionBlock || prevRelBlockPos == camPos) { return; }
            if (this.CharacMgr.MainPlayer != null && prevRelBlockPos == this.CharacMgr.MainPlayer.BlockPosition) { return; }
            /*if      (face == CubeFace.underFace) { relBlockPos.y--; }
                else if (face == CubeFace.upperFace) { relBlockPos.y++; }
                else if (face == CubeFace.leftFace) { relBlockPos.x--; }
                else if (face == CubeFace.rightFace) { relBlockPos.x++; }
                else if (face == CubeFace.backFace) { relBlockPos.z--; }
                else  /*(face == CubeFace.frontFace)* { relBlockPos.z++; }*/

            string material = this.MainGUI.GetMaterial();
            if (material != "") { island.addBlockToScene(prevRelBlockPos, material); }
            this.mWorld.onCreation(prevRelBlockPos);
            //this.mCharacMgr.StateMgr.WriteOnConsole("Face : " + Enum.GetName(typeof(CubeFace), face));
            //this.mCharacMgr.StateMgr.WriteOnConsole("Added : " + material);
        }

    }
}

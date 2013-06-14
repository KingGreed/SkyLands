using System.Linq;
using System.Windows.Forms;
using Mogre;

using API.Generic;
using API.Geo.Cuboid;

using Game.BaseApp;
using Game.States;
using Game.World;
using Game.Input;
using Game.World.Blocks;
using Game.CharacSystem;
using Game.World.Display;
using Game.GUIs;

namespace Game
{
    public class User
    {
        private const float DIST_MIN_SELECTION = 30;
        private const float DIST_MAX_SELECTION = 250;

        private readonly StateManager  mStateMgr;
        private readonly API.Geo.World mWorld;

        private CameraMan mCameraMan;
        private SceneNode mCamYawNode, mCamPitchNode;
        private readonly SceneNode mWireCube;

        private Vector3           mSelectedBlockPos;
        private Block             mSelectedBlock;
        private readonly Selector mSelector;
        private readonly Keys[]   mFigures;

        public Selector Selector { get { return this.mSelector; } }
        public bool IsAllowedToMoveCam { get; set; }
        public bool IsFreeCamMode { get; private set; }

        public User(StateManager stateMgr, API.Geo.World world)
        {
            this.mStateMgr = stateMgr;
            this.mWorld = world;

            this.mCameraMan = null;
            this.IsAllowedToMoveCam = true;
            this.IsFreeCamMode = true;

            this.mSelector = new Selector();

            this.mFigures = new Keys[10];
            for (int i = 0; i < this.mFigures.Length - 1; i++)
                this.mFigures[i] = (Keys)(((int)Keys.D1) + i);
            this.mFigures[this.mFigures.Length - 1] = Keys.D0;

            ManualObject[] wires = new ManualObject[12];
            wires[0] = StaticRectangle.CreateLine(this.mStateMgr.SceneMgr, Vector3.ZERO, Vector3.UNIT_X * Cst.CUBE_SIDE);
            wires[1] = StaticRectangle.CreateLine(this.mStateMgr.SceneMgr, Vector3.UNIT_X * Cst.CUBE_SIDE, new Vector3(1, 0, 1) * Cst.CUBE_SIDE);
            wires[2] = StaticRectangle.CreateLine(this.mStateMgr.SceneMgr, new Vector3(1, 0, 1) * Cst.CUBE_SIDE, Vector3.UNIT_Z * Cst.CUBE_SIDE);
            wires[3] = StaticRectangle.CreateLine(this.mStateMgr.SceneMgr, Vector3.UNIT_Z * Cst.CUBE_SIDE, Vector3.ZERO);
            wires[4] = StaticRectangle.CreateLine(this.mStateMgr.SceneMgr, Vector3.UNIT_Y * Cst.CUBE_SIDE, new Vector3(1, 1, 0) * Cst.CUBE_SIDE);
            wires[5] = StaticRectangle.CreateLine(this.mStateMgr.SceneMgr, new Vector3(1, 1, 0) * Cst.CUBE_SIDE, Vector3.UNIT_SCALE * Cst.CUBE_SIDE);
            wires[6] = StaticRectangle.CreateLine(this.mStateMgr.SceneMgr, Vector3.UNIT_SCALE * Cst.CUBE_SIDE, new Vector3(0, 1, 1) * Cst.CUBE_SIDE);
            wires[7] = StaticRectangle.CreateLine(this.mStateMgr.SceneMgr, new Vector3(0, 1, 1) * Cst.CUBE_SIDE, Vector3.UNIT_Y * Cst.CUBE_SIDE);
            wires[8] = StaticRectangle.CreateLine(this.mStateMgr.SceneMgr, Vector3.ZERO, Vector3.UNIT_Y * Cst.CUBE_SIDE);
            wires[9] = StaticRectangle.CreateLine(this.mStateMgr.SceneMgr, Vector3.UNIT_X * Cst.CUBE_SIDE, new Vector3(1, 1, 0) * Cst.CUBE_SIDE);
            wires[10] = StaticRectangle.CreateLine(this.mStateMgr.SceneMgr, new Vector3(1, 0, 1) * Cst.CUBE_SIDE, Vector3.UNIT_SCALE * Cst.CUBE_SIDE);
            wires[11] = StaticRectangle.CreateLine(this.mStateMgr.SceneMgr, Vector3.UNIT_Z * Cst.CUBE_SIDE, new Vector3(0, 1, 1) * Cst.CUBE_SIDE);

            this.mWireCube = this.mStateMgr.SceneMgr.RootSceneNode.CreateChildSceneNode();
            foreach (ManualObject wire in wires)
                this.mWireCube.AttachObject(wire);

            this.mWireCube.SetVisible(false);
        }

        public void InitCamera()
        {
            VanillaPlayer mainPlayer = this.mStateMgr.MainState.CharacMgr.MainPlayer;
            if (mainPlayer != null)
                mainPlayer.Node.SetVisible(false);

            this.mCamYawNode = this.mStateMgr.MainState.CharacMgr.MainPlayer.Node.CreateChildSceneNode();
            this.mCamYawNode.SetPosition(0, 3.6f, 0); // Camera is set at eyes level
            this.mCamYawNode.Yaw(new Degree(180));

            this.mCamPitchNode = this.mCamYawNode.CreateChildSceneNode();
            this.mCamPitchNode.AttachObject(this.mStateMgr.Camera);

            this.mStateMgr.Camera.Position = Vector3.ZERO;
            this.mStateMgr.Camera.Orientation = Quaternion.IDENTITY;
        }

        public void Update(float frameTime)
        {
            MainState mainState = this.mStateMgr.MainState;
            VanillaPlayer mainPlayer = mainState.CharacMgr.MainPlayer;
            if (this.IsFreeCamMode && mainPlayer != null)
            {
                bool ctrlPressed = this.mStateMgr.Controller.IsKeyDown(Keys.ControlKey);
                mainPlayer.SetIsAllowedToMove(ctrlPressed, false);
                this.IsAllowedToMoveCam = !ctrlPressed;
            }
            
            /* Move camera */
            if (this.IsAllowedToMoveCam)
            {
                if (this.IsFreeCamMode)
                {
                    this.mCameraMan.MouseMovement(this.mStateMgr.Controller.Yaw, this.mStateMgr.Controller.Pitch);
                    this.mCameraMan.UpdateCamera(frameTime, this.mStateMgr.Controller);
                }
                else if (mainPlayer.MovementInfo.IsAllowedToMove) // Just pitch the camera
                {
                    this.mCamPitchNode.Pitch(new Degree(this.mStateMgr.Controller.Pitch));

                    if (2 * new Degree(Math.ACos(this.mCamPitchNode.Orientation.w)).ValueAngleUnits > 90.0f) // Limit the pitch between -90 degrees and +90 degrees
                        this.mCamPitchNode.SetOrientation(Math.Sqrt(0.5f), Math.Sqrt(0.5f) * Math.Sign(this.mCamPitchNode.Orientation.x), 0, 0);
                }

                /* Cube addition and suppression */
                float dist = this.UpdateSelectedBlock();
                if (!(this.mStateMgr.GameInfo.IsInEditorMode ^ mainState.User.IsFreeCamMode))    // Allow world edition
                {
                    if (this.mStateMgr.Controller.HasActionOccured(UserAction.MainAction) && !this.mSelector.IsBullet && this.mWorld.onLeftClick(this.mSelectedBlockPos))
                        this.AddBlock(dist);
                    if (this.mStateMgr.Controller.HasActionOccured(UserAction.SecondaryAction) && this.mWorld.onRightClick(this.mSelectedBlockPos))
                        this.DeleteBlock();
                }
            }

            /* Move Selector */
            int selectorPos = this.mSelector.SelectorPos;
            if (this.mStateMgr.Controller.HasActionOccured(UserAction.MoveSelectorLeft)) { selectorPos--; }
            if (this.mStateMgr.Controller.HasActionOccured(UserAction.MoveSelectorRight)) { selectorPos++; }
            for (int i = 0; i < this.mFigures.Length; i++)
            {
                if (this.mStateMgr.Controller.WasKeyPressed(this.mFigures[i]))
                {
                    selectorPos = i;
                    break;
                }
            }
            this.mSelector.SelectorPos = selectorPos;
        }

        public void SwitchFreeCamMode()
        {
            VanillaPlayer mainPlayer = this.mStateMgr.MainState.CharacMgr.MainPlayer;
            if (mainPlayer == null) { return; }

            this.IsFreeCamMode = !this.IsFreeCamMode;

            mainPlayer.SwitchFreeCamMode();

            if (this.IsFreeCamMode)
            {
                Camera cam = this.mStateMgr.Camera;
                Vector3 position = cam.RealPosition;
                Quaternion orientation = cam.RealOrientation;
                cam.DetachFromParent();
                cam.Position = position;
                cam.Orientation = orientation;

                this.mCameraMan = new CameraMan(cam);
                mainPlayer.SetIsAllowedToMove(false);
            }
            else
            {
                this.InitCamera();
                mainPlayer.SetIsAllowedToMove(true);
                this.mCameraMan = null;
            }
        }

        private float UpdateSelectedBlock()   // return the distance with the selected block
        {
            Vector3 relBlockPos;
            Block actBlock;
            float distance = VanillaBlock.getBlockOnRay(this.mWorld.getIsland(), this.mStateMgr.Camera.GetCameraToViewportRay(0.5f, 0.5f), DIST_MAX_SELECTION,
                                                        DIST_MIN_SELECTION, out relBlockPos, out actBlock);

            if (distance > DIST_MAX_SELECTION)
            {
                this.mSelectedBlockPos = Vector3.ZERO;
                this.mSelectedBlock = null;
                this.mWireCube.SetVisible(false);
                return 0;
            }

            this.mSelectedBlockPos = relBlockPos;
            this.mSelectedBlock = actBlock;
            this.mWireCube.SetVisible(true);
            this.mWireCube.Position = (this.mSelectedBlockPos + Vector3.NEGATIVE_UNIT_Z) * Cst.CUBE_SIDE;

            return distance;
        }

        private float Vector3Sum(Vector3 v) { return v.x + v.y + v.z; }

        private void DeleteBlock()
        {
            this.mWorld.onDeletion(this.mSelectedBlockPos);
            this.mWorld.getIsland().removeFromScene(this.mSelectedBlockPos);
        }

        private void AddBlock(float dist)
        {
            string material = this.mSelector.Material;
            if (this.mSelectedBlock is Air || this.mSelectedBlock is ConstructionBlock || material == "") { return; }
            
            /* Determine the face */
            Ray ray = this.mStateMgr.Camera.GetCameraToViewportRay(0.5f, 0.5f);
            Vector3 realIntersection = ray.GetPoint(dist);
            Vector3 absPosBlock = this.mSelectedBlockPos * Cst.CUBE_SIDE;

            int index = 0;
            float minDist = -1;
            Vector3[] points = World.Generator.VanillaMultiBlock.blockPointCoords;
            for (int i = 0; i < points.Length; i += 4)
            {
                Plane p = new Plane(points[i] + absPosBlock, points[i + 1] + absPosBlock, points[i + 2] + absPosBlock);
                Vector3 intersection = ray.GetPoint(- (p.d + Vector3Sum(p.normal * ray.Origin)) / Vector3Sum(p.normal * ray.Direction));

                float actDist = (realIntersection - intersection).SquaredLength;    // No need to take the real distance

                if (minDist < 0 || actDist < minDist)
                {
                    minDist = actDist;
                    index = i / 4;
                }
            }

            BlockFace face = (BlockFace)index;
            Vector3 addedBlockPos = this.mSelectedBlockPos;

            switch (face)
            {
                case BlockFace.underFace:
                    addedBlockPos.y--;
                    break;
                case BlockFace.upperFace:
                    addedBlockPos.y++;
                    break;
                case BlockFace.leftFace:
                    addedBlockPos.x--;
                    break;
                case BlockFace.rightFace:
                    addedBlockPos.x++;
                    break;
                case BlockFace.backFace:
                    addedBlockPos.z--;
                    break;
                default:    // frontFace
                    addedBlockPos.z++;
                    break;
            }

            if (addedBlockPos == MainWorld.AbsToRelative(this.mStateMgr.Camera.Position) || !(this.mWorld.getIsland().getBlock(addedBlockPos, false) is Air) ||
               (this.mStateMgr.MainState.CharacMgr.MainPlayer != null &&
                this.mStateMgr.MainState.CharacMgr.MainPlayer.CollisionMgr.GetHitPoints().Any(v => MathHelper.isInBlock(addedBlockPos * Cst.CUBE_SIDE, v, Cst.CUBE_SIDE))))
                { return; }

            this.mWorld.getIsland().addBlockToScene(addedBlockPos, material);
            this.mWorld.onCreation(addedBlockPos);
        }
    }
}

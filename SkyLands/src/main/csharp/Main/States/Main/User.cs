using System.Linq;
using System.Windows.Forms;
using Mogre;

using API.Generic;
using API.Geo.Cuboid;

using Game.BaseApp;
using Game.States;
using Game.World;
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
        
        private CameraMan              mCameraMan;
        private readonly StateManager  mStateMgr;
        private readonly API.Geo.World mWorld;
        private readonly SceneNode     mWireCube;
        private Vector3                mSelectedBlockPos;
        private Block                  mSelectedBlock;
        private Selector               mSelector;
        private Keys[]                 mFigures;

        public Selector Selector { get { return this.mSelector; } }
        public bool IsAllowedToMoveCam { get; set; }
        public bool IsFreeCamMode { get; private set; }

        public User(StateManager stateMgr, API.Geo.World world)
        {
            this.mStateMgr = stateMgr;
            this.mWorld = world;
            this.mCameraMan = null;
            this.IsAllowedToMoveCam = true;
            this.mSelector = new Selector(this.mStateMgr.WebView);

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

        public void Update(float frameTime)
        {
            /* Move camera */
            VanillaPlayer mainPlayer = this.mStateMgr.MainState.CharacMgr.MainPlayer;
            if (this.IsFreeCamMode && mainPlayer != null)
                this.IsAllowedToMoveCam = !mainPlayer.MovementInfo.IsAllowedToMove;

            if (this.mCameraMan != null && this.IsAllowedToMoveCam)
            {
                this.mCameraMan.MouseMovement(this.mStateMgr.Controller.Yaw, this.mStateMgr.Controller.Pitch);
                this.mCameraMan.UpdateCamera(frameTime, this.mStateMgr.Controller);
            }

            float dist = this.UpdateSelectedBlock();

            /* Cube addition and suppression */
            int selectorPos = this.mSelector.SelectorPos;
            if (this.mStateMgr.Controller.HasActionOccured(Controller.UserAction.MoveSelectorLeft)) { selectorPos--; }
            if (this.mStateMgr.Controller.HasActionOccured(Controller.UserAction.MoveSelectorRight)) { selectorPos++; }
            for (int i = 0; i < this.mFigures.Length; i++)
            {
                if (this.mStateMgr.Controller.WasKeyPressed(this.mFigures[i]))
                {
                    selectorPos = i;
                    break;
                }
            }
            this.mSelector.SelectorPos = selectorPos;

            bool leftClick = this.mStateMgr.Controller.HasActionOccured(Controller.UserAction.MainAction);
            bool rightClick = this.mStateMgr.Controller.HasActionOccured(Controller.UserAction.SecondaryAction);
            bool allowWorldEdition = !(this.mStateMgr.GameInfo.IsInEditorMode ^ this.mStateMgr.MainState.User.IsFreeCamMode);
            if (allowWorldEdition && (leftClick || rightClick))
            {
                if (leftClick && this.mWorld.onLeftClick(this.mSelectedBlockPos) && !this.mSelector.IsBullet) { this.AddBlock(dist); }
                if (rightClick && this.mWorld.onRightClick(this.mSelectedBlockPos)) { this.DeleteBlock(); }
            }
        }

        public void SwitchFreeCamMode()
        {
            this.IsFreeCamMode = !this.IsFreeCamMode;

            MainState mainState = this.mStateMgr.MainState;
            VanillaPlayer mainPlayer = mainState.CharacMgr.MainPlayer;

            if (mainPlayer != null)
            {
                //mainState.MainGUI.SwitchVisibility();
                mainPlayer.SwitchFreeCamMode();
            }

            if (this.IsFreeCamMode)
            {
                Camera cam = this.mStateMgr.Camera;
                Vector3 position = cam.RealPosition;
                Quaternion orientation = cam.RealOrientation;
                cam.DetachFromParent();
                cam.Position = position;
                cam.Orientation = orientation;

                this.mCameraMan = new CameraMan(cam);
            }
            else
            {
                if (mainPlayer != null)
                {
                    mainPlayer.MainPlayerCam.InitCamera();
                    mainPlayer.MovementInfo.IsAllowedToMove = true;
                }
                this.mCameraMan = null;
            }
        }

        private float UpdateSelectedBlock()   // return the distance with the selected block
        {
            float distance = DIST_MIN_SELECTION;
            Ray ray = this.mStateMgr.Camera.GetCameraToViewportRay(0.5f, 0.5f);
            Vector3 actRelBlockPos = MainWorld.AbsToRelative(ray.GetPoint(distance));
            Vector3 prevRelBlockPos = actRelBlockPos;
            Block actBlock;

            do
            {
                while (prevRelBlockPos == actRelBlockPos)
                {
                    distance += 3;
                    actRelBlockPos = MainWorld.AbsToRelative(ray.GetPoint(distance));
                }
                prevRelBlockPos = actRelBlockPos;
                actBlock = this.mWorld.getIsland().getBlock(actRelBlockPos, false);
            } while (actBlock is AirBlock && distance <= DIST_MAX_SELECTION);

            if (distance > DIST_MAX_SELECTION)
            {
                this.mSelectedBlockPos = Vector3.ZERO;
                this.mSelectedBlock = null;
                this.mWireCube.SetVisible(false);
                return 0;
            }

            this.mSelectedBlockPos = actRelBlockPos;
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
            if (this.mSelectedBlock is AirBlock || this.mSelectedBlock is ConstructionBlock || material == "") { return; }
            
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

            if (addedBlockPos == MainWorld.AbsToRelative(this.mStateMgr.Camera.Position) || !(this.mWorld.getIsland().getBlock(addedBlockPos, false) is AirBlock) ||
               (this.mStateMgr.MainState.CharacMgr.MainPlayer != null &&
                this.mStateMgr.MainState.CharacMgr.MainPlayer.CollisionMgr.GetHitPoints().Any(v => MathHelper.isInBlock(addedBlockPos * Cst.CUBE_SIDE, v, Cst.CUBE_SIDE))))
                { return; }

            this.mWorld.getIsland().addBlockToScene(addedBlockPos, material);
            this.mWorld.onCreation(addedBlockPos);
        }
    }
}

using System.Linq;
using Game.RTS;
using Game.csharp.Main.RTS.Buildings;
using Mogre;

using API.Generic;
using API.Geo.Cuboid;

using Game.BaseApp;
using Game.States;
using Game.Input;
using Game.World;
using Game.World.Blocks;
using Game.World.Generator;
using Game.World.Display;
using Game.CharacSystem;
using Game.GUIs;

namespace Game
{
    public class User
    {
        private const float DIST_MIN_SELECTION = 0;
        private const float DIST_MAX_SELECTION = 250;

        private readonly StateManager  mStateMgr;
        private readonly API.Geo.World mWorld;

        private CameraMan mCameraMan;
        private SceneNode mCamYawNode, mCamPitchNode;
        private readonly SceneNode mWireCube;
        private bool mIsInventoryOpen, mIsBuilderOpen, mIsMainGUIOpen;

        private readonly MOIS.KeyCode[] mFigures;
        private Timer mTimeSinceGUIOpen;

        public static bool OpenBuilder         { get; set; }
        public static bool RequestBuilderClose { get; set; }
        public bool IsAllowedToMoveCam     { get; set; }
        public bool IsFreeCamMode          { get; private set; }
        public Inventory Inventory         { get; private set; }
        public BuildingManager BuildingMgr { get; set; }
        public Vector3 SelectedBlockPos    { get; private set; }
        public Block SelectedBlock         { get; private set; }

        public User(StateManager stateMgr, API.Geo.World world)
        {
            this.mStateMgr = stateMgr;
            this.mWorld = world;
            this.mTimeSinceGUIOpen = new Timer();

            this.mCameraMan = null;
            this.IsAllowedToMoveCam = true;
            this.IsFreeCamMode = true;
            Camera cam = this.mStateMgr.Camera;
            cam.Position = new Vector3(-203, 633, -183);
            cam.Orientation = new Quaternion(0.3977548f, -0.1096644f, -0.8781486f, -0.2421133f);
            this.mCameraMan = new CameraMan(cam);

            this.mFigures = new MOIS.KeyCode[10];
            for (int i = 0; i < 9; i++)
                this.mFigures[i] = (MOIS.KeyCode)System.Enum.Parse(typeof(MOIS.KeyCode), "KC_" + (i + 1));
            this.mFigures[9] = MOIS.KeyCode.KC_0;

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

            this.Inventory = new Inventory(10, 4, new int[] { 3, 0, 1, 2 }, true);
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

        public void SwitchGUIVisibility(bool visible)
        {
            this.mStateMgr.Controller.SwitchCursorVisibility();
            if(this.mStateMgr.MainState.CharacMgr.MainPlayer != null)
                this.mStateMgr.MainState.CharacMgr.MainPlayer.SetIsAllowedToMove(!visible);
            this.IsAllowedToMoveCam = !visible;
            this.mStateMgr.Controller.BlockMouse = !visible;
        }

        public void Update(float frameTime)
        {
            MainState mainState = this.mStateMgr.MainState;
            VanillaPlayer mainPlayer = mainState.CharacMgr.MainPlayer;

            if (this.mIsMainGUIOpen && this.mTimeSinceGUIOpen.Milliseconds >= 3500 && this.mStateMgr.GameInfo.IsInEditorMode)
            {
                this.mIsMainGUIOpen = false;
                GUI.Visible = false;
            }

            if (this.mStateMgr.Controller.HasActionOccured(UserAction.Start))
            {
                if (!GUI.Visible)
                {
                    this.mStateMgr.MainState.OpenMainGUI();
                    this.SwitchGUIVisibility(true);
                    this.mIsMainGUIOpen = true;
                    this.mTimeSinceGUIOpen.Reset();
                }
                else
                {
                    if (this.mIsInventoryOpen)
                    {
                        this.SwitchInventory(false);
                        this.mIsInventoryOpen = false;
                    }
                    else if (this.mIsBuilderOpen)
                    {
                        this.mIsBuilderOpen = false;
                        if (this.BuildingMgr.HasActualBuilding())
                            this.BuildingMgr.GetActualBuilding().WaitForRessources();
                    }
                    else
                    {
                        this.mIsMainGUIOpen = false;
                        /*if (this.mStateMgr.GameInfo.IsInEditorMode)
                            (this.mStateMgr.MainState as StoryEditorState).OnExit();*/
                    }

                    this.SwitchGUIVisibility(false);
                    GUI.Visible = false;
                }
            }

            if (RequestBuilderClose && this.mIsBuilderOpen)
            {
                RequestBuilderClose = false;
                this.mIsBuilderOpen = false;
                this.SwitchGUIVisibility(false);
                GUI.Visible = false;
            }

            if (!this.mStateMgr.Controller.IsActionOccuring(UserAction.MainAction))
            {
                if (this.mStateMgr.Controller.HasActionOccured(UserAction.Inventory) && (!GUI.Visible || this.mIsInventoryOpen))
                {
                    this.mIsInventoryOpen = !this.mIsInventoryOpen;
                    this.SwitchInventory(this.mIsInventoryOpen);

                    this.SwitchGUIVisibility(this.mIsInventoryOpen);
                }
            }

            if (OpenBuilder)
            {
                Builder.OnOpen = this.OnBuilderOpen;
                new Builder(this.BuildingMgr);
                OpenBuilder = false;
                this.mIsBuilderOpen = true;
                this.SwitchGUIVisibility(true);
            }

            /* Move camera */
            if (this.IsAllowedToMoveCam)
            {
                if (this.IsFreeCamMode)
                    this.mCameraMan.UpdateCamera(frameTime, this.mStateMgr.Controller);
                else if (mainPlayer.MovementInfo.IsAllowedToMove) // Just pitch the camera
                {
                    this.mCamPitchNode.Pitch(new Degree(this.mStateMgr.Controller.Pitch));

                    if (2 * new Degree(Math.ACos(this.mCamPitchNode.Orientation.w)).ValueAngleUnits > 90.0f) // Limit the pitch between -90 degrees and +90 degrees
                        this.mCamPitchNode.SetOrientation(Math.Sqrt(0.5f), Math.Sqrt(0.5f) * Math.Sign(this.mCamPitchNode.Orientation.x), 0, 0);
                }

                /* Cube addition and suppression */
                float dist = this.UpdateSelectedBlock();
                if (!(this.mStateMgr.GameInfo.IsInEditorMode ^ mainState.User.IsFreeCamMode) && this.SelectedBlock != null)    // Allow world edition
                {
                    if (this.mStateMgr.Controller.HasActionOccured(UserAction.MainAction) && !Selector.IsBullet && this.mWorld.onLeftClick(this.SelectedBlockPos))
                        this.AddBlock(dist);
                    if (this.mStateMgr.Controller.HasActionOccured(UserAction.SecondaryAction) && this.mWorld.onRightClick(this.SelectedBlockPos))
                        this.DeleteBlock();
                }
            }

            /* Move Selector */
            int selectorPos = Selector.SelectorPos;
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
            if(Selector.SelectorPos != selectorPos)
                Selector.SelectorPos = selectorPos;
        }

        private void SwitchInventory(bool open)
        {
            if (open)
                new InventoryGUI(this.OnInventoryOpen, this.Inventory.OnCraft);
            else {
                this.Inventory.OnClose();
                GUI.Visible = false;
            }
        }

        public void OnBuilderOpen()
        {
            System.Console.WriteLine("OnBuilderOpen : ");
            this.OnInventoryOpen();
            if (this.BuildingMgr.HasActualBuilding() && !this.BuildingMgr.GetActualBuilding().Placed)
                this.BuildingMgr.GetActualBuilding().DrawRemainingRessource();
        }

        public void OnOpen(Inventory inventory, int initIndex)
        {
            for (int y = 0; y < inventory.Y; y++)
            {
                for (int x = 0; x < inventory.X; x++)
                {
                    Slot s = inventory.getSlot(x, y);
                    if (s == null) { continue; }
                    string texture = Inventory.MagicCubes.ContainsKey(s.item) ? Inventory.MagicCubes[s.item] :
                                     VanillaChunk.staticBlock[VanillaChunk.byteToString[s.item]].getItemTexture();
                    GUI.SetBlockAt(initIndex + x + y * inventory.X, texture, s.amount);
                }
            }
        }

        public void OnInventoryOpen() { this.OnOpen(this.Inventory, 0); }

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
                this.SelectedBlockPos = Vector3.ZERO;
                this.SelectedBlock = null;
                this.mWireCube.SetVisible(false);
                return 0;
            }

            this.SelectedBlockPos = relBlockPos;
            this.SelectedBlock = actBlock;
            this.mWireCube.SetVisible(true);
            this.mWireCube.Position = (this.SelectedBlockPos + Vector3.NEGATIVE_UNIT_Z) * Cst.CUBE_SIDE;

            if (!this.mStateMgr.GameInfo.IsInEditorMode)
            {
                ConstructionBlock constr = this.SelectedBlock as ConstructionBlock;
                if (constr != null) { this.BuildingMgr.ActConsBlockPos = this.SelectedBlockPos; }
                else if (!this.mIsBuilderOpen && this.BuildingMgr.ActConsBlockPos != -Vector3.UNIT_SCALE)
                    this.BuildingMgr.ActConsBlockPos = -Vector3.UNIT_SCALE;
            }

            return distance;
        }

        private float Vector3Sum(Vector3 v) { return v.x + v.y + v.z; }

        private void DeleteBlock()
        {
            this.mWorld.getIsland().removeFromScene(this.SelectedBlockPos);
            this.mWorld.onDeletion(this.SelectedBlockPos);

            this.Inventory.Add(this.SelectedBlock.getId());
        }

        private void AddBlock(float dist)
        {
            string name = VanillaChunk.byteToString[Selector.SelectedId];
            if (name == "") { return; }
            
            /* Determine the face */
            Ray ray = this.mStateMgr.Camera.GetCameraToViewportRay(0.5f, 0.5f);
            Vector3 realIntersection = ray.GetPoint(dist);
            Vector3 absPosBlock = this.SelectedBlockPos * Cst.CUBE_SIDE;

            int index = 0;
            float minDist = -1;
            Vector3[] points = VanillaMultiBlock.blockPointCoords;
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
            Vector3 addedBlockPos = this.SelectedBlockPos;

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
               (this.mStateMgr.MainState.CharacMgr.MainPlayer != null && this.mStateMgr.MainState.CharacMgr.MainPlayer.CollisionMgr.GetHitPoints().Any
               (v => MathHelper.isInBlock(addedBlockPos * Cst.CUBE_SIDE, v, Cst.CUBE_SIDE)))) { return; }

            this.mWorld.getIsland().addBlockToScene(addedBlockPos, name);
            this.mWorld.onCreation(addedBlockPos);

            this.Inventory.removeAt(Selector.SelectorPos, 3, 1);
        }
    }
}

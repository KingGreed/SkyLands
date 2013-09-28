using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
using Math = Mogre.Math;
using Timer = Mogre.Timer;

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
        private bool mIsInventoryOpen, mIsBuilderOpen, mIsMainGUIOpen, mAreAllCharacSelected;
        private HashSet<VanillaNonPlayer> mSelectedAllies;
        private Random mRandom;

        private readonly MOIS.KeyCode[] mFigures;
        private readonly Timer mTimeSinceGUIOpen;

        public static bool OpenBuilder         { get; set; }
        public static bool RequestBuilderClose { get; set; }
        public bool IsAllowedToMoveCam     { get; set; }
        public bool IsFreeCamMode          { get; private set; }
        public Inventory Inventory         { get; private set; }
        public BuildingManager BuildingMgr { get; set; }
        public Vector3 SelectedBlockPos    { get; private set; }
        public Block SelectedBlock         { get; private set; }
        public PlayerRTS PlayerRTS         { get; set; }

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
            this.mSelectedAllies = new HashSet<VanillaNonPlayer>();
            this.mRandom = new Random();

            this.mFigures = new MOIS.KeyCode[10];
            for (int i = 0; i < 9; i++)
                this.mFigures[i] = (MOIS.KeyCode)System.Enum.Parse(typeof(MOIS.KeyCode), "KC_" + (i + 1));
            this.mFigures[9] = MOIS.KeyCode.KC_0;

            this.mWireCube = this.mStateMgr.SceneMgr.RootSceneNode.CreateChildSceneNode();
            this.mWireCube.AttachObject(StaticRectangle.CreateRectangle(this.mStateMgr.SceneMgr, Vector3.UNIT_SCALE * Cst.CUBE_SIDE));
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
                        if (this.BuildingMgr.HasActualBuilding() && !this.BuildingMgr.GetActualBuilding().Built)
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

            if (this.mStateMgr.Controller.HasActionOccured(UserAction.CreateUnit))
                this.PlayerRTS.CreateRobot(1);

            if (this.mStateMgr.Controller.HasActionOccured(UserAction.SelectAll))
            {
                this.mAreAllCharacSelected = !this.mAreAllCharacSelected;
                foreach (VanillaNonPlayer charac in this.mStateMgr.MainState.CharacMgr.GetFactionCharacters(Faction.Blue).OfType<VanillaNonPlayer>())
                {
                    if (!charac.IsFriendlySelected() && this.mAreAllCharacSelected)
                        this.mSelectedAllies.Add(charac);
                    else if (charac.IsFriendlySelected() && !this.mAreAllCharacSelected)
                        this.mSelectedAllies.Remove(charac);

                    charac.SetSelected(this.mAreAllCharacSelected);
                }
            }

            if (this.mStateMgr.Controller.HasActionOccured(UserAction.GoTo))
            {
                int nbAllies = this.mSelectedAllies.Count;
                if (nbAllies <= 0) { return; }
                VanillaNonPlayer npc = this.GetSelectedNPC();
                if (npc != null && npc.Info.Faction == Faction.Red)
                    foreach (VanillaNonPlayer ally in this.mSelectedAllies)
                        ally.SetTargetAndFollow(npc);
                else if (this.SelectedBlockPos != Vector3.ZERO)
                {
                    int randomMax = (int) ((nbAllies / 4f + 0.5f) * Cst.CUBE_SIDE);
                    Vector3 imprecision = nbAllies == 1 ? Vector3.ZERO : new Vector3(this.mRandom.Next(0, randomMax), 0, this.mRandom.Next(0, randomMax));
                    foreach (VanillaNonPlayer ally in this.mSelectedAllies)
                        ally.MoveTo((this.SelectedBlockPos + new Vector3(0.5f, 1, -0.5f)) * Cst.CUBE_SIDE + imprecision);
                }
            }

            if (this.mStateMgr.Controller.HasActionOccured(UserAction.FollowMe))
            {
                foreach (VanillaNonPlayer ally in this.mSelectedAllies)
                {
                    if(ally.IsFollowingCharac(mainPlayer))
                        ally.StopFollowing();
                    else
                        ally.Follow(mainPlayer, Cst.CUBE_SIDE*2 + this.mRandom.Next(0, 100));
                }
            }

            /* Move camera */
            if (this.IsAllowedToMoveCam)
            {
                bool secondaryActionHandled = false;
                if (this.IsFreeCamMode)
                    this.mCameraMan.UpdateCamera(frameTime, this.mStateMgr.Controller);
                else if (mainPlayer.MovementInfo.IsAllowedToMove) // Just pitch the camera
                {
                    this.mCamPitchNode.Pitch(new Degree(this.mStateMgr.Controller.Pitch));

                    if (2 * new Degree(Math.ACos(this.mCamPitchNode.Orientation.w)).ValueAngleUnits > 90.0f) // Limit the pitch between -90 degrees and +90 degrees
                        this.mCamPitchNode.SetOrientation(Math.Sqrt(0.5f), Math.Sqrt(0.5f) * Math.Sign(this.mCamPitchNode.Orientation.x), 0, 0);

                    if (this.mStateMgr.Controller.HasActionOccured(UserAction.SecondaryAction))
                        secondaryActionHandled = this.UpdateSelectedNPC();
                }

                /* Cube addition and suppression */
                float dist = this.UpdateSelectedBlock();
                if (!(this.mStateMgr.GameInfo.IsInEditorMode ^ mainState.User.IsFreeCamMode) && this.SelectedBlock != null)    // Allow world edition
                {
                    if (this.mStateMgr.Controller.HasActionOccured(UserAction.MainAction) && !Selector.IsBullet && this.mWorld.onLeftClick(this.SelectedBlockPos))
                        this.AddBlock(dist);
                    if (!secondaryActionHandled && this.mStateMgr.Controller.HasActionOccured(UserAction.SecondaryAction) && this.mWorld.onRightClick(this.SelectedBlockPos))
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
            if (Selector.SelectorPos != selectorPos)
                Selector.SelectorPos = selectorPos;
        }

        public void RemoveSelectedAlly(VanillaNonPlayer charac)
        {
            this.mSelectedAllies.Remove(charac);
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
            this.OnInventoryOpen();
            if (this.BuildingMgr.HasActualBuilding() && !this.BuildingMgr.GetActualBuilding().Built)
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

        /*public void OpenDeathMenu()
        {
            new DeathMenu(this.mStateMgr);
            this.SwitchGUIVisibility(true);
            this.mTimeSinceGUIOpen.Reset();
        }*/

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

        private bool UpdateSelectedNPC() // Return if the secondaryAction was meant to select a NPC
        {
            VanillaNonPlayer selectedCharac = this.GetSelectedNPC();
            if (selectedCharac != null && selectedCharac.Info.Faction == Faction.Blue)
            {
                Console.WriteLine("Ally selected");
                if (this.mSelectedAllies.Contains(selectedCharac))
                {
                    selectedCharac.SetSelected(false);
                    this.mSelectedAllies.Remove(selectedCharac);
                }
                else
                {
                    selectedCharac.SetSelected(true);
                    this.mSelectedAllies.Add(selectedCharac);
                }
            }

            return selectedCharac != null;
        }

        private VanillaNonPlayer GetSelectedNPC()
        {
            RaySceneQuery raySceneQuery = this.mStateMgr.SceneMgr.CreateRayQuery(this.mStateMgr.Camera.GetCameraToViewportRay(0.5f, 0.5f));
            raySceneQuery.SetSortByDistance(true);

            foreach (RaySceneQueryResultEntry raySQREntry in raySceneQuery.Execute())
            {
                if (raySQREntry.movable != null)
                {
                    string name = raySQREntry.movable.Name;
                    if (!name.Contains("CharacterEnt_")) { continue; }
                    int id = int.Parse(name.Split('_')[1]);
                    VanillaNonPlayer npc = this.mStateMgr.MainState.CharacMgr.GetCharacterById(id) as VanillaNonPlayer;
                    if(npc != null)
                        return npc;
                }
            }
            return null;
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
            this.mWorld.getIsland().removeBlock(this.SelectedBlockPos);
            this.mWorld.onDeletion(this.SelectedBlockPos);

            if (!this.mStateMgr.GameInfo.IsInEditorMode)
                this.Inventory.Add(this.SelectedBlock.getId());
        }

        private void AddBlock(float dist)
        {
            string name = VanillaChunk.byteToString[Selector.SelectedId];
            if (name == "") { return; }

            if (this.GetSelectedNPC() != null) { return; }
            
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

            this.mWorld.getIsland().setBlockAt(addedBlockPos, name, true);
            this.mWorld.onCreation(addedBlockPos);

            if (!this.mStateMgr.GameInfo.IsInEditorMode)
                this.Inventory.removeAt(Selector.SelectorPos, 3, 1);
        }
    }
}

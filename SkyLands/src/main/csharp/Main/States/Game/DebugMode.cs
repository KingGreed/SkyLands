using System;
using Mogre;

using Game.BaseApp;
using Game.CharacSystem;
using Game.GUICreator;

namespace Game
{
    public class DebugMode
    {
        private bool        mIsDebugMode, mIsConsoleMode, mIsAllowedToMoveCam;
        private CameraMan   mCameraMan;
        private MoisManager mInput;
        private CharacMgr   mCharacMgr;
        private GameGUI     mGUI;

        public bool IsDebugMode   { get { return this.mIsDebugMode; }   set { this.mIsDebugMode = value; } }
        public bool IsConsoleMode { get { return this.mIsConsoleMode; } set { this.mIsConsoleMode = value; } }
        public bool IsAllowedToMoveCam { get { return this.mIsAllowedToMoveCam; } set { this.mIsAllowedToMoveCam = value; } }

        private SceneNode selected = null;

        public DebugMode(MoisManager input, CharacMgr characMgr, GameGUI gui)
        {
            this.mInput = input;
            this.mCharacMgr = characMgr;
            this.mIsDebugMode = false;
            this.mCameraMan = null;
            this.mIsConsoleMode = false;
            this.mIsAllowedToMoveCam = true;
            this.mGUI = gui;
        }

        public void Update(float frameTime, bool enablePlayerMove = true)
        {
            if(this.mIsDebugMode && this.mInput.WasMouseButtonPressed(MOIS.MouseButtonID.MB_Left)) {
                if(selected == null) {
                    Ray ray = this.mCharacMgr.MainPlayerCam.Camera.GetCameraToViewportRay(0.5f, 0.5f);
                    RaySceneQuery raySceneQuery = this.mCharacMgr.SceneMgr.CreateRayQuery(new Ray());
                    raySceneQuery.Ray = ray;

                    RaySceneQueryResult result = raySceneQuery.Execute();
                    RaySceneQueryResult.Enumerator itr = (RaySceneQueryResult.Enumerator)(result.GetEnumerator());

                    while(itr != null && itr.MoveNext()) {
                        if(itr.Current.movable.Name.Contains("CharacterEnt_")) {
                            selected = ((SceneNode)itr.Current.movable.ParentNode);
                            selected.ShowBoundingBox = true;
                        }
                    }
                } else {
                    int characId = Convert.ToInt32(selected.Name.Substring(14));
                    selected.ShowBoundingBox = false;
                    this.mCharacMgr.GetCharacterById(characId).moveTo(this.mCharacMgr.World.getSpawnPoint() + Vector3.UNIT_Y);
                    selected = null;
                    
                }
            }

            if (this.mInput.WasKeyPressed(MOIS.KeyCode.KC_F1))
            {
                this.mIsDebugMode = !this.mIsDebugMode;
                this.mGUI.SwitchGUIForDebugCam();

                ((VanillaPlayer)this.mCharacMgr.GetCharacterByListPos()).IsDebugMode = this.mIsDebugMode;

                if (this.mIsDebugMode)
                {
                    Camera cam = this.mCharacMgr.MainPlayerCam.Camera;
                    Vector3 position = cam.RealPosition;
                    Quaternion orientation = cam.RealOrientation;
                    cam.DetachFromParent();
                    cam.Position = position;
                    cam.Orientation = orientation;

                    this.mCameraMan = new CameraMan(cam);
                }
                else
                    this.mCharacMgr.MainPlayerCam.InitCamera();
            }


            this.mCharacMgr.GetCharacterByListPos().IsAllowedToMove = !this.mIsConsoleMode && this.mIsAllowedToMoveCam;
            this.mCharacMgr.Update(frameTime);

            if (!this.mCharacMgr.GetCharacterByListPos().IsAllowedToMove && !this.mIsConsoleMode && this.mIsAllowedToMoveCam)
            {
                this.mCameraMan.MouseMovement(this.mInput.MouseMoveX, this.mInput.MouseMoveY);
                this.mCameraMan.UpdateCamera(frameTime, this.mInput);
            }
        }

        public void Dispose() { this.mCharacMgr.Dispose(); }
    }
}

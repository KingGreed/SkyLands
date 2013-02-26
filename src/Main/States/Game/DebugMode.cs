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
            if (this.mInput.WasKeyPressed(MOIS.KeyCode.KC_F1))
            {
                this.mIsDebugMode = !this.mIsDebugMode;
                this.mGUI.SwitchGUIForDebugCam();

                ((VanillaPlayer)this.mCharacMgr.GetCharacter()).IsDebugMode = this.mIsDebugMode;

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

            if(this.mInput.WasKeyPressed(MOIS.KeyCode.KC_F3)) {

                Vector3 spawn = this.mCharacMgr.GetCharacter().FeetPosition;
                spawn /= World.MainWorld.CUBE_SIDE;
                spawn.y -= 1;
                spawn.x = Mogre.Math.IFloor(spawn.x);
                spawn.y = Mogre.Math.IFloor(spawn.y);
                spawn.z = Mogre.Math.IFloor(spawn.z);


                this.mCharacMgr.World.getIslandAt(new Vector3(0, 0, 0)).removeFromScene(spawn);
            }

            this.mCharacMgr.GetCharacter().IsAllowedToMove = !this.mIsConsoleMode && this.mIsAllowedToMoveCam;
            this.mCharacMgr.Update(frameTime);

            if (!this.mCharacMgr.GetCharacter().IsAllowedToMove && !this.mIsConsoleMode && this.mIsAllowedToMoveCam)
            {
                this.mCameraMan.MouseMovement(this.mInput.MouseMoveX, this.mInput.MouseMoveY);
                this.mCameraMan.UpdateCamera(frameTime, this.mInput);
            }
        }

        public void Dispose() { this.mCharacMgr.Dispose(); }
    }
}

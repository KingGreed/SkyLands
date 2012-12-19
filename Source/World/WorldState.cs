using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.CharacSystem;
using Game.Terrain;
using Game.States;
using Game.BaseApp;

using Mogre;

namespace Game
{
    public partial class World
    {
        private void CreateCamera()
        {
            this.mCamera = this.mStateMgr.SceneManager.CreateCamera("DebugCam");

            this.mCamera.Position = new Vector3(0, 100, 250);

            this.mCamera.LookAt(new Vector3(0, 50, 0));
            this.mCamera.NearClipDistance = 5;
            this.mCamera.FarClipDistance = 3500;

            this.mCameraMan = new CameraMan(this.mCamera);
        }

        private void CreateViewports()
        {
            ColourValue fadeColor = new ColourValue(0.1f, 0.1f, 0.1f);

            var vp = this.mStateMgr.Window.AddViewport(mCamera);
            vp.BackgroundColour = fadeColor;

            this.mCamera.AspectRatio = (vp.ActualWidth / vp.ActualHeight);

            //this.mStateMgr.SceneManager.SetFog(FogMode.FOG_LINEAR, fadeColor, 1, 2800, 3500);
        }

        public override void Update(float frameTime)
        {
            MoisManager input = this.mStateMgr.Input;
            this.mCameraMan.UpdateCamera(frameTime, input);
            
            this.mCharacMgr.Update(frameTime);
        }

        public override void Shutdown()
        {
            if (this.mStateMgr == null) { return; }

            this.mStateMgr = null;
            this.mIsStartedUp = false;
        }
    }
}

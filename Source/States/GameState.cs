using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

using Game.CharacSystem;

namespace Game.States
{
    class GameState : State
    {
        private OverlayElement[] mOverlayElements;
        private CharacMgr mCharacMgr;
        
        public GameState() : base()
        {
            mOverlayElements = new OverlayElement[6];
            mCharacMgr = new CharacMgr();
        }

        public override bool Startup(StateManager stateMgr)
        {
            if (isStartedUp)
                return false;

            mStateMgr = stateMgr;
            isStartedUp = true;

            if (!CreateScene())
                return false;

            return true;
        }

        public override void Shutdown()
        {
            if (mStateMgr == null)
                return;
            
            mStateMgr = null;
            isStartedUp = false;
        }

        public override void Update(float frameTime)
        {
            if (mStateMgr.Input.WasKeyPressed(MOIS.KeyCode.KC_ESCAPE))
                mStateMgr.RequestShutdown();

            UpdateCamera(frameTime);
            UpdateOverlay();
        }

        private bool CreateScene()
        {
            LogManager.Singleton.DefaultLog.LogMessage("Creating scene");

            Overlay overlay = OverlayManager.Singleton.GetByName("Core/DebugOverlay");
            mOverlayElements[0] = OverlayManager.Singleton.GetOverlayElement("Core/CurrFps");
            mOverlayElements[1] = OverlayManager.Singleton.GetOverlayElement("Core/AverageFps");
            mOverlayElements[2] = OverlayManager.Singleton.GetOverlayElement("Core/WorstFps");
            mOverlayElements[3] = OverlayManager.Singleton.GetOverlayElement("Core/BestFps");
            mOverlayElements[4] = OverlayManager.Singleton.GetOverlayElement("Core/NumTris");
            mOverlayElements[5] = OverlayManager.Singleton.GetOverlayElement("Core/NumBatches");
            overlay.Show();

            mCharacMgr.addPlayer(new Race(mStateMgr.Engine, "Sinbad.mesh"), new CharacterInfo("Sinbad", new Vector3(0, 0, -250)));

            return true;
        }

        /* Updates the deplacement of a third person camera which is linked to nothing. *
         * The player won't control this Camera.                                        */
        private void UpdateCamera(float frameTime)
        {
            MoisManager input = mStateMgr.Input;
            Camera cam = mStateMgr.Engine.Camera;

            if (input.WasMouseMoved())
            {
                cam.Yaw(new Radian(-(float) input.MouseMoveX * 0.2f * frameTime));
                cam.Pitch(new Radian(-(float)input.MouseMoveY * 0.15f * frameTime));
                cam.Roll(new Radian(-(float)input.MouseMoveZ * 0.005f * frameTime));
            }

            float speed = 90 * frameTime;
            Vector3 move = new Vector3();

            /* KeyCode below are for qwerty */
            if (input.IsKeyDown(MOIS.KeyCode.KC_D))
                move.x += speed;

            if (input.IsKeyDown(MOIS.KeyCode.KC_A)) // Q in azerty
                move.x -= speed;

            if (input.IsKeyDown(MOIS.KeyCode.KC_Q)) // A in azerty
                move.y += speed;

            if (input.IsKeyDown(MOIS.KeyCode.KC_E))
                move.y -= speed;

            if (input.IsKeyDown(MOIS.KeyCode.KC_S))
                move.z += speed;

            if (input.IsKeyDown(MOIS.KeyCode.KC_W)) // Z in azerty
                move.z -= speed;

            cam.Move(move);
        }

        private void UpdateOverlay()
        {
            mOverlayElements[0].Caption = "Current FPS: " + mStateMgr.Engine.Window.LastFPS.ToString();
            mOverlayElements[1].Caption = "Average FPS: " + mStateMgr.Engine.Window.AverageFPS.ToString();
            mOverlayElements[2].Caption = "Worst FPS: " + mStateMgr.Engine.Window.WorstFPS.ToString();
            mOverlayElements[3].Caption = "Best FPS: " + mStateMgr.Engine.Window.BestFPS.ToString();
            mOverlayElements[4].Caption = "Triangle Count: " + mStateMgr.Engine.Window.TriangleCount.ToString();
            mOverlayElements[5].Caption = "Batch Count: " + mStateMgr.Engine.Window.BatchCount.ToString();
        }
    }
}

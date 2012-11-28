using Mogre;
using MOIS;
using System;
using System.Windows.Forms;



namespace Game.BaseApp
{
    public abstract partial class BaseApplication
    {
        protected Keyboard mKeyboard;
        protected Mouse    mMouse;

        protected virtual void InitializeInput()
        {
            LogManager.Singleton.LogMessage("*** Initializing OIS ***");

            int windowHnd;
            mWindow.GetCustomAttribute("WINDOW", out windowHnd);
            var inputMgr = MOIS.InputManager.CreateInputSystem((uint)windowHnd);

            this.mKeyboard = (MOIS.Keyboard)inputMgr.CreateInputObject(MOIS.Type.OISKeyboard, true);
            this.mMouse    = (MOIS.Mouse)   inputMgr.CreateInputObject(MOIS.Type.OISMouse,    true);

            this.mKeyboard.KeyPressed  += new KeyListener.KeyPressedHandler(     OnKeyPressed);
            this.mKeyboard.KeyReleased += new KeyListener.KeyReleasedHandler(    OnKeyReleased);
            this.mMouse.MouseMoved     += new MouseListener.MouseMovedHandler(   OnMouseMoved);
            this.mMouse.MousePressed   += new MouseListener.MousePressedHandler( OnMousePressed);
            this.mMouse.MouseReleased  += new MouseListener.MouseReleasedHandler(OnMouseReleased);
        }

        protected void ProcessInput() { this.mKeyboard.Capture(); this.mMouse.Capture(); }

        protected virtual bool OnKeyPressed(KeyEvent evt)
        {
            switch (evt.key)
            {
                case KeyCode.KC_W:
                case KeyCode.KC_UP:   this.mCameraMan.GoingForward = true; break;

                case KeyCode.KC_S:
                case KeyCode.KC_DOWN: this.mCameraMan.GoingBack = true; break;
                
                case KeyCode.KC_A:
                case KeyCode.KC_LEFT: this.mCameraMan.GoingLeft = true; break;

                case KeyCode.KC_D:
                case KeyCode.KC_RIGHT: this.mCameraMan.GoingRight = true; break;

                case KeyCode.KC_E:
                case KeyCode.KC_PGUP: this.mCameraMan.GoingUp = true;  break;

                case KeyCode.KC_Q:
                case KeyCode.KC_PGDOWN: this.mCameraMan.GoingDown = true; break;

                case KeyCode.KC_LSHIFT:
                case KeyCode.KC_RSHIFT: this.mCameraMan.FastMove = true; break;

                case KeyCode.KC_T: this.CycleTextureFilteringMode();break;
                case KeyCode.KC_R: this.CyclePolygonMode();    break;
                case KeyCode.KC_F5: this.ReloadAllTextures();  break;
                case KeyCode.KC_SYSRQ:  this.TakeScreenshot(); break;
                case KeyCode.KC_ESCAPE: this.Shutdown();       break;
            }

            return true;
        }

        protected virtual bool OnKeyReleased(KeyEvent evt)
        {
            switch (evt.key)
            {
                case KeyCode.KC_W:
                case KeyCode.KC_UP: this.mCameraMan.GoingForward = false;    break;

                case KeyCode.KC_S:
                case KeyCode.KC_DOWN:   this.mCameraMan.GoingBack = false;   break;

                case KeyCode.KC_A:
                case KeyCode.KC_LEFT:    this.mCameraMan.GoingLeft = false;  break;

                case KeyCode.KC_D:
                case KeyCode.KC_RIGHT:   this.mCameraMan.GoingRight = false; break;

                case KeyCode.KC_E:
                case KeyCode.KC_PGUP:    this.mCameraMan.GoingUp = false;    break;

                case KeyCode.KC_Q:
                case KeyCode.KC_PGDOWN:  this.mCameraMan.GoingDown = false;  break;

                case KeyCode.KC_LSHIFT:
                case KeyCode.KC_RSHIFT:  this.mCameraMan.FastMove = false;   break;
            }			

            return true;
        }

        protected virtual bool OnMouseMoved(MouseEvent evt) { this.mCameraMan.MouseMovement(evt.state.X.rel, evt.state.Y.rel); return true; }
        protected virtual bool OnMousePressed(MouseEvent evt, MouseButtonID id)  { return true; }
        protected virtual bool OnMouseReleased(MouseEvent evt, MouseButtonID id) { return true; }
    }
}
using System;
using MOIS;

namespace Game
{
    public class MoisManager
    {
        private InputManager mInputMgr;
        private Keyboard mKeyboard;
        private Mouse mMouse;
        private bool[] mKeyDown;
        private bool[] mKeyPressed;
        private bool[] mMouseDown;
        private bool[] mMousePressed;
        private Vector3 mMouseMove;
        private Vector3 mMousePos;
        private Vector3 mMousePressedPos;
        private Vector3 mMouseReleasedPos;

        // Get last relative mouse movement (and wheel movement on Z axis)
        public int MouseMoveX { get { return (int)mMouseMove.x; } }
        public int MouseMoveY { get { return (int)mMouseMove.y; } }
        public int MouseMoveZ { get { return (int)mMouseMove.z; } }

        // Get absolute mouse position within window bounds
        public int MousePosX { get { return (int)mMousePos.x; } }
        public int MousePosY { get { return (int)mMousePos.y; } }

        // Get last absolute mouse position when a mouse button was pressed
        public int MousePressedPosX { get { return (int)mMousePressedPos.x; } }
        public int MousePressedPosY { get { return (int)mMousePressedPos.y; } }

        // Get last absolute mouse position when a mouse button was released
        public int MouseReleasedPosX { get { return (int)mMouseReleasedPos.x; } }
        public int MouseReleasedPosY { get { return (int)mMouseReleasedPos.y; } }

        internal MoisManager()
        {
            mInputMgr = null;
            mKeyboard = null;
            mMouse = null;
            mKeyDown = new bool[256];
            mKeyPressed = new bool[256];
            mMouseDown = new bool[8];
            mMousePressed = new bool[8];
            mMouseMove = new Vector3();
            mMousePos = new Vector3();
            mMousePressedPos = new Vector3();
            mMouseReleasedPos = new Vector3();
        }

        internal bool Startup(IntPtr windowHandle, int width, int height)
        {
            if (mInputMgr != null)
                return false;

            // initialize input manager
            ParamList pl = new ParamList();
            pl.Insert("WINDOW", windowHandle.ToString());
            mInputMgr = InputManager.CreateInputSystem(pl);
            if (mInputMgr == null)
                return false;

            // initialize keyboard
            mKeyboard = (Keyboard)mInputMgr.CreateInputObject(MOIS.Type.OISKeyboard, true);
            if (mKeyboard == null)
                return false;

            // set up keyboard event handlers
            mKeyboard.KeyPressed += OnKeyPressed;
            mKeyboard.KeyReleased += OnKeyReleased;

            // initialize mouse
            mMouse = (Mouse)mInputMgr.CreateInputObject(MOIS.Type.OISMouse, true);
            if (mMouse == null)
                return false;

            // set up area for absolute mouse positions
            MouseState_NativePtr state = mMouse.MouseState;
            state.width = width;
            state.height = height;

            // set up mouse event handlers
            mMouse.MouseMoved += OnMouseMoved;
            mMouse.MousePressed += OnMousePressed;
            mMouse.MouseReleased += OnMouseReleased;

            Clear();

            return true;
        }

        internal void Shutdown()
        {
            if (mMouse != null)
            {
                mInputMgr.DestroyInputObject(mMouse);
                mMouse = null;
            }

            if (mKeyboard != null)
            {
                mInputMgr.DestroyInputObject(mKeyboard);
                mKeyboard = null;
            }

            if (mInputMgr != null)
            {
                InputManager.DestroyInputSystem(mInputMgr);
                mInputMgr = null;
            }
        }

        internal void Update()
        {
            ClearKeyPressed();
            ClearMousePressed();
            ClearMouseMove();

            mKeyboard.Capture();
            mMouse.Capture();
        }

        public void Clear()
        {
            ClearKeyPressed();
            ClearKeyDown();
            ClearMousePressed();
            ClearMouseDown();
            ClearMouseMove();
        }

        public bool IsKeyDown(KeyCode key)
        {
            return mKeyDown[(int) key];
        }

        public bool WasKeyPressed(KeyCode key)
        {
            return mKeyPressed[(int) key];
        }

        public bool IsMouseButtonDown(MouseButtonID button)
        {
            return mMouseDown[(int) button];
        }

        public bool WasMouseButtonPressed(MouseButtonID button)
        {
            return mMousePressed[(int) button];
        }

        public bool WasMouseMoved()
        {
            return mMouseMove.x != 0 || mMouseMove.y != 0 || mMouseMove.z != 0;
        }

        private void ClearKeyPressed()
        {
            for (int i = 0; i < mKeyPressed.Length; ++i)
                mKeyPressed[i] = false;
        }

        private void ClearKeyDown()
        {
            for (int i = 0; i < mKeyDown.Length; ++i)
                mKeyDown[i] = false;
        }

        private void ClearMousePressed()
        {
            for (int i = 0; i < mMousePressed.Length; ++i)
                mMousePressed[i] = false;
        }

        private void ClearMouseDown()
        {
            for (int i = 0; i < mMouseDown.Length; ++i)
                mMouseDown[i] = false;
        }

        private void ClearMouseMove()
        {
            mMouseMove.x = 0;
            mMouseMove.y = 0;
            mMouseMove.z = 0;
        }

        private bool OnKeyPressed(KeyEvent arg)
        {
            mKeyDown[(int)arg.key] = true;
            mKeyPressed[(int)arg.key] = true;
            return true;
        }

        private bool OnKeyReleased(KeyEvent arg)
        {
            mKeyDown[(int)arg.key] = false;
            return true;
        }

        private bool OnMouseMoved(MouseEvent arg)
        {
            mMouseMove.x = arg.state.X.rel;
            mMouseMove.y = arg.state.Y.rel;
            mMouseMove.z = arg.state.Z.rel;
            mMousePos.x = arg.state.X.abs;
            mMousePos.y = arg.state.Y.abs;
            return true;
        }

        private bool OnMousePressed(MouseEvent arg, MouseButtonID id)
        {
            mMouseDown[(int)id] = true;
            mMousePressed[(int)id] = true;
            mMousePos.x = arg.state.X.abs;
            mMousePos.y = arg.state.Y.abs;
            mMousePressedPos.x = arg.state.X.abs;
            mMousePressedPos.y = arg.state.Y.abs;
            return true;
        }

        private bool OnMouseReleased(MouseEvent arg, MouseButtonID id)
        {
            mMouseDown[(int)id] = false;
            mMousePos.x = arg.state.X.abs;
            mMousePos.y = arg.state.Y.abs;
            mMouseReleasedPos.x = arg.state.X.abs;
            mMouseReleasedPos.y = arg.state.Y.abs;
            return true;
        }
    }
}

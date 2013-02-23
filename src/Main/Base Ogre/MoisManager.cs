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
        private bool[] mKeyReleased;
        private bool[] mMouseDown;
        private bool[] mMousePressed;
        private Vector3 mMouseMove;
        private Vector3 mMousePos;
        private Vector3 mMousePressedPos;
        private Vector3 mMouseReleasedPos;
        public delegate bool IsKeyEvent(MOIS.KeyCode key);   // Represents WasKeyPressed, WasKeyReleased or IsKeyDown

        public Keyboard KeyBoard { get { return this.mKeyboard; } }
        public Mouse Mouse { get { return this.mMouse; } }

        // Get last relative mouse movement (and wheel movement on Z axis)
        public int MouseMoveX { get { return (int)this.mMouseMove.x; } }
        public int MouseMoveY { get { return (int)this.mMouseMove.y; } }
        public int MouseMoveZ { get { return (int)this.mMouseMove.z; } }

        // Get absolute mouse position within window bounds
        public int MousePosX { get { return (int)this.mMousePos.x; } }
        public int MousePosY { get { return (int)this.mMousePos.y; } }

        // Get last absolute mouse position when a mouse button was pressed
        public int MousePressedPosX { get { return (int)this.mMousePressedPos.x; } }
        public int MousePressedPosY { get { return (int)this.mMousePressedPos.y; } }

        // Get last absolute mouse position when a mouse button was released
        public int MouseReleasedPosX { get { return (int)this.mMouseReleasedPos.x; } }
        public int MouseReleasedPosY { get { return (int)this.mMouseReleasedPos.y; } }

        internal MoisManager()
        {
            this.mInputMgr = null;
            this.mKeyboard = null;
            this.mMouse = null;
            this.mKeyDown = new bool[256];
            this.mKeyPressed = new bool[256];
            this.mKeyReleased = new bool[256];
            this.mMouseDown = new bool[8];
            this.mMousePressed = new bool[8];
            this.mMouseMove = new Vector3();
            this.mMousePos = new Vector3();
            this.mMousePressedPos = new Vector3();
            this.mMouseReleasedPos = new Vector3();
        }

        internal bool Startup(int windowHandle, uint width, uint height)
        {
            if (this.mInputMgr != null)
                return false;

            // initialize input manager
            ParamList pl = new ParamList();
            pl.Insert("WINDOW", windowHandle.ToString());
            this.mInputMgr = InputManager.CreateInputSystem(pl);
            if (this.mInputMgr == null)
                return false;

            // initialize keyboard
            this.mKeyboard = (Keyboard)this.mInputMgr.CreateInputObject(MOIS.Type.OISKeyboard, true);
            if (this.mKeyboard == null)
                return false;

            // set up keyboard event handlers
            this.mKeyboard.KeyPressed += OnKeyPressed;
            this.mKeyboard.KeyReleased += OnKeyReleased;

            // initialize mouse
            this.mMouse = (Mouse)this.mInputMgr.CreateInputObject(MOIS.Type.OISMouse, true);
            if (this.mMouse == null)
                return false;

            // set up area for absolute mouse positions
            MouseState_NativePtr state = this.mMouse.MouseState;
            state.width = (int) width;
            state.height = (int) height;

            // set up mouse event handlers
            this.mMouse.MouseMoved += OnMouseMoved;
            this.mMouse.MousePressed += OnMousePressed;
            this.mMouse.MouseReleased += OnMouseReleased;

            this.Clear();

            return true;
        }

        internal void Update()
        {
            this.ClearKeyPressed();
            this.ClearKeyReleased();
            this.ClearMousePressed();
            this.ClearMouseMove();

            this.mKeyboard.Capture();
            this.mMouse.Capture();
        }

        public void Clear()
        {
            this.ClearKeyPressed();
            this.ClearKeyReleased();
            this.ClearKeyDown();
            this.ClearMousePressed();
            this.ClearMouseDown();
            this.ClearMouseMove();
        }

        public bool IsKeyDown(KeyCode key)                      { return this.mKeyDown[(int) key]; }
        public bool WasKeyPressed(KeyCode key)                  { return this.mKeyPressed[(int) key]; }
        public bool WasKeyReleased(KeyCode key)                 { return this.mKeyReleased[(int)key]; }
        public bool IsMouseButtonDown(MouseButtonID button)     { return this.mMouseDown[(int) button]; }
        public bool WasMouseButtonPressed(MouseButtonID button) { return this.mMousePressed[(int) button]; }
        public bool WasMouseMoved()                             { return this.mMouseMove.x != 0 || this.mMouseMove.y != 0 || this.mMouseMove.z != 0; }

        public bool IsOneKeyEventTrue(IsKeyEvent keyEvent, params MOIS.KeyCode[] keys)
        {
            foreach (MOIS.KeyCode key in keys)
                if (keyEvent(key)) { return true; }

            return false;
        }

        public bool AreAllKeyEventTrue(IsKeyEvent keyEvent, params MOIS.KeyCode[] keys)
        {
            return !this.IsOneKeyEventTrue(keyEvent, keys);
        }

        private void ClearKeyPressed()
        {
            for (int i = 0; i < this.mKeyPressed.Length; ++i)
                this.mKeyPressed[i] = false;
        }

        private void ClearKeyReleased()
        {
            for (int i = 0; i < this.mKeyReleased.Length; ++i)
                this.mKeyReleased[i] = false;
        }

        private void ClearKeyDown()
        {
            for (int i = 0; i < this.mKeyDown.Length; ++i)
                this.mKeyDown[i] = false;
        }

        private void ClearMousePressed()
        {
            for (int i = 0; i < this.mMousePressed.Length; ++i)
                this.mMousePressed[i] = false;
        }

        private void ClearMouseDown()
        {
            for (int i = 0; i < this.mMouseDown.Length; ++i)
                this.mMouseDown[i] = false;
        }

        private void ClearMouseMove()
        {
            this.mMouseMove.x = 0;
            this.mMouseMove.y = 0;
            this.mMouseMove.z = 0;
        }

        private bool OnKeyPressed(KeyEvent arg)
        {
            this.mKeyDown[(int)arg.key] = true;
            this.mKeyPressed[(int)arg.key] = true;
            return true;
        }

        private bool OnKeyReleased(KeyEvent arg)
        {
            this.mKeyDown[(int)arg.key] = false;
            this.mKeyReleased[(int)arg.key] = true;
            return true;
        }

        public string GetText()
        {
            string txt = "";
            foreach (KeyCode code in Enum.GetValues(typeof(KeyCode)))
            {
                if (this.WasKeyPressed(code))
                {
                    char c = this.GetKeyCodeChar(code);
                    if(c != '\0')
                        txt += c;
                }
            }

            return txt;
        }

        public char GetKeyCodeChar(KeyCode code)
        {
            char c = (char)0;
            string txt;
            if (code == KeyCode.KC_SPACE)
                c = ' ';
            else if (code == KeyCode.KC_LBRACKET)
                c = '(';
            else if (code == KeyCode.KC_RBRACKET)
                c = ')';
            else
            {
                txt = Enum.GetName(typeof(KeyCode), code);
                txt = txt.Substring(3);

                if (txt.Length == 1)    // Consider the letters and numbers
                {
                    char tmp = txt.ToLower()[0];

                    if ((tmp >= 'a' && tmp <= 'z') || (tmp >= '0' && tmp <= '9'))
                        c = tmp;
                }
            }

            return c;
        }

        private bool OnMouseMoved(MouseEvent arg)
        {
            this.mMouseMove.x = arg.state.X.rel;
            this.mMouseMove.y = arg.state.Y.rel;
            this.mMouseMove.z = arg.state.Z.rel;
            this.mMousePos.x = arg.state.X.abs;
            this.mMousePos.y = arg.state.Y.abs;
            return true;
        }

        private bool OnMousePressed(MouseEvent arg, MouseButtonID id)
        {
            this.mMouseDown[(int)id] = true;
            this.mMousePressed[(int)id] = true;
            this.mMousePos.x = arg.state.X.abs;
            this.mMousePos.y = arg.state.Y.abs;
            this.mMousePressedPos.x = arg.state.X.abs;
            this.mMousePressedPos.y = arg.state.Y.abs;
            return true;
        }

        private bool OnMouseReleased(MouseEvent arg, MouseButtonID id)
        {
            this.mMouseDown[(int)id] = false;
            this.mMousePos.x = arg.state.X.abs;
            this.mMousePos.y = arg.state.Y.abs;
            this.mMouseReleasedPos.x = arg.state.X.abs;
            this.mMouseReleasedPos.y = arg.state.Y.abs;
            return true;
        }

        internal void Shutdown()
        {
            if (this.mMouse != null)
            {
                this.mInputMgr.DestroyInputObject(this.mMouse);
                this.mMouse = null;
            }

            if (this.mKeyboard != null)
            {
                this.mInputMgr.DestroyInputObject(this.mKeyboard);
                this.mKeyboard = null;
            }

            if (this.mInputMgr != null)
            {
                InputManager.DestroyInputSystem(this.mInputMgr);
                this.mInputMgr = null;
            }
        }
    }
}

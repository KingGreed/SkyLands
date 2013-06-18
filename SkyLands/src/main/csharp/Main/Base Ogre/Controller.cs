using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using Mogre;
using MOIS;
using XInputDotNetPure;

using Game.BaseApp;

namespace Game.Input
{
    public enum UserAction
    {
        Jump,
        Dance,
        Sprint,
        Levitate,
        MainAction,
        SecondaryAction,
        MoveSelectorLeft,
        MoveSelectorRight,
        Inventory,
        Start
    }

    public class Controller
    {
        private const float YAW_SENSIVITY = - 0.4f * 0.7f;
        private const float PITCH_SENSIVITY = - 0.15f * 0.7f;

        private readonly OgreForm    mOgreForm;
        private readonly XmlDocument mCommands;
        private readonly Keyboard    mKeyBoard;
        private readonly bool[]      mKeyDown;
        private readonly bool[]      mKeyPressed;
        private readonly bool[]      mKeyReleased;
        private readonly Dictionary<MouseButtons, bool> mMouseDown;
        private readonly Dictionary<MouseButtons, bool> mMousePressed;
        private readonly Dictionary<MouseButtons, bool> mMouseReleased;
        private readonly bool[] mUserActionsEnded;
        private readonly bool[] mUserActionsOccured;
        private readonly bool[] mUserActions;
        private Mogre.Vector3 mMovementFactor;
        private Mogre.Vector3 mMousePos;
        private Mogre.Vector3 mMouseMove;
        private bool          mUpdateMovement = true;
        private bool          mCursorVisibility;
        private float         mTimeToVibrate;

        public Mogre.Vector3 MovementFactor  { get { return this.mMovementFactor; } private set { this.mMovementFactor = value; } }
        public float         Pitch           { get; private set; }
        public float         Yaw             { get; private set; }
        public Mogre.Vector3 MousePos        { get { return this.mMousePos; } }
        public Mogre.Vector3 MouseMove       { get { return this.mMouseMove; } }
        public bool          BlockMouse      { get; set; }
        public bool          CursorVisibility
        {
            get { return this.mCursorVisibility; }
            set
            {
                if (this.mCursorVisibility == value) { return; }
                this.mCursorVisibility = value;
                if (this.mCursorVisibility)
                {
                    Cursor.Show();
                }
                else
                {
                    Cursor.Hide();
                }
            }
        }

        public GamePadState GamePadState { get { return GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes); } }

        public Controller(OgreForm ogreForm, int windowHnd)
        {
            this.mOgreForm = ogreForm;
            InputManager inputMgr;
            this.mKeyDown = new bool[256];
            this.mKeyPressed = new bool[256];
            this.mKeyReleased = new bool[256];

            ParamList pl = new ParamList();
            pl.Insert("WINDOW", windowHnd.ToString());
            inputMgr = InputManager.CreateInputSystem(pl);
            if (inputMgr == null) { return; }

            // initialize keyboard
            this.mKeyBoard = (Keyboard)inputMgr.CreateInputObject(MOIS.Type.OISKeyboard, true);
            if (this.mKeyBoard == null)
                return;
            this.mKeyBoard.KeyPressed += OnKeyPressed;
            this.mKeyBoard.KeyReleased += OnKeyReleased;

            this.mMouseDown = new Dictionary<MouseButtons, bool>();
            this.mMousePressed = new Dictionary<MouseButtons, bool>();
            this.mMouseReleased = new Dictionary<MouseButtons, bool>();
            foreach (MouseButtons button in Enum.GetValues(typeof(MouseButtons)))
            {
                this.mMouseDown.Add(button, false);
                this.mMousePressed.Add(button, false);
                this.mMouseReleased.Add(button, false);
            }

            this.mMousePos = new Mogre.Vector3();
            this.BlockMouse = false;
            this.mCursorVisibility = true;

            this.mCommands = new XmlDocument();
            this.LoadCommands();

            this.mUserActions = new bool[Enum.GetValues(typeof(UserAction)).Length];
            this.mUserActionsOccured = new bool[this.mUserActions.Length];
            this.mUserActionsEnded = new bool[this.mUserActions.Length];
            this.MovementFactor = new Mogre.Vector3();

            this.mOgreForm.MouseMove += OnMouseMoved;
            this.mOgreForm.MouseWheel += OnMouseWheel;
            this.mOgreForm.MouseDown += OnMousePressed;
            this.mOgreForm.MouseUp += OnMouseReleased;
            this.mOgreForm.MouseLeave += OnMouseLeave;
        }

        private void LoadCommands()  { this.mCommands.Load("commands.xml"); }

        public bool WasKeyPressed(KeyCode key)                  { return this.mKeyPressed[(int)key]; }
        public bool WasKeyReleased(KeyCode key)                 { return this.mKeyReleased[(int)key]; }
        public bool IsKeyDown(KeyCode key)                      { return this.mKeyDown[(int)key]; }
        public bool IsMouseButtonDown(MouseButtons button)      { return this.mMouseDown[button]; }
        public bool WasMouseButtonPressed(MouseButtons button)  { return this.mMousePressed[button]; }
        public bool WasMouseButtonReleased(MouseButtons button) { return this.mMouseReleased[button]; }
        public bool WasMouseMoved()                             { return this.mMouseMove.x != 0 || this.mMouseMove.y != 0; }
        public bool HasActionOccured(UserAction a)              { return this.mUserActionsOccured[(int)a]; }
        public bool HasActionEnded(UserAction a)                { return this.mUserActionsEnded[(int)a]; }
        public bool IsActionOccuring(UserAction a)              { return this.mUserActions[(int)a]; }

        public void Update(float frameTime)
        {
            /*if((this.mMousePos.x < 1 || this.mMousePos.x >= this.mOgreForm.Size.Width - 1 ||
               this.mMousePos.y < 1 || this.mMousePos.y >= this.mOgreForm.Size.Height - 1) && this.BlockMouse) {
                Cursor.Position = new System.Drawing.Point(this.mOgreForm.Location.X + this.mOgreForm.Size.Width / 2,
                                                           this.mOgreForm.Location.Y + this.mOgreForm.Size.Height / 2);
                this.mUpdateMovement = false;
                this.mMousePos = new Mogre.Vector3(Cursor.Position.X, Cursor.Position.Y, this.mMousePos.z);
            }*/
            
            XmlNodeList nodes = this.mCommands.ChildNodes[1].ChildNodes;
            if (this.GamePadState.IsConnected)
            {
                if (this.mTimeToVibrate > 0) { this.mTimeToVibrate -= frameTime; }
                if (this.mTimeToVibrate < 0) { this.Vibrate(0, 0); }

                this.Yaw = this.GamePadState.ThumbSticks.Right.X * 3.5f;
                this.Pitch = -this.GamePadState.ThumbSticks.Right.Y * 6.5f;
                this.mMovementFactor = new Mogre.Vector3(-this.GamePadState.ThumbSticks.Left.X,
                    this.GamePadState.DPad.Up == XInputDotNetPure.ButtonState.Pressed ? 1 : (this.GamePadState.DPad.Down == XInputDotNetPure.ButtonState.Pressed ? -1 : 0),
                                                   this.GamePadState.ThumbSticks.Left.Y);
            }

            this.mKeyBoard.Capture();
            int i = 0;
            for (; i < 6; i++) // Continuous event
            {
                float value = this.GetFloatValue(nodes[i]);
                switch (nodes[i].Attributes[0].Value)
                {
                    case "MoveLeft":
                        this.mMovementFactor.x += value;
                        break;

                    case "MoveRight":
                        this.mMovementFactor.x -= value;
                        break;

                    case "MoveUp":
                        this.mMovementFactor.y += value;
                        break;

                    case "MoveDown":
                        this.mMovementFactor.y -= value;
                        break;

                    case "MoveForward":
                        this.mMovementFactor.z += value;
                        break;

                    case "MoveBack":
                        this.mMovementFactor.z -= value;
                        break;
                }
            }

            this.Yaw += this.MouseMove.x;
            this.Yaw *= YAW_SENSIVITY;
            this.Pitch += this.MouseMove.y;
            this.Pitch *= PITCH_SENSIVITY;

            for (; i < nodes.Count; i++) // Single event
            {
                int actionId = (int)Enum.Parse(typeof(UserAction), nodes[i].Attributes[0].Value);
                bool newValue = this.GetBoolValue(nodes[i]);
                if (newValue && !this.mUserActions[actionId])
                    this.mUserActionsOccured[actionId] = true;
                if(!newValue && this.mUserActions[actionId])
                    this.mUserActionsEnded[actionId] = true;
                this.mUserActions[actionId] = newValue;
            }
        }

        public void SwitchCursorVisibility() { this.CursorVisibility = !this.CursorVisibility; }

        private float GetFloatValue(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                if (child.LocalName == "Keyboard" && this.IsKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), child.InnerText)))
                    return 1;

            return 0;
        }

        private bool GetBoolValue(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                switch (child.LocalName)
                {
                    case "Xbox":
                        if (this.GamePadState.IsConnected)
                        {
                            float trigger = this.GamePadState.Triggers.Left - this.GamePadState.Triggers.Right;
                            if (child.InnerText.Contains("Trigger"))
                            {
                                if (trigger != 0 && !(trigger > 0 ^ child.InnerText.Contains("Left")))
                                    return true;
                            }
                            else if (child.InnerText.Contains("DPad"))
                            {
                                if (((XInputDotNetPure.ButtonState)this.GamePadState.DPad.GetType().GetProperty(child.InnerText.Replace("DPad", "")).GetValue(this.GamePadState.DPad, null))
                                   == XInputDotNetPure.ButtonState.Pressed)
                                    return true;
                            }
                            else if (((XInputDotNetPure.ButtonState)this.GamePadState.Buttons.GetType().GetProperty(child.InnerText).GetValue(this.GamePadState.Buttons, null))
                                    == XInputDotNetPure.ButtonState.Pressed)
                                return true;
                        }
                        break;

                    case "Mouse":
                        if (child.InnerText.Contains("MoveZ"))
                        {
                            if (this.MouseMove.z != 0 && !(this.MouseMove.z > 0 ^ child.InnerText.Contains("+")))
                                return true;
                        }
                        else if (this.IsMouseButtonDown((MouseButtons)Enum.Parse(typeof(MouseButtons), child.InnerText)))
                            return true;
                        break;

                    case "Keyboard":
                        if (this.IsKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), child.InnerText)))
                            return true;
                        break;
                }
            }

            return false;
        }

        public void Vibrate(float left, float right, float time = 0)
        {
            GamePad.SetVibration(PlayerIndex.One, left, right);
            this.mTimeToVibrate = time;
        }

        internal bool OnKeyPressed(KeyEvent e)
        {
            this.mKeyPressed[(int)e.key] = !this.mKeyDown[(int)e.key];
            this.mKeyDown[(int)e.key] = true;
            return true;
        }

        internal bool OnKeyReleased(KeyEvent e)
        {
            this.mKeyReleased[(int)e.key] = this.mKeyDown[(int)e.key];
            this.mKeyDown[(int)e.key] = false;
            return true;
        }

        private void OnMouseWheel(object sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            if (this.mUpdateMovement) { this.mMouseMove.z = args.Delta - this.mMousePos.z; }
            this.mMousePos.z += args.Delta;
        }

        private void OnMouseMoved(object sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            if (this.mUpdateMovement)
            {
                this.mMouseMove.x = args.X - this.mMousePos.x;
                this.mMouseMove.y = args.Y - this.mMousePos.y;
            }
            this.mUpdateMovement = true;
            this.mMousePos.x = args.X;
            this.mMousePos.y = args.Y;
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (this.BlockMouse)
            {
                Cursor.Position = new System.Drawing.Point(this.mOgreForm.Location.X + this.mOgreForm.Size.Width / 2,
                                                           this.mOgreForm.Location.Y + this.mOgreForm.Size.Height / 2);
                this.mUpdateMovement = false;
                this.mMousePos = new Mogre.Vector3(Cursor.Position.X, Cursor.Position.Y, this.mMousePos.z);
            }
        }

        private void OnMousePressed(object sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            this.mMouseDown[args.Button] = true;
            this.mMousePressed[args.Button] = true;
        }

        private void OnMouseReleased(object sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            this.mMouseDown[args.Button] = false;
            this.mMouseReleased[args.Button] = true;
        }

        private void ClearKeyPressed()
        {
            for(int i = 0; i < this.mKeyPressed.Length; i++)
                this.mKeyPressed[i] = false;
        }

        private void ClearKeyReleased()
        {
            for (int i = 0; i < this.mKeyReleased.Length; i++)
                this.mKeyReleased[i] = false;
        }

        private void ClearKeyDown()
        {
            for (int i = 0; i < this.mKeyReleased.Length; i++)
                this.mKeyDown[i] = false;
        }

        private void ClearMousePressed()
        {
            foreach (MouseButtons button in Enum.GetValues(typeof(MouseButtons)))
                this.mMousePressed[button] = false;
        }

        private void ClearMouseReleased()
        {
            foreach (MouseButtons button in Enum.GetValues(typeof(MouseButtons)))
                this.mMouseReleased[button] = false;
        }

        private void ClearMouseDown()
        {
            foreach (MouseButtons button in Enum.GetValues(typeof(MouseButtons)))
                this.mMouseDown[button] = false;
        }

        private void ClearCursorMove()
        {
            this.mMouseMove.x = 0;
            this.mMouseMove.y = 0;
            this.mMouseMove.z = 0;
        }

        private void ClearUserActions()
        {
            for (int i = 0; i < this.mUserActionsOccured.Length; i++)
                this.mUserActionsOccured[i] = false;

            for (int i = 0; i < this.mUserActionsEnded.Length; i++)
                this.mUserActionsEnded[i] = false;

            this.MovementFactor = new Mogre.Vector3();
            this.Pitch = 0;
            this.Yaw = 0;
        }

        public void Clear()
        {
            this.ClearKeyPressed();
            this.ClearKeyReleased();
            this.ClearMousePressed();
            this.ClearMouseReleased();
            this.ClearCursorMove();
            this.ClearUserActions();
        }

        public void ClearAll()
        {
            this.Clear();
            this.ClearKeyDown();
            this.ClearMouseDown();
        }
    }
}

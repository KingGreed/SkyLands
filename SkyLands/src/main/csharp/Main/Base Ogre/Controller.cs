using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using Mogre;
using XInputDotNetPure;

using Game.BaseApp;

namespace Game
{
    public class Controller
    {
        public enum InputName { Keyboard, Xbox }

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
            Up,
            Down,
            Inventory,
            Start
        }

        private readonly XmlDocument mCommands;
        private readonly Dictionary<Keys, bool> mKeyDown;
        private readonly Dictionary<Keys, bool> mKeyPressed;
        private readonly Dictionary<Keys, bool> mKeyReleased;
        private readonly Dictionary<MouseButtons, bool> mMouseDown;
        private readonly Dictionary<MouseButtons, bool> mMousePressed;
        private readonly Dictionary<MouseButtons, bool> mMouseReleased;
        private readonly bool[] mUserActionsEnded;
        private readonly bool[] mUserActionsOccured;
        private readonly bool[] mUserActions;
        protected InputName mInputName;
        private Vector2 mMovementFactor;
        private Vector3 mMousePos;
        private Vector3 mMouseMove;
        private bool mUpdateMovement = true;

        public InputName ActualInputName { get { return this.mInputName; } set { this.mInputName = value; this.LoadCommands(); } }
        public Vector2   MovementFactor  { get { return this.mMovementFactor; } private set { this.mMovementFactor = value; } }
        public float     Pitch           { get; private set; }
        public float     Yaw             { get; private set; }
        public Vector3 MousePos          { get { return this.mMousePos; } }
        public Vector3 MouseMove         { get { return this.mMouseMove; } }
        public bool BlockMouse           { get; set; }

        public GamePadState GamePadState { get { return GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes); } }

        public Controller(OgreForm ogreForm)
        {
            this.mKeyDown = new Dictionary<Keys, bool>();
            this.mKeyPressed = new Dictionary<Keys, bool>();
            this.mKeyReleased = new Dictionary<Keys, bool>();
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (!this.mKeyDown.ContainsKey(key))
                {
                    this.mKeyDown.Add(key, false);
                    this.mKeyPressed.Add(key, false);
                    this.mKeyReleased.Add(key, false);
                }
            }

            this.mMouseDown = new Dictionary<MouseButtons, bool>();
            this.mMousePressed = new Dictionary<MouseButtons, bool>();
            this.mMouseReleased = new Dictionary<MouseButtons, bool>();
            foreach (MouseButtons button in Enum.GetValues(typeof(MouseButtons)))
            {
                this.mMouseDown.Add(button, false);
                this.mMousePressed.Add(button, false);
                this.mMouseReleased.Add(button, false);
            }

            this.mMousePos = new Vector3();
            this.BlockMouse = true;

            this.mCommands = new XmlDocument();
            this.LoadCommands();

            this.mUserActions = new bool[Enum.GetValues(typeof(UserAction)).Length];
            this.mUserActionsOccured = new bool[this.mUserActions.Length];
            this.mUserActionsEnded = new bool[this.mUserActions.Length];
            this.MovementFactor = new Vector2();
            
            ogreForm.MouseMove += OnMouseMoved;
            ogreForm.MouseDown += OnMousePressed;
            ogreForm.MouseUp += OnMouseReleased;
            ogreForm.MouseLeave += OnMouseLeave;
        }

        private void LoadCommands()  { this.mCommands.Load("commands.xml"); }

        public bool WasKeyPressed(Keys key)                      { return this.mKeyPressed[key]; }
        public bool WasKeyReleased(Keys key)                     { return this.mKeyReleased[key]; }
        public bool IsKeyDown(Keys key)                          { return this.mKeyDown[key]; }
        public bool IsMouseButtonDown(MouseButtons button)       { return this.mMouseDown[button]; }
        public bool WasMouseButtonPressed(MouseButtons button)   { return this.mMousePressed[button]; }
        public bool WasMouseButtonReleased(MouseButtons button)  { return this.mMouseReleased[button]; }
        public bool WasMouseMoved()                              { return this.mMouseMove.x != 0 || this.mMouseMove.y != 0; }
        public bool HasActionOccured(UserAction a)               { return this.mUserActionsOccured[(int)a]; }
        public bool HasActionEnded(UserAction a)                 { return this.mUserActionsEnded[(int)a]; }
        public bool IsActionOccuring(UserAction a)               { return this.mUserActions[(int)a]; }

        public void Update()
        {
            XmlNodeList nodes = this.mCommands.ChildNodes[1].ChildNodes;
            if (this.GamePadState.IsConnected)
            {
                this.Yaw = this.GamePadState.ThumbSticks.Right.X * 5;
                this.Pitch = -this.GamePadState.ThumbSticks.Right.Y * 8;
                this.mMovementFactor = new Vector2(-this.GamePadState.ThumbSticks.Left.X,
                                                   this.GamePadState.ThumbSticks.Left.Y);
            }
            for (int i = 0; i < 4; i++) // Continuous event
            {
                switch (nodes[i].Attributes[0].Value)
                {
                    case "MoveLeft":
                        this.mMovementFactor.x += this.GetFloatValue(nodes[i].ChildNodes[0]);
                        break;

                    case "MoveRight":
                        this.mMovementFactor.x -= this.GetFloatValue(nodes[i].ChildNodes[0]);
                        break;

                    case "MoveUp":
                        this.mMovementFactor.y += this.GetFloatValue(nodes[i].ChildNodes[0]);
                        break;

                    case "MoveDown":
                        this.mMovementFactor.y -= this.GetFloatValue(nodes[i].ChildNodes[0]);
                        break;
                }
            }

            this.Yaw += this.MouseMove.x;
            this.Pitch += this.MouseMove.y;

            for (int i = 4; i < nodes.Count; i++) // Single event
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

        private float GetFloatValue(XmlNode node)
        {
            if (node.LocalName == InputName.Keyboard.ToString())
                return this.IsKeyDown((Keys)Enum.Parse(typeof(Keys), node.InnerText)) ? 1 : 0;

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
                        else if (this.WasMouseButtonPressed((MouseButtons)Enum.Parse(typeof(MouseButtons), child.InnerText)))
                            return true;
                        break;

                    case "Keyboard":
                        if (this.WasKeyPressed((Keys)Enum.Parse(typeof(Keys), child.InnerText)))
                            return true;
                        break;
                }
            }

            return false;
        }

        public void Vibrate(float left, float right) { GamePad.SetVibration(PlayerIndex.One, left, right); }

        public void SetCursorVisibility(bool vis)
        {
            if (vis)
                Cursor.Show();
            else
                Cursor.Hide();
        }

        internal void OnKeyPressed(object sender, EventArgs e)
        {
            KeyEventArgs args = (KeyEventArgs)e;
            this.mKeyPressed[args.KeyCode] = !this.mKeyDown[args.KeyCode];
            this.mKeyDown[args.KeyCode] = true;
        }

        internal void OnKeyReleased(object sender, EventArgs e)
        {
            KeyEventArgs args = (KeyEventArgs)e;
            this.mKeyReleased[args.KeyCode] = this.mKeyDown[args.KeyCode];
            this.mKeyDown[args.KeyCode] = false;
        }

        private void OnMouseMoved(object sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            if (this.mUpdateMovement)
            {
                this.mMouseMove.x = args.X - this.mMousePos.x;
                this.mMouseMove.y = args.Y - this.mMousePos.y;
                this.mMouseMove.z = args.Delta - this.mMousePos.z;
            }
            this.mUpdateMovement = true;
            this.mMousePos.x = args.X;
            this.mMousePos.y = args.Y;
            this.mMousePos.z = args.Delta;
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            if (this.BlockMouse)
            {
                Cursor.Position = new System.Drawing.Point(OgreForm.WindowPosition.X + OgreForm.WindowSize.Width/2,
                                                           OgreForm.WindowPosition.Y + OgreForm.WindowSize.Height/2);
                this.mUpdateMovement = false;
                this.mMousePos = new Vector3(Cursor.Position.X, Cursor.Position.Y, this.mMousePos.z);
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
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                this.mKeyPressed[key] = false;
        }

        private void ClearKeyReleased()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                this.mKeyReleased[key] = false;
        }

        private void ClearKeyDown()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
                this.mKeyDown[key] = false;
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

            this.MovementFactor = new Vector2();
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

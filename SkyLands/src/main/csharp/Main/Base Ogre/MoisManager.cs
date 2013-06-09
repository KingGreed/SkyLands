using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mogre;
using XInputDotNetPure;

using Game.BaseApp;

namespace Game
{
    public class MoisManager
    {
        private readonly Dictionary<Keys, bool> mKeyDown;
        private readonly Dictionary<Keys, bool> mKeyPressed;
        private readonly Dictionary<Keys, bool> mKeyReleased;
        private readonly Dictionary<MouseButtons, bool> mMouseDown;
        private readonly Dictionary<MouseButtons, bool> mMousePressed;
        private readonly Dictionary<MouseButtons, bool> mMouseReleased;
        private Vector3 mMousePos;
        private Vector3 mMouseMove;
        private bool mUpdateMovement = true;
        public delegate bool IsKeyEvent(Keys key);   // Represents WasKeyPressed, WasKeyReleased or IsKeyDown

        public Vector3 MousePos   { get { return this.mMousePos; } }
        public Vector3 MouseMove  { get { return this.mMouseMove; } }
        public bool BlockMouse { get; set; }

        public GamePadState GamePadState { get { return GamePad.GetState(PlayerIndex.One, GamePadDeadZone.IndependentAxes); } }

        internal MoisManager(OgreForm ogreForm)
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

            ogreForm.KeyDown += OnKeyPressed;
            ogreForm.KeyUp += OnKeyReleased;
            
            ogreForm.MouseMove += OnMouseMoved;
            ogreForm.MouseDown += OnMousePressed;
            ogreForm.MouseUp += OnMouseReleased;
            ogreForm.MouseLeave += OnMouseLeave;
            this.BlockMouse = true;
        }

        internal virtual void Update()
        {
            this.ClearKeyPressed();
            this.ClearKeyReleased();
            this.ClearMousePressed();
            this.ClearMouseReleased();
            this.ClearCursorMove();
        }

        public void Clear()
        {
            this.ClearKeyPressed();
            this.ClearKeyReleased();
            this.ClearKeyDown();
            this.ClearMousePressed();
            this.ClearMouseReleased();
            this.ClearMouseDown();
            this.ClearCursorMove();
        }

        public bool WasKeyPressed(Keys key)                      { return this.mKeyPressed[key]; }
        public bool IsKeyDown(Keys key)                          { return this.mKeyDown[key]; }
        public bool WasKeyReleased(Keys key)                     { return this.mKeyReleased[key]; }
        public bool IsMouseButtonDown(MouseButtons button)       { return this.mMouseDown[button]; }
        public bool WasMouseButtonPressed(MouseButtons button)   { return this.mMousePressed[button]; }
        public bool WasMouseButtonReleased(MouseButtons button)  { return this.mMouseReleased[button]; }
        public bool WasMouseMoved()                              { return this.mMouseMove.x != 0 || this.mMouseMove.y != 0; }

        public bool IsOneKeyEventTrue(IsKeyEvent keyEvent, params Keys[] keys)
        {
            return keys.Any(key => keyEvent(key));
        }

        public bool AreAllKeyEventTrue(IsKeyEvent keyEvent, params Keys[] keys)
        {
            return keys.All(key => keyEvent(key));
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

        private void OnKeyPressed(object sender, System.EventArgs e)
        {
            KeyEventArgs args = (KeyEventArgs)e;
            this.mKeyPressed[args.KeyCode] = !this.mKeyDown[args.KeyCode];
            this.mKeyDown[args.KeyCode] = true;
            LogManager.Singleton.DefaultLog.LogMessage("Pressed");
        }

        private void OnKeyReleased(object sender, System.EventArgs e)
        {
            KeyEventArgs args = (KeyEventArgs)e;
            this.mKeyReleased[args.KeyCode] = this.mKeyDown[args.KeyCode];
            this.mKeyDown[args.KeyCode] = false;
        }

        private void OnMouseMoved(object sender, System.EventArgs e)
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

        private void OnMouseLeave(object sender, System.EventArgs e)
        {
            Cursor.Position = new System.Drawing.Point(OgreForm.WindowPosition.X + OgreForm.WindowSize.Width / 2,
                                                       OgreForm.WindowPosition.Y + OgreForm.WindowSize.Height / 2);
            this.mUpdateMovement = !this.BlockMouse;
            this.mMousePos = new Vector3(Cursor.Position.X, Cursor.Position.Y, this.mMousePos.z);
        }

        private void OnMousePressed(object sender, System.EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            this.mMouseDown[args.Button] = true;
            this.mMousePressed[args.Button] = true;
        }

        private void OnMouseReleased(object sender, System.EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            this.mMouseDown[args.Button] = false;
            this.mMouseReleased[args.Button] = true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using Mogre;

using Game.BaseApp;

namespace Game
{
    public class Controller : MoisManager
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

        private XmlDocument     mCommands;
        private readonly bool[] mUserActionsOccured;
        private readonly bool[] mUserActions;
        private Vector2         mMovementFactor;
        protected InputName     mInputName;
        public Vector2   MovementFactor  { get { return this.mMovementFactor; } private set { this.mMovementFactor = value; } }
        public float     Pitch           { get; private set; }
        public float     Yaw             { get; private set; }
        public InputName ActualInputName { get { return this.mInputName; } set { this.mInputName = value; this.LoadCommands(); } }

        public Controller(OgreForm ogreForm) : base(ogreForm)
        {
            this.mCommands = new XmlDocument();
            this.LoadCommands();
            
            this.mUserActions = new bool[Enum.GetValues(typeof(UserAction)).Length];
            this.mUserActionsOccured = new bool[Enum.GetValues(typeof(UserAction)).Length];
            this.MovementFactor = new Vector2();
        }

        private void LoadCommands()
        {
            this.mCommands.Load("commands.xml");
        }

        internal override void Update()
        {
            base.Update();
            this.ClearUserActions();

            XmlNodeList nodes = this.mCommands.ChildNodes[0].ChildNodes;
            if (this.GamePadState.IsConnected)
            {
                this.Yaw = this.GamePadState.ThumbSticks.Right.X * 5;
                this.Pitch = -this.GamePadState.ThumbSticks.Right.Y * 5;
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

                this.Yaw += this.MouseMove.x;
                this.Pitch += this.MouseMove.y;
            }

            for (int i = 4; i < nodes.Count; i++) // Single event
            {
                int actionId = (int)Enum.Parse(typeof(UserAction), nodes[i].Attributes[0].Value);
                bool newValue = this.GetBoolValue(nodes[i]);
                if (newValue && !this.mUserActions[actionId])
                    this.mUserActionsOccured[actionId] = true;
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
                switch(child.LocalName)
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

        public bool HasActionOccured(UserAction a) { return this.mUserActionsOccured[(int)a]; }

        public bool IsActionOccuring(UserAction a) { return this.mUserActions[(int)a]; }

        public void SetCursorVisibility(bool vis)
        {
            if (vis)
                Cursor.Show();
            else
                Cursor.Hide();
        }

        private void ClearUserActions()
        {
            for (int i = 0; i < this.mUserActionsOccured.Length; i++)
                this.mUserActionsOccured[i] = false;

            this.MovementFactor = new Vector2();
            this.Pitch = 0;
            this.Yaw = 0;
        }
    }
}

using System;
using System.Collections.Generic;
using Mogre;

using Miyagi.Common.Events;
using Miyagi.Common;
using Miyagi.Common.Data;
using Miyagi.Common.Resources;
using Miyagi.UI.Controls;
using Miyagi.UI.Controls.Layout;
using Miyagi.UI.Controls.Styles;

using Game.GUICreator;
using Game.States;

namespace Game.IGConsole
{
    public class MyConsole : GUIFactory
    {
        public delegate void ConsoleEvent(string command);
        public event ConsoleEvent OnCommandEntered;

        private bool mIsEnable, mKeepPanelOpen;
        private Panel mLog, mTextBox;
        private Label mNewLabel;
        private Queue<Label> mOldLabels;
        private MoisManager mInput;
        private Timer mTimer;

        public Miyagi.UI.GUI GUI { get { return this.mGUI; } }
        
        public bool KeepPanelOpen
        {
            get { return this.mKeepPanelOpen; }
            set
            {
                if (this.mKeepPanelOpen != value)
                {
                    this.mKeepPanelOpen = value;
                    if (this.mKeepPanelOpen)  { this.PanelVisibility(true); }
                    else                      { this.mTimer.Reset(); }
                }
            }
        }

        public bool Enable
        {
            get { return this.mIsEnable; }
            set
            {
                if (this.mIsEnable != value)
                {
                    this.mIsEnable = value;
                    if (this.mIsEnable)
                    {
                        this.TextBoxVisibility(true);
                        this.PanelVisibility(true);
                    }
                    else
                    {
                        this.WriteLine(this.mNewLabel.Text);
                        this.TextBoxVisibility(false);
                        this.mTimer.Reset();
                    }
                }
            }
        }

        public MyConsole(StateManager stateMgr) : base(stateMgr, "Game GUI")
        {
            this.mInput = stateMgr.Input;
            this.mTimer = new Timer();
            this.mKeepPanelOpen = false;
        }

        protected override void CreateGUI()
        {
            this.mTextBox = new Panel("TextBox1")
            {
                Size = new Size(this.mOriginalWndSize.Width / 3, 32),
                Padding = new Thickness(9, 0, 8, 0),
                TextStyle =
                {
                    Alignment = Alignment.MiddleLeft,
                    ForegroundColour = Colours.White
                },
                Skin = this.mMiyagiMgr.Skins["Console"],
            };
            this.mTextBox.Location = new Point(0, this.mOriginalWndSize.Height - this.mTextBox.Size.Height);
            this.mTextBox.TabIndex = 2;

            TextStyle style = new TextStyle();
            style.Alignment = Alignment.MiddleLeft;
            style.Font = this.mMiyagiMgr.Fonts["Console"];
            style.ForegroundColour = Colours.White;
            this.mNewLabel = new Label();
            this.mNewLabel.Text = "";
            this.mNewLabel.Size = this.mTextBox.Size;
            this.mNewLabel.Location = this.mTextBox.Location;
            this.mNewLabel.TextStyle = style;
            this.mNewLabel.TabIndex = 0;
            this.mGUI.Controls.Add(this.mNewLabel);

            this.mLog = new Panel("Panel1")
            {
                TabStop = false,
                TabIndex = 0,
                Throwable = true,
                Size = new Size(this.mTextBox.Size.Width, this.mOriginalWndSize.Height / 4 - this.mTextBox.Size.Height),
                MinSize = new Size(0, 0),
                ResizeThreshold = new Thickness(0),
                BorderStyle =
                {
                    Thickness = new Thickness(8, 16, 8, 8)
                },
                HScrollBarStyle =
                {
                    Extent = 16,
                    ThumbStyle =
                    {
                        BorderStyle =
                        {
                            Thickness = new Thickness(2, 2, 2, 2)
                        }
                    }
                },
                VScrollBarStyle =
                {
                    Extent = 16,
                    ThumbStyle =
                    {
                        BorderStyle =
                        {
                            Thickness = new Thickness(2, 2, 2, 2)
                        }
                    }
                },
                Skin = this.mMiyagiMgr.Skins["Console"]
            };
            mLog.Location = new Point(0, this.mOriginalWndSize.Height - mLog.Size.Height - this.mTextBox.Size.Height);
            this.mLog.TabIndex = 3;

            this.mOldLabels = new Queue<Label>();

            this.mIsEnable = false;
            this.TextBoxVisibility(false);
            this.PanelVisibility(false);

            this.mGUI.Controls.Add(this.mTextBox);
            this.mGUI.Controls.Add(this.mLog);
        }

        public void WriteLine(object o) { this.WriteLine(o.ToString()); }
        public void WriteLine(string txt)
        {
            if (!this.Enable)
            {
                this.PanelVisibility(true);
                this.mTimer.Reset();
            }
            
            Label newL = new Label();
            newL.Text = txt;
            newL.AutoSize = true;
            newL.MaxSize = this.mTextBox.Size;
            newL.SuccessfulHitTest += (s, e) => e.Cancel = true;
            newL.TextStyle = this.mNewLabel.TextStyle;

            if (this.mOldLabels.Count >= this.mLog.Size.Height / this.mNewLabel.Size.Height)
            {
                this.mLog.Controls.Remove(this.mOldLabels.Dequeue());
                foreach (Label l in this.mOldLabels)
                    l.Location = new Point(0, l.Location.Y - this.mNewLabel.Size.Height);
            }

            int lastYLoc = this.mOldLabels.Count * this.mNewLabel.Size.Height;

            newL.Location = new Point(0, lastYLoc);

            this.mLog.Controls.Add(newL);
            this.mOldLabels.Enqueue(newL);
            this.mNewLabel.Text = "";

            if(this.OnCommandEntered != null)
                this.OnCommandEntered(txt);
        }

        public void Update()
        {
            if (this.mInput.WasKeyPressed(MOIS.KeyCode.KC_RETURN)) { this.Enable = !this.Enable; }
            if (this.mInput.WasKeyPressed(MOIS.KeyCode.KC_TAB))    { this.KeepPanelOpen = !this.KeepPanelOpen; }

            if (this.Enable)
            {
                string txt = this.mInput.GetText();
                if (txt != "")
                    this.mNewLabel.Text += txt;
            }
            else
            {
                if (((float)this.mTimer.Milliseconds) / 1000f > 3)
                    this.PanelVisibility(false);
            }
        }

        public void UpdateZOder(int zOrder) { this.mGUI.ZOrder = zOrder; }

        private void TextBoxVisibility(bool vis)
        {
            this.mTextBox.Visible = vis;
            this.mNewLabel.Visible = vis;
        }

        private void PanelVisibility(bool vis)
        {
            if (vis || !this.mKeepPanelOpen)
            {
                this.mLog.Visible = vis;
                foreach (Label l in this.mOldLabels)
                    l.Visible = vis;
            }
        }
    }
}
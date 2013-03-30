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
    public struct CommandInfo
    {
        public Action<string[]> Action { get; private set; }
        public string CommandName      { get; private set; }
        public int NbArgsMin           { get; private set; }
        public int NbArgsMax           { get; private set; }

        public CommandInfo(string name, int nbArgsMin, Action<string[]> action) : this(name, nbArgsMin, nbArgsMin, action) { }
        public CommandInfo(string name, int nbArgsMin, int nbArgsMax, Action<string[]> action) : this()
        {
            if (name[0] != '\\') { name.Insert(0, "\\"); }
            CommandName = name;
            Action = action;
            NbArgsMin = nbArgsMin;
            NbArgsMax = nbArgsMax;
        }
    }

    public class MyConsole : GUIFactory
    {
        private List<CommandInfo> mCommands;

        private bool mVisible, mEnabled, mVerr;
        private TextBox[] mTextBoxes;
        private MoisManager mInput;
        private Timer mTimer;
        private int mNbLines = 6;

        public Miyagi.UI.GUI GUI { get { return this.mGUI; } }
        private TextBox CurrentTextBox { get { return this.mTextBoxes[this.mTextBoxes.Length - 1]; } }
        public bool Visible
        {
            get { return this.mVisible; }
            set
            {
                this.mVisible = value;
                foreach (TextBox b in this.mTextBoxes) { b.Visible = this.mVisible; }
                if (!this.mVisible) { this.Enabled = false; }
            }
        }
        public bool Enabled
        {
            get { return this.mEnabled; }
            set
            {
                this.mEnabled = value;
                this.CurrentTextBox.Focused = this.mEnabled;
                if (this.mEnabled) { this.Visible = true; }
                else               { this.mTimer.Reset(); }
            }
        }
        public bool Verr
        {
            get { return this.mVerr; }
            private set
            {
                if (this.mVerr != value)
                {
                    this.mVerr = value;
                    if (this.mVerr) { this.Visible = true; }
                    else            { this.mTimer.Reset(); }
                }
            }
        }

        public MyConsole(StateManager stateMgr) : base(stateMgr, "Game GUI")
        {
            this.mInput = stateMgr.Input;
            this.mTimer = new Timer();
            this.mCommands = new List<CommandInfo>();
            this.mVisible = false;
            this.mEnabled = false;
            this.mVerr = false;
        }

        protected override void CreateGUI()
        {
            this.mTextBoxes = new TextBox[this.mNbLines];
            Size txtBoxSize = new Size((int)((float)this.mOriginalWndSize.Width / 2.5f), 32);
            Point consoleLoc = new Point(0, this.mOriginalWndSize.Height - txtBoxSize.Height * this.mNbLines);

            for (int i = 0; i < this.mNbLines; i++)
            {
                TextBox textBox = new TextBox();
                textBox.Size = txtBoxSize;
                textBox.TextStyle.ForegroundColour = new Colour(255, 255, 255, 255);
                textBox.TextStyle.Alignment = Alignment.MiddleLeft;
                textBox.TextStyle.Font = this.mMiyagiMgr.Fonts["Small_BlueHighway"];
                textBox.Location = consoleLoc + new Point(0, i * textBox.Size.Height);
                textBox.Skin = this.mMiyagiMgr.Skins["Console"];
                textBox.Visible = false;
                this.mGUI.Controls.Add(textBox);
                this.mTextBoxes[i] = textBox;
            }

            this.CurrentTextBox.Submit += new EventHandler<ValueEventArgs<string>>(this.Submit);
        }

        public void AddCommand(CommandInfo command) { this.mCommands.Add(command); }
        public void AddCommands(CommandInfo[] commands)
        {
            foreach(CommandInfo command in commands)
                this.mCommands.Add(command);
        }
        public void DeleteCommand(CommandInfo command) { this.mCommands.Remove(command); }
        public void DeleteCommands(CommandInfo[] commands)
        {
            foreach (CommandInfo command in commands)
                this.mCommands.Remove(command);
        }

        public void WriteLine(object o) { this.WriteLine(MyConsole.GetString(o)); }
        public void WriteLine(string txt)   // Won't execute a command if we use WriteLine in the code. Use ThrowCommand instead.
        {
            if (txt.Length > 0)
            {
                if (!this.mEnabled)
                {
                    this.Enabled = true;
                    this.mTimer.Reset();
                }

                int index = this.mTextBoxes.Length - 2; // The second last textBox
                while (index > 0 && this.mTextBoxes[index - 1].Text == "") // Find the index of the first non empty textBox
                    index--;
                if (this.mTextBoxes[index].Text != "")    // If all the textBoxes are used
                {
                    for (int i = 1; i <= index; i++)    // Move the texts one step up
                        this.mTextBoxes[i - 1].Text = this.mTextBoxes[i].Text;
                }
                this.mTextBoxes[index].Text = txt;
            }
        }

        public bool ThrowCommand(string command)    // Return whether a command has been thrown. command must begin by '\\'
        {
            if (command[0] == '\\')
            {
                command = command.Remove(0, 1); // Remove the '\\'
                /* Determines the first word entered */
                string commandNameEntered = "";
                foreach (char c in command)
                {
                    if (c == ' ')
                        break;
                    else
                        commandNameEntered += c;
                }

                /* Throw the delegate of the wanted command */
                for (int i = 0; i < mCommands.Count; i++)
                {
                    if (commandNameEntered == mCommands[i].CommandName)
                    {
                        string[] args;
                        if (!GetArgs(command, mCommands[i], out args))
                            return false;

                        mCommands[i].Action(args);
                        return true;
                    }
                }

                this.WriteLine("Unknown command.");
            }
            return false;
        }

        private bool GetArgs(string command, CommandInfo info, out string[] args)
        {
            List<string> readArgs = new List<string>();
            args = null;

            bool isCommandName = true;  // bool to know that the first word is the command name but not an arg
            bool isQuoted = false;  // If this args is quoted, it avoids the ' ' in an arg issue
            string arg = "";
            for (int i = 0; i < command.Length; i++)
            {
                char c = command[i];
                if (c == ' ' && isCommandName)
                {
                    isCommandName = false;
                    continue;
                }

                if (!isCommandName)
                {
                    if (c == '"')
                    {
                        isQuoted = !isQuoted;
                        continue;
                    }

                    if (isQuoted || c != ' ')
                        arg += c;
                    else
                    {
                        readArgs.Add(arg);
                        arg = "";
                    }
                }
            }

            if (arg != "")
                readArgs.Add(arg);

            if (readArgs.Count < info.NbArgsMin)
                this.WriteLine("Not enough arguments given. At least " + info.NbArgsMin + " are expected.");
            else if (readArgs.Count > info.NbArgsMax)
                this.WriteLine("Too many arguments given. A maximum of " + info.NbArgsMax + " are expected");
            else
            {
                args = readArgs.ToArray();
                return true;
            }

            return false;
        }

        private void Submit(object sender, ValueEventArgs<string> e)
        {
            string txt = e.Data;
            this.CurrentTextBox.Text = "";
            if (txt.Length > 0)
            {
                this.WriteLine(txt);
                this.ThrowCommand(txt);
            }
        }

        public void Update()
        {
            if (this.mInput.WasKeyPressed(MOIS.KeyCode.KC_RETURN)) { this.Enabled = !this.Enabled; }
            if (this.mInput.WasKeyPressed(MOIS.KeyCode.KC_TAB) && !this.mInput.IsKeyDown(MOIS.KeyCode.KC_LMENU)) { this.Verr = !this.Verr; }

            if (!this.Enabled && !this.Verr && ((float)this.mTimer.Milliseconds) / 1000f > 3)
                this.Visible = false;
        }

        public void UpdateZOder(int zOrder) { this.mGUI.ZOrder = zOrder; }

        public static string GetString(object o)
        {
            if (o.GetType() == typeof(Mogre.Vector3))
            {
                Mogre.Vector3 v = (Mogre.Vector3)o;
                return "(" + v.x + "; " + v.y + "; " + v.z + ")";
            }
            else { return o.ToString(); }
        }
    }
}
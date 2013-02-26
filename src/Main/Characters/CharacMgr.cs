using System;
using System.Collections.Generic;
using Mogre;

using Game.World;
using Game.States;
using Game.IGConsole;

namespace Game.CharacSystem
{
    public class CharacMgr
    {
        private List<VanillaCharacter> mCharacList;
        private MainPlayerCamera       mMainPlayerCam;
        private StateManager           mStateMgr;
        private MoisManager            mInput;
        private MainWorld              mWorld;
        private string                 mMeshName = "Sinbad.mesh";

        public StateManager     StateMgr      { get { return this.mStateMgr; } }
        public SceneManager     SceneMgr      { get { return this.mStateMgr.SceneMgr; } }
        public MoisManager      Input         { get { return this.mInput; } }
        public MainWorld        World         { get { return this.mWorld; } }
        public MainPlayerCamera MainPlayerCam { get { return this.mMainPlayerCam; } }

        public CharacMgr(StateManager stateMgr, MainWorld world)
        {
            this.mStateMgr = stateMgr;
            this.mInput = stateMgr.Input;
            this.mWorld = world;
            this.mCharacList = new List<VanillaCharacter>();

            this.mStateMgr.MyConsole.OnCommandEntered += new MyConsole.ConsoleEvent(this.OnCommandEntered);
        }

        public void AddCharacter(CharacterInfo info)
        {
            string type;

            if (this.mCharacList.Count == 0 || info.IsPlayer)
            {
                type = "Player";
                this.mCharacList.Add(new VanillaPlayer(this, this.mMeshName, info, this.mInput));
                if (this.mCharacList.Count == 1)
                {
                    this.mMainPlayerCam = new MainPlayerCamera(this.mStateMgr.Camera, (this.mCharacList[0] as VanillaPlayer), this.mStateMgr.Window.Width, this.mStateMgr.Window.Height);
                    (this.GetCharacter() as VanillaPlayer).AttachCamera(this.mMainPlayerCam);
                }
            }
            else
            {
                type = "NonPlayer";
                this.mCharacList.Add(new VanillaNonPlayer(this, this.mMeshName, info));
            }

            LogManager.Singleton.DefaultLog.LogMessage(type + " " + info.Name + " added");
        }

        public VanillaCharacter GetCharacter(int index = 0) { return this.mCharacList[index]; }    // By default return the main player
        public int getNumberOfCharacter() { return this.mCharacList.Count; }

        private void OnCommandEntered(string command)
        {
            string[] args = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (args.Length >= 2)
            {

                if (args[0] == "/get_charac_pos")
                {
                    int index;
                    if (int.TryParse(args[1], out index))
                    {
                        Vector3 tmp = this.GetCharacter(index).FeetPosition / MainWorld.CUBE_SIDE;
                        tmp.x = Mogre.Math.IFloor(tmp.x);
                        tmp.y = Mogre.Math.IFloor(tmp.y);
                        tmp.z = Mogre.Math.IFloor(tmp.z);

                        this.mStateMgr.WriteOnConsole("FeetPosition : " + '(' + tmp.x + ',' + tmp.y + ',' + tmp.z + ')');
                    }
                }
                else if (args[0] == "/get_charac_yaw")
                {
                    int index;
                    if (int.TryParse(args[1], out index))
                    {
                        this.mStateMgr.WriteOnConsole("Yaw : " + this.GetCharacter(index).Node.Orientation.Yaw.ValueAngleUnits);
                        this.mStateMgr.WriteOnConsole("W : " + this.GetCharacter(index).Node.Orientation.w);
                        this.mStateMgr.WriteOnConsole("X : " + this.GetCharacter(index).Node.Orientation.x);
                        this.mStateMgr.WriteOnConsole("Y : " + this.GetCharacter(index).Node.Orientation.y);
                        this.mStateMgr.WriteOnConsole("Z : " + this.GetCharacter(index).Node.Orientation.z);
                    }
                }
                else if (args[0] == "/flip")
                {
                    int index;
                    if (int.TryParse(args[1], out index) && index < 8)
                    {
                        SceneNode node = this.GetCharacter().Points[index];
                        node.FlipVisibility();
                        this.mStateMgr.WriteOnConsole("Flipped : " + MyConsole.GetString(node.Position));
                    }
                }
            }
        }



        public void Update(float frameTime)
        {
            foreach (VanillaCharacter charac in this.mCharacList)
                charac.Update(frameTime);

            this.mMainPlayerCam.Update();
        }

        public void Dispose()
        {
            this.mCharacList.Clear();
            this.mMainPlayerCam.Dispose();
        }
    }
}

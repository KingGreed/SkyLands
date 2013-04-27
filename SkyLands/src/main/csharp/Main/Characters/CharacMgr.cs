using System;
using System.Collections.Generic;
using Mogre;

using Game.World;
using Game.States;
using Game.IGConsole;
using Game.Shoot;

namespace Game.CharacSystem
{
    public class CharacMgr
    {
        private CommandInfo[]          mCommands;
        private List<VanillaCharacter> mCharacList;
        private MainPlayerCamera       mMainPlayerCam;
        private StateManager           mStateMgr;
        private MoisManager            mInput;
        private MainWorld              mWorld;
        private BulletManager          mBulletMgr;
        private string                 mMeshName = "Sinbad.mesh";

        public StateManager     StateMgr      { get { return this.mStateMgr; } }
        public SceneManager     SceneMgr      { get { return this.mStateMgr.SceneMgr; } }
        public MoisManager      Input         { get { return this.mInput; } }
        public MainWorld        World         { get { return this.mWorld; } }
        public MainPlayerCamera MainPlayerCam { get { return this.mMainPlayerCam; } }
        public BulletManager    BulletMgr     { get { return this.mBulletMgr; } }

        public CharacMgr(StateManager stateMgr, MainWorld world, BulletManager bulletMgr)
        {
            this.mStateMgr = stateMgr;
            this.mInput = stateMgr.Input;
            this.mWorld = world;
            this.mBulletMgr = bulletMgr;
            this.mCharacList = new List<VanillaCharacter>();

            this.mCommands = new CommandInfo[]
            {
                new CommandInfo("getCharacPos", 1, delegate(string[] args)
                    {
                        int index;
                        if (int.TryParse(args[0], out index))
                            this.mStateMgr.WriteOnConsole("FeetPosition : " + MyConsole.GetString(this.GetCharacterByListPos(index).FeetPosition));
                    }),
                new CommandInfo("getCharacYaw", 1, delegate(string[] args)
                    {
                        int index;
                        if (int.TryParse(args[0], out index))
                            this.mStateMgr.WriteOnConsole(("Yaw : " + this.GetCharacterByListPos(index).GetYaw().ValueAngleUnits));
                    })
            };
            this.mStateMgr.MyConsole.AddCommands(this.mCommands);
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
                    (this.GetCharacterByListPos() as VanillaPlayer).AttachCamera(this.mMainPlayerCam);
                }
            }
            else
            {
                type = "NonPlayer";
                this.mCharacList.Add(new VanillaNonPlayer(this, this.mMeshName, info));
            }

            LogManager.Singleton.DefaultLog.LogMessage(type + " " + info.Name + " added");
        }

        public void RemoveCharac(VanillaCharacter charac) { this.mCharacList.Remove(charac); }

        public VanillaCharacter GetCharacterByListPos(int index = 0) { return this.mCharacList[index]; }    // By default return the main player
        public VanillaCharacter GetCharacterById(int id = 0) { return this.mCharacList.Find(charac => charac.Info.Id == id); }    // By default return the main player
        public int getNumberOfCharacter() { return this.mCharacList.Count; }

        public void Update(float frameTime)
        {
            for (int i = 0; i < this.mCharacList.Count; i++)
                this.mCharacList[i].Update(frameTime);

            this.mMainPlayerCam.Update();
        }

        public void Dispose()
        {
            this.mCharacList.Clear();
            this.mCharacList = null;
            this.mStateMgr.MyConsole.DeleteCommands(this.mCommands);
            this.mMainPlayerCam.Dispose();
            this.mMainPlayerCam = null;
        }
    }
}

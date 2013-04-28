using System;
using System.Collections.Generic;
using Mogre;

using Game.World;
using Game.States;
using Game.IGConsole;
using Game.Shoot;
using Game.GUICreator;
using Game.RTS;

using API.Ent;

namespace Game.CharacSystem
{
    public class CharacMgr : CharacterMgr
    {
        private CommandInfo[]          mCommands;
        private List<VanillaCharacter> mCharacList;
        private MainPlayerCamera       mMainPlayerCam;
        private StateManager           mStateMgr;
        private MoisManager            mInput;
        private MainWorld              mWorld;
        private BulletManager          mBulletMgr;
        private HUD                    mHUD;
        private PlayerRTS              mPlayerRTS;
        private string                 mSinbadMesh = "Sinbad.mesh";
        private string                 mRobotMesh = "robot.mesh";

        public StateManager     StateMgr      { get { return this.mStateMgr; } }
        public SceneManager     SceneMgr      { get { return this.mStateMgr.SceneMgr; } }
        public MoisManager      Input         { get { return this.mInput; } }
        public MainWorld        World         { get { return this.mWorld; } }
        public MainPlayerCamera MainPlayerCam { get { return this.mMainPlayerCam; } }
        public BulletManager    BulletMgr     { get { return this.mBulletMgr; } }

        public CharacMgr(StateManager stateMgr, MainWorld world, BulletManager bulletMgr, HUD hud, PlayerRTS playerRTS)
        {
            this.mStateMgr = stateMgr;
            this.mInput = stateMgr.Input;
            this.mWorld = world;
            this.mBulletMgr = bulletMgr;
            this.mHUD = hud;
            this.mPlayerRTS = playerRTS;
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

            if (info.IsPlayer)
            {
                type = "Player";
                VanillaPlayer player = new VanillaPlayer(this, this.mSinbadMesh, info, this.mInput);
                this.mCharacList.Add(player);
                if (this.GetMainPlayer() == null)
                {
                    this.mMainPlayerCam = new MainPlayerCamera(this.mStateMgr.Camera, player, this.mStateMgr.Window.Width, this.mStateMgr.Window.Height);
                    player.MainPlayer(this.mMainPlayerCam, this.mHUD);
                }
            }
            else
            {
                type = "NonPlayer";
                this.mCharacList.Add(new VanillaNonPlayer(this, this.mRobotMesh, info));
            }

            LogManager.Singleton.DefaultLog.LogMessage(type + " " + info.Name + " added");
        }

        public void RemoveCharac(VanillaCharacter charac)
        {
            this.mCharacList.Remove(charac);
            charac.Dispose();
        }

        public VanillaCharacter GetCharacterByListPos(int index) { return this.mCharacList[index]; }
        public VanillaCharacter GetCharacterById(int id)             { return this.mCharacList.Find(charac => charac.Info.Id == id); }
        public VanillaPlayer GetMainPlayer() { return (VanillaPlayer)this.mCharacList.Find(c => c.Info.IsPlayer == true && (c as VanillaPlayer).IsMainPlayer); }

        public int GetNumberOfCharacter() { return this.mCharacList.Count; }

        public void Update(float frameTime)
        {
            for (int i = 0; i < this.mCharacList.Count; i++)
                this.mCharacList[i].Update(frameTime);

            this.mMainPlayerCam.Update();
        }

        public void Dispose()
        {
            while (this.mCharacList.Count > 0)
            {
                this.mCharacList[0].Dispose();
                this.mCharacList.RemoveAt(0);
            }

            CharacterInfo.ResetID();
            this.mStateMgr.MyConsole.DeleteCommands(this.mCommands);
            this.mMainPlayerCam.Dispose();
            this.mMainPlayerCam = null;
        }
    }
}

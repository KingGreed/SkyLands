using System;
using System.Collections.Generic;
using Mogre;

using Game.World;
using Game.States;
using Game.IGConsole;
using Game.Shoot;
using Game.GUICreator;

using API.Ent;

namespace Game.CharacSystem
{
    public class CharacMgr : CharacterMgr
    {
        private const string SINBAD_MESH = "Sinbad.mesh";
        private const string ROBOT_MESH = "robot.mesh";

        private readonly CommandInfo[]          mCommands;
        private readonly List<VanillaCharacter> mCharacList;
        private readonly StateManager           mStateMgr;
        private readonly MoisManager            mInput;
        private readonly MainWorld              mWorld;
        private readonly BulletManager          mBulletMgr;
        private readonly HUD                    mHUD;
        private readonly CameraMgr              mCameraMgr;

        public StateManager  StateMgr   { get { return this.mStateMgr; } }
        public SceneManager  SceneMgr   { get { return this.mStateMgr.SceneMgr; } }
        public MoisManager   Input      { get { return this.mInput; } }
        public MainWorld     World      { get { return this.mWorld; } }
        public VanillaPlayer MainPlayer { get; private set; }
        public BulletManager BulletMgr  { get { return this.mBulletMgr; } }

        public CharacMgr(StateManager stateMgr, MainWorld world, CameraMgr cameraMgr)
        {
            this.mStateMgr = stateMgr;
            this.mInput = stateMgr.Input;
            this.mWorld = world;
            this.mCameraMgr = cameraMgr;
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

        public CharacMgr(StateManager stateMgr, MainWorld world, BulletManager bulletMgr, CameraMgr cameraMgr) : this(stateMgr, world, cameraMgr)
        {
            this.mBulletMgr = bulletMgr;
            this.mHUD = (HUD)stateMgr.MainState.MainGUI;
        }

        public void AddCharacter(CharacterInfo info)
        {
            string type;

            if (info.IsPlayer)
            {
                if (this.mStateMgr.GameInfo.IsInEditorMode)
                    throw new Exception("Can't add player in the story editor");
                
                type = "Player";
                VanillaPlayer player = new VanillaPlayer(this, SINBAD_MESH, info, this.mInput);
                this.mCharacList.Add(player);
                if (this.MainPlayer == null)
                {
                    player.MakeHimMainPlayer(this.mCameraMgr, new MainPlayerCamera(this.mStateMgr.Camera, player), this.mHUD);
                    this.MainPlayer = player;
                }
            }
            else
            {
                type = "NonPlayer";
                this.mCharacList.Add(new VanillaNonPlayer(this, ROBOT_MESH, info));
            }

            LogManager.Singleton.DefaultLog.LogMessage(type + " " + info.Name + " added");
        }

        public VanillaCharacter GetCharacterByListPos(int index) { return this.mCharacList[index]; }
        public VanillaCharacter GetCharacterById(int id)             { return this.mCharacList.Find(charac => charac.Info.Id == id); }

        public int GetNumberOfCharacter() { return this.mCharacList.Count; }

        public void Update(float frameTime)
        {
            for (int i = 0; i < this.mCharacList.Count; i++)
            {
                if (this.mCharacList[i].WaitForRemove)
                {
                    this.mCharacList[i].Dispose();
                    this.mCharacList.Remove(this.mCharacList[i]);
                    continue;
                }
                this.mCharacList[i].Update(frameTime);
            }

            if (this.MainPlayer != null && this.mCameraMgr.IsAllowedToMoveCam) { this.MainPlayer.MainPlayerCam.Update(); }
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
        }
    }
}

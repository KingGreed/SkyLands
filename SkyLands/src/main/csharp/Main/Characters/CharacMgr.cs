using System;
using System.Collections.Generic;
using System.Linq;
using Mogre;

using Game.World;
using Game.States;
using Game.Input;
using Game.Shoot;

using API.Ent;

namespace Game.CharacSystem
{
    public enum Faction { Blue, Red }

    public class CharacMgr : CharacterMgr
    {
        private const string SINBAD_MESH = "Sinbad.mesh";
        private const string ROBOT_MESH = "robot.mesh";

        private readonly List<VanillaCharacter>[] mCharacters;
        private readonly StateManager             mStateMgr;
        private readonly Controller               mController;
        private readonly MainWorld                mWorld;
        private readonly BulletManager            mBulletMgr;
        private readonly User                     mCameraMgr;

        public StateManager  StateMgr   { get { return this.mStateMgr; } }
        public SceneManager  SceneMgr   { get { return this.mStateMgr.SceneMgr; } }
        public Controller    Controller { get { return this.mController; } }
        public MainWorld     World      { get { return this.mWorld; } }
        public VanillaPlayer MainPlayer { get; private set; }
        public BulletManager BulletMgr  { get { return this.mBulletMgr; } }

        public CharacMgr(StateManager stateMgr, MainWorld world, User cameraMgr, BulletManager bulletMgr = null)
        {
            this.mStateMgr = stateMgr;
            this.mController = stateMgr.Controller;
            this.mWorld = world;
            this.mCameraMgr = cameraMgr;
            this.mBulletMgr = bulletMgr;
            this.mCharacters = new List<VanillaCharacter>[Enum.GetValues(typeof(Faction)).Length];
            for(int i = 0; i < this.mCharacters.Length; i++)
                this.mCharacters[i] = new List<VanillaCharacter>();
        }

        public void AddCharacter(CharacterInfo info)
        {
            if (info.IsPlayer)
            {
                if (this.mStateMgr.GameInfo.IsInEditorMode)
                    throw new Exception("Can't add player in the story editor");
                
                VanillaPlayer player = new VanillaPlayer(this, SINBAD_MESH, info);
                this.mCharacters[(int)info.Faction].Add(player);
                if (this.MainPlayer == null)
                {
                    player.MakeHimMainPlayer(this.mCameraMgr, new MainPlayerCamera(this.mStateMgr.Camera, player));
                    this.MainPlayer = player;
                }
            }
            else { this.mCharacters[(int)info.Faction].Add(new VanillaNonPlayer(this, ROBOT_MESH, info)); }

            LogManager.Singleton.DefaultLog.LogMessage("Character " + info.Name + " added");
        }

        public VanillaCharacter GetCharacterById(int id)
        {
            return this.mCharacters.Select(characList => characList.Find(charac => charac.Info.Id == id)).FirstOrDefault(charac => charac != null);
        }

        public void Update(float frameTime)
        {
            foreach (List<VanillaCharacter> characList in mCharacters)
            {
                for (int i = 0; i < characList.Count; i++)
                {
                    if (characList[i].WaitForRemove)
                    {
                        characList[i].Dispose();
                        characList.Remove(characList[i]);
                        continue;
                    }
                    characList[i].Update(frameTime);
                }
            }

            if (this.MainPlayer != null && this.mCameraMgr.IsAllowedToMoveCam) { this.MainPlayer.MainPlayerCam.Update(); }
        }

        public void Dispose()
        {
            foreach (List<VanillaCharacter> characList in mCharacters)
            {
                while (characList.Count > 0)
                {
                    characList[0].Dispose();
                    characList.RemoveAt(0);
                }
            }

            CharacterInfo.ResetID();
        }
    }
}

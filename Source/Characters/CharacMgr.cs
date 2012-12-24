using System;
using System.Collections.Generic;
using Mogre;


namespace Game.CharacSystem
{
    public class CharacMgr
    {
        public static float YAW_SENSIVITY = 0.1f;   // Temp
        public static float PITCH_SENSIVITY = 0.15f;   // Temp
        
        private List<Character> mCharacList = new List<Character>();
        private MainPlayerCamera mMainPlayerCam;

        public MainPlayerCamera MainPlayerCam { get { return this.mMainPlayerCam; } }
        
        public CharacMgr(Camera cam)
        {
            this.mMainPlayerCam = new MainPlayerCamera(cam);
        }

        public void AddPlayer(SceneManager sceneMgr, string meshName, CharacterInfo info, MoisManager input)
        {
            this.mCharacList.Add(new Player(sceneMgr, meshName, info, input));
            if (this.mCharacList.Count == 1) { this.mMainPlayerCam.AttachToPlayer((Player)this.mCharacList[0]); }

            LogManager.Singleton.DefaultLog.LogMessage("Player added");
        }

        public Character GetCharacter(int index = 0)   // By default, return the main character which is the actual player
        {
            return this.mCharacList[index];
        }

        public void Update(float frameTime)
        {
            foreach (Character charac in this.mCharacList)
            {
                if (charac.GetType() == typeof(Player)) { ((Player)charac).Update(frameTime); }
                else                                    { ((NonPlayer)charac).Update(frameTime); }
            }

            this.mMainPlayerCam.Update();
        }

        /*private void FirstPersonUpdate(float yawValue, float pitchValue)
        {
            /* Pitch the camera 
            Radian newPitch = Mogre.Math.Abs(pitchValue + this.mMainPlayerCarac.CamPitchNode.Orientation.Pitch);
            if (newPitch < new Radian(Mogre.Math.PI / 2 - MainPlayerCamera.PITCH_OFFSET) || newPitch > new Radian(Mogre.Math.PI / 2 + MainPlayerCamera.PITCH_OFFSET))
                this.mMainPlayerCarac.CamPitchNode.Pitch(pitchValue);
        }

        private void ThirdPersonUpdate(float yawValue, float pitchValue) { }*/
    }
}

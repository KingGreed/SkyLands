using System;
using Mogre;


namespace Game.CharacSystem
{
    class Player : Character
    {
        public Player(Race race, CharacterInfo info) : base(race, info)
        {
        }

        public void Update(float frameTime, MoisManager input)
        {
            Vector3 direction = new Vector3();

            if (input.IsKeyDown(MOIS.KeyCode.KC_LCONTROL) || input.IsKeyDown(MOIS.KeyCode.KC_RCONTROL))
            {
                if (input.IsKeyDown(MOIS.KeyCode.KC_W) || input.IsKeyDown(MOIS.KeyCode.KC_UP))    { direction.z = 1; }
                if (input.IsKeyDown(MOIS.KeyCode.KC_S) || input.IsKeyDown(MOIS.KeyCode.KC_DOWN))  { direction.z = -1; }
                if (input.IsKeyDown(MOIS.KeyCode.KC_A) || input.IsKeyDown(MOIS.KeyCode.KC_LEFT))  { direction.x = 1; }
                if (input.IsKeyDown(MOIS.KeyCode.KC_D) || input.IsKeyDown(MOIS.KeyCode.KC_RIGHT)) { direction.x = -1; }
            }
            
            base.Move(direction);
            
            base.Update(frameTime);
        }

        private void UpdateCameraGoal(float deltaYaw, float deltaPitch, float deltaZoom)
        {

        }
    }
}

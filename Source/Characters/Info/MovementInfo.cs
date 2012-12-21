using System;
using Mogre;

namespace Game.CharacSystem
{
    public class MovementInfo
    {
        //public enum AnimEvent : byte {jump, land, dance}
        //private const int NUMBER_EVENT = 3;
        
        private Vector3 mMoveDirection;
        private float mYawValue;
        //private bool[] mEvents;

        public Vector3 MoveDirection { get { return this.mMoveDirection; } set { this.mMoveDirection = value; this.mMoveDirection.Normalise(); } }
        public float YawValue        { get { return this.mYawValue; }      set { this.mYawValue = value; } }
        //public bool[] Events         { get { return this.mEvents; } }

        public MovementInfo()
        {
            this.mMoveDirection = new Vector3();
            //this.mEvents = new bool[NUMBER_EVENT];

            this.ClearInfo();
        }

        public void ClearInfo()
        {
            this.mMoveDirection = Vector3.ZERO;
            /*for (int i = 0; i < this.mEvents.Length; i++)
                this.mEvents[i] = false;*/
            this.mYawValue = 0;
        }

        /*public void AddEvent(AnimEvent animEvent)
        {

        }*/
    }
}

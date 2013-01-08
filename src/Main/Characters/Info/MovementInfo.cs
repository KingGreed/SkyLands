using System;
using Mogre;

namespace Game.CharacSystem
{
    public class MovementInfo
    {
        private Vector3 mMoveDirection;
        private float mYawValue;
        private bool mIsMoving;

        public Vector3 MoveDirection { get { return this.mMoveDirection; } set { this.mMoveDirection = value; this.mMoveDirection.Normalise(); } }
        public float YawValue        { get { return this.mYawValue; }      set { this.mYawValue = value; } }
        public bool IsMoving         { get { return this.mIsMoving; }      set { this.mIsMoving = value; } }

        public MovementInfo()
        {
            this.mMoveDirection = new Vector3();
            this.ClearInfo();
        }

        public void ClearInfo()
        {
            this.mMoveDirection = Vector3.ZERO;
            this.mYawValue = 0;
        }
    }
}

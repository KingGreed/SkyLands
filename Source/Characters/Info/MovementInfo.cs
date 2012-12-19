using System;
using Mogre;

namespace Game.CharacSystem
{
    public struct MovementInfo
    {
        private Vector3 mMoveDirection;
        private float mYawValue;
        private bool mHasJumped;

        public Vector3 MoveDirection { get { return this.mMoveDirection; } set { this.mMoveDirection = value; this.mMoveDirection.Normalise(); } }
        public float YawValue        { get { return this.mYawValue; }      set { this.mYawValue = value; } }
        public bool HasJumped        { get { return this.mHasJumped; }     set { this.mHasJumped = value; } }

        public MovementInfo(Vector3 moveDirection, float yawValue, bool hasJumped)
        {
            this.mMoveDirection = moveDirection;
            this.mMoveDirection.Normalise();
            this.mYawValue = yawValue;
            this.mHasJumped = hasJumped;
        }
    }
}

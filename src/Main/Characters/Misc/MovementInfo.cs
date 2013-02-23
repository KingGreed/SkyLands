using System;
using Mogre;

namespace Game.CharacSystem
{
    public class MovementInfo
    {
        public delegate void MovementEvent(bool actualState);
        private MovementEvent mOnFall, mOnJump;
        private bool mIsAllowedToMove, mIsFalling, mIsJumping;
        private Vector3 mMoveDirection;
        private float mYawValue;

        public float   YawValue         { get { return this.mYawValue; } set { this.mYawValue = value; } }
        public bool    IsAllowedToMoved { get { return this.mIsAllowedToMove; } set { this.mIsAllowedToMove = value; } }
        public bool    IsFalling
        {
            get { return mIsFalling; }
            set 
            {
                if (this.mIsFalling != value)
                { this.mIsFalling = value; this.mOnFall(this.IsFalling); }
            }
        }
        public bool IsJumping
        { 
            get { return mIsJumping; }
            set
            {
                if ((!value || !this.IsFalling) && this.mIsJumping != value)
                {
                    this.mIsJumping = value;
                    this.mOnJump(this.IsJumping);
                }
            }
        }
        public Vector3 MoveDirection    // MoveDirection value must be set once per frame
        {
            get { return this.mMoveDirection; }
            set
            {
                this.mMoveDirection = value;
                if (this.mMoveDirection.y > 0 && !this.IsFalling) { this.IsJumping = true; }
            }
        }

        public MovementInfo(MovementEvent onFall, MovementEvent onJump)
        {
            this.mOnFall = onFall;
            this.mOnJump = onJump;
            this.mIsAllowedToMove = true;
            this.mIsFalling = false;
            this.mIsJumping = false;
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

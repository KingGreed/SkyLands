using Mogre;

namespace Game.CharacSystem
{
    public class MovementInfo
    {
        public delegate void MovementEvent(bool actualState);
        private readonly MovementEvent mOnFall, mOnJump;
        private bool mIsFalling, mIsJumping;
        private Vector3 mMoveDirection;

        public float YawValue                 { get; set; }
        public bool IsAllowedToMove           { get; set; }
        public bool IsMovementForced          { get; set; }
        public bool IsPushedByArcaneLevitator { get; set; }
        public bool Sprint                    { get; set; }

        public bool IsFalling
        {
            get { return this.mIsFalling; }
            set
            {
                if (this.mIsFalling != value && !this.IsPushedByArcaneLevitator)
                {
                    this.mIsFalling = value;
                    this.mOnFall(this.IsFalling);
                }
            }
        }
        public bool IsJumping
        {
            get { return this.mIsJumping; }
            set { if (this.mIsJumping != value && !this.IsPushedByArcaneLevitator) { this.mIsJumping = value; this.mOnJump(this.IsJumping); } }
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
            this.IsAllowedToMove = true;
            this.mIsFalling = false;
            this.mIsJumping = false;
            this.IsPushedByArcaneLevitator = false;
            this.IsMovementForced = false;
            this.mMoveDirection = new Vector3();
            this.ClearInfo();
        }

        public void ClearInfo()
        {
            this.mMoveDirection = Vector3.ZERO;
            this.YawValue = 0;
            this.Sprint = false;
        }
    }
}

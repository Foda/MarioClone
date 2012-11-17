using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    class NPC_Turtle : NPC_Base
    {
        //Animation
        const float aniRunCycle = 100;
        float aniTimer = 0;
        int aniCurrFrame = 0;

        private float speedMoveXShell = 3.0f;
        private float speedXDrag = 0.055f;

        //Unique shell vars
        public bool isShell = false;

        int spriteOffset = 0;
        private bool red = false;
        public bool isRed
        {
            get
            {
                return red;
            }
            set
            {
                if (value == true)
                    spriteOffset = 48;
                else
                    spriteOffset = 0;
                red = value;
            }
        }

        public bool isHeld = false;
        private bool isKicked = false;

        public override void HandleCollision(ConstHelper.CollisionDir cDir, String meta)
        {
            if (cDir == ConstHelper.CollisionDir.LEFT)
            {
                isMovingLeft = false;
                vVelocity.X *= -1;
                this.FlipHorz = true;

                AudioManager.PlaySfx("shell_rico");
            }
            else if (cDir == ConstHelper.CollisionDir.RIGHT)
            {
                isMovingLeft = true;
                vVelocity.X *= -1;
                this.FlipHorz = false;

                AudioManager.PlaySfx("shell_rico");
            }

            //I got jumped on :(
            if (cDir == ConstHelper.CollisionDir.UP && meta == "player" && isShell == false)
            {
                isShell = true;
                isDeadly = false;
                AudioManager.PlaySfx("stomp");
                this.Source = new Rectangle(0 + spriteOffset, 36, 16, 16);

                doesThrow = true;
                canThrow = true;
                throwableObject = ConstHelper.NPCType.LILTURTLE;
                ThrowObject(1);
            }
            else if (meta == "player" && isShell == true && isKicked == false && airState == ConstHelper.AirState.GROUND)
            {
                isKicked = true;
            }
            
            if (cDir == ConstHelper.CollisionDir.UP && meta == "player" && isShell == true && isKicked == true
                && airState == ConstHelper.AirState.GROUND)
            {
                isDeadly = false;
                isKicked = false;
                vVelocity.X = 0;
            }
        }

        public void KickShell(float direction)
        {
            if (isKicked == false)
            {
                if (direction != 0)
                {
                    if (direction > 0)
                        isMovingLeft = false;
                    else if(direction < 0)
                        isMovingLeft = true;
                    vVelocity.X = isMovingLeft ? -speedMoveX * 8 : speedMoveX * 8;
                }
                isKicked = true;
                Collide = true;
            }
        }

        public override void Update(GameTime time)
        {
            if (isShell == true && vVelocity.Y == 0 && vVelocity.X == 0 && isKicked == true)
            {
                isKicked = false;
                isDeadly = false;
            }

            if (isShell == false)
            {   
                Position.X += isMovingLeft ? -speedMoveX : speedMoveX;
                Position.Y += vVelocity.Y;

                aniTimer += time.ElapsedGameTime.Milliseconds;
                if (aniTimer > aniRunCycle)
                {
                    aniTimer = 0;
                    aniCurrFrame++;

                    if (aniCurrFrame > 1)
                        aniCurrFrame = 0;

                    if (aniCurrFrame == 0)
                        this.Source = new Rectangle(0 + spriteOffset, 0, 16, 26);
                    else if (aniCurrFrame == 1)
                        this.Source = new Rectangle(16 + spriteOffset, 0, 16, 26);
                }

                vVelocity.Y += speedGravity;
                vVelocity.Y = MathHelper.Clamp(vVelocity.Y, -speedFallMax, speedFallMax);
            }
            else if (isShell == true && isHeld == false)
            {
                Position.X += vVelocity.X;
                Position.Y += vVelocity.Y;

                if (vVelocity.X > 2.5f || vVelocity.X < -2.5f)
                {
                    isDeadly = true;

                    aniTimer += time.ElapsedGameTime.Milliseconds;
                    if (aniTimer > aniRunCycle / 2)
                    {
                        aniTimer = 0;
                        aniCurrFrame++;

                        if (aniCurrFrame > 2)
                            aniCurrFrame = 0;

                        if (aniCurrFrame == 0)
                            this.Source = new Rectangle(0 + spriteOffset, 36, 16, 16);
                        else if (aniCurrFrame == 1)
                            this.Source = new Rectangle(16 + spriteOffset, 36, 16, 16);
                        else if (aniCurrFrame == 2)
                            this.Source = new Rectangle(32 + spriteOffset, 36, 16, 16);
                    }
                }
                else
                {
                    if (airState == ConstHelper.AirState.AIR)
                        speedXDrag = 0.0055f;
                    else
                        speedXDrag = 0.055f;

                    if (vVelocity.X <= 0.1f && vVelocity.X > 0)
                        vVelocity.X = 0;
                    else if (vVelocity.X >= -0.1f && vVelocity.X < 0)
                        vVelocity.X = 0;

                    if (vVelocity.X > 0)
                        vVelocity.X -= speedXDrag;
                    else if (vVelocity.X < 0)
                        vVelocity.X += speedXDrag;
                }

                vVelocity.X = MathHelper.Clamp(vVelocity.X, -4, 4);
                vVelocity.Y += speedGravity;
                vVelocity.Y = MathHelper.Clamp(vVelocity.Y, -20, speedFallMax);
            }

            base.Update(time);
        }

        public override Rectangle GetRect()
        {
            return new Rectangle((int)Position.X + 4, (int)Position.Y, 12, 16);
        }
    }
}

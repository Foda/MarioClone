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
        public bool isRed = false;
        public bool isHeld = false;
        private bool isKicked = false;

        public override void HandleCollision(Phys_Const.CollisionDir cDir, String meta)
        {
            if (cDir == Phys_Const.CollisionDir.LEFT)
            {
                isMovingLeft = false;
                vVelocity.X *= -1;
                this.FlipHorz = true;

                AudioManager.PlaySfx("shell_rico");
            }
            else if (cDir == Phys_Const.CollisionDir.RIGHT)
            {
                isMovingLeft = true;
                vVelocity.X *= -1;
                this.FlipHorz = false;

                AudioManager.PlaySfx("shell_rico");
            }

            //I got jumped on :(
            if (cDir == Phys_Const.CollisionDir.UP && meta == "player" && isShell == false)
            {
                isShell = true;
                isDeadly = false;
                AudioManager.PlaySfx("stomp");
                this.Source = new Rectangle(0, 36, 16, 16);
            }
            else if (meta == "player" && isShell == true && isKicked == false && airState == Phys_Const.AirState.GROUND)
            {
                isKicked = true;
                isDeadly = true;
            }
            
            if (cDir == Phys_Const.CollisionDir.UP && meta == "player" && isShell == true && isKicked == true
                && airState == Phys_Const.AirState.GROUND)
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

            if (isShell == true && vVelocity.X != 0 && isKicked == true && airState == Phys_Const.AirState.GROUND
                && vVelocity.Y == 0)
            {
                isDeadly = true;
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
                        this.Source = new Rectangle(0, 0, 16, 26);
                    else if (aniCurrFrame == 1)
                        this.Source = new Rectangle(16, 0, 16, 26);
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
                    aniTimer += time.ElapsedGameTime.Milliseconds;
                    if (aniTimer > aniRunCycle / 2)
                    {
                        aniTimer = 0;
                        aniCurrFrame++;

                        if (aniCurrFrame > 2)
                            aniCurrFrame = 0;

                        if (aniCurrFrame == 0)
                            this.Source = new Rectangle(0, 36, 16, 16);
                        else if (aniCurrFrame == 1)
                            this.Source = new Rectangle(16, 36, 16, 16);
                        else if (aniCurrFrame == 2)
                            this.Source = new Rectangle(32, 36, 16, 16);
                    }
                }
                else
                {
                    if (airState == Phys_Const.AirState.AIR)
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
        }

        public override Rectangle GetRect()
        {
            return new Rectangle((int)Position.X + 4, (int)Position.Y, 12, 16);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    class NPC_LilTurtle: NPC_Base
    {
        //Animation
        const float aniRunCycle = 100;
        float aniTimer = 0;
        int aniCurrFrame = 0;

        private float speedXDrag = 0.055f;

        //Life after being squished in milliseconds
        const int timerDie = 350;
        int timerToRemove = 0;
        const int timerDeadly = 100;
        int slideTimerDeadly = 0;

        //Default to moving left
        public bool isSquished = false;
        bool isPlonked = false;
        bool isSliding = false;

        public override void HandleCollision(ConstHelper.CollisionDir cDir, String meta)
        {
            if (cDir == ConstHelper.CollisionDir.LEFT)
            {
                isMovingLeft = false;
                this.FlipHorz = true;
            }
            else if (cDir == ConstHelper.CollisionDir.RIGHT)
            {
                isMovingLeft = true;
                this.FlipHorz = false;
            }

            //I got jumped on :(
            if (cDir == ConstHelper.CollisionDir.UP && meta == "player")
            {
                isSquished = true;
                isDeadly = false;
                AudioManager.PlaySfx("stomp");
                this.Source = new Rectangle(64, 0, 16, 16);
            }

            //Plonk'd by a turtle shell
            if (meta == "turtle")
            {
                Collide = false;
                isDeadly = false;
                isPlonked = true;
                vVelocity.Y = -1.0f;
                vVelocity.X = -1.0f;
            }

            //Hop back into my shell!
            if (meta == "turtle_static")
            {
                Collide = false;
                isDeadly = false;
                isVisible = false;
                Remove = true;
            }
        }

        public void Eject(bool left)
        {
            this.Source = new Rectangle(32, 0, 16, 16);
            isSliding = true;
            isDeadly = false;
            vVelocity.X = -2;
        }

        public override void Update(GameTime time)
        {
            if (isSliding == true)
            {
                slideTimerDeadly += time.ElapsedGameTime.Milliseconds;
                if (slideTimerDeadly > timerDeadly)
                    isDeadly = true;

                aniTimer += time.ElapsedGameTime.Milliseconds;
                if (aniTimer > aniRunCycle)
                {
                    aniTimer = 0;
                    aniCurrFrame++;

                    if (aniCurrFrame > 1)
                        aniCurrFrame = 0;

                    if (aniCurrFrame == 0)
                        this.Source = new Rectangle(32, 0, 16, 16);
                    else if (aniCurrFrame == 1)
                        this.Source = new Rectangle(48, 0, 16, 16);
                }
            }
            if (isPlonked)
            {
                Position.X += vVelocity.X;
                Position.Y += vVelocity.Y;

                vVelocity.Y += speedGravity;
                vVelocity.Y = MathHelper.Clamp(vVelocity.Y, -speedFallMax, speedFallMax);

                timerToRemove += time.ElapsedGameTime.Milliseconds;
                if (timerToRemove > timerDie*2)
                    Remove = true;
            }
            else if (isSquished == false)
            {
                if (this.airState == ConstHelper.AirState.AIR || isSliding == true)
                {
                    Position.X += vVelocity.X;

                    if (vVelocity.X <= 0.1f && vVelocity.X > 0)
                    {
                        vVelocity.X = 0;
                        isSliding = false;
                    }
                    else if (vVelocity.X >= -0.1f && vVelocity.X < 0)
                    {
                        vVelocity.X = 0;
                        isSliding = false;
                    }

                    if (vVelocity.X > 0)
                        vVelocity.X -= speedXDrag;
                    else if (vVelocity.X < 0)
                        vVelocity.X += speedXDrag;
                }

                Position.X += isMovingLeft ? -speedMoveX : speedMoveX;
                Position.Y += vVelocity.Y;

                if (isSliding == false)
                {
                    aniTimer += time.ElapsedGameTime.Milliseconds;
                    if (aniTimer > aniRunCycle)
                    {
                        aniTimer = 0;
                        aniCurrFrame++;

                        if (aniCurrFrame > 1)
                            aniCurrFrame = 0;

                        if (aniCurrFrame == 0)
                            this.Source = new Rectangle(0, 0, 16, 16);
                        else if (aniCurrFrame == 1)
                            this.Source = new Rectangle(16, 0, 16, 16);
                    }
                }

                vVelocity.Y += speedGravity;
                vVelocity.Y = MathHelper.Clamp(vVelocity.Y, -speedFallMax, speedFallMax);
            }
            else
            {
                timerToRemove += time.ElapsedGameTime.Milliseconds;
                if (timerToRemove > timerDie)
                    Remove = true;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    class Player : Sprite
    {
        public enum MovementState
        {
            STANDING,
            WALKING,
            RUNNING,
            SPRINT,
            SKID,
            DUCK
        }

        const float GLOBAL_MOD = 1.25f;

        #region Movement vars
        //Flat ground, in pixels per frame (1/60)
        const float speedWalkMax = 1.5f * GLOBAL_MOD;
        const float speedRunMax = 2.5f * GLOBAL_MOD;
        const float speedSprintMax = 3.5f * GLOBAL_MOD;

        //subsubpixel, (1/16 of 1/16 of a pixel)
        //Also use this for deceleration too, and air movement!
        const float speedXAccel = 0.055f * GLOBAL_MOD;

        //Air speeds
        const float speedJumpStand = 3.4f * GLOBAL_MOD;
        const float speedJumpWalk = 3.6f * GLOBAL_MOD;
        const float speedJumpRun = 3.7f * GLOBAL_MOD;
        const float speedJumpSprint = 3.95f * GLOBAL_MOD;
        const float speedFallMax = 4.3f * GLOBAL_MOD;
        const float speedGravity = 0.3f * GLOBAL_MOD;
        const float speedGravityJumpHeld = 0.0625f * GLOBAL_MOD;
        const float speedGravityJump = 0.3125f * GLOBAL_MOD;
        #endregion

        #region Animation States
        const float aniRunCycle = 100;
        float aniTimer = 0;
        int aniCurrFrame = 0;
        bool isLeft = false;
        #endregion

        //Key states
        KeyboardState oldKey;
        private bool isJumpKeyDown = false;
        private bool isUpKeyDown = false;

        public Vector2 vPosition = new Vector2(0, 0);
        public Vector2 vVelocity = new Vector2(0, 0);
        private Vector2 vPosLast = new Vector2(0, 0);

        //Airmove states
        public MovementState moveState = MovementState.STANDING;
        public Phys_Const.AirState airState = Phys_Const.AirState.AIR;
        public bool isJumping = false;

        //Calculated camera pos for this player
        public Vector2 vCameraPos = new Vector2(0, 0);
        private float camXChange = 0;
        private bool hasChangedDir = false;

        //Turtle shell/held objects
        const int speedShellYKick = -8;
        public bool canGrab = false;
        public NPC_Turtle tHeld = null;

        //Hit fx
        Sprite sprHit = new Sprite();
        const int durHitLife = 5;
        int timerHitLife = 0;
        bool isHitTimer = false;

        //Big Mario
        public bool isBig = false;
        private bool isGrowing = false;
        private bool isShrinking = false;
        private bool isInvincible = false;
        private bool isRev = false;

        //Am I invincible?
        const int durInvTime = 750;
        float durBlink = 0;
        float timerInvLife = 0;

        public void LoadContent(ContentManager content)
        {
            sprHit.LoadContent(content, System.IO.Path.GetFullPath("Assets\\particle_hit.png"));
            sprHit.Size = new Rectangle(0, 0, 16, 16);
            sprHit.Source = new Rectangle(0, 0, 16, 16);

            this.Height = 16;
        }

        /// <summary>
        /// Update physics speeds
        /// </summary>
        public void Update(GameTime time, Camera cam)
        {
            if (isGrowing == false && isShrinking == false)
            {
                vPosLast = vPosition;

                //Clamp the X velocity
                if (moveState != MovementState.SPRINT)
                    vVelocity.X = MathHelper.Clamp(vVelocity.X, -speedRunMax, speedRunMax);
                else if (moveState == MovementState.SPRINT)
                    vVelocity.X = MathHelper.Clamp(vVelocity.X, -speedSprintMax, speedSprintMax);

                //Handle X + Y Movement
                HandleXMovement();
                HandleYMovement();

                //Update Position
                vPosition.X += vVelocity.X;
                vPosition.Y += vVelocity.Y;

                if (tHeld != null)
                {
                    if (isLeft)
                        tHeld.Position = new Vector2(vPosition.X - 10, Position.Y + Height - 16);
                    else
                        tHeld.Position = new Vector2(vPosition.X + 10, Position.Y + Height - 16);
                }

                //Update camera
                //Moving left, but camera thinks I'm still moving right
                if (isLeft == true && cam.isLeft == false)
                {
                    //Record distance change
                    camXChange += vPosLast.X - vPosition.X;
                    cam.XStick = true;
                    if (camXChange >= 16)
                    {
                        cam.isLeft = true;
                        camXChange = 0;
                        cam.XStick = false;
                    }
                }
                else if (isLeft == false && cam.isLeft == true)
                {
                    camXChange += vPosition.X - vPosLast.X;
                    cam.XStick = true;
                    if (camXChange >= 16)
                    {
                        cam.isLeft = false;
                        camXChange = 0;
                        cam.XStick = false;
                    }
                }
                else
                    cam.XStick = false;

                cam.plyPos = vPosition;

                //Do hit fx timer
                if (isHitTimer)
                {
                    timerHitLife++;
                    if (timerHitLife > durHitLife)
                        isHitTimer = false;
                }

                this.Position = vPosition;
            }

            //Handle animation
            UpdateAnimations(time);
        }
        private void UpdateAnimations(GameTime time)
        {
            if (isGrowing == true)
            {
                aniTimer += time.ElapsedGameTime.Milliseconds;
                if (aniTimer > 75)
                {
                    this.Source = new Rectangle(16 * aniCurrFrame, 48, 16, 32);
                    
                    aniCurrFrame++;
                    if (aniCurrFrame > 8)
                    {
                        aniCurrFrame = 0;
                        isGrowing = false;
                    }
                    aniTimer = 0;
                }
            }
            else if (isShrinking == true)
            {
                aniTimer += time.ElapsedGameTime.Milliseconds;
                if (aniTimer > 75)
                {
                    this.Source = new Rectangle(16 * aniCurrFrame, 48, 16, 32);

                    aniCurrFrame--;
                    if (aniCurrFrame < 0)
                    {
                        aniCurrFrame = 0;
                        isShrinking = false;
                    }
                    aniTimer = 0;
                }
            }
            else
            {
                if (isInvincible == true)
                {
                    timerInvLife += time.ElapsedGameTime.Milliseconds;
                    durBlink += time.ElapsedGameTime.Milliseconds;

                    if (durBlink > 100)
                        durBlink = 0;
                    else if (durBlink > 50)
                        isVisible = false;
                    else if (durBlink < 50)
                        isVisible = true;

                    if (timerInvLife > durInvTime)
                    {
                        isInvincible = false;
                        isVisible = true;
                    }
                }

                if (vVelocity.X == 0 && airState == Phys_Const.AirState.GROUND && isBig == true)
                {
                    this.Source = new Rectangle(0, 16, 16, 32);
                }

                if (vVelocity.X != 0 && airState == Phys_Const.AirState.GROUND)
                {
                    float aniSpeed = vVelocity.X;
                    if (vVelocity.X < 0)
                        aniSpeed *= -1;

                    aniTimer += time.ElapsedGameTime.Milliseconds;
                    if (aniTimer > MathHelper.Clamp((aniRunCycle / aniSpeed), 25, 100))
                    {
                        aniTimer = 0;
                        if (isBig == false)
                        {
                            aniCurrFrame++;

                            if (aniCurrFrame > 1)
                                aniCurrFrame = 0;

                            if (aniCurrFrame == 0)
                                this.Source = new Rectangle(0, 0, 16, 16);
                            else if (aniCurrFrame == 1)
                                this.Source = new Rectangle(16, 0, 16, 16);
                        }
                        else
                        {
                            aniCurrFrame++;
                            if (aniCurrFrame > 2)
                                aniCurrFrame = 0;

                            if (aniCurrFrame == 0)
                                this.Source = new Rectangle(0, 16, 16, 32);
                            else if (aniCurrFrame == 1)
                                this.Source = new Rectangle(16, 16, 16, 32);
                            else if (aniCurrFrame == 2)
                                this.Source = new Rectangle(32, 16, 16, 32);
                        }
                    }
                }

                if (vVelocity.X < 0 && moveState != MovementState.SKID)
                {
                    this.FlipHorz = true;
                }
                else if (vVelocity.X > 0 && moveState != MovementState.SKID)
                {
                    this.FlipHorz = false;
                }

                if (isLeft == false && airState == Phys_Const.AirState.AIR)
                {
                    this.FlipHorz = false;
                }
                else if (isLeft == true && airState == Phys_Const.AirState.AIR)
                {
                    this.FlipHorz = true;
                }

                if (vVelocity.X == 0 && moveState != MovementState.SKID)
                {
                    if (isBig == false)
                        this.Source = new Rectangle(0, 0, 16, 16);
                    else
                        this.Source = new Rectangle(0, 16, 16, 32);
                }
                if (moveState == MovementState.SKID && airState == Phys_Const.AirState.GROUND)
                {
                    if (isBig == false)
                        this.Source = new Rectangle(32, 0, 16, 16);
                    else
                        this.Source = new Rectangle(64, 16, 16, 32);
                }

                if (airState == Phys_Const.AirState.AIR)
                {
                    if (isBig == false)
                        this.Source = new Rectangle(48, 0, 16, 16);
                    else
                    {
                        if (isJumping == false && vVelocity.Y > 3)
                            this.Source = new Rectangle(96, 16, 16, 32);
                        else
                            this.Source = new Rectangle(48, 16, 16, 32);
                    }
                }

                if (moveState == MovementState.DUCK)
                {
                    if (isBig == false)
                        this.Source = new Rectangle(48, 0, 16, 16);
                    else
                        this.Source = new Rectangle(80, 16, 16, 32);
                }
            }
        }

        public void Grow()
        {
            if (isBig == false)
            {
                isBig = true;
                isGrowing = true;
                Height = 32;
                vPosition.Y -= 16;
                Position.Y -= 16;

                AudioManager.PlaySfx("powerup");
            }
        }
        public void Shrink()
        {
            if (isBig == true)
            {
                isBig = false;
                isShrinking = true;
                isInvincible = true;
                Height = 16;
                aniCurrFrame = 9;

                AudioManager.PlaySfx("powerdown");
            }
        }

        void HandleXMovement()
        {
            if (airState == Phys_Const.AirState.GROUND)
            {
                if (moveState == MovementState.STANDING && vVelocity.X != 0)
                {
                    if (vVelocity.X <= 0.1f && vVelocity.X > 0)
                        vVelocity.X = 0;
                    else if (vVelocity.X >= -0.1f && vVelocity.X < 0)
                        vVelocity.X = 0;

                    if (vVelocity.X > 0)
                        vVelocity.X -= speedXAccel;
                    else if (vVelocity.X < 0)
                        vVelocity.X += speedXAccel;
                }
                else if ((moveState == MovementState.SKID || moveState == MovementState.DUCK) 
                    && vVelocity.X != 0)
                {
                    if (vVelocity.X <= 0.1f && vVelocity.X > 0)
                        vVelocity.X = 0;
                    else if (vVelocity.X >= -0.1f && vVelocity.X < 0)
                        vVelocity.X = 0;

                    if (vVelocity.X > 0)
                        vVelocity.X -= speedXAccel * 4;
                    else if (vVelocity.X < 0)
                        vVelocity.X += speedXAccel * 4;
                }
            }
            else if (airState == Phys_Const.AirState.AIR)
            {
                if (moveState == MovementState.SKID && vVelocity.X != 0)
                {
                    if (vVelocity.X <= 0.1f && vVelocity.X > 0)
                        vVelocity.X = 0;
                    else if (vVelocity.X >= -0.1f && vVelocity.X < 0)
                        vVelocity.X = 0;

                    if (vVelocity.X > 0)
                        vVelocity.X -= speedXAccel * 4;
                    else if (vVelocity.X < 0)
                        vVelocity.X += speedXAccel * 4;
                }

                //In air, but I'm going slower than my walk speed
                if (moveState == MovementState.WALKING &&
                    ((vVelocity.X <= speedWalkMax) && (vVelocity.X >= -speedWalkMax)))
                {
                    vVelocity.X = MathHelper.Clamp(vVelocity.X, -speedWalkMax, speedWalkMax);
                }
                else if (moveState == MovementState.RUNNING &&
                    ((vVelocity.X <= speedRunMax) && (vVelocity.X >= -speedRunMax)))
                {
                    vVelocity.X = MathHelper.Clamp(vVelocity.X, -speedRunMax, speedRunMax);
                }
            }
        }
        void HandleYMovement()
        {
            //Jumping, handle gravity oddly
            if (isJumping && vVelocity.Y <= -2 && isJumpKeyDown)
                vVelocity.Y += speedGravityJumpHeld;
            else if (isJumping && vVelocity.Y > -2)
                vVelocity.Y += speedGravityJump;
            else if (isJumpKeyDown == false || isJumping == false)
                vVelocity.Y += speedGravity;

            if (vVelocity.Y > 0)
                isJumping = false;

            vVelocity.Y = MathHelper.Clamp(vVelocity.Y, -speedFallMax, speedFallMax);
        }

        private void DoHitFX()
        {
            sprHit.Position = new Vector2(vPosition.X, vPosition.Y + Height/2);
            isHitTimer = true;
            timerHitLife = 0;
        }

        public void DoJump(bool doHitFx = false)
        {
            if (vVelocity.X <= 1 && vVelocity.X >= -1)
                vVelocity.Y = -speedJumpStand;
            else if ((vVelocity.X > 1 && vVelocity.X <= 2) || (vVelocity.X < -1 && vVelocity.X >= -2))
                vVelocity.Y = -speedJumpWalk;
            else if ((vVelocity.X > 2 && vVelocity.X <= 3) || (vVelocity.X < -2 && vVelocity.X > -3))
                vVelocity.Y = -speedJumpRun;
            else if ((vVelocity.X >= 3) || (vVelocity.X <= -3))
                vVelocity.Y = -speedJumpSprint;

            if (doHitFx)
            {
                DoHitFX();
                AudioManager.PlaySfx("shell_stop");
            }

            isJumping = true;
        }
        private void CheckJump()
        {
            if (airState == Phys_Const.AirState.GROUND)
            {
                DoJump();
                AudioManager.PlaySfx("jump");
            }
        }

        //Add a grabbed shell
        public void GrabShell(NPC_Turtle shell)
        {
            tHeld = shell;
            tHeld.Collide = false;
        }
        //Kick a shell if I got one
        public void KickShell()
        {
            if (tHeld != null)
            {
                if (isUpKeyDown == true)
                {
                    tHeld.Position.Y -= 8;
                    tHeld.KickShell(0);
                    if (isLeft)
                    {
                        tHeld.isMovingLeft = true;
                        tHeld.Position.X -= 8;
                    }
                    else
                    {
                        tHeld.isMovingLeft = false;
                        tHeld.Position.X += 8;
                    }

                    tHeld.vVelocity.X += MathHelper.Clamp(vVelocity.X, -2f, 2f);
                    tHeld.vVelocity.Y = speedShellYKick;
                }
                else
                {
                    tHeld.vVelocity.Y = -2;
                    if (isLeft)
                    {
                        tHeld.isMovingLeft = true;
                        tHeld.Position.X -= 16;
                        tHeld.KickShell(-1);
                    }
                    else
                    {
                        tHeld.isMovingLeft = false;
                        tHeld.Position.X += 16;
                        tHeld.KickShell(1);
                    }
                    tHeld.vVelocity.X += vVelocity.X;
                }

                DoHitFX();
                tHeld.isHeld = false;
                tHeld.Collide = true;
                tHeld = null;

                AudioManager.PlaySfx("kick");
            }
        }

        public void Input(KeyboardState keyboard)
        {
            if (isGrowing || isShrinking)
                return;

            if(keyboard.IsKeyDown(Keys.L))
                canGrab = true;
            else if(keyboard.IsKeyUp(Keys.L))
                canGrab = false;

            if (keyboard.IsKeyDown(Keys.S))
            {
                moveState = MovementState.DUCK;
            }
            else if (keyboard.IsKeyDown(Keys.D))
            {
                //Changing direction
                if (vVelocity.X < 0 && (moveState == MovementState.RUNNING || moveState == MovementState.SPRINT ||
                    moveState == MovementState.SKID))
                {
                    vVelocity.X += speedXAccel;
                    moveState = MovementState.SKID;
                }
                else
                {
                    vVelocity.X += speedXAccel;
                    moveState = MovementState.RUNNING;
                }

                if (keyboard.IsKeyDown(Keys.L) && moveState != MovementState.SKID)
                    moveState = MovementState.SPRINT;

                isLeft = false;
            }
            else if (keyboard.IsKeyDown(Keys.A))
            {
                if (vVelocity.X > 0 && (moveState == MovementState.RUNNING || moveState == MovementState.SPRINT ||
                    moveState == MovementState.SKID))
                {
                    vVelocity.X -= speedXAccel;
                    moveState = MovementState.SKID;
                }
                else
                {
                    vVelocity.X -= speedXAccel;
                    moveState = MovementState.RUNNING;
                }

                if (keyboard.IsKeyDown(Keys.L) && moveState != MovementState.SKID)
                    moveState = MovementState.SPRINT;

                isLeft = true;
            }
            else
                moveState = MovementState.STANDING;

            if (keyboard.IsKeyDown(Keys.P) && oldKey.IsKeyUp(Keys.P))
            {
                CheckJump();
                isJumpKeyDown = true;
            }

            if(keyboard.IsKeyDown(Keys.W))
                isUpKeyDown = true;
            else if(keyboard.IsKeyUp(Keys.W))
                isUpKeyDown = false;

            if (keyboard.IsKeyUp(Keys.P) && oldKey.IsKeyDown(Keys.P))
                isJumpKeyDown = false;

            if (keyboard.IsKeyUp(Keys.L) && oldKey.IsKeyDown(Keys.L))
                KickShell();

            oldKey = keyboard;
        }

        public override Rectangle GetRect()
        {
            if (isBig)
                return new Rectangle((int)Position.X, (int)Position.Y, 16, 30);
            else
                return base.GetRect();
        }

        public override void Draw(SpriteBatch theSpriteBatch)
        {
            if (isVisible == true)
            {
                if (FlipHorz == true)
                    theSpriteBatch.Draw(mSpriteTexture, new Vector2((int)Position.X, (int)Position.Y), Source,
                        color, 0.0f, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
                else
                    theSpriteBatch.Draw(mSpriteTexture, new Vector2((int)Position.X, (int)Position.Y), Source,
                    color, 0.0f, Vector2.Zero, 1, SpriteEffects.None, 0);
            }

            if (isHitTimer)
                sprHit.Draw(theSpriteBatch);
        }
    }
}

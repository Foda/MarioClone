using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SFML_Test
{
    class Item_Bubble : Item_Base
    {
        private Player respawnPlayer;
        float sinSpeed = 0.05f;
        float sinValue = 0;

        //Animation
        const float aniRunCycle = 150;
        float aniTimer = 0;
        int aniCurrFrame = 0;

        public Item_Bubble()
        {
            isVisible = false;
        }

        public void SetPlayer(Player ply)
        {
            respawnPlayer = ply;
            isVisible = true;
        }

        public void Pop()
        {
            if (respawnPlayer != null)
            {
                respawnPlayer.isInBubble = false;
                respawnPlayer.MakeInvinvible();
                respawnPlayer.vVelocity = Vector2.Zero;
                respawnPlayer.vPosition = Position;
                respawnPlayer = null;

                isVisible = false;
            }
        }

        public void Update(GameTime time)
        {
            if (isVisible)
            {
                //Animations
                aniTimer += time.ElapsedGameTime.Milliseconds;
                if (aniTimer > aniRunCycle)
                {
                    aniTimer = 0;
                    aniCurrFrame++;

                    if (aniCurrFrame > 2)
                        aniCurrFrame = 0;

                    this.Source = new Rectangle(32 * aniCurrFrame, 0, 32, 32);
                }

                sinValue += (float)time.ElapsedRealTime.Milliseconds / 50 * sinSpeed;

                //Position.X += speedMoveX;
                Position.Y = ((float)Math.Sin(sinValue) * 150) + 200;

                //Ply pos
                if (respawnPlayer != null)
                    respawnPlayer.Position = new Vector2(Position.X + 8, Position.Y + 8);
            }
        }
    }
}

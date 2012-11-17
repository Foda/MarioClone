using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    class Block : Sprite
    {
        public enum BlockType
        {
            BRICK, //Breakable bricks
            ITEM   //Item, such as mushroom, etc
        }

        //Position of this block in the 16x16 world grid
        public Vector2 tilePosition;

        const int moveTimer = 1;
        int moveOffset = 4; //When hit, how much to move upwords
        int movetime = 0;
        bool isMoveHit = false;

        //Contents of this block
        public List<Item_Base> itemContents = new List<Item_Base>();
        public BlockType blockType = BlockType.BRICK;
        private bool isHit = false;

        //Animation
        const float aniRunCycle = 135;
        float aniTimer = 0;
        int aniCurrFrame = 0;

        /// <summary>
        /// Get position
        /// </summary>
        /// <returns>Gets the position of this block in world cords</returns>
        public Vector2 GetWorldPosition()
        {
            return new Vector2(tilePosition.X * 16, tilePosition.Y * 16);
        }
        
        /// <summary>
        /// Gets the rectangle bounding area of this block
        /// </summary>
        /// <returns>The rectangle in world cords</returns>
        public Rectangle GetRect()
        {
            return new Rectangle((int)tilePosition.X * 16 - 1, (int)tilePosition.Y * 16 + 1, 18, 16);
        }

        public void AddItem(Item_Base item)
        {
            item.isVisible = false;
            item.parentBlock = this;
            item.Position = GetWorldPosition();

            itemContents.Add(item);
        }

        public void RemoveItem(Item_Base item)
        {
            item.Remove = true;
            itemContents.Remove(item);
        }

        /// <summary>
        /// Collision handling
        /// </summary>
        /// <param name="CollDir">Direction of the collision</param>
        /// <param name="meta">Extra data as to what hit me</param>
        public void HandleCollision(ConstHelper.CollisionDir CollDir, String meta)
        {
            //Player hit me from below
            if ((CollDir == ConstHelper.CollisionDir.DOWN && meta == "player") || meta == "shell")
            {
                if (blockType == BlockType.ITEM && isHit == false && itemContents != null)
                {
                    foreach (Item_Base item in itemContents)
                    {
                        item.DeployItem();
                        isHit = true;
                        item.isVisible = true;
                        Source = new Rectangle(16*4, 0, 16, 16);
                    }
                    Position.Y -= moveOffset;
                    isMoveHit = true;
                }
            }
        }

        public void Update(GameTime time)
        {
            foreach (Item_Base item in itemContents)
                item.Update();

            if (isMoveHit == true && moveOffset > 0)
            {
                movetime++;
                if (movetime > moveTimer)
                {
                    Position.Y += 1;
                    moveOffset--;
                    movetime = 0;
                }
            }

            //Not hit, update animations
            if (isHit == false)
            {
                aniTimer += time.ElapsedGameTime.Milliseconds;
                if (aniTimer > aniRunCycle)
                {
                    aniTimer = 0;
                    aniCurrFrame++;

                    if (aniCurrFrame > 3)
                        aniCurrFrame = 0;

                    this.Source = new Rectangle(16*aniCurrFrame, 0, 16, 16);
                }
            }
        }

        public override void Draw(SpriteBatch theSpriteBatch)
        {
            foreach (Item_Base item in itemContents)
            {
                item.Draw(theSpriteBatch);
            }

            if (FlipHorz == true)
                theSpriteBatch.Draw(mSpriteTexture, new Vector2((int)Position.X, (int)Position.Y), Source,
                    color, 0.0f, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
            else
                theSpriteBatch.Draw(mSpriteTexture, new Vector2((int)Position.X, (int)Position.Y), Source,
                color, 0.0f, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    struct Tile
    {
        public bool isSolid;
        public bool isTop;
        public bool isStone;
        public bool isSlope;
        public bool isLit;
        public bool isVisible;

        public int[,] slopePts;
    };

    class Level
    {
        public Tile[,] tileData = new Tile[200, 50];

        public Sprite blockSprite;
        public Sprite blockFrostingSprite;

        public Sprite litSprite;
        public Sprite stoneSprite;
        public Sprite slopeSprite;
        public Sprite mushSprite;
        public Sprite itemBlockSprite;

        Color blockColor;

        public List<Block> blockTest = new List<Block>();

        public NPC_Manager npcMan = new NPC_Manager();
        public Item_Manager itemMan = new Item_Manager();

        #region Slope test
        private int[,] slope = new int[,]{
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1},
                {0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1},
                {0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1},
                {0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1},
                {0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1},
                {0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1},
                {0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1},
                {0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1},
                {0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1},
                {0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1},
                {0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
            };
        #endregion

        public Level()
        {
            for (int i = 0; i < 200; i++)
            {
                for (int n = 0; n < 50; n++)
                {
                    tileData[i, n] = new Tile();
                    tileData[i, n].isSolid = false;

                    if (n > 25)
                    {
                        tileData[i, n].isSolid = true;
                        tileData[i, n].isVisible = true;
                    }

                    if (n == 26)
                    {
                        tileData[i, n].isTop = true;
                        tileData[i, n].isVisible = true;
                    }
                }
            }
            tileData[20, 25].isSolid = true;
            tileData[20, 25].isStone = true;
            tileData[20, 25].isVisible = true;
            tileData[20, 25].isVisible = true;

            tileData[51, 25].isSolid = true;
            tileData[51, 25].isVisible = true;

            tileData[40, 25].isSolid = true;
            tileData[51, 20].isSolid = true;
            tileData[52, 20].isSolid = true;
            tileData[53, 20].isSolid = true;

            tileData[40, 25].isStone = true;
            tileData[51, 25].isStone = true;
            tileData[51, 20].isStone = true;
            tileData[52, 20].isStone = true;
            tileData[53, 20].isStone = true;

            tileData[40, 25].isVisible = true;
            tileData[51, 20].isVisible = true;
            tileData[52, 20].isVisible = true;
            tileData[53, 20].isVisible = true;

            blockSprite = new Sprite();
            blockSprite.Source = new Rectangle(0, 0, 16, 16);
        }

        public void TestGround(ref Player ply)
        {
            for (int i = 0; i < 60; i++)
            {
                for (int n = 0; n < 50; n++)
                {
                    tileData[i, n].isLit = false;
                }
            }

            int y1 = (int)((ply.vPosition.Y + ply.Height) / 16);
            int y2 = (int)((ply.vPosition.Y + ply.Height) / 16);

            int x1 = (int)(ply.vPosition.X + 4) / 16;
            int x2 = (int)(ply.vPosition.X + 14) / 16;

            //For slopes
            int x3 = (int)(ply.vPosition.X + 8) / 16;

            if (tileData[x1, y1].isSolid == true && tileData[x1, y1].isSlope == false)
            {
                ply.airState = Phys_Const.AirState.GROUND;
                ply.isJumping = false;
            }
            else if (tileData[x2, y1].isSolid == true && tileData[x2, y1].isSlope == false)
            {
                ply.airState = Phys_Const.AirState.GROUND;
                ply.isJumping = false;
            }
            else
                ply.airState = Phys_Const.AirState.AIR;
            
            //DEBUG
            //tileData[x3, y2].isLit = true;

            if ((tileData[x3, y2].isSlope || tileData[x3, y2+1].isSlope) &&
                ply.isJumping == false)
            {
                int testY = 0;
                if (tileData[x3, y2].isSlope) 
                    testY = y2;
                else 
                    testY = y1;

                int testX = (((x3 * 16) + 16) - ((int)ply.vPosition.X + 8)) - 1;

                for (int i = 0; i < 16; i++)
                {
                    if (tileData[x3, testY].slopePts != null)
                    {
                        if (tileData[x3, testY].slopePts[testX, i] == 1)
                        {
                            ply.vVelocity.Y = 0;
                            ply.vPosition.Y = (testY * 16) - i;

                            ply.airState = Phys_Const.AirState.GROUND;
                            ply.isJumping = false;
                            break;
                        }
                    }
                }
            }
            else if (ply.airState == Phys_Const.AirState.GROUND)
            {
                if(ply.vVelocity.Y > 0)
                    ply.vVelocity.Y = 0;
                ply.vPosition.Y = (y1 * 16) - ply.Height;
            }
            else
                ply.airState = Phys_Const.AirState.AIR;

            //Test NPC collisions
            npcMan.DoCollisionPlayer(ply);

            //Test item collisions
            itemMan.DoCollisionPlayer(ply);
        }
        public void WallTest(ref Player ply)
        {
            if (ply.vVelocity.X == 0)
                return;

            int y1 = (int)((ply.vPosition.Y) / 16);
            int y2 = (int)((ply.vPosition.Y) / 16);

            //If we're big, also do a check in the middle
            int y3 = (int)((ply.vPosition.Y + ply.Height/2) / 16);
            
            int x1 = (int)(ply.vPosition.X)/ 16;
            int x2 = (int)(ply.vPosition.X + 16) / 16;

            int x3 = (int)(ply.vPosition.X + 8) / 16;

            if (tileData[x3, y1].isSlope == false)
            {
                if (ply.vVelocity.X > 0)
                {
                    if ((tileData[x2, y1].isSolid == true || tileData[x2, y2].isSolid == true ||
                         tileData[x2, y3].isSolid == true) &&
                         tileData[x2, y1].isSlope == false)
                    {
                        ply.vPosition.X = (x2 * 16) - 16;
                        ply.vVelocity.X = 0f;
                    }
                }
                else if (ply.vVelocity.X < 0)
                {
                    if ((tileData[x1, y1].isSolid == true || tileData[x1, y2].isSolid == true ||
                         tileData[x1, y3].isSolid == true) &&
                         tileData[x1, y1].isSlope == false)
                    {
                        ply.vPosition.X = (x1 * 16) + 16;
                        ply.vVelocity.X = 0f;
                    }
                }
            }
        }
        public void TestCeiling(ref Player ply)
        {
            if (ply.vVelocity.Y < 0)
            {
                int y1 = (int)((ply.vPosition.Y) / 16);

                int x1 = (int)(ply.vPosition.X) / 16;
                int x2 = (int)(ply.vPosition.X + 14) / 16;

                if (tileData[x1, y1].isSolid == true)
                {
                    ply.vVelocity.Y = 0;
                    ply.vPosition.Y = (y1 * 16) + 16;
                }
                else if (tileData[x2, y1].isSolid == true)
                {
                    ply.vVelocity.Y = 0;
                    ply.vPosition.Y = (y1 * 16) + 16;
                }
            }

            if (ply.GetRect().Intersects(blockTest[0].GetRect()) == true &&
                ply.vVelocity.Y < 0 &&
                ply.vPosition.Y > blockTest[0].GetWorldPosition().Y + 16)
            {
                blockTest[0].HandleCollision(Phys_Const.CollisionDir.DOWN, "player");
            }
        }

        public void AddTestItem()
        {
            blockTest.Add(new Block());
            blockTest[0].blockType = Block.BlockType.ITEM;
            blockTest[0].tilePosition = new Vector2(30, 20);
            blockTest[0].Position = blockTest[0].tilePosition * 16;
            blockTest[0].Source = new Rectangle(0, 0, 16, 16);
            blockTest[0].mSpriteTexture = itemBlockSprite.mSpriteTexture;

            for (int i = 0; i < 3; i++)
            {
                Item_Mushroom item = new Item_Mushroom();
                item.mSpriteTexture = mushSprite.mSpriteTexture;
                item.Source = new Rectangle(0, 0, 16, 16);

                blockTest[0].AddItem((Item_Base)item);
                itemMan.listItem.Add((Item_Base)item);
            }

            tileData[30, 20].isSolid = true;
            tileData[30, 20].isVisible = false;
        }

        public void Update(GameTime time)
        {
            npcMan.Update(time, tileData, blockTest);
            itemMan.Update(tileData);
            blockTest[0].Update();
        }

        public void Draw(SpriteBatch spritebatch)
        {
            for (int i = 0; i < 200; i++)
            {
                for (int n = 0; n < 30; n++)
                {
                    if (tileData[i, n].isVisible)
                    {
                        if (tileData[i, n].isLit == true)
                        {
                            litSprite.Position = new Vector2(i * 16, n * 16);
                            litSprite.Draw(spritebatch);
                        }
                        else
                        {
                            if (tileData[i, n].isStone == true)
                            {
                                stoneSprite.Position = new Vector2(i * 16, n * 16);
                                stoneSprite.Draw(spritebatch);
                            }
                            else if (tileData[i, n].isSlope == true)
                            {
                                //slopeSprite.Position = new Vector2(i * 16, n * 16);
                                //slopeSprite.Draw(spritebatch);
                            }
                            else
                            {
                                blockSprite.Position = new Vector2(i * 16, n * 16);

                                if (tileData[i, n].isSolid == true && tileData[i, n].isTop == true)
                                {
                                    blockSprite.Source = new Rectangle(16, 16, 16, 16);

                                    blockFrostingSprite.Position = blockSprite.Position;
                                    blockFrostingSprite.Source = new Rectangle(16, 0, 16, 16);
                                    
                                    blockSprite.Draw(spritebatch);
                                    blockFrostingSprite.Draw(spritebatch);
                                }
                                else if (tileData[i, n].isSolid == true)
                                {
                                    blockSprite.Source = new Rectangle(16, 16, 16, 16);
                                    blockSprite.Draw(spritebatch);
                                }
                            }
                        }
                    }
                }
            }

            npcMan.Draw(spritebatch);
            blockTest[0].Draw(spritebatch);
        }
    }
}

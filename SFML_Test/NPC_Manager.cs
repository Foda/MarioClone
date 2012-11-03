using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    class NPC_Manager
    {
        public List<NPC_Base> listNPC = new List<NPC_Base>();

        public void Update(GameTime time, Tile[,] tileData, List<Block> blocks)
        {
            for (int i = 0; i < listNPC.Count; i++)
            {
                NPC_Base npc = listNPC[i];
                npc.Update(time);

                if (npc.Collide == true)
                {
                    TestGroundNPC(npc, tileData);
                    WallTestNPC(npc, tileData);
                    TestCeilingNPC(npc, tileData, blocks);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (NPC_Base npc in listNPC)
                npc.Draw(spriteBatch);
        }

        public void TestGroundNPC(NPC_Base spr, Tile[,] tileData)
        {
            int y1 = (int)((spr.Position.Y + spr.Height) / 16);

            int x1 = (int)(spr.Position.X + 4) / 16;
            int x2 = (int)(spr.Position.X + 14) / 16;

            if (tileData[x1, y1].isSolid == true)
            {
                spr.airState = Phys_Const.AirState.GROUND;
            }
            else if (tileData[x2, y1].isSolid == true)
            {
                spr.airState = Phys_Const.AirState.GROUND;
            }
            else
                spr.airState = Phys_Const.AirState.AIR;

            if (spr.airState == Phys_Const.AirState.GROUND)
            {
                spr.Position.Y = (y1 * 16) - spr.Source.Height;
                spr.vVelocity.Y = 0;
            }
            else
                spr.airState = Phys_Const.AirState.AIR;
        }
        public void WallTestNPC(NPC_Base spr, Tile[,] tileData)
        {
            int y1 = (int)((spr.Position.Y + (spr.Source.Height - 16)) / 16);
            //int y2 = (int)(((spr.Position.Y + 16)) / 16);

            int x1 = (int)(spr.Position.X - 2) / 16;
            int x2 = (int)(spr.Position.X + 16) / 16;

            if (spr.isMovingLeft == false)
            {
                if (tileData[x2, y1].isSolid == true)
                {
                    spr.Position.X = (x2 * 16) - 16;
                    spr.HandleCollision(Phys_Const.CollisionDir.RIGHT, "wall");
                }
            }
            else
            {
                if (tileData[x1, y1].isSolid == true)
                {
                    spr.Position.X = (x1 * 16) + 16;
                    spr.HandleCollision(Phys_Const.CollisionDir.LEFT, "wall");
                }
            }
        }
        public void TestCeilingNPC(NPC_Base spr, Tile[,] tileData, List<Block> blocks)
        {
            if (spr.vVelocity.Y < 0)
            {
                int y1 = (int)((spr.Position.Y) / 16);

                int x1 = (int)(spr.Position.X + 4) / 16;
                int x2 = (int)(spr.Position.X + 14) / 16;

                if (tileData[x1, y1].isSolid == true)
                {
                    spr.vVelocity.Y = 0;
                    spr.Position.Y = (y1 * 16) + 16;
                }
                else if (tileData[x2, y1].isSolid == true)
                {
                    spr.vVelocity.Y = 0;
                    spr.Position.Y = (y1 * 16) + 16;
                }
            }

            foreach (NPC_Base npc in listNPC.ToArray())
            {
                if (npc is NPC_Turtle)
                {
                    NPC_Turtle turtle = (NPC_Turtle)npc;
                    foreach (Block blockTest in blocks)
                    {
                        if (turtle.GetRect().Intersects(blockTest.GetRect()) &&
                            turtle.isShell && turtle.isHeld == false)
                        {
                            blockTest.HandleCollision(Phys_Const.CollisionDir.DOWN, "shell");
                        }
                    }
                }
            }
        }

        public void DoCollisionPlayer(Player ply)
        {
            foreach (NPC_Base npc in listNPC.ToArray())
            {
                if(npc.GetRect().Intersects(ply.GetRect()) &&
                    (ply.airState == Phys_Const.AirState.GROUND ||
                    ply.vVelocity.Y < 0) &&
                    npc.isDeadly == true)
                {
                    ply.Shrink();
                }
                else
                {
                    if (npc is NPC_Goomba)
                    {
                        NPC_Goomba goomba = (NPC_Goomba)npc;
                        if ((ply.GetRect().Intersects(goomba.GetRect()) == true) &&
                            ply.vVelocity.Y > 0.05f &&
                            goomba.isSquished == false &&
                            (ply.vPosition.Y + ply.Height/2 <= goomba.Position.Y))
                        {
                            goomba.HandleCollision(Phys_Const.CollisionDir.UP, "player");
                            ply.DoJump();

                            //Do PFX
                            //Play SFX
                            //Add score
                            //Add multiplyer
                        }

                        if (goomba.Remove)
                            listNPC.Remove(goomba);
                    }

                    if (npc is NPC_Turtle)
                    {
                        NPC_Turtle turtle = (NPC_Turtle)npc;

                        if (turtle.Collide)
                        {
                            if (turtle.isShell && turtle.vVelocity.X != 0)
                            {
                                foreach (NPC_Base npcHit in listNPC.ToArray())
                                {
                                    if (npcHit.GetRect().Intersects(turtle.GetRect()) == true)
                                        npcHit.HandleCollision(Phys_Const.CollisionDir.DOWN, "turtle");
                                }
                            }

                            if ((ply.GetRect().Intersects(turtle.GetRect()) == true) &&
                                ply.vVelocity.Y > 0 && turtle.isShell == false &&
                                (ply.vPosition.Y + ply.Height/2 <= turtle.Position.Y))
                            {
                                turtle.HandleCollision(Phys_Const.CollisionDir.UP, "player");
                                ply.DoJump();
                            }
                            else if ((ply.GetRect().Intersects(turtle.GetRect()) == true) &&
                                turtle.isShell == true)
                            {
                                //Do this so that the shell stops if jumped on, otherwise kick that shit
                                if (ply.airState == Phys_Const.AirState.AIR &&
                                    (ply.vPosition.Y + 8 <= turtle.Position.Y))
                                {
                                    turtle.HandleCollision(Phys_Const.CollisionDir.UP, "player");
                                    ply.DoJump(true);
                                }
                                else
                                {
                                    if (ply.canGrab == true && (turtle.vVelocity.X < 2 && turtle.vVelocity.X > -2))
                                        ply.GrabShell(turtle);
                                    else if (ply.airState == Phys_Const.AirState.GROUND)
                                        turtle.KickShell(ply.vVelocity.X);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

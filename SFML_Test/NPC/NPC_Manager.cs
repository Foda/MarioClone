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
        public Level currentLevel;

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
                spr.airState = ConstHelper.AirState.GROUND;
            }
            else if (tileData[x2, y1].isSolid == true)
            {
                spr.airState = ConstHelper.AirState.GROUND;
            }
            else
            {
                if (spr is NPC_Turtle)
                {
                    NPC_Turtle turtle = (NPC_Turtle)spr;
                    if (turtle.isRed == true && turtle.isShell == false)
                    {
                        if (turtle.airState == ConstHelper.AirState.GROUND)
                        {
                            turtle.isMovingLeft = !turtle.isMovingLeft;
                            turtle.FlipHorz = !turtle.FlipHorz;
                        }
                    }
                    else
                        spr.airState = ConstHelper.AirState.AIR;
                }
                else
                    spr.airState = ConstHelper.AirState.AIR;
            }

            if (spr.airState == ConstHelper.AirState.GROUND)
            {
                spr.Position.Y = (y1 * 16) - spr.Source.Height;
                spr.vVelocity.Y = 0;
            }
            else
                spr.airState = ConstHelper.AirState.AIR;
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
                    spr.HandleCollision(ConstHelper.CollisionDir.RIGHT, "wall");
                }
            }
            else
            {
                if (tileData[x1, y1].isSolid == true)
                {
                    spr.Position.X = (x1 * 16) + 16;
                    spr.HandleCollision(ConstHelper.CollisionDir.LEFT, "wall");
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
                            blockTest.HandleCollision(ConstHelper.CollisionDir.DOWN, "shell");
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
                    (ply.airState == ConstHelper.AirState.GROUND ||
                    ply.vVelocity.Y < 0) &&
                    npc.isDeadly == true)
                {
                    if (ply.isBig)
                        ply.Shrink();
                    else
                    {
                        //Isszzz Ded D:
                        ply.isInBubble = true;
                        currentLevel.bubble.SetPlayer(ply);
                    }
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
                            goomba.HandleCollision(ConstHelper.CollisionDir.UP, "player");
                            ply.DoJump();

                            //Do PFX
                            //Play SFX
                            //Add score
                            //Add multiplyer
                        }

                        if (goomba.Remove)
                            listNPC.Remove(goomba);
                    }

                    if (npc is NPC_LilTurtle)
                    {
                        NPC_LilTurtle lt = (NPC_LilTurtle)npc;
                        if ((ply.GetRect().Intersects(lt.GetRect()) == true) &&
                            ply.vVelocity.Y > 0.05f &&
                            lt.isSquished == false &&
                            (ply.vPosition.Y + ply.Height / 2 <= lt.Position.Y))
                        {
                            lt.HandleCollision(ConstHelper.CollisionDir.UP, "player");
                            ply.DoJump();
                        }

                        if (lt.Remove)
                            listNPC.Remove(lt);
                    }

                    if (npc is NPC_Turtle)
                    {
                        NPC_Turtle turtle = (NPC_Turtle)npc;

                        if (turtle.Collide)
                        {
                            foreach (NPC_Base npcHit in listNPC.ToArray())
                            {
                                if (turtle.isShell && turtle.vVelocity.X != 0)
                                {
                                    if (npcHit.GetRect().Intersects(turtle.GetRect()) == true)
                                        npcHit.HandleCollision(ConstHelper.CollisionDir.DOWN, "turtle");
                                }
                                else if (turtle.isShell)
                                {
                                    //Handle the lil'turtle jumping back into his shell
                                    if (npcHit.GetRect().Intersects(turtle.GetRect()) == true &&
                                        npcHit is NPC_LilTurtle &&
                                        npcHit.isDeadly)
                                    {
                                        npcHit.HandleCollision(ConstHelper.CollisionDir.DOWN, "turtle_static");
                                        turtle.isShell = false;
                                        turtle.isDeadly = true;
                                        turtle.vVelocity = Vector2.Zero;
                                        turtle.Height = 26;
                                    }
                                }
                            }

                            if ((ply.GetRect().Intersects(turtle.GetRect()) == true) &&
                                ply.vVelocity.Y > 0 && turtle.isShell == false &&
                                (ply.vPosition.Y + ply.Height/2 <= turtle.Position.Y))
                            {
                                turtle.HandleCollision(ConstHelper.CollisionDir.UP, "player");
                                ply.DoJump();
                            }
                            else if ((ply.GetRect().Intersects(turtle.GetRect()) == true) &&
                                turtle.isShell == true)
                            {
                                //Do this so that the shell stops if jumped on, otherwise kick that shit
                                if (ply.airState == ConstHelper.AirState.AIR &&
                                    (ply.vPosition.Y + 8 <= turtle.Position.Y))
                                {
                                    turtle.HandleCollision(ConstHelper.CollisionDir.UP, "player");
                                    ply.DoJump(true);
                                }
                                else
                                {
                                    if (ply.canGrab == true && (turtle.vVelocity.X < 2 && turtle.vVelocity.X > -2))
                                        ply.GrabShell(turtle);
                                    else if (ply.airState == ConstHelper.AirState.GROUND)
                                        turtle.KickShell(ply.vVelocity.X);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CreateNPC(ConstHelper.NPCType type, Vector2 position, Vector2 velocity)
        {
            if (type == ConstHelper.NPCType.GOOMBA)
            {
                NPC_Goomba g = new NPC_Goomba();
                g.Load(ConstHelper.content, "Assets\\goomba.png", 16, 16);
                g.Position = position;
                g.Height = 16;
                g.vVelocity = velocity;
                //g.requestCreateNPCEvent += g_RequestCreateNPCEvent;

                listNPC.Add(g);
            }
            else if (type == ConstHelper.NPCType.TURTLE)
            {
                NPC_Turtle t = new NPC_Turtle();
                t.Load(ConstHelper.content, "Assets\\turtle.png", 16, 26);
                t.Position = position;
                t.Height = 26;
                t.vVelocity = velocity;
                t.isRed = false;
                t.requestCreateNPCEvent += g_RequestCreateNPCEvent;

                listNPC.Add(t);
            }
            else if (type == ConstHelper.NPCType.LILTURTLE)
            {
                NPC_LilTurtle t = new NPC_LilTurtle();
                t.Load(ConstHelper.content, "Assets\\lilturtle.png", 16, 26);
                t.Position = position;
                t.Height = 16;
                t.Eject(true);

                listNPC.Add(t);
            }
        }

        private void g_RequestCreateNPCEvent(object sender, NPCCreateArgs e)
        {
            this.CreateNPC(e.NpcType, e.Position, e.Velocity);
        }
    }
}

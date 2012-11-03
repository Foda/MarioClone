using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    class Item_Manager
    {
        public List<Item_Base> listItem = new List<Item_Base>();

        public void Update(Tile[,] tileData)
        {
            for (int i = 0; i < listItem.Count; i++)
            {
                Item_Base item = listItem[i];
                item.Update();

                if (item.isWithinBlock == false)
                {
                    TestGroundItem(item, tileData);
                    WallTestItem(item, tileData);
                    TestCeilingItem(item, tileData);
                }
            }
        }

        //Item collision testing
        public void TestGroundItem(Item_Base spr, Tile[,] tileData)
        {
            int y1 = (int)((spr.Position.Y + spr.Height) / 16);

            int x1 = (int)(spr.Position.X) / 16;
            int x2 = (int)(spr.Position.X + 16) / 16;

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
        public void WallTestItem(Item_Base spr, Tile[,] tileData)
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
        public void TestCeilingItem(Item_Base spr, Tile[,] tileData)
        {
            if (spr.vVelocity.Y < 0)
            {
                int y1 = (int)((spr.Position.Y) / 16);

                int x1 = (int)(spr.Position.X) / 16;
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
        }

        public void DoCollisionPlayer(Player ply)
        {
            //Item collisions
            foreach (Item_Base item in listItem.ToArray())
            {
                if (item is Item_Mushroom)
                {
                    Item_Mushroom mushroom = (Item_Mushroom)item;
                    if (ply.GetRect().Intersects(mushroom.GetRect()) == true &&
                        mushroom.isWithinBlock == false)
                    {
                        item.parentBlock.RemoveItem(item);
                        listItem.Remove(item);
                        ply.Grow();
                    }
                }
            }
        }
    }
}

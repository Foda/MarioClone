using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    class Item_Mushroom : Item_Base
    {
        public override void HandleCollision(Phys_Const.CollisionDir CollDir, String meta)
        {
            if (CollDir == Phys_Const.CollisionDir.LEFT)
                isMovingLeft = false;
            else if (CollDir == Phys_Const.CollisionDir.RIGHT)
                isMovingLeft = true;
        }

        public override void Update()
        {
            if (isDeploying == false && isWithinBlock == false)
            {
               // if (airState == Phys_Const.AirState.AIR)
               //     Position.X += vVelocity.X;
               // else 
                    Position.X += isMovingLeft ? -speedMoveX : speedMoveX;

                Position.Y += vVelocity.Y;

                vVelocity.Y += speedGravity;
                vVelocity.Y = MathHelper.Clamp(vVelocity.Y, -speedFallMax*2, speedFallMax);
            }
            base.Update();
        }
    }
}

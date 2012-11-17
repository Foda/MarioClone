using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    class Item_Base : Sprite
    {
        const float speedDeployY = 0.25f;
        public float speedMoveX = 0.65f;
        public float speedFallMax = 2f;
        public const float speedGravity = 0.1f;

        public bool isDeploying = false;
        public bool isWithinBlock = true;
        public Block parentBlock = null;

        public Vector2 vVelocity;
        public bool isMovingLeft = false;
        public bool Remove = false;

        public bool isInstantDeploy = false;

        /// <summary>
        /// Collision handling for NPCs, etc
        /// </summary>
        /// <param name="CollDir">Direction of the collision</param>
        /// <param name="meta">Extra data as to what hit me</param>
        public virtual void HandleCollision(ConstHelper.CollisionDir CollDir, String meta)
        {
            return;
        }

        //Pop out the item
        public void DeployItem()
        {
            if (isInstantDeploy)
            {
                Position = parentBlock.GetWorldPosition();
                Position.Y = parentBlock.GetWorldPosition().Y - 17;

                if (ConstHelper.rand.Next(0, 100) < 50)
                {
                    vVelocity.X = (float)(ConstHelper.rand.Next(50, 100) / 100f * -1f);
                    isMovingLeft = true;
                }
                else
                {
                    vVelocity.X = (float)(ConstHelper.rand.Next(50, 100) / 100f);
                    isMovingLeft = false;
                }

                vVelocity.Y = (float)(ConstHelper.rand.Next(100, 350) / 100f) * -1f;
                
                isDeploying = false;
                isWithinBlock = false;
            }
            else
            {
                Position = parentBlock.GetWorldPosition();
                isDeploying = true;
            }

            //Play sounds, etc
            AudioManager.PlaySfx("deployitem");
        }

        public virtual void Update()
        {
            if (parentBlock != null && isWithinBlock == true)
            {
                if (isDeploying == true)
                {
                    Position.Y -= speedDeployY;
                    if (Position.Y + 18 < parentBlock.GetWorldPosition().Y)
                    {
                        isDeploying = false;
                        isWithinBlock = false;
                    }
                }
            }
        }
    }
}

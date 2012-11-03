using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    class NPC_Base : Sprite
    {
        public Phys_Const.AirState airState = Phys_Const.AirState.AIR;

        //Left/right movement speed
        public float speedMoveX = 0.35f;
        public float speedFallMax = 4.3f;
        public const float speedGravity = 0.3f;

        public bool isMovingLeft = true;
        public bool Remove = false;
        public bool Collide = true;
        public bool isDeadly = true;

        //Movement
        public Vector2 vVelocity = new Vector2(0, 0);

        public void Load(ContentManager contentManager, string asset, int width, int height)
        {
            this.LoadContent(contentManager, System.IO.Path.GetFullPath(asset));
            this.Source = new Rectangle(0, 0, width, height);
        }

        /// <summary>
        /// Collision handling for NPCs, etc
        /// </summary>
        /// <param name="CollDir">Direction of the collision</param>
        /// <param name="meta">Extra data as to what hit me</param>
        public virtual void HandleCollision(Phys_Const.CollisionDir CollDir, String meta)
        {
            return;
        }

        public virtual void Update(GameTime time)
        {
            return;
        }
    }
}

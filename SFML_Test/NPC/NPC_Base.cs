using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    delegate void RequestCreateNPCEventHandler(object sender, NPCCreateArgs a);
    class NPCCreateArgs : EventArgs
    {
        public NPCCreateArgs(ConstHelper.NPCType type, Vector2 position, Vector2 velocity)
        {
            npctype = type;
            pos = position;
            vel = velocity;
        }

        private ConstHelper.NPCType npctype;
        private Vector2 pos;
        private Vector2 vel;

        public ConstHelper.NPCType NpcType
        {
            get { return npctype; }
        }

        public Vector2 Position
        {
            get { return pos; }
        }

        public Vector2 Velocity
        {
            get { return vel; }
        }
    }

    class NPC_Base : Sprite
    {
        public ConstHelper.AirState airState = ConstHelper.AirState.AIR;

        //Left/right movement speed
        public float speedMoveX = 0.35f;
        public float speedFallMax = 4.3f;
        public const float speedGravity = 0.3f;

        public bool isMovingLeft = true;
        public bool Remove = false;
        public bool Collide = true;
        public bool isDeadly = true;

        public float throwTimer = 0;
        public float throwDelay = 100f;
        public bool doesThrow = false;
        public ConstHelper.NPCType throwableObject;
        public Vector2 vThrowVelocity;
        public bool canThrow = false;

        public event RequestCreateNPCEventHandler requestCreateNPCEvent;

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
        public virtual void HandleCollision(ConstHelper.CollisionDir CollDir, String meta)
        {
            return;
        }

        public virtual void ThrowObject(int count)
        {
            if (throwableObject != null && doesThrow && canThrow)
            {
                canThrow = false;
                requestCreateNPCEvent.Invoke(this, new NPCCreateArgs(throwableObject, this.Position, vThrowVelocity));
            }
        }

        public virtual void Update(GameTime time)
        {
            if (throwableObject != null && doesThrow)
            {
                throwTimer += time.ElapsedGameTime.Milliseconds;
                if (throwTimer > throwDelay)
                {
                    canThrow = true;
                    throwTimer = 0;
                }
            }

            return;
        }
    }
}

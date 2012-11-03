using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    class Item_Coin : Item_Base
    {
        public override void HandleCollision(Phys_Const.CollisionDir CollDir, String meta)
        {
            //Remove!
            Remove = true;
        }

        public override void Update()
        {
            //Animation
            return;
        }
    }
}

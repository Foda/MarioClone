using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    static class ConstHelper
    {
        public enum AirState
        {
            GROUND,
            AIR
        }
        public enum CollisionDir
        {
            LEFT,
            RIGHT,
            UP,
            DOWN
        }

        public static Random rand = new Random();

        public static ContentManager content;

        public enum NPCType
        {
            GOOMBA,
            TURTLE,
            LILTURTLE
        };
    }
}

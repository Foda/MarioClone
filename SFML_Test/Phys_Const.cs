using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    static class Phys_Const
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
    }
}

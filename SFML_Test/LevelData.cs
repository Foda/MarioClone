using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    struct TileHeader
    {
        enum TileType
        {
            SOLID_EDGE,
            SOLID_GROUND,
            SOLID_FILLER,

            BLOCK_INVIS_COIN,
            BLOCK_COIN,
            BLOCK_MUSHROOM,
            BLOCK_FIRE,
            BLOCK_ICE,
            BLOCK_UP,

            DOOR,
            
            BULLET_SHOOTER,
            COIN
        }
    };


    class LevelData
    {
        int levelLength = 4; //x640
        int levelVert = 2;   //x480
        string levelMusic = "";

        private TileHeader[,] tileData;

        public void Init()
        {
            tileData = new TileHeader[(levelLength * 640) / 16, (levelVert * 640) / 16];
        }
    }
}

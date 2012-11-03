using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFML_Test
{
    class Program
    {
        private static MainWindow game;

        static void Main()
        {
            game = new MainWindow();
            game.Run();
        }
    }
}

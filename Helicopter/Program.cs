using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helicopter
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game(800, 600, "Helicopter")) {
                game.Run(60.0);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cw2
{
    class Asteroid : cw2.Entity
    {
        public Asteroid(int X, int Y, string boardString = "A") : base(X, Y, boardString)
        {

        }

        override public bool onContact(Board board)
        {
            return false;
        }
    }
}

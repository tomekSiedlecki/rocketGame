using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cw2
{
    class Entity
    {
        public int X;
        public int Y;
        public string boardString;

        public Entity(int X, int Y, string boardString)
        {
            this.X = X;
            this.Y = Y;
            this.boardString = boardString;
        }

        public virtual bool onContact(Board board)
        {
            return true;
        }
    }
}

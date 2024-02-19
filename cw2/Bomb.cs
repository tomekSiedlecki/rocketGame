using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cw2
{
    class Bomb : Entity
    { 
        public Bomb(int X, int Y, string boardString = "X") : base(X, Y, boardString)
        {   

        }

        override public bool onContact(Board board)
        {
            board.RemoveCloseObjects(board.rocketX,board.rocketY);
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace HuntTheWumpus2D
{
    class Wumpus
    {
        private int position;
        private Map map;
        private Random ran = new Random();

        //Gathers map information and its position.
        public void setMap(Map m, int pos)
        {
            map = m;
            position = pos;
        }


        //Moves the wumpus
        public void movePosition(int pos)
        {
            position = pos;
        }
    }
}

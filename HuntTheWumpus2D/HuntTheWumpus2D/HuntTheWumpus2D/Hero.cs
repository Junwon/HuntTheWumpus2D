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
    class Hero
    {
        private int arrows = 5, position;
        private Boolean shot_wumpus = false, shot_self = false;
        private int[] room_list;
        private Map map;
        private Random ran = new Random();

        //Method to shoot arrows.
        public string shoot(int[] shots)
        {
            string message = "You didn't hit anything...";
            room_list = shots;

            //See if player shot the wumpus or shot self.
            for (int k = 0; k < room_list.Length-1; k++)
            {
                if (map.getRoom(room_list[k]).hasWumpus())
                {
                    message = "You got the wumpus! Yay.\nYou won!";
                    shot_wumpus = true;
                    break;
                }
            }
            arrows--;

            return message;
        }

        //Returns true if player has shot wumpus
        public Boolean shotWumpus()
        {
            return shot_wumpus;
        }

        //Returns true if player has shot self
        public Boolean shotSelf()
        {
            return shot_self;
        }

        //Checks if player has any arrows left.
        public Boolean haveArrows()
        {
            if (arrows <= 0)
            {
                return false;
            }

            return true;
        }

        //Gathers map information and its position.
        public void setMap(Map m, int pos)
        {
            map = m;
            position = pos;
        }

        //Moves position of hero
        public void movePosition(int pos)
        {
            position = pos;
        }
    }
}

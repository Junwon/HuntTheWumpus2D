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
    class Room
    {
        private bool has_wumpus = false, has_bats = false, has_pit = false;
        private int room_number;
        private int[] neighbor_number = new int[3];
        private Vector2 vector;

        //Sets the room number for Room object
        public void setRoomNumber(int num)
        {
            room_number = num;
        }

        //Gets the room number of Room object
        public int getRoomNumber()
        {
            return room_number;
        }

        //Sets the neighboring room
        public void setNeighbors(int a, int b, int c)
        {
            neighbor_number[0] = a;
            neighbor_number[1] = b;
            neighbor_number[2] = c;
        }

        //See if index i is a neighbor of this Room object
        public Boolean isNeighbor(int i)
        {
            foreach (int n in neighbor_number)
            {
                if (n == i)
                {
                    return true;
                }
            }

            return false;
        }

        //Prints the neighbors' numbers
        public String printNeighbors()
        {
            const string format = "{0,-3} {1,-3} {2,-3}";
            string print = "";
            print = string.Format(format, neighbor_number[0], neighbor_number[1], neighbor_number[2]);
            return print;
        }

        //Gets the neighbor of index
        public int getNeighbor(int index)
        {
            return neighbor_number[index];
        }

        //Removes the wumpus from this Room
        public void removeWumpus()
        {
            has_wumpus = false;
        }

        //Sets the wumpus for this Room
        public bool setWumpus()
        {
            if (!has_bats && !has_pit)
            {
                has_wumpus = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        //Sets the superbats for this Room
        public bool setBats()
        {
            if (!has_wumpus && !has_pit && !has_bats)
            {
                has_bats = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        //Sets the pit for this Room
        public bool setPit()
        {
            if (!has_bats && !has_wumpus && !has_pit)
            {
                has_pit = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        //Checks to see if wumpus is in this Room
        public bool hasWumpus()
        {
            if (has_wumpus)
                return true;

            return false;
        }

        //Checks to see if superbats are in this Room
        public bool hasBats()
        {
            if (has_bats)
                return true;

            return false;
        }

        //Checks to see if a pit is in this Room
        public bool hasPit()
        {
            if (has_pit)
                return true;

            return false;
        }

        public void setVector(int x, int y)
        {
            vector.X = x;
            vector.Y = y;
        }

        public Vector2 getVector()
        {
            return vector;
        }
    }
}

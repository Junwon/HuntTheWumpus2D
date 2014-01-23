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
    class Map
    {
        private Random random_num = new Random();
        private Room[] rooms = new Room[20];
        private int wumpus_number;

        //Fill room with 1 superbats, 2 pits, and 1 wumpus. Also initializes neighboring rooms for each rooms.
        public void initialize_rooms()
        {
            for (int i = 0; i < rooms.Length; i++)
            {
                rooms[i] = new Room();
                rooms[i].setRoomNumber(i + 1);
            }

            wumpus_number = random_num.Next(20);
            rooms[wumpus_number].setWumpus();

            while (rooms[random_num.Next(20)].setBats() == false)
            {
            }

            while (rooms[random_num.Next(20)].setPit() == false)
            {
            }

            while (rooms[random_num.Next(20)].setPit() == false)
            {
            }

            rooms[0].setNeighbors(3, 6, 12);
            rooms[1].setNeighbors(13, 16, 18);
            rooms[2].setNeighbors(1, 11, 17);
            rooms[3].setNeighbors(9, 10, 12);
            rooms[4].setNeighbors(13, 14, 19);
            rooms[5].setNeighbors(1, 9, 14);
            rooms[6].setNeighbors(11, 12, 20);
            rooms[7].setNeighbors(13, 15, 17);
            rooms[8].setNeighbors(4, 6, 19);
            rooms[9].setNeighbors(4, 16, 20);
            rooms[10].setNeighbors(3, 7, 15);
            rooms[11].setNeighbors(1, 4, 7);
            rooms[12].setNeighbors(2, 5, 8);
            rooms[13].setNeighbors(5, 6, 17);
            rooms[14].setNeighbors(8, 11, 18);
            rooms[15].setNeighbors(2, 10, 19);
            rooms[16].setNeighbors(3, 8, 14);
            rooms[17].setNeighbors(2, 15, 20);
            rooms[18].setNeighbors(5, 9, 16);
            rooms[19].setNeighbors(7, 10, 18);

            rooms[0].setVector(160, 25);
            rooms[1].setVector(535, 345);
            rooms[2].setVector(85, 345);
            rooms[3].setVector(660, 95);
            rooms[4].setVector(360, 235);
            rooms[5].setVector(260, 95);
            rooms[6].setVector(835, 345);
            rooms[7].setVector(310, 435);
            rooms[8].setVector(460, 95);
            rooms[9].setVector(685, 235);
            rooms[10].setVector(460, 615);
            rooms[11].setVector(760, 25);
            rooms[12].setVector(385, 345);
            rooms[13].setVector(235, 235);
            rooms[14].setVector(460, 505);
            rooms[15].setVector(560, 235);
            rooms[16].setVector(210, 345);
            rooms[17].setVector(610, 435);
            rooms[18].setVector(460, 165);
            rooms[19].setVector(710, 345);


        }

        //Moves the wumpus to a random room that isn't the player's room
        public void moveWumpus()
        {
            rooms[whereWumpus()].removeWumpus();

            wumpus_number = random_num.Next(20);

            while (rooms[wumpus_number].setWumpus() == false)
            {
                wumpus_number = random_num.Next(20);
            }
        }

        //Function to see where the wumpus is. Not used in actual game
        public int whereWumpus()
        {
            return wumpus_number;
        }



        //Returns the room of index i in room_lists
        public Room getRoom(int i)
        {
            return rooms[i];
        }

        //Checks the neighbors of room index i in room_lists
        public string checkNeighbors(int i)
        {
            int neighbor;
            string s = "";

            for (int k = 0; k < 3; k++)
            {
                neighbor = rooms[i].getNeighbor(k) - 1;

                if (rooms[neighbor].hasBats())
                {
                    s += "There are bats nearby.\n";
                }
                else if (rooms[neighbor].hasPit())
                {
                    s += "I feel a draft.\n";
                }
                else if (rooms[neighbor].hasWumpus())
                {
                    s += "I smell a wumpus!\n";
                }
            }

            return s;
        }

        //Checks to see if wumpus is in neighboring rooms
        public Boolean wumpusNearby(int i)
        {
            int neighbor;

            for (int k = 0; k < 3; k++)
            {
                neighbor = rooms[i].getNeighbor(k) - 1;

                if (rooms[neighbor].hasWumpus())
                {
                    return true;
                }
            }

            return false;
        }

        //Checks to see if hazards are in neighboring rooms
        public Boolean checkRoom(int i)
        {
            if (rooms[i].hasBats())
            {
                return true;
            }
            else if (rooms[i].hasPit())
            {
                return true;
            }
            else if (rooms[i].hasWumpus())
            {
                return true;
            }

            return false;
        }

        //Check if room index i in room_list has a wumpus
        public Boolean isWumpusRoom(int i)
        {
            if (rooms[i].hasWumpus())
            {
                return true;
            }
            return false;
        }

        //Check if room index i in room_list has superbats
        public Boolean isSuperBatsRoom(int i)
        {
            if (rooms[i].hasBats())
            {
                return true;
            }
            return false;
        }

        //Check if room index i in room_list has a pit
        public Boolean isPitRoom(int i)
        {
            if (rooms[i].hasPit())
            {
                return true;
            }
            return false;
        }
    }
}

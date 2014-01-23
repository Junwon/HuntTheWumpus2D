/*
 *John Nguyen  
 *CS4332 Introduction to Programming Video Games 
 *Second Assignment: 2D Sprites 
 * 
 * Credits: player.png is from Final Fantasy Tactis Archer sprite
 *          wumpus.png is from Final Fantasy 6 Ultros sprite
 *          bat.png is from The Legend of Zelda Keese sprite
 *          pit.png is from The Legend of Zelda: Link to the Past Map sprite
 *          
 */
using System;

namespace HuntTheWumpus2D
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game2D game = new Game2D())
            {
                game.Run();
            }
        }
    }
#endif
}


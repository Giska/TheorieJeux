using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameTheory
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new manager
            Thjx.Manager Manager = new Thjx.Manager("brunel_a", "7SrR39a0t2YP4ghv", "scia.epita.fr", 5632);

            // Start the manager using newGame as the main function
            Manager.Run(NewGame);
        }

        static void NewGame(Thjx.Game Game)
        {
            // A new match has been found
            System.Console.WriteLine("new game " + Game.You.Name + "<->" + Game.Challenger.Name);

            // Init the strategy put the 100 as we want
            // TODO: initiate the strategy
            //       => Put 100 il all the pure strat
            Thjx.Request InitialStock = new Thjx.Request();
            InitialStock.S = 10;
            InitialStock.G = 10;
            InitialStock.T = 60;
            InitialStock.N = 20;

            Game.Select(InitialStock);
            int challenger_N = 0;

            //Start the game
            while (!Game.Ended)
            {
                // Play strategy exemple using the strategy N
                if (challenger_N < 2 && Game.You.N > 6)
                    Game.Act(Thjx.Game.Startegy.N);
                else if (challenger_N > 2 && Game.You.T > 0)
                    Game.Act(Thjx.Game.Startegy.T);
                else if (Game.You.S > 0)
                    Game.Act(Thjx.Game.Startegy.S);
                else if (Game.You.G > 0)
                    Game.Act(Thjx.Game.Startegy.G);
                else
                    Game.Act(Thjx.Game.Startegy.T);

                //Purchase
                Thjx.Request PurchaseReq = new Thjx.Request();
                if (Game.You.T < 3 && Game.You.Score > 12)
                {
                    PurchaseReq.T = 1;
                }
                Console.WriteLine("Purchase");
                Game.Purchase(PurchaseReq);

                Console.WriteLine("Surrender");
                Console.WriteLine("My Score: " + Game.You.Score);
                Console.WriteLine("Your Choice: " + Game.You.LastActionPlayed);
                Console.WriteLine("Challenger Score: " + Game.Challenger.Score);
                Console.WriteLine("Challenger Choice: " + Game.Challenger.LastActionPlayed);
                if (Game.Challenger.LastActionPlayed.ToString() == "N")
                    challenger_N++;
                else
                    challenger_N = 0;
                Console.WriteLine(challenger_N);
                if (Game.You.N < 5)
                {
                    // This is not good we have no more N. We should surrender
                    bool Proposed = Game.SurrenderProposition(true);
                    Console.WriteLine("Proposed surrender");
                    if (Proposed)
                        Game.SurrenderAcceptation(true); // The challenger has proposed to surrender we accept
                    else
                        Game.SurrenderAcceptation(false);
                }
                else
                {
                    Console.WriteLine("Refuse propose surrender");
                    // Don't care the situation is not problematic
                    // Just don't accept any surrender request
                    Game.SurrenderProposition(false);
                    Game.SurrenderAcceptation(false);
                }

                Console.WriteLine("New Turn");
            }
        }
    }
}

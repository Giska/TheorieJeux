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
            Thjx.Manager Manager = new Thjx.Manager("thomas_f", "0mg0207nfRP2fr3G", "http://scia.epita.fr/", 5632);

            // Start the manager using newGame as the main function
            Manager.Run(NewGame);
        }

        static void NewGame(Thjx.Game Game)
        {
            // A new match has been found
            System.Console.WriteLine("new game " + Game.You.Name + "<->" + Game.Challenger.Name);

            // Init the strategy put the 100 as we want
            Thjx.Request InitialStock = new Thjx.Request();
            InitialStock.N = 100;
            Game.Select(InitialStock);

            //Start the game
            while (!Game.Ended)
            {
                // Play strategy exemple using the strategy N
                Game.Act(Thjx.Game.Startegy.N);

                //Buy N if our score is higher than 30. (Because N cost 30 points)
                Thjx.Request PurchaseReq = new Thjx.Request();
                if (Game.You.Score > 30)
                {
                    PurchaseReq.N = 1;
                }
                Game.Purchase(PurchaseReq);

                if (Game.You.N < 5)
                {
                    // This is not good we have no more N. We should surrender
                    bool Proposed = Game.SurrenderProposition(true);
                    if (Proposed)
                        Game.SurrenderAcceptation(true); // The challenger has proposed to surrender we accept
                    else
                        Game.SurrenderAcceptation(false);
                }
                else
                {
                    // Don't care the situation is not problematic
                    // Just don't accept any surrender request
                    Game.SurrenderProposition(false);
                    Game.SurrenderAcceptation(false);
                }

            }
        }
    }
}

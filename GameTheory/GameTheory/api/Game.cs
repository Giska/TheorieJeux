using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thjx
{
    public class Game
    {
        internal bool _Ended = false;
        public bool Ended { get { return _Ended; } }
        internal Manager _Parent = null;
        Info _You = new Info(); public Info You { get { return _You; } }
        Info _Challenger = new Info(); public Info Challenger { get { return _Challenger; } }

        enum Action
        {
            Select,
            Act,
            Buy,
            SurrenderProp,
            SurrenderAcc
        }

        Action _Step = Action.Select;
        void _NextStep()
        {
            switch (_Step)
            {
                case Action.Select:
                    _Step = Action.Act;
                    return;
                case Action.Act:
                    _Step = Action.Buy;
                    return;
                case Action.Buy:
                    _Step = Action.SurrenderProp;
                    return;
                case Action.SurrenderProp:
                    _Step = Action.SurrenderAcc;
                    return;
                case Action.SurrenderAcc:
                    _Step = Action.Act;
                    return;
            }
        }
        void _SurrenderPorposition() { }

        /// <summary>
        /// Select all the strategies that will be used during the game
        /// </summary>
        /// <param name="Request">A request representing the strategies that must be selected. The sum must be equal to 100.</param>
        public void Select(Request Request)
        {
            if (_Step != Action.Select)
            {
                Console.WriteLine("[ERROR] you can use select only at the beining of the game");
                throw new Exception("Invalid Action");
            }
            if (Request.Count != 100)
            {
                Console.WriteLine("[ERROR] You must select 100 startegies");
                throw new Exception("Invalid command");
            }
            JSON.Object obj = new JSON.Object();
            obj["type"] = "select";
            JSON.Object strategies = new JSON.Object();
            strategies["N"] = Request.N;
            strategies["T"] = Request.T;
            strategies["M"] = Request.M;
            strategies["E"] = Request.E;
            strategies["G"] = Request.G;
            strategies["S"] = Request.S;
            strategies["W"] = Request.W;
            obj["strategies"] = strategies;
            _Parent._Write(obj);
            JSON.Object result = _Parent._ReadObject();
            {
                try
                {
                    JSON.Object you = result["you"] as JSON.Object;
                    _UpdateStock(_You, you["strategies"] as JSON.Object);
                }
                catch { }

                try
                {
                    JSON.Object challenger = result["challenger"] as JSON.Object;
                    _UpdateStock(_Challenger, challenger["strategies"] as JSON.Object);
                }
                catch { }
            }
            _NextStep();
        }

        public enum Startegy
        {
            W = 0, S = 1, G = 2, E = 3, M = 4, T = 5, N = 6, Nothing = -1
        }

        void _UpdateStock(Info Info, JSON.Object Stock)
        {
            try { Info._W = (int)((Stock["W"] as JSON.Number).Value); }
            catch { }
            try { Info._S = (int)((Stock["S"] as JSON.Number).Value); }
            catch { }
            try { Info._G = (int)((Stock["G"] as JSON.Number).Value); }
            catch { }
            try { Info._E = (int)((Stock["E"] as JSON.Number).Value); }
            catch { }
            try { Info._M = (int)((Stock["M"] as JSON.Number).Value); }
            catch { }
            try { Info._N = (int)((Stock["N"] as JSON.Number).Value); }
            catch { }
            try { Info._T = (int)((Stock["T"] as JSON.Number).Value); }
            catch { }
        }

        /// <summary>
        /// Play using a specific strategy
        /// </summary>
        /// <param name="strategy">The strategy that must be used</param>
        /// <returns>True if you win this turn false otherwise</returns>
        public bool Act(Startegy strategy)
        {
            if (_Step != Action.Act)
            {
                Console.WriteLine("[ERROR] you can use action only at the beining of a turn");
                throw new Exception("Invalid Action");
            }
            if (strategy == Startegy.Nothing)
            {
                Console.WriteLine("[ERROR] you cannot use action nothing");
                throw new Exception("Invalid Action");
            }
            JSON.Object action = new JSON.Object();
            action["type"] = "action";
            string act = "N";
            switch (strategy)
            {
                case Startegy.W: act = "W"; break;
                case Startegy.S: act = "S"; break;
                case Startegy.G: act = "G"; break;
                case Startegy.E: act = "E"; break;
                case Startegy.M: act = "M"; break;
                case Startegy.N: act = "N"; break;
                case Startegy.T: act = "T"; break;
            }

            action["action_played"] = act;
            _Parent._Write(action);

            JSON.Object result = new JSON.Object();
            result = _Parent._ReadObject();
            try
            {
                JSON.Object you = result["you"] as JSON.Object;
                _UpdateStock(_You, you["strategies"] as JSON.Object);
                try { _You._Score = (int)(you["score"] as JSON.Number).Value; }
                catch { }
                try { _You._LastActionPlayed = Info.StrategyFromString((you["action_played"] as JSON.String).ToString()); }
                catch { }
            }
            catch
            {
                Console.WriteLine("[ERROR] Unexpected server response, data have not been updated");
            }

            try
            {
                JSON.Object challenger = result["challenger"] as JSON.Object;
                _UpdateStock(_Challenger, challenger["strategies"] as JSON.Object);
                try { _Challenger._Score = (int)(challenger["score"] as JSON.Number).Value; }
                catch { }
                try { _Challenger._LastActionPlayed = Info.StrategyFromString((challenger["action_played"] as JSON.String).ToString()); }
                catch { }
            }
            catch
            {
                Console.WriteLine("[ERROR] Unexpected server response, data have not been updated");
            }

            _NextStep();

            try
            {
                if ((result["ended"] as JSON.Boolean).State)
                {
                    _Ended = true;
                }
            }
            catch
            {
            }

            try
            {
                string winner = (result["action_winner"] as JSON.String).Value;
                return winner == _You.Name;
            }
            catch { return false; }
        }

        /// <summary>
        /// Purchase some strategy using your points
        /// </summary>
        /// <param name="Request">The strategies that must be purchase. Check if Cost is lower or equal to your score before using it.</param>
        public void Purchase(Request Request)
        {
            if (_Step != Action.Buy)
            {
                Console.WriteLine("[ERROR] you can use purchase only after the action");
                throw new Exception("Invalid Action");
            }

            JSON.Object buy = new JSON.Object();
            buy["type"] = "purchase";

            if (Request == null)
            {

            }
            else
            {
                if (_You.Score < Request.Cost)
                {
                    Console.WriteLine("[ERROR] you are trying to buy more than you can");
                    throw new Exception("Invalid Action");
                }
                JSON.Object strategies = new JSON.Object();
                strategies["N"] = Request.N;
                strategies["T"] = Request.T;
                strategies["M"] = Request.M;
                strategies["E"] = Request.E;
                strategies["G"] = Request.G;
                strategies["S"] = Request.S;
                strategies["W"] = Request.W;
                buy["strategies"] = strategies;
            }

            _Parent._Write(buy);
            JSON.Object result = _Parent._ReadObject();
            try
            {
                try
                {
                    JSON.Object you = result["you"] as JSON.Object;
                    _UpdateStock(_You, you["strategies"] as JSON.Object);
                    try { _You._Score = (int)(you["score"] as JSON.Number).Value; }
                    catch { }
                }
                catch
                {
                    Console.WriteLine("[ERROR] Unexpected server response, data have not been updated");
                }

                try
                {
                    JSON.Object challenger = result["challenger"] as JSON.Object;
                    _UpdateStock(_Challenger, challenger["strategies"] as JSON.Object);
                    try { _Challenger._Score = (int)(challenger["score"] as JSON.Number).Value; }
                    catch { }
                }
                catch
                {
                    Console.WriteLine("[ERROR] Unexpected server response, data have not been updated");
                }
            }
            catch { }
            _NextStep();
        }

        /// <summary>
        /// Offer to the other player a "Surrender proposition"
        /// </summary>
        /// <param name="Value">True if a surrender proposition must be sended, false otherwise</param>
        /// <returns>True if the challeger has sended a surrender proposition, false otherwise</returns>
        public bool SurrenderProposition(bool Value)
        {
            if (_Step != Action.SurrenderProp)
            {
                Console.WriteLine("[ERROR] you can use surrender proposition only after the buying stage");
                throw new Exception("Invalid Action");
            }
            JSON.Object Surrender = new JSON.Object();
            Surrender["type"] = "surrender_proposition";
            Surrender["value"] = Value;
            _Parent._Write(Surrender);
            _NextStep();
            try
            {
                JSON.Object prop = _Parent._ReadObject();
                return (prop["value"] as JSON.Boolean).State;
            }
            catch
            {
                Console.WriteLine("[ERROR] Unexpected server response, data have not been updated");
                return false;
            }


        }

        /// <summary>
        /// Accept or decline a surrender proposition.
        /// </summary>
        /// <param name="Value">true to accept the proposoition false to decline.</param>
        /// <remarks>You must send false even if the other player has not sended a surrender proposition.</remarks>
        public void SurrenderAcceptation(bool Value)
        {
            if (_Step != Action.SurrenderAcc)
            {
                Console.WriteLine("[ERROR] you can use surrender acceptation only after the surrender stage");
                throw new Exception("Invalid Action");
            }

            JSON.Object Surrender = new JSON.Object();
            Surrender["type"] = "surrender_acceptation";
            Surrender["value"] = Value;
            _Parent._Write(Surrender);

            try
            {
                JSON.Object rep = _Parent._ReadObject();
                if ((rep["ended"] as JSON.Boolean).State)
                {
                    _Ended = true;
                }
            }
            catch
            {
                Console.WriteLine("[ERROR] Unexpected server response, data have not been updated");
            }

            _NextStep();
        }

    }
}
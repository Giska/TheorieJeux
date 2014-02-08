using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thjx
{
    public class Info
    {
        internal string _Name = ""; public string Name { get { return _Name; } }
        internal int _W = 0; public int W { get { return _W; } }
        internal int _S = 0; public int S { get { return _S; } }
        internal int _G = 0; public int G { get { return _G; } }
        internal int _E = 0; public int E { get { return _E; } }
        internal int _M = 0; public int M { get { return _M; } }
        internal int _T = 0; public int T { get { return _T; } }
        internal int _N = 0; public int N { get { return _N; } }
        internal int _Score = 0; public int Score { get { return _Score; } }

        public int StrategyCount { get { return _W + _S + _G + _E + _M + _T + _N; } }

        public int GetCount(Game.Startegy Strategy)
        {
            switch (Strategy)
            {
                case Game.Startegy.E: return _E;
                case Game.Startegy.G: return _G;
                case Game.Startegy.M: return _M;
                case Game.Startegy.N: return _N;
                case Game.Startegy.S: return _S;
                case Game.Startegy.T: return _T;
                case Game.Startegy.W: return _W;
            }
            return 0;
        }

        internal Game.Startegy _LastActionPlayed = Game.Startegy.Nothing;
        public Game.Startegy LastActionPlayed { get { return _LastActionPlayed; } }

        public static Game.Startegy StrategyFromString(string Strategy)
        {
            Strategy = Strategy.ToLower();
            while (Strategy.StartsWith(" ") || Strategy.StartsWith("\"") || Strategy.StartsWith("\'")) { Strategy = Strategy.Substring(1); }

            if (Strategy == "") { return Game.Startegy.Nothing; }
            if (Strategy[0] == 'e') { return Game.Startegy.E; }
            if (Strategy[0] == 'g') { return Game.Startegy.G; }
            if (Strategy[0] == 'm') { return Game.Startegy.M; }
            if (Strategy[0] == 'n') { return Game.Startegy.N; }
            if (Strategy[0] == 's') { return Game.Startegy.S; }
            if (Strategy[0] == 't') { return Game.Startegy.T; }
            if (Strategy[0] == 'w') { return Game.Startegy.W; }
            return Game.Startegy.Nothing;
        }

        public override string ToString()
        {
            return "{" + _Name + ": Score=" + _Score + " [W:" + _W.ToString() + " S:" + _S.ToString() + " G:" + _G.ToString() + " E:" + _E.ToString() + " M:" + _M.ToString() + " N:" + _N.ToString() + " T:" + _T.ToString() + "]}";
        }
    }
}
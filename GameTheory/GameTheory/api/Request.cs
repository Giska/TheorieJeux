using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thjx
{
    public class Request
    {
        internal int _W = 0; public int W { get { return _W; } set { _W = value; if (_W < 0) { _W = 0; } } }
        internal int _S = 0; public int S { get { return _S; } set { _S = value; if (_S < 0) { _S = 0; } } }
        internal int _G = 0; public int G { get { return _G; } set { _G = value; if (_G < 0) { _G = 0; } } }
        internal int _E = 0; public int E { get { return _E; } set { _E = value; if (_E < 0) { _E = 0; } } }
        internal int _M = 0; public int M { get { return _M; } set { _M = value; if (_M < 0) { _M = 0; } } }
        internal int _T = 0; public int T { get { return _T; } set { _T = value; if (_T < 0) { _T = 0; } } }
        internal int _N = 0; public int N { get { return _N; } set { _N = value; if (_N < 0) { _N = 0; } } }
        /// <summary>
        /// Represent the total cost of this request: W*5+S*7+G*7+E*8+M*8+T*12+N*30
        /// </summary>
        public int Cost { get { return _W * 5 + _S * 7 + _G * 7 + _E * 8 + _M * 8 + _T * 12 + _N * 30; } }
        /// <summary>
        /// Represent the number of strategies contained in this request: W+S+G+E+M+T+N
        /// </summary>
        public int Count { get { return _W + _S + _G + _E + _M + _T + _N; } }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thjx
{
    public class Manager
    {
        System.Net.Sockets.TcpClient _Client = null;
        string _Username = "";
        string _Password = "";
        string _Server = "";
        int _Port = 0;

        public delegate void GameLoop(Game Game);

        /// <summary>
        /// Create a new Manager
        /// </summary>
        /// <param name="Username">The username (the one that you have recieved by e-mail)</param>
        /// <param name="Password">The password (the one that you have recieved by e-mail)</param>
        /// <param name="Server">The server name</param>
        /// <param name="Port">The port used by the server</param>
        public Manager(string Username, string Password, string Server, int Port)
        {
            _Username = Username;
            byte[] spassword = System.Text.Encoding.UTF8.GetBytes(Username + "_thjx_" + Password);
            byte[] bpassword = System.Security.Cryptography.SHA512.Create().ComputeHash(spassword);
            StringBuilder builder = new StringBuilder();
            for (int n = 0; n < bpassword.Length; n++) { builder.Append(bpassword[n].ToString("X2")); }
            _Password = builder.ToString().ToUpper();
            _Port = Port;
        }

        internal JSON.Object _ReadObject()
        {
            try
            {
                string value = _Read();
                return JSON.Parse(value) as JSON.Object;
            }
            catch
            {
                Console.WriteLine("[ERROR] An error occurs while parsing the server response (FATAL)");
                System.Environment.Exit(1);
            }
            return null;
        }

        internal string _Read()
        {
            StringBuilder result = new StringBuilder();
            int count = 0;
            bool init = false;
            List<byte> array = new List<byte>();
            try
            {
                while (true)
                {
                    if (!_Client.Connected)
                    {
                        Console.WriteLine("[ERROR] The server has been disconnected (FATAL)");
                        System.Environment.Exit(1);
                    }
                    while (_Client.Available >= 1)
                    {
                        int value = _Client.GetStream().ReadByte();
                        if (value < 0) 
                        {
                            Console.WriteLine("[ERROR] An error occurs while reading the server response (FATAL)");
                            System.Environment.Exit(1);
                        }
                        byte bvalue = (byte)value;
                        array.Add(bvalue);
                        if (array.Count > 1024 * 1024)
                        {
                            Console.WriteLine("[ERROR] Response is too big (FATAL)");
                            System.Environment.Exit(1); 
                        }
                        if (bvalue == '{')
                        {
                            if (init)
                            {
                                count++;
                            }
                            else { init = true; }
                        }
                        else
                        {
                            if (!init)
                            {
                                if (bvalue != '\n' && bvalue != '\r' && bvalue != ' ' && bvalue != '\t')
                                {
                                    Console.WriteLine("[ERROR] Invalid server response (FATAL)");
                                    System.Environment.Exit(1);
                                }
                            }
                            else if (bvalue == '}')
                            {
                                if (count == 0) { return System.Text.Encoding.UTF8.GetString(array.ToArray()); }
                                count--;
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(100);
                }
            }
            catch
            {
                Console.WriteLine("[ERROR] An error occurs while reading the server response (FATAL)");
                System.Environment.Exit(1);
            }
            return "";
        }

        internal void _Write(JSON.Object obj)
        {
            _Write(obj.ToString());
        }
        internal void _Write(string value)
        {
            try
            {
                byte[] array = System.Text.Encoding.UTF8.GetBytes(value);
                _Client.GetStream().Write(array, 0, array.Length);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("[ERROR] Impossible to communicate with the server (FATAL)");
                System.Environment.Exit(1);
            }
        }

        /// <summary>
        /// Start the system. This method must be called.
        /// </summary>
        /// <param name="Loop">The game loop called for each game.</param>
        public void Run(GameLoop Loop)
        {
            try
            {
                _Client = new System.Net.Sockets.TcpClient(_Server, _Port);
            }
            catch
            {
                System.Console.WriteLine("[ERROR] Impossible to join the server (FATAL)");
                System.Environment.Exit(1);
            }
            JSON.Object connect = new JSON.Object();
            connect["login"] = _Username;
            connect["password"] = _Password;
            _Write(connect);
            try
            {
                while (true)
                {
                    JSON.Object newgame = _ReadObject();
                    Game game = new Game();
                    game._Parent = this;
                    try
                    {
                        if ((newgame["error"] as JSON.String).Value != "")
                        {
                            Console.WriteLine("[ERROR] Invalid connection check your login and your password");
                            System.Environment.Exit(1);
                        }
                    }
                    catch { }
                    try { game.You._Name = (newgame["you"] as JSON.String).Value; }
                    catch { game.You._Name = "noname"; }
                    try { game.Challenger._Name = (newgame["challenger"] as JSON.String).Value; }
                    catch { game.Challenger._Name = "noname"; }
                    try
                    {
                        game.You._Matrix = new int[][]
                        {
                            (int[])((newgame["matrix"] as JSON.Array).Values[0] as JSON.Array),
                            (int[])((newgame["matrix"] as JSON.Array).Values[1] as JSON.Array),
                            (int[])((newgame["matrix"] as JSON.Array).Values[2] as JSON.Array),
                            (int[])((newgame["matrix"] as JSON.Array).Values[3] as JSON.Array),
                            (int[])((newgame["matrix"] as JSON.Array).Values[4] as JSON.Array),
                            (int[])((newgame["matrix"] as JSON.Array).Values[5] as JSON.Array),
                            (int[])((newgame["matrix"] as JSON.Array).Values[6] as JSON.Array),
                        };
                    }
                    catch { }
                    Loop(game);
                    if (!game._Ended) 
                    {
                        System.Console.WriteLine("[ERROR] Game loop has ended before the end of the game (FATAL)");
                        System.Environment.Exit(1);
                    }
                    JSON.Object ready = new JSON.Object();
                    ready["type"] = "ready";
                    _Write(ready);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[INFO]  An exception occurs inside the game loop:\n" + e.ToString());
            }
            try 
            {
                _Client.Close();
            }
            catch { }
        }

    }
}

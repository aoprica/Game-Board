using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Network;
using Game;

namespace SimpleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            var server = new GameServer(NetUtil.GetIP().ToString(), 1337);
            server.Start();

            Console.ReadLine();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex  = e.ExceptionObject as Exception;

            Log.Error(ex);

            Console.Write(ex.StackTrace);
            Console.ReadLine();
        }
    }
}

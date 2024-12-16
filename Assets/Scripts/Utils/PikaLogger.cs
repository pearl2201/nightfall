using System;
using System.Collections.Generic;
using System.Text;

namespace Pika.Base.Utils
{
    public class PikaLogger
    {
        public static void Info(string message)
        {
            Console.WriteLine(message);
        }

        public static void Error( string message)
        {
            Console.WriteLine(message);
        }

        public static void Error(Exception ex, string message)
        {
            Console.WriteLine(message);
        }

        public static void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public static void Debug(string message, params object[] elems)
        {
            Console.WriteLine(message);
        }

        public static void Info(string message, params object[] elems)
        {
            Console.WriteLine(message);
        }
    }
}

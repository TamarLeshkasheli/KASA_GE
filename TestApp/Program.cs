using BDO_DatecsDP25;
using System;


namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var datec = new Datecs();
            var r = datec.InitDp25("com10");
            r = datec.Exec(74);
            
            Console.ReadLine();
        }

    }
}

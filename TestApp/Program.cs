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
            var s = datec.ReadStatus();

            r = datec.Exec(100,"-112001");
            r = datec.Exec(60);
            s = datec.ReadStatus();
            
            r = datec.Exec(48, "1", "1", "1", "0");
            r = datec.Exec(49, "ყველი", "1", "250000.00", "1", "", "", "1");
            r = datec.Exec(49, "პური", "1", "250000.00", "1", "", "", "1");
            r = datec.Exec(49, "ხაჭაპური", "1", "250000.00", "1", "", "", "1");
            r = datec.Exec(49, "წიწილა", "1", "250000.00", "1", "", "", "1");
            r = datec.Exec(51, "1", "", "", "");
            r = datec.Exec(53, "0", "");
            r = datec.Exec(56);


        }

    }
}

using BDO_DatecsDP25;
using System;


namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var datec = new Datecs();

            var s = datec.ReadStatus();
            var r = datec.InitDp25("com10");
            s = datec.ReadStatus();

            r = datec.Exec(100, "-110024");

            // ზეტის დაბეჭდვა
            //r = datec.Exec(69,"Z");

            // ფისკალური ჩეკის გაუქმება
            //r = datec.Exec(60);
            //r = datec.Exec(39);

            // ფისკალური ჩეკის ბეჭდვა
            //r = datec.Exec(48, "1", "1", "1", "0");
            //r = datec.Exec(49, "ყველი", "1", "25.00", "1", "", "", "1");
            //r = datec.Exec(49, "პური", "1", "25.00", "1", "", "", "1");
            //r = datec.Exec(49, "ხაჭაპური", "1", "25.00", "1", "", "", "1");
            //r = datec.Exec(49, "წიწილა", "1", "25.00", "1", "", "", "1");
            //r = datec.Exec(51, "1", "", "", "");
            //r = datec.Exec(53, "", "");
            //r = datec.Exec(56);
        }

    }
}

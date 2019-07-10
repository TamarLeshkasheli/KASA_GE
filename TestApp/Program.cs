using BDO_DatecsDP25;
using System;

namespace TestApp
{

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                testDP25();
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();

            //System.IO.File.AppendAllLines("cmd.log", new[] { "a", "b" });
        }

        static void testDP25() {
            using(var p = new Dp25("com10"))
            {
                p.AddLogger(str => {
                    System.IO.File.AppendAllText("log01.txt", DateTime.Now.ToLongTimeString() + ":" + str + "\n");
                });
                p.OpenFiscalReceipt("5", "5");
                p.RegisterSale("ყველი", 11.6m, 1, 1, BDO_DatecsDP25.Commands.TaxCode.A);
                p.RegisterSale("პური", 2m, 1, 1, BDO_DatecsDP25.Commands.TaxCode.A);
                p.RegisterSale("ხაჭაპური", 3m, 1, 1, BDO_DatecsDP25.Commands.TaxCode.A);
                p.RegisterSale("წიწილა", 4m, 1, 1, BDO_DatecsDP25.Commands.TaxCode.A);
                p.Total(BDO_DatecsDP25.Commands.PaymentMode.Cash);
                p.CloseFiscalReceipt();
            }
        }

        static void testRaw() {
            using (var p = new FP700Printer("com10"))
            {
                p.Exec(48, "5\t5\t1\t0\t");
                p.Exec(49, "ყველი\t1\t11.60\t1\t\t\t1\t");
                p.Exec(49, "პური\t1\t2\t1\t\t\t2\t");
                p.Exec(49, "ხაჭაპური\t1\t3\t1\t\t\t3\t");
                p.Exec(49, "წიწილა\t1\t4\t1\t\t\t4\t");
                p.Exec(51, "1\t\t\t\t");
                p.Exec(53, "0\t\t\t");
                p.Exec(56, "");
            }
        }
    }
}

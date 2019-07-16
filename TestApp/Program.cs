using BDO_DatecsDP25;
using System;


namespace TestApp
{
    class TestPrinter : IFP700Printer
    {
        readonly Action<int, string> execf;
        public FP700Result Exec(int cmd, string data)
        {
            execf(cmd, data);
            return FP700Result.Empty;
        }
        public void Dispose()
        {
        }
        public TestPrinter(Action<int, string> exec)
        {
            execf = exec;
        }

    }
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                System.IO.File.AppendAllText("log01.txt", "\n\n");
                using (var p = new Dp25("com10"))
                {
                    p.AddLogger(cmd => {
                        System.IO.File.AppendAllText("log01.txt", cmd+"\n");
                    });
                    //p.PrintReport(BDO_DatecsDP25.Commands.ReportType.Z);
                    testDP25(p);
                    //Console.WriteLine("");
                    testDP25(p);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("press enter");
            Console.ReadLine();

            //System.IO.File.AppendAllLines("cmd.log", new[] { "a", "b" });
        }

        static void testReport(Dp25 p)
        {
            p.PrintReport(BDO_DatecsDP25.Commands.ReportType.Z);
        }

        static void testDP25(Dp25 p)
        {
            p.OpenFiscalReceipt("5", "5");
            p.RegisterSale("ყველი", 11.6m, 1.21m, 1, BDO_DatecsDP25.Commands.TaxCode.A);
            p.RegisterSale("პური", 0.98m, 3, 1, BDO_DatecsDP25.Commands.TaxCode.A);
            //p.RegisterSale("ხაჭაპური", 3m, 1, 1, BDO_DatecsDP25.Commands.TaxCode.A);
            //p.RegisterSale("წიწილა", 4m, 1, 1, BDO_DatecsDP25.Commands.TaxCode.A);
            //p.SubTotal();
            p.Total(BDO_DatecsDP25.Commands.PaymentMode.Cash);
            p.CloseFiscalReceipt();
        }

        static void testRaw(Dp25 p)
        {
            p.SendMessage(48, "5\t5\t1\t0\t");
            p.SendMessage(49, "ყველი\t1\t11.60\t1\t\t\t1\t");
            //p.SendMessage(49, "პური\t1\t2\t1\t\t\t2\t");
            //p.SendMessage(49, "ხაჭაპური\t1\t3\t1\t\t\t3\t");
            //p.SendMessage(49, "წიწილა\t1\t4\t1\t\t\t4\t");
            //p.SendMessage(51, "1\t\t\t\t");
            p.SendMessage(53, "0\t\t\t");
            p.SendMessage(56, "");
        }
    }
}

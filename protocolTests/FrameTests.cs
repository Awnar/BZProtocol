using Microsoft.VisualStudio.TestTools.UnitTesting;
using protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace protocol.Tests
{
    [TestClass()]
    public class FrameTests
    {

        [TestMethod()]
        public void FrameTest()
        {
            var x =new Datagram();
            x.ID = 12;
            x.Operacja = 2;
            x.Status = 3;

            var z = new Datagram(x.gen());

            Console.WriteLine(z.ID +"\n"+z.Operacja+"\n"+z.Status+"\n"+z.Checksum+"\n"+z.IleLiczb);
            Console.WriteLine(z.L1 + "\n" + z.L2 + "\n" + z.L3);
        }

        [TestMethod()]
        public void FrameTest2()
        {
            var x = new Datagram();
            x.ID = 1;
            x.Operacja = 2;
            x.Status = 3;

            x.L1 = 28;
            x.L2 = 38;
            
            var z = new Datagram(x.gen());

            Console.WriteLine(z.ID + "\n" + z.Operacja + "\n" + z.Status + "\n" + z.Checksum + "\n" + z.IleLiczb);
            Console.WriteLine(z.L1 + "\n" + z.L2 + "\n" + z.L3);
        }
    }
}
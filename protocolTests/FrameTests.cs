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

        [TestMethod()]
        public void FrameTestTF2()
        {
            var x = new Datagram();
            x.ID = 3;
            x.Operacja = 1;
            x.Status = 4;
            x.L1 = 25;

            var data = x.gen2();
            foreach (var item in data)
            {
                Console.WriteLine("{0:X}", item);
            }

            Console.WriteLine();
            var data2 = new byte[8];
            for (int j = 0; j < 8; j++)
            {
                data2[j] = (byte)(data[j] << 2);
                data2[j] += (byte)(data[j + 1] >> 6);
            }
            foreach (var item in data2)
            {
                Console.WriteLine("{0:X}", item);
            }
            Console.WriteLine("\n" + BitConverter.ToInt64(data2, 0));

            x._konstruktor2(data);
            var z = x;

            Console.WriteLine("ID "+z.ID + "\nO " + z.Operacja + "\nS " + z.Status + "\nC " + z.Checksum);
            Console.WriteLine(z.L1);
        }

        [TestMethod()]
        public void FrameTestTF()
        {
            FrameTestF(1234);
            FrameTestF(-1234);
            FrameTestF(999999999999);
        }


        private void FrameTestF(long nr)
        {
            /*
            *2b pole operacji
            *pola liczb(64b)
            *4b status
            *4b ID
            *6b suma K
            */

            Console.WriteLine(nr + "\n");

            //var data = x.fuckthis();

            var data = new byte[11];
            var tmp = BitConverter.GetBytes(nr);

            foreach (var item in tmp)
            {
                Console.WriteLine("{0:X}", item);
            }

            Console.WriteLine();

            for (int j = 0; j < tmp.Length; j++)
            {
                data[j] += (byte)(tmp[j] >> 2);
                data[j + 1] = (byte)(tmp[j] << 6);
            }

            foreach (var item in data)
            {
                Console.WriteLine("{0:X}",item);
            }

            Console.WriteLine();
            var data2 = new byte[8];
            for (int j = 0; j < 8; j++)
            {
                data2[j] = (byte)(data[j] << 2);
                data2[j] += (byte)(data[j+1] >> 6);
            }
            foreach (var item in data2)
            {
                Console.WriteLine("{0:X}", item);
            }
            Console.WriteLine("\n"+BitConverter.ToInt64(data2,0));

        }
    }
}
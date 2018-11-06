﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var x =new Frame();
            x.ID = 12;
            x.Operacja = 2;
            x.Status = 3;

            var z = new Frame(x.gen());

            Console.WriteLine(z.ID +"\n"+z.Operacja+"\n"+z.Status+"\n"+z.Checksum);
            z.Liczby.ForEach(item => Console.WriteLine(item));

        }

        [TestMethod()]
        public void FrameTest2()
        {
            var x = new Frame();
            x.ID = 1;
            x.Operacja = 2;
            x.Status = 3;

            var q = new List<double>();
            q.Add(34);
            q.Add(28);

            x.Liczby = q;
            
            var z = new Frame(x.gen());

            Console.WriteLine(z.ID + "\n" + z.Operacja + "\n" + z.Status + "\n" + z.Checksum);
            z.Liczby.ForEach(item=>Console.WriteLine(item));
        }
    }
}
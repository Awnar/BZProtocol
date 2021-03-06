﻿//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Sockets;
//using System.Security.Cryptography.X509Certificates;
//using System.Threading;
//using System.Timers;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using protocol;
//using serwer;
//using Timer = System.Threading.Timer;

//namespace LiczTest
//{
//    [TestClass]
//    public class UnitTest1
//    {
//        /* Statusy:
//         *
//         * ---0 - prośba
//         * ---1 - akceptacja
//         *
//         * 000-(0) - nawiązanie połączenia
//         * 001-(2) - przesłanie liczb/-y
//         * 010-(4) - wykonaj operację
//         * 011-(6) - 
//         * 100-(8) - 
//         * 101-(10) - błąd kody w L1
//         * 110-(12) - przekroczono zakres zmiennej
//         * 111-(14) - zakończenie transmisji
//         */

//        /* Kody błędów
//         * 1 - brak wolnych sesji
//         * 2 - za dużo liczb
//         * 3 - nie znana sesja
//         * 100 - nieznany błąd
//         */

//        public List<Frame> FR = new List<Frame>();

//        [TestMethod]
//        public void dowolnyPakiet()
//        {
//            var __serwer = new Thread(new ThreadStart(serwer));
//            __serwer.Start();

//            IPEndPoint _serwer = new IPEndPoint(IPAddress.Loopback, 8080);

//            var x = new Frame();
//            x.ID = 12;
//            x.Operacja = 2;
//            x.Status = 3;

//            var data = x.gen();
//            var licz = new Processing(data, _serwer);
//            licz.Run();

//        }

//        [TestMethod]
//        public void nieznanaSesja()
//        {
//            var __serwer = new Thread(new ThreadStart(serwer));
//            __serwer.Start();

//            IPEndPoint _serwer = new IPEndPoint(IPAddress.Loopback, 8080);

//            var x = new Frame();
//            x.ID = 12;
//            x.Status = 2;
//            x.L1 = 2;

//            var data = x.gen();
//            var licz = new Processing(data, _serwer);
//            licz.Run();
//        }

//        [TestMethod]
//        public void TworenieSesji()
//        {
//            var __serwer = new Thread(new ThreadStart(serwer));
//            __serwer.Start();

//            IPEndPoint _serwer = new IPEndPoint(IPAddress.Loopback, 8080);

//            for (int i = 0; i <= 16; i++)
//            {
//                var x = new Frame();
//                var data = x.gen();
//                var licz = new Processing(data, _serwer);
//                licz.Run();
//                Thread.Sleep(10);
//            }
//        }

//        [TestMethod]
//        public void liczenie()
//        {
//            var __serwer = new Thread(new ThreadStart(serwer));
//            __serwer.Start();

//            IPEndPoint _serwer = new IPEndPoint(IPAddress.Loopback, 8080);

//            //sesja
//            var x = new Frame();
//            var data = x.gen();
//            var licz = new Processing(data, _serwer);
//            licz.Run();

//            while (FR.Count == 0)
//                Thread.Sleep(100);

//            //wysłanie liczby
//            x.ID = FR[0].ID;
//            x.L1 = 12;
//            x.Status = 2;
//            data = x.gen();
//            licz = new Processing(data, _serwer);
//            licz.Run();

//            //wysłanie liczby
//            x.ID = FR[0].ID;
//            x.L1 = 6;
//            x.Status = 2;
//            data = x.gen();
//            licz = new Processing(data, _serwer);
//            licz.Run();

//            //licz
//            x.ID = FR[0].ID;
//            x.Status = 4;
//            x.Operacja = 2;
//            data = x.gen();
//            licz = new Processing(data, _serwer);
//            licz.Run();
//        }

//        [TestMethod]
//        public void liczenie2()
//        {
//            var __serwer = new Thread(new ThreadStart(serwer));
//            __serwer.Start();

//            IPEndPoint _serwer = new IPEndPoint(IPAddress.Loopback, 8080);

//            //sesja
//            var x = new Frame();
//            var data = x.gen();
//            var licz = new Processing(data, _serwer);
//            licz.Run();

//            while (FR.Count == 0)
//                Thread.Sleep(100);

//            //wysłanie liczby
//            x.ID = FR[0].ID;
//            x.L1 = 120;
//            x.Status = 2;
//            data = x.gen();
//            licz = new Processing(data, _serwer);
//            licz.Run();

//            //wysłanie liczby
//            x.ID = FR[0].ID;
//            x.L1 = 6;
//            x.Status = 2;
//            data = x.gen();
//            licz = new Processing(data, _serwer);
//            licz.Run();

//            //licz
//            x.ID = FR[0].ID;
//            x.Status = 6;
//            x.L1 = 5;
//            x.Operacja = 3;
//            data = x.gen();
//            licz = new Processing(data, _serwer);
//            licz.Run();

//            x.ID = FR[0].ID;
//            x.Status = 14;
//            data = x.gen();
//            licz = new Processing(data, _serwer);
//            licz.Run();
//        }

//        private void xserwer()
//        {
//            var client = new UdpClient(8080);
//            IPEndPoint tmp = new IPEndPoint(IPAddress.Any, 0);

//            bool x = true;

//            System.Timers.Timer t = new System.Timers.Timer();
//            t.Elapsed += new ElapsedEventHandler(this.fail);
//            t.Interval = 2000;
//            t.Start();

//            while (x)
//            {
//                var data = client.Receive(ref tmp);
//                var z = new Frame(data);
//                FR.Add(z);
//                Console.WriteLine(z.ID + "\n" + z.Operacja + "\n" + z.Status + "\n" + z.Checksum + "\n" + z.IleLiczb);
//                Console.WriteLine(z.L1 + "\n" + z.L2 + "\n" + z.L3);
//                Console.WriteLine("---------------");
//                if (z.Wersja > 10) x = false;
//            }
//        }

//        private void serwer()
//        {
//            var timeToWait = TimeSpan.FromSeconds(10);
//            var udpClient = new UdpClient(8080);

//            while (true)
//            {
//                var asyncResult = udpClient.BeginReceive(null, null);
//                asyncResult.AsyncWaitHandle.WaitOne(timeToWait);
//                if (asyncResult.IsCompleted)
//                {
//                    try
//                    {
//                        IPEndPoint remoteEP = null;
//                        byte[] data = udpClient.EndReceive(asyncResult, ref remoteEP);
//                        var z = new Frame(data);
//                        FR.Add(z);
//                        Console.WriteLine(z.ID + "\n" + z.Operacja + "\n" + z.Status + "\n" + z.Checksum + "\n" + z.IleLiczb);
//                        Console.WriteLine(z.L1 + "\n" + z.L2 + "\n" + z.L3);
//                        Console.WriteLine("---------------");
//                    }
//                    catch (Exception ex)
//                    {
//                        // EndReceive failed and we ended up here
//                    }
//                }
//                else
//                {
//                    // The operation wasn't completed before the timeout and we're off the hook
//                    Console.WriteLine("+++++++++++++");
//                }
//            }
//        }

//        private void fail(object sender, EventArgs e)
//        {
//            try
//            {
//                Console.WriteLine("++++++++++");
//                Thread.CurrentThread.Abort();
//            }
//            catch (Exception)
//            {
//            }
//            finally
//            {
//                try
//                {
//                    Console.WriteLine("++++++++++");
//                    Assert.Fail();
//                }
//                catch (Exception)
//                {
//                }
//                finally
//                {
//                    try
//                    {
//                        Console.WriteLine("++++++++++");
//                        Environment.Exit(1);
//                    }
//                    catch (Exception)
//                    {
//                    }
//                }
//            }
//        }
//    }
//}

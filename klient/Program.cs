using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using protocol;

namespace klient
{
    class Program
    {
        private static IPAddress DEFAULT_SERVER = IPAddress.Parse("127.0.0.1");
        private static int DEFAULT_PORT = 9999;

        private static UdpClient serwer = new UdpClient();

        private static byte ID;

        static void Main(string[] args)
        {
            Console.WriteLine("Witaj");
            
            serwer.Connect(DEFAULT_SERVER, DEFAULT_PORT);

            Frame frame = new Frame();
            Send(frame);

            bool running = true;
            while (running)
            {
                try
                {
                    var odp = new Frame(ReceiveLoop());
                    odpS(odp);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }


                frame =new Frame();
                frame.ID = ID;
                // ustalenie statusu

                Console.WriteLine("Co chcesz zrobić?");
                Console.WriteLine("1 - przeslanie liczb");
                Console.WriteLine("2 - wykonanie operacji");
                Console.WriteLine("3 - przeslanie liczb i wykonanie operacji");
                Console.WriteLine("4 - zakonczenie transmisji");
                int choice = Int32.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        frame.Status = 2;
                        break;
                    case 2:
                        frame.Status = 4;
                        break;
                    case 3:
                        frame.Status = 6;
                        break;
                    case 4:
                        frame.Status = 14;
                        break;
                    default:
                        Console.WriteLine("Bład wejscia. Ustawiam na domyślny (przeslanie liczb)");
                        frame.Status = 2;
                        break;
                }

                if (frame.Status == 4 || frame.Status == 6)
                {
                    //ustalenie operacji

                    Console.WriteLine("\nJaka operacje na liczbach chcesz wykonac?");
                    Console.WriteLine("1 - mnozenie");
                    Console.WriteLine("2 - dodawanie");
                    Console.WriteLine("3 - odejmowanie");
                    Console.WriteLine("4 - srednia");
                    choice = Int32.Parse(Console.ReadLine());
                    switch (choice)
                    {
                        case 1:
                            frame.Operacja = 0;
                            break;
                        case 2:
                            frame.Operacja = 1;
                            break;
                        case 3:
                            frame.Operacja = 2;
                            break;
                        case 4:
                            frame.Operacja = 3;
                            break;
                        default:
                            Console.WriteLine("Nie ma takiej operacji. Ustawiam na domyślny (dodawanie)");
                            frame.Operacja = 0;
                            break;
                    }
                }

                if (frame.Status == 2 || frame.Status == 6)
                {
                    // ustalenie ilości liczb i ich wartości
                    void wpiszLiczby(int i)
                    {
                        if (i % 3 == 1)
                        {
                            Console.WriteLine("\nWpisz pierwsza liczbe:");
                            frame.L1 = Int64.Parse(Console.ReadLine());
                        }

                        if (i % 3 == 2)
                        {
                            wpiszLiczby(1);
                            Console.WriteLine("Wpisz druga liczbe:");
                            frame.L2 = Int64.Parse(Console.ReadLine());
                        }

                        if (i % 3 == 0)
                        {
                            wpiszLiczby(2);
                            Console.WriteLine("Wpisz trzecia liczbe:");
                            frame.L3 = Int64.Parse(Console.ReadLine());
                        }
                    }

                    Console.WriteLine("Ile liczb chcesz przeslac? (1-3)");
                    choice = Int32.Parse(Console.ReadLine());
                    if (choice == 1 || choice == 2 || choice == 3)
                        wpiszLiczby(choice);
                    else
                    {
                        Console.WriteLine("Bład wejscia. Ustawiam na domyślny (jedna liczba)");
                        wpiszLiczby(1);
                    }
                }

                Send(frame);

            }
        }

        private static void odpS(Frame fr)
        {
            switch (fr.Status)
            {
                case 1:
                    ID = fr.ID;
                    Console.WriteLine("Nawiązano połączenie, ID to "+ID);
                    break;
                case 3:
                    Console.WriteLine("Liczby dotarły");
                    break;
                case 5:
                case 7:
                    Console.WriteLine("Wynik to: "+fr.L1);
                    break;
                case 10:
                    serwerError(fr.L1);
                    break;
                case 15:
                    Console.WriteLine("Koniec sesji\nNaciśnij dowolny klawisz by zakończyć pracę");
                    Console.ReadKey();
                    Environment.Exit(1);
                    break;
                default:
                    Console.WriteLine("Nieznana odpowiedz serwera");
                    break;
            }
        }

        private static void serwerError(long frL1)
        {
            switch (frL1)
            {
                case 1:
                    Console.WriteLine("Brak dosępnych sesji");
                    Console.ReadKey();
                    Environment.Exit(1);
                    break;
                case 2:
                    Console.WriteLine("Przekroczono zakres zmiennej");
                    break;
                case 3:
                    Console.WriteLine("Brak takiej sesji");
                    break;
                case 100:
                    Console.WriteLine("Serwer nie mógł rozpoznać żądania");
                    break;
                default:
                    Console.WriteLine("Nie znany błąd");
                    break;
            }
        }

        private static void Send(Frame fr)
        {
            var data = fr.gen();
            serwer.Send(data, data.Length);
        }

        private static byte[] ReceiveLoop(byte i = 0)
        {
            try
            {
                return Receive();
            }
            catch (Exception)
            {
                Console.WriteLine("Serwer nie odsyła odpowiedzi, próba:" + (i + 1));
                if (i++ < 3)
                {
                    return ReceiveLoop(i);
                }
                else
                {
                    throw new Exception("Brak odpowiedzi od serwera");
                }
            }
        }


        private static byte[] Receive()
        {
            var timeToWait = TimeSpan.FromSeconds(10);

            var asyncResult = serwer.BeginReceive(null, null);
            asyncResult.AsyncWaitHandle.WaitOne(timeToWait);
            if (asyncResult.IsCompleted)
                try
                {
                    IPEndPoint remoteEP = null;
                    return serwer.EndReceive(asyncResult, ref remoteEP);
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            throw new Exception();
        }
    }
}
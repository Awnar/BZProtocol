using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using protocol;

namespace serwer
{
    public class ThreadLicz
    {
        private Frame _frame;
        private IPEndPoint _client;
        private static DB db = new DB();

        public ThreadLicz(byte[] data, IPEndPoint client)
        {
            try
            {
                _client = client;
                _frame = new Frame(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Run()
        {
           /* Statusy:
            *
            * ---0 - prośba
            * ---1 - akceptacja
            *
            * 000- - nawiązanie połączenia
            * 001- - przesłanie liczb/-y
            * 010- - wykonaj operację
            * 011- - liczby + wykonaj operację
            * 100- - *do przyszłego zastosowania*
            * 101- - błąd (w L1)
            * 110- - przekroczono zakres zmiennej
            * 111- - zakończenie transmisji
            *
            * Błędy:
            * 
            * 1 - brak wolnych miejsc w sesji
            * 2 - przekroczono zakres zmiennej
            * 3 - nieznana sesja
            * 100 - nieznany błąd
            */

            switch (_frame.Status)
            {
                case 0:
                    newSession();
                    break;
                case 2:
                    addnumber();
                    break;
                case 4:
                    calculate();
                    break;
                case 12:
                    endSession();
                    break;
                default:
                    error();
                    break;
            }
        }

        private void calculate()
        {
            var tmp = new Frame();
            tmp.ID = _frame.ID;
            tmp.Status = 5;
            try
            {
                switch (_frame.Operacja)
                {
                    case 0:
                        dodawanie();
                        break;
                    case 1:
                    case 2:
                    case 3:
                        break;
                    default:
                        break;
                }


            }
            catch (Exception )
            {
                tmp.Status = 12;
            }
            finally
            {
                send(tmp);
            }
        }

        private long dodawanie()
        {
            return db.getNumbers(_frame.ID).Sum(item => item);
        }

        private void addnumber()
        {
            var tmp = new Frame();
            try
            {
                switch (_frame.IleLiczb)
                {
                    case 3:
                        db.addNumbers(_frame.ID, _frame.L3);
                        db.addNumbers(_frame.ID, _frame.L2);
                        db.addNumbers(_frame.ID, _frame.L1);
                        break;
                    case 2:
                        db.addNumbers(_frame.ID, _frame.L2);
                        db.addNumbers(_frame.ID, _frame.L1);
                        break;
                    case 1:
                        db.addNumbers(_frame.ID, _frame.L1);
                        break;
                }
                tmp.ID = _frame.ID;
                tmp.Status = 3;
            }
            catch (Exception)
            {
                tmp.Status = 10;
                tmp.L1 = 3;
            }
            finally
            {
                send(tmp);
            }
        }

        private void endSession()
        {
            db.unlockID(_frame.ID);
            var tmp = new Frame();

            tmp.Status = 13;
            tmp.Status = _frame.ID;

            send(tmp);
        }

        private void newSession()
        {
            var nID = db.getFreeID();

            var tmp = new Frame();

            if (nID == 0)
            {
                tmp.Status = 10;
                tmp.L1 = 1;
            }
            else
                tmp.Status = 1;
            tmp.ID = nID;

            send(tmp);
        }

        private void error()
        {
            var tmp = new Frame();
            tmp.Status = 10;
            tmp.L1 = 100;
            send(tmp);
        }

        private void send(Frame frame)
        {
            var data = frame.gen().ToArray();
            UdpClient a = new UdpClient();
            a.Connect(_client);
            a.Send(data, data.Length);
            a.Close();
        }
    }
}
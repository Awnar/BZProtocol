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
        private static DB db = new DB();
        private byte[] data;

        public ThreadLicz(byte[] data)
        {
            try
            {
                _frame = new Frame(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public byte[] Run()
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

            Console.WriteLine(DateTime.Now + " ID:" + _frame.ID + " S:" + _frame.Status);
            lock (db)
            {
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
                    case 6:
                        addnumber(false);
                        calculate(2);
                        break;
                    case 14:
                        endSession();
                        break;
                    default:
                        error();
                        break;
                }
            }
            return data;
        }

        private void calculate(byte s = 0)
        {
            var tmp = new Frame();
            tmp.ID = _frame.ID;
            tmp.Status = (byte)(s + 5);
            try
            {
                switch (_frame.Operacja)
                {
                    case 0:
                        tmp.L1 = mnozenie();
                        break;
                    case 1:
                        tmp.L1 = dodawanie();
                        break;
                    case 2:
                        tmp.L1 = odejmowanie();
                        break;
                    case 3:
                        tmp.L1 = srednia();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                tmp.Status = 12;
            }
            finally
            {
                data = tmp.gen().ToArray();
            }
        }

        private long dodawanie()
        {
            return db.getNumbers(_frame.ID).Sum(item => item);
        }

        private long odejmowanie()
        {
            var tmp = db.getNumbers(_frame.ID);
            var result = tmp[0];
            for (int i = 1; i < tmp.Count; i++)
                result -= tmp[i];
            return result;
        }

        private long mnozenie()
        {
            var tmp = db.getNumbers(_frame.ID);
            var result = tmp[0];
            for (int i = 1; i < tmp.Count; i++)
                result *= tmp[i];
            return result;
        }

        private long srednia()
        {
            return (long)db.getNumbers(_frame.ID).Average(item => item);
        }


        private void addnumber(bool x = true)
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
                data = tmp.gen().ToArray();
            }
        }

        private void endSession()
        {
            db.unlockID(_frame.ID);
            var tmp = new Frame();

            tmp.Status = 15;
            tmp.ID = _frame.ID;

            data = tmp.gen().ToArray();
        }

        private void newSession()
        {
            var tmp = new Frame();
            try
            {
                var nID = db.getFreeID();
                tmp.Status = 1;
                tmp.ID = nID;
            }
            catch (Exception)
            {
                tmp.Status = 10;
                tmp.L1 = 1;
            }
            finally
            {
                data = tmp.gen().ToArray();
            }
        }

        private void error()
        {
            var tmp = new Frame();
            tmp.Status = 10;
            tmp.L1 = 100;
            data = tmp.gen().ToArray();
        }
    }
}
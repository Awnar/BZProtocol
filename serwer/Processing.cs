using System;
using System.Collections.Generic;
using System.Data;
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
    public class Processing
    {
        private Datagram _frame;
        private static DB db = new DB();
        private Datagram data;

        public Processing(byte[] data)
        {
            try
            {
                _frame = new Datagram();
                _frame._konstruktor2(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
             * 4 - brak liczb
             * 100 - nieznany błąd
             * 101 - nie obsługiwana wersja protokołu
             */

            Console.WriteLine(DateTime.Now + " Client O: " + _frame.Operacja + " DATA: " + _frame.L1 + " S: " +
                              _frame.Status + " ID: " + _frame.ID + " SK: " + _frame.Checksum);

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

            //return data.gen2().ToArray();
            //var a = data.gen2().ToArray();
            var b = data.gen2().ToArray();
            data._konstruktor2(b);

            Console.WriteLine(DateTime.Now + " Server O: " + data.Operacja + " DATA: " + data.L1 + " S: " +
                              data.Status + " ID: " + data.ID + " SK: " + data.Checksum);

            return data.gen2().ToArray();
        }

        private void calculate(byte s = 0)
        {
            var tmp = new Datagram();
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
            catch (InvalidExpressionException)
            {
                tmp.Status = 10;
                tmp.L1 = 3;
            }
            catch (OverflowException)
            {
                tmp.Status = 10;
                tmp.L1 = 2;
            }
            catch (Exception)
            {
                tmp.Status = 10;
                tmp.L1 = 4;
            }
            finally
            {
                db.clearNumbers(_frame.ID);
                data = tmp;
            }
        }

        private long dodawanie()
        {
            if(db.getNumbers(_frame.ID)==null) throw new Exception();
            checked
            {
                return db.getNumbers(_frame.ID).Sum(item => item);
            }
        }

        private long odejmowanie()
        {
            checked
            {
                var tmp = db.getNumbers(_frame.ID);
                if (tmp == null) throw new Exception();
                var result = tmp[0];
                for (int i = 1; i < tmp.Count; i++)
                    result -= tmp[i];
                return result;
            }
        }

        private long mnozenie()
        {
            checked
            {
                var tmp = db.getNumbers(_frame.ID);
                if (tmp == null) throw new Exception();
                var result = tmp[0];
                for (int i = 1; i < tmp.Count; i++)
                    result *= tmp[i];
                return result;
            }
        }

        private long srednia()
        {
            if (db.getNumbers(_frame.ID) == null) throw new Exception();
            checked
            {
                return (long) db.getNumbers(_frame.ID).Average(item => item);
            }
        }


        private void addnumber(bool x = true)
        {
            var tmp = new Datagram();
            try
            {
                switch (_frame.IleLiczb)
                {
                    case 3:
                        db.addNumbers(_frame.ID, _frame.L1);
                        db.addNumbers(_frame.ID, _frame.L2);
                        db.addNumbers(_frame.ID, _frame.L3);
                        break;
                    case 2:
                        db.addNumbers(_frame.ID, _frame.L1);
                        db.addNumbers(_frame.ID, _frame.L2);
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
                data = tmp;
            }
        }

        private void endSession()
        {
            db.unlockID(_frame.ID);
            var tmp = new Datagram();

            tmp.Status = 15;
            tmp.ID = _frame.ID;

            data = tmp;
        }

        private void newSession()
        {
            var tmp = new Datagram();
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
                data = tmp;
            }
        }

        private void error()
        {
            var tmp = new Datagram();
            tmp.Status = 10;
            tmp.L1 = 100;
            data = tmp;
        }
    }
}
﻿using System;
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
             * 000-(0) - nawiązanie połączenia
             * 001-(2) - przesłanie liczb/-y
             * 010-(4) - wykonaj operację
             * 011-(6) - liczby + wykonaj operację
             * 100-(8) - 
             * 101-(10) - błąd kody w L1
             * 110-(12) - przekroczono zakres zmiennej
             * 111-(14) - zakończenie transmisji
             */

            /* Kody błędów
             * 1 - brak wolnych sesji
             * 2 - za dużo liczb
             * 3 - nie znana sesja
             * 100 - nieznany błąd
             */
            Console.WriteLine(DateTime.Now + " ID:"+_frame.ID+" S:"+_frame.Status);
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
                        addnumber();
                        calculate(2);
                        break;
                    case 12:
                        endSession();
                        break;
                    default:
                        error();
                        break;
                }
            }
        }

        private void calculate(byte s=0)
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
            catch (Exception )
            {
                tmp.Status = 12;
            }
            finally
            {
                send(tmp);
            }
        }

        private double dodawanie()
        {
            return db.getNumbers(_frame.ID).Sum(item => item);
        }

        private double odejmowanie()
        {
            var tmp = db.getNumbers(_frame.ID);
            var result = tmp[0];
            for (int i = 1; i < tmp.Count; i++)
                result -= tmp[i];
            return result;
        }

        private double mnozenie()
        {
            var tmp = db.getNumbers(_frame.ID);
            var result = tmp[0];
            for (int i = 1; i < tmp.Count; i++)
                result *= tmp[i];
            return result;
        }

        private double srednia()
        {
            return db.getNumbers(_frame.ID).Average(item => item);
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
            Console.WriteLine(DateTime.Now + "Odpowiedz dla ID:" + frame.ID + " S:" + frame.Status);
        }
    }
}
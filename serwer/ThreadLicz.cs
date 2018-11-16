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
    class ThreadLicz
    {
        private Frame _frame;
        private IPEndPoint _client;

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
             * 011-(6) - 
             * 100-(8) - 
             * 101-(10) - błąd kody w L1
             * 110-(12) - przekroczono zakres zmiennej
             * 111-(14) - zakończenie transmisji
             */

             /* Kody błędów
              * 1 - brak wolnych sesji
              * 2 - za dużo liczb
              * 100 - nieznany błąd
              */


            switch (_frame.Status)
            {
                case (0):
                    newSession();
                    break;
                case (2):
                    addnumber();
                    break;
                case (12):
                    endSession();
                    break;
                default:
                    error();
                    break;
            }
        }

        private void addnumber()
        {
            //if()

            throw new NotImplementedException();
        }

        private void endSession()
        {
            DB.unlockID(_frame.ID);
            var tmp = new Frame();

            tmp.Status = 13;
            tmp.Status = _frame.ID;

            var data = tmp.gen().ToArray();
            UdpClient a = new UdpClient(_client);
            a.Send(data, data.Length);
        }

        private void newSession()
        {
            var nID = DB.getFreeID();

            var tmp = new Frame();

            if (nID == 0)
            {
                tmp.Status = 10;
                tmp.L1 = 1;
            }
            else
                tmp.Status = 1;
            tmp.ID = nID;

            var data = tmp.gen().ToArray();
            UdpClient a=new UdpClient(_client);
            a.Send(data, data.Length);
        }

        private void error()
        {
            var tmp = new Frame();
            tmp.Status = 10;
            tmp.L1 = 100;
            var data = tmp.gen().ToArray();
            UdpClient a = new UdpClient(_client);
            a.Send(data, data.Length);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace protocol
{
    public class Frame
    {
        /* ?b wersji
         * 
         * 
         * 2b pole operacji
         * -----------------------
         * 4b status
         * 4b ID
         * -----------------------
         * 16b suma kontrolna
         * -----------------------
         * pola liczb (64b)
         * 
         */

        byte _pole1, _pole2;
        char _sumaKomtrolna;
        List<double> _liczby;

        public Frame(List<byte> bytes)
        {
            this.Operacja = 11;
        }

        public byte Operacja
        {
            get
            {
                return (byte)(_pole1 & 0x03);
            }
            set
            {
                _pole1 = (byte)((_pole1 & 0xfc) + (value & 0x03));
            }
        }

        public byte Status
        {
            get
            {
                return (byte)(_pole2 & 0xf0);
            }
            set
            {
                _pole2 = (byte)((_pole2 & 0x0f) + (value << 4));
            }
        }

        public byte ID
        {
            get
            {
                return (byte)(_pole2 & 0x0f);
            }
            set
            {
                _pole2 = (byte)((_pole2 & 0xf0) + (value & 0x0f));
            }
        }

        public bool Checksum
        {
            get
            {
                return false;
            }
        }

        private byte _Checksum()
        {
            int tmp = _pole1 + _pole2;
            foreach (var it in _liczby)
                tmp += BitConverter.GetBytes(it).Sum(item => item);

        }
    }
}

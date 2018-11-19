using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace protocol
{
    public class Frame
    {
        /* 4b wersji
         * 4b status
         * -----------------------
         * 2b pole operacji
         * 2b ile liczb
         * 4b ID
         * -----------------------
         * pola liczb (64b)*3
         * -----------------------
         * 16b suma kontrolna
         */

        /* Statusy:
         *
         * ---0 - prośba
         * ---1 - akceptacja
         *
         * 000- - nawiązanie połączenia
         * 001- - przesłanie liczb/-y
         * 010- - wykonaj operację
         * 011- - liczby + wykonaj operację
         * 100- - 
         * 101- - błędy w L1
         * 110- - przekroczono zakres zmiennej
         * 111- - zakończenie transmisji
         */

        byte _pole1, _pole2;
        char _sumaKomtrolna;
        bool _sumaKomtrolnaB;
        double[] _liczby;

        static byte _wersja = 1;

        public Frame()
        {
            Wersja = _wersja;
            _liczby = new double[3] {0, 0, 0};
        }

        public byte[] gen()
        {
            var gen = new List<byte>();
            gen.Add(_pole1);
            gen.Add(_pole2);

            gen.AddRange(BitConverter.GetBytes(_liczby[0]));
            gen.AddRange(BitConverter.GetBytes(_liczby[1]));
            gen.AddRange(BitConverter.GetBytes(_liczby[2]));

            _sumaKomtrolna = _Checksum();
            gen.AddRange(BitConverter.GetBytes(_sumaKomtrolna));
            _sumaKomtrolnaB = true;

            return gen.ToArray();
        }

        public Frame(List<byte> bytes)
        {
            _konstruktor(bytes.ToArray());
        }

        public Frame(byte[] bytes)
        {
            _konstruktor(bytes);
        }

        private void _konstruktor(byte[] bytes)
        {
            if (bytes.Length != 28) throw new Exception();

            _pole1 = bytes[0];

            //if(Wersja!=1) throw new Exception();

            _pole2 = bytes[1];

            _liczby = new double[3];

            _liczby[0] = BitConverter.ToDouble(bytes, 2);
            _liczby[1] = BitConverter.ToDouble(bytes, 10);
            _liczby[2] = BitConverter.ToDouble(bytes, 18);

            _sumaKomtrolna = BitConverter.ToChar(bytes, 26);

            if (_Checksum() == _sumaKomtrolna)
                _sumaKomtrolnaB = true;
            else
                _sumaKomtrolnaB = false;

        }

        public byte Operacja
        {
            get { return (byte) (_pole2 >> 6); }
            set { _pole2 = (byte) ((_pole2 & 0x3f) + (value << 6)); }
        }

        public byte Status
        {
            get { return (byte) (_pole1 & 0x0f); }
            set { _pole1 = (byte) ((_pole1 & 0xf0) + (value & 0x0f)); }
        }

        public byte ID
        {
            get { return (byte) (_pole2 & 0x0f); }
            set { _pole2 = (byte) ((_pole2 & 0xf0) + (value & 0x0f)); }
        }

        public bool Checksum
        {
            get { return _sumaKomtrolnaB; }
        }

        public byte IleLiczb
        {
            get { return (byte) ((_pole2 & 0x30)>>4); }
            private set { _pole2 = (byte) ((_pole2 & 0xcf) + (value << 4)); }
        }

        public byte Wersja
        {
            get { return (byte) (_pole1 >> 4); }
            set { _pole1 = (byte) ((_pole1 & 0x0f) + (value << 4)); }
        }

        public double L1
        {
            get { return _liczby[0]; }
            set
            {
                if (IleLiczb < 1) IleLiczb = 1;
                _liczby[0] = value;
            }
        }

        public double L2
        {
            get { return _liczby[1]; }
            set
            {
                if (IleLiczb < 2) IleLiczb = 2;
                _liczby[1] = value;
            }
        }

        public double L3
        {
            get { return _liczby[2]; }
            set
            {
                if (IleLiczb < 3) IleLiczb = 3;
                _liczby[2] = value;
            }
        }

        private char _Checksum()
        {
            int tmp = _pole1 + _pole2;
            if (_liczby != null)
                foreach (var it in _liczby)
                    tmp += BitConverter.GetBytes(it).Sum(item => item);
            while (tmp > 0xffff)
            {
                tmp = (tmp & 0xffff) + (tmp >> 16);
            }

            return (char) tmp;
        }
    }
}
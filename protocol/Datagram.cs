using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace protocol
{
    public class Datagram
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
         *
         *
         *
         *
         * 2b pole operacji
         * pola liczb (64b)
         * 4b status
         * 4b ID
         * 6b suma K
         * 
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
         */

        byte _pole1, _pole2;
        char _sumaKomtrolna;
        bool _sumaKomtrolnaB;
        long[] _liczby;
        private long liczba;

        static byte _wersja = 1;

        public Datagram()
        {
            Wersja = _wersja;
            _liczby = new long[3] {0, 0, 0};
        }

        public byte[] fuckthis()
        {
            liczba = L1;
            var data = new byte[10];
            //for (int i = 0; i < 11; i++)
            //{
            //    data[i] = 0xff;
            //}
            data[0] = (byte) (Operacja << 6);
            var tmp = BitConverter.GetBytes(liczba);
            for (int j = 0; j < tmp.Length; j++)
            {
                data[j] += (byte) (tmp[j] >> 2);
                data[j + 1] = (byte) (tmp[j] << 6);
            }

            data[8] += (byte) ((0x0f & Status) << 2);
            data[8] += (byte)((0x0f & ID) >> 2);
            data[9] += (byte)((0x0f & ID) << 6);
            data[9] += (byte) 0x3f;
            return data;
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

            return (char)tmp;
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

        private void _konstruktor(byte[] bytes)
        {
            if (bytes.Length != 28) throw new Exception("Datagram ma złą długość");

            _pole1 = bytes[0];

            if (Wersja != _wersja) throw new Exception("Zła wersja odebranego protokołu");

            _pole2 = bytes[1];

            _liczby = new long[3];

            _liczby[0] = BitConverter.ToInt64(bytes, 2);
            _liczby[1] = BitConverter.ToInt64(bytes, 10);
            _liczby[2] = BitConverter.ToInt64(bytes, 18);

            _sumaKomtrolna = BitConverter.ToChar(bytes, 26);

            _sumaKomtrolnaB = _Checksum() == _sumaKomtrolna;

            if(Checksum==false) throw new Exception("Błąd sumy kontrolnej");
        }

        public Datagram(List<byte> bytes)
        {
            _konstruktor(bytes.ToArray());
        }

        public Datagram(byte[] bytes)
        {
            _konstruktor(bytes);
        }

        public byte Operacja
        {
            get => (byte) (_pole2 >> 6);
            set => _pole2 = (byte) ((_pole2 & 0x3f) + (value << 6));
        }

        public byte Status
        {
            get => (byte) (_pole1 & 0x0f);
            set => _pole1 = (byte) ((_pole1 & 0xf0) + (value & 0x0f));
        }

        public byte ID
        {
            get => (byte) (_pole2 & 0x0f);
            set => _pole2 = (byte) ((_pole2 & 0xf0) + (value & 0x0f));
        }

        public bool Checksum => _sumaKomtrolnaB;

        public byte IleLiczb
        {
            get => (byte) ((_pole2 & 0x30)>>4);
            set => _pole2 = (byte) ((_pole2 & 0xcf) + (value << 4));
        }

        public byte Wersja
        {
            get => (byte) (_pole1 >> 4);
            set => _pole1 = (byte) ((_pole1 & 0x0f) + (value << 4));
        }

        public long L1
        {
            get => _liczby[0];
            set
            {
                if (IleLiczb < 1) IleLiczb = 1;
                _liczby[0] = value;
            }
        }

        public long L2
        {
            get => _liczby[1];
            set
            {
                if (IleLiczb < 2) IleLiczb = 2;
                _liczby[1] = value;
            }
        }

        public long L3
        {
            get => _liczby[2];
            set
            {
                if (IleLiczb < 3) IleLiczb = 3;
                _liczby[2] = value;
            }
        }
    }
}
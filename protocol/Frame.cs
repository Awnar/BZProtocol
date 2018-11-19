﻿using System;
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
        long[] _liczby;

        static byte _wersja = 1;

        public Frame()
        {
            Wersja = _wersja;
            _liczby = new long[3] {0, 0, 0};
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
            if (bytes.Length != 28) throw new Exception();

            //if(Wersja != 1) throw new Exception();

            _pole1 = bytes[0];
            _pole2 = bytes[1];

            _liczby = new long[3];

            _liczby[0] = BitConverter.ToInt64(bytes, 2);
            _liczby[1] = BitConverter.ToInt64(bytes, 10);
            _liczby[2] = BitConverter.ToInt64(bytes, 18);

            _sumaKomtrolna = BitConverter.ToChar(bytes, 26);

            _sumaKomtrolnaB = _Checksum() == _sumaKomtrolna;
        }

        public Frame(List<byte> bytes)
        {
            _konstruktor(bytes.ToArray());
        }

        public Frame(byte[] bytes)
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
            private set => _pole2 = (byte) ((_pole2 & 0xcf) + (value << 4));
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
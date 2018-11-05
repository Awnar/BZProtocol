﻿using System;
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

        /* Statusy:
         *
         * ---0 - prośba
         * ---1 - akceptacja
         *
         * 000- - nawiązanie połączenia
         * 001- - przesłanie liczb/-y
         * 010- - wykonaj operację
         * 011- - 
         * 100- - 
         * 101- - 
         * 110- - przekroczono zakres zmiennej
         * 111- - zakończenie transmisji
         */

        byte _pole1, _pole2;
        char _sumaKomtrolna;
        bool _sumaKomtrolnaB;
        List<double> _liczby;

        public Frame(List<byte> bytes)
        {
            _konstruktor(bytes.ToArray());
        }

        public Frame(byte[] bytes)
        {
            _konstruktor(bytes.ToArray());
        }

        private void _konstruktor(byte[] bytes)
        {
            _pole1 = bytes[0];
            _pole2 = bytes[1];

            _sumaKomtrolna = BitConverter.ToChar(bytes, 2);

            if (_Checksum() == _sumaKomtrolna)
                _sumaKomtrolnaB = true;
            else
                _sumaKomtrolnaB = false;

            for (int i = 4; i < bytes.Length; i+=8)
            {
                _liczby.Add(BitConverter.ToDouble(bytes, 4));
            }
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
                return (byte)((_pole2 & 0xf0) >> 4);
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
                return _sumaKomtrolnaB;
            }
        }

        private char _Checksum()
        {
            int tmp = _pole1 + _pole2;
            foreach (var it in _liczby)
                tmp += BitConverter.GetBytes(it).Sum(item => item);
            while (tmp>0xffff)
            {
                tmp = (tmp & 0xffff) + (tmp >> 16);
            }
            return (char)tmp;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace serwer
{
    public static class DB
    {
        private struct session
        {
            public List<double> numbers;
            public byte lastSStatus;
        }

        private const byte MAX =15;
        private static session[] db=new session[MAX];
        private static bool[] dbb = new bool[MAX];

        public static List<double> getNumbers(byte ID)
        {
            if (dbb[ID] == true)
                return db[ID].numbers;
            return null;
        }

        public static void addNumbers(byte ID,List<double>numbers)
        {
            if (dbb[ID] == true)
                db[ID].numbers.AddRange(numbers);
            else
                throw new Exception();
        }

        public static void addNumbers(byte ID, double numbers)
        {
            if (dbb[ID] == true)
                db[ID].numbers.Add(numbers);
            else
                throw new Exception();
        }

        public static void clearNumbers(byte ID)
        {
            if (dbb[ID] == true)
                db[ID].numbers.Clear();
        }

        public static byte getStatus(byte ID)
        {
            if (dbb[ID] == true)
                return db[ID].lastSStatus;
            return 0;
        }

        public static void setStatus(byte ID, byte status)
        {
            if (dbb[ID] == true)
                db[ID].lastSStatus = status;
        }

        public static byte getFreeID()
        {
            for (byte i = 1; i < MAX; i++)
                if (dbb[i] == false)
                {
                    dbb[i] = true;
                    db[i]=new session();
                    db[i].numbers=new List<double>();
                    return i;
                }
            return 0;
        }

        public static void unlockID(byte ID)
        {
            dbb[ID] = false;
            clearNumbers(ID);
        }
    }
}

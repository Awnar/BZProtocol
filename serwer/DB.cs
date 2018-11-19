using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace serwer
{
    public class DB
    {
        private struct session
        {
            public List<double> numbers;
            public byte lastSStatus;
        }

        private const byte MAX =15;
        private static session[] db=new session[MAX];
        private static bool[] dbb = new bool[MAX];

        public List<double> getNumbers(byte ID)
        {
            if (dbb[ID] == true)
                return db[ID].numbers;
            return null;
        }

        public void addNumbers(byte ID,List<double>numbers)
        {
            if (dbb[ID] == true)
                db[ID].numbers.AddRange(numbers);
            else
                throw new Exception();
        }

        public void addNumbers(byte ID, double numbers)
        {
            if (dbb[ID] == true)
                db[ID].numbers.Add(numbers);
            else
                throw new Exception();
        }

        public void clearNumbers(byte ID)
        {
            if (dbb[ID] == true)
                db[ID].numbers.Clear();
        }

        public byte getStatus(byte ID)
        {
            if (dbb[ID] == true)
                return db[ID].lastSStatus;
            return 0;
        }

        public void setStatus(byte ID, byte status)
        {
            if (dbb[ID] == true)
                db[ID].lastSStatus = status;
        }

        public byte getFreeID()
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

        public void unlockID(byte ID)
        {
            dbb[ID] = false;
            clearNumbers(ID);
        }
    }
}

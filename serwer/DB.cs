using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
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
            public List<long> numbers;
        }

        private const byte MAX = 16;
        private static session[] db = new session[MAX];
        private static bool[] dbb = new bool[MAX];

        public List<long> getNumbers(byte ID)
        {
            return dbb[ID] ? db[ID].numbers : null;
        }

        public void addNumbers(byte ID, List<long> numbers)
        {
            if (dbb[ID])
                db[ID].numbers.AddRange(numbers);
            else
                throw new InvalidExpressionException();
        }

        public void addNumbers(byte ID, long numbers)
        {
            if (dbb[ID])
                db[ID].numbers.Add(numbers);
            else
                throw new InvalidExpressionException();
        }

        public void clearNumbers(byte ID)
        {
            if (dbb[ID])
                db[ID].numbers.Clear();
        }

        public byte getFreeID()
        {
            for (byte i = 0; i < MAX; i++)
                if (dbb[i] == false)
                {
                    dbb[i] = true;
                    db[i] = new session {numbers = new List<long>()};
                    return i;
                }
            throw new Exception();
        }

        public void unlockID(byte ID)
        {
            dbb[ID] = false;
            clearNumbers(ID);
        }
    }
}

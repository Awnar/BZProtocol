using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace serwer
{
    static class DB
    {
        private struct session
        {
            double[] L;
            byte ileLiczb;
            byte lastSStatus;
        }


        private const byte MAX =15;
        private static session[] db=new session[MAX];
        private static bool[] dbb = new bool[MAX];



        public static byte getFreeID()
        {
            for (byte i = 1; i <= MAX; i++)
                if (dbb[i] == true)
                {
                    dbb[i] = false;
                    db[i]=new session();
                    return i;
                }
            return 0;
        }

        public static void unlockID(byte ID)
        {
            dbb[ID] = true;
        }
    }
}

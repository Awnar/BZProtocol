using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using protocol;

namespace serwer
{
    class ThreadLicz
    {
        private Frame _frame;
        private UdpClient _client;

        public void Start(byte[] data, UdpClient client)
        {
            _client = client;
            _frame = new Frame(data);
            run();
        }

        private void run()
        {
            switch (_frame.Status)
            {
            }
        }
    }
}
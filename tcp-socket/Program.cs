using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace tcp_socket
{
    class Program
    {
        static void Main(string[] args)
        {
            Program program= new();

            program.Connect(8080, "127.0.0.1");
        }

        private void Connect(int port, string adress)
        {

            TcpClient client = new(adress, port);

            NetworkStream stream = client.GetStream();

            Thread readThread = new Thread(() => ReadMessage(stream));
            readThread.Start();

            while (true)
            {
                byte[] messageBuff = CreateMessage(1, Console.ReadLine());
                stream.Write(messageBuff);
            }
       
        }

        private void ReadMessage(NetworkStream stream)
        {
            while (true)
            {
                var message = new byte[8096];
                stream.Read(message);

                var mtype = BitConverter.ToUInt32(message[..4]);
                var mlen = BitConverter.ToUInt32(message[4..8]);
                var rMessage = Encoding.UTF8.GetString(message[8..]);

                String s = String.Format(" {0} {1} {2}", mtype, mlen, rMessage);
                Console.WriteLine(s);
            }
           
        }

        private byte[] CreateMessage(int mtype,string data)
        {
            var packet = new List<byte>(4+4+data.Length);

            packet.AddRange(BitConverter.GetBytes((uint)mtype));
            
            var message = Encoding.UTF8.GetBytes(data);
            packet.AddRange(BitConverter.GetBytes((uint)data.Length));
            packet.AddRange(message);


            return packet.ToArray();

        }
    }
}

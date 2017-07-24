using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
//using System.Runtime.Serialization.Json;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace TCPLib
{
    public class Client
    {

        private byte[] buffer;
        private int buffersize = 1024;

        private ConcurrentQueue<Package> packageQueue;
        private ConcurrentQueue<Package> sendQueue;
        //private ConcurrentQueue<byte> byteQueue;
        private ConcurrentQueue<byte> byteQueue;

        private Thread mReceiveThread;
        private Thread mParseThread;
        private Thread mSendingThread;

        /// <summary>
        /// 最大的监听数量6
        /// </summary>
        private int maxClientCount;
        public int MaxClientCount
        {
            get { return maxClientCount; }
            set { maxClientCount = value; }
        }

        /// <summary>
        /// IP地址
        /// </summary>
        private string ip;
        public string IP
        {
            get { return ip; }
            set { ip = value; }
        }

        /// <summary>
        /// 端口号
        /// </summary>
        private int port;
        public int Port
        {
            get { return port; }
            set { port = value; }
        }


        /// <summary>
        /// IP终端
        /// </summary>
        private IPEndPoint ipEndPoint;


        /// <summary>
        /// 当前客户端Socket
        /// </summary>
        private Socket mClientSocket;

        public Socket ClientSocket
        {
            get { return mClientSocket; }
            set { mClientSocket = value; }
        }



        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="count">监听的最大数目</param>
        public Client(string ip, int port, int buffersize = 1024)
        {
            this.ip = ip;
            this.port = port;
            this.buffersize = buffersize;
            this.buffer = new byte[buffersize];


            this.packageQueue = new ConcurrentQueue<Package>();
            this.byteQueue = new ConcurrentQueue<byte>();
            this.sendQueue = new ConcurrentQueue<Package>();

            //初始化IP终端
            this.ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            //initialize the client socket
            this.mClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
        }

        /// <summary>
        /// 定义一个Start方法将构造函数中的方法分离出来
        /// </summary>
        public void Start()
        {
            // Connect to server
            this.mClientSocket.Connect(this.ipEndPoint);
            
            this.mReceiveThread = new Thread(this.ReceiveMsg);
            this.mReceiveThread.Start();

            this.mParseThread = new Thread(this.ParseMsg);
            this.mParseThread.Start();

            this.mSendingThread = new Thread(this.PackageSender);
            this.mSendingThread.Start();

        }

        public void Stop()
        {
            //this.mReceiveThread.Abort();
            //this.mParseThread.Abort();
            //this.mSendingThread.Abort();
            this.mReceiveThread.IsBackground = true;
            this.mParseThread.IsBackground = true;
            this.mSendingThread.IsBackground = true;
        }



        /// <summary>
        /// Fetch a request sent from clients, if the queue is empty, return null.
        /// </summary>
        /// <returns></returns>
        public Package FetchRequest()
        {
            bool flag;
            Package pac;
            flag = packageQueue.TryDequeue(out pac);
            if (flag) return pac;
            return null;
        }

        public void SendMsg(string msg)
        {
            Package pac = new Package(Encoding.UTF8.GetBytes(msg));
            this.sendQueue.Enqueue(pac);
        }



        /// <summary>
        /// 接收客户端消息的方法
        /// </summary>
        private void ReceiveMsg()
        {

            System.Console.WriteLine("Recieve Client thread initialized.");

            // 循环标志位
            bool flag = true;
            while (flag)
            {
                try
                {
                    //获取数据长度
                    int receiveLength = mClientSocket.Receive(buffer);
                    //获取客户端消息
                    for (int i = 0; i < receiveLength; i++) 
                    {
                        byte b = buffer[i];
                        byteQueue.Enqueue(b);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Receive thread error" + e);
                    break;
                }
            }

        }

        private void PackageSender()
        {
            Console.WriteLine("Package sender intialized");
            Package pac = null;
            while (true)
            {
                if (!sendQueue.TryDequeue(out pac)) continue;
                this.SendPackage(pac);
            }
        }

        /// <summary>
        /// Parse the bytes
        /// </summary>
        /// <param name="obj"></param>
        private void ParseMsg()
        {
            System.Console.WriteLine("Parse Client thread initialized.");
            ConcurrentQueue<byte> Q = byteQueue;
            int length, cnt;
            length = -2;
            cnt = -1;
            List<byte> list = new List<byte>();
            while (true)
            {
                byte b;
                bool flag = Q.TryDequeue(out b);
                if (!flag)
                {
                    continue;
                }
                if (length == -2)
                {
                    if (cnt == -1)
                    {
                        cnt = (int)b;
                    }
                    else
                    {
                        length = cnt + 256 * (int)b;
                        cnt = 0;
                    }
                }
                else
                {
                    list.Add(b);
                    cnt++;
                    if (cnt == length)
                    {
                        byte[] data;
                        data = list.ToArray();
                        packageQueue.Enqueue(new Package(data));
                        length = -2;
                        cnt = -1;
                    }
                }
            }
        }

        private void SendPackage(Package pack)
        {
            this.mClientSocket.Send(pack.Data);
        }



        //public static void Main(string[] args)
        //{
        //    Client client = new Client("10.172.150.34", 10010);
        //    System.Console.WriteLine(string.Format("The IP of server is {0}", client.IP));
        //    string rootpath = "D:/v-hanqw/3D_DeepLearning_dataset/ShapeNet";
        //    client.Start();
        //    while (true)
        //    {
        //        string command = Console.ReadLine();
        //        if (command.CompareTo("fetch") == 0)
        //        {
        //            Package rq = client.FetchRequest();
        //            if (rq == null)
        //            {
        //                Console.WriteLine("Request queue is empty.");
        //                continue;
        //            }
        //            Console.WriteLine(Encoding.UTF8.GetString(rq.Msg));
        //        }
        //        if (command.CompareTo("msg") == 0)
        //        {
        //            string msg = Console.ReadLine();
        //            client.SendMsg(msg);

        //        }
        //    }
        //}
    }
}



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
    public class TCPServer
    {
        
        private byte[] buffer ;
        private int buffersize = 1024;

        private ConcurrentQueue<Request> fileQueue;
        private ConcurrentQueue<Request> requestQueue;
        //private ConcurrentQueue<byte> byteQueue;
        private Dictionary<Socket, ConcurrentQueue<byte>> byteQueues;
        
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
        /// 客户端列表
        /// </summary>
        private List<Socket> mClientSockets;
        public List<Socket> ClientSockets
        {
            get { return mClientSockets; }
        }

        /// <summary>
        /// IP终端
        /// </summary>
        private IPEndPoint ipEndPoint;

        /// <summary>
        /// 服务端Socket
        /// </summary>
        private Socket mServerSocket;

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
        /// <param name="port">端口号</param>
        /// <param name="count">监听的最大树目</param>
        public TCPServer(int port, int count, int buffersize = 1024)
        {
            this.ip = IPAddress.Any.ToString();
            this.port = port;
            this.maxClientCount = count;
            this.buffersize = buffersize;
            this.buffer = new byte[buffersize];

            this.mClientSockets = new List<Socket>();
            this.fileQueue = new ConcurrentQueue<Request>();
            this.requestQueue = new ConcurrentQueue<Request>();
            this.byteQueues = new Dictionary<Socket, ConcurrentQueue<byte>>();

            //初始化IP终端
            this.ipEndPoint = new IPEndPoint(IPAddress.Any, port);
            //初始化服务端Socket
            this.mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //端口绑定
            this.mServerSocket.Bind(this.ipEndPoint);
            //设置监听数目
            this.mServerSocket.Listen(maxClientCount);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="count">监听的最大数目</param>
        public TCPServer(string ip, int port, int count, int buffersize = 1024)
        {
            this.ip = ip;
            this.port = port;
            this.maxClientCount = count;
            this.buffersize = buffersize;
            this.buffer = new byte[buffersize];

            this.mClientSockets = new List<Socket>();
            this.fileQueue = new ConcurrentQueue<Request>();
            this.requestQueue = new ConcurrentQueue<Request>();
            this.byteQueues = new Dictionary<Socket, ConcurrentQueue<byte>>();

            //初始化IP终端
            this.ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            //初始化服务端Socket
            this.mServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //端口绑定
            this.mServerSocket.Bind(this.ipEndPoint);
            //设置监听数目
            this.mServerSocket.Listen(maxClientCount);

        }

        /// <summary>
        /// 定义一个Start方法将构造函数中的方法分离出来
        /// </summary>
        public void Start()
        {
            //创建服务端线程，实现客户端连接请求的循环监听
            var mServerThread = new Thread(this.ListenClientConnect);
            //服务端线程开启
            mServerThread.Start();

            //Start the File sending thread
            var mSendingThread = new Thread(this.FileSender);

            mSendingThread.Start();

        }

        /// <summary>
        /// Add a file to the sending queue
        /// </summary>
        /// <param name="path"></param>
        /// <param name="s"></param>
        public void AddFile(string path, Socket s)
        {
            fileQueue.Enqueue(new Request(path, s));
        }

        /// <summary>
        /// Fetch a request sent from clients, if the queue is empty, return null.
        /// </summary>
        /// <returns></returns>
        public Request FetchRequest()
        {
            bool flag;
            Request request;
            flag = requestQueue.TryDequeue(out request);
            return request;
        }

        /// <summary>
        /// 监听客户端链接
        /// </summary>
        private void ListenClientConnect()
        {
            System.Console.WriteLine("Listen Client thread initialized.");

            //设置循环标志位
            bool flag = true;
            while (flag)
            {
                //获取连接到服务端的客户端
                this.ClientSocket = this.mServerSocket.Accept();
                //将获取到的客户端添加到客户端列表
                this.mClientSockets.Add(this.ClientSocket);

                System.Console.WriteLine(string.Format("Connect to Client {0}", this.ClientSocket.RemoteEndPoint));

                //创建客户端消息线程，实现客户端消息的循环监听
                var mReveiveThread = new Thread(this.ReceiveClient);
                //注意到ReceiveClient方法传入了一个参数
                //实际上这个参数就是此时连接到服务器的客户端
                //即ClientSocket
                byteQueues[mClientSocket] = new ConcurrentQueue<byte>();
                mReveiveThread.Start(this.ClientSocket);
                

                var mParseThread = new Thread(this.ParseClient);

                mParseThread.Start(this.ClientSocket);

            }
        }

        /// <summary>
        /// 接收客户端消息的方法
        /// </summary>
        private void ReceiveClient(object obj)
        {

            System.Console.WriteLine("Recieve Client thread initialized.");

            //获取当前客户端
            //因为每次发送消息的可能并不是同一个客户端，所以需要使用var来实例化一个新的对象
            //可是我感觉这里用局部变量更好一点
            var mClientSocket = (Socket)obj;
            
            // 循环标志位
            bool flag = true;
            while (flag)
            {
                try
                {
                    //获取数据长度
                    int receiveLength = mClientSocket.Receive(buffer);
                    //获取客户端消息
                    foreach( byte b in buffer){
                        byteQueues[mClientSocket].Enqueue(b);
                    }
                    //string clientMessage = Encoding.UTF8.GetString(buffer, 0, receiveLength);
                    
                    //System.Console.WriteLine(string.Format("Get a message from client {0} : {1}", mClientSocket.RemoteEndPoint, clientMessage));
                    
                    //requestQueue.Enqueue(new Request(clientMessage, mClientSocket));

                }
                catch (Exception e)
                {
                    //从客户端列表中移除该客户端
                    this.mClientSockets.Remove(mClientSocket);

                    System.Console.WriteLine(string.Format("The client {0} is offline now.", mClientSocket.RemoteEndPoint));

                    //向其它客户端告知该客户端下线
                    //this.SendMessage(string.Format("服务器发来消息:客户端{0}从服务器断开,断开原因:{1}", mClientSocket.RemoteEndPoint, e.Message));
                    //断开连接
                    mClientSocket.Shutdown(SocketShutdown.Both);
                    mClientSocket.Close();
                    break;
                }
            }

        }


        /// <summary>
        /// Parse the bytes
        /// </summary>
        /// <param name="obj"></param>
        private void ParseClient(object obj)
        {
            System.Console.WriteLine("Parse Client thread initialized.");
            var mClientSocket = (Socket)obj;
            ConcurrentQueue<byte> Q = byteQueues[mClientSocket];
            int length, cnt;
            length = -2;
            cnt = -1;
            List<byte> list = new List<byte>();
            while (true)
            {
                byte b;
                bool flag = Q.TryDequeue(out b);
                if (!flag){
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
                        requestQueue.Enqueue(new Request(Encoding.UTF8.GetString(data), mClientSocket));
                        length = -2;
                        cnt = -1;
                    }
                }
            }
        }
        ///// <summary>
        ///// 向所有的客户端群发消息
        ///// </summary>
        ///// <param name="msg">message</param>
        //private void SendMessage(string msg)
        //{
        //    //确保消息非空以及客户端列表非空
        //    if (msg == string.Empty || this.mClientSockets.Count <= 0) return;
        //    //向每一个客户端发送消息
        //    foreach (Socket s in this.mClientSockets)
        //    {
        //        (s as Socket).Send(Encoding.UTF8.GetBytes(msg));
        //    }
        //}


        private void SendPackage(Socket client,Package pack)
        {
            client.Send(pack.Data);
        }

        private void FileSender()
        {
            System.Console.WriteLine("File Sender strat.");
            bool flag = true;
            while (flag)
            {
                Request file;
                fileQueue.TryDequeue(out file);
                if (file != null)
                {
                    string path = file._Msg;
                    Socket soc = file._Socket;
                    SendFile(path, soc);
                }
                    
            }
        }

        
        private void SendFile(string path, Socket toward)
        {
            try
            {
                FileInfo file = new FileInfo(path);
                FileStream filestream = file.OpenRead();
                int packetCount = (int)(file.Length / buffersize);
                int lastPacketSize = (int)file.Length % buffersize;
                
                //System.Console.WriteLine("Length is "+file.Length.ToString()+"packetcount "+packetCount.ToString());
                
                toward.Send(new Package(Encoding.UTF8.GetBytes("_File")).Data);   //Send the recieve header
                toward.Send(new Package(Encoding.UTF8.GetBytes(file.Name)).Data); //Send the file name
                toward.Send(new Package(Encoding.UTF8.GetBytes(file.Length.ToString())).Data); //Send the file size
                for (int i = 0; i < packetCount; i++)
                {
                    filestream.Read(buffer, 0, buffersize);
                    toward.Send(new Package(buffer).Data);
                }
                if (lastPacketSize != 0)
                {
                    byte[] last = new byte[lastPacketSize];
                    filestream.Read(last, 0, lastPacketSize);
                    toward.Send(new Package(last).Data);
                }
            }
            catch (System.Exception ex){
                System.Console.WriteLine("SendFile Error");
                System.Console.WriteLine(ex);
            }
        
        }


        //public static void Main(string[] args)
        //{
        //    TCPServer server = new TCPServer( 10010, 20);
        //    server.Start();
        //    System.Console.WriteLine(string.Format("The IP of server is {0}", server.IP));
        //    string rootpath = "D:/v-hanqw/3D_DeepLearning_dataset/ShapeNet";
        //    while (true)
        //    {
        //        string command = Console.ReadLine();
        //        if (command.CompareTo("file") == 0)
        //        {
        //            string file = Console.ReadLine();
        //            string path = rootpath + '/' + file;
        //            Socket s = server.ClientSockets.ElementAt(0);
        //            server.AddFile(path, s);
        //        }
        //        if (command.CompareTo("fetch") == 0)
        //        {
        //            Request rq = server.FetchRequest();
        //            if (rq == null)
        //            {
        //                Console.WriteLine("Request queue is empty.");
        //                continue;
        //            }
        //            Console.WriteLine(rq._Msg + " " + rq._Socket.ToString());
        //        }
        //        if (command.CompareTo("msg") == 0)
        //        {
        //            string msg = Console.ReadLine();
        //            Socket s = server.ClientSockets.ElementAt(0);
        //            server.SendPackage(s, new Package(Encoding.UTF8.GetBytes(msg)));
                    
        //        }
        //    }
        //}
    }

    public class Request
    {
        /// <summary>
        /// members: _msg , _socket
        /// </summary>
        private string _msg;
        public string _Msg
        {
            get { return this._msg; }
        }

        private Socket _socket;
        public Socket _Socket
        {
            get { return this._socket; }
        }
        public Request(string msg, Socket socket)
        {
            this._msg = msg;
            this._socket = socket;
        }

    }

    public class Package
    {
        private byte[] data;

        public byte[] Data
        {
            get { return this.data; }
        }

        private int length;

        public int Length
        {
            get { return this.length; }
        }

        private byte[] msg;

        public byte[] Msg
        {
            get { return this.msg; }
        }

        public Package(byte[] _data)
        {
            this.length = _data.Length;
            List<byte> list = new List<byte>();
            byte l1, l2;
            l1 = (byte)(this.length % 256);
            l2 = (byte)(this.length / 256);
            list.Add(l1);
            list.Add(l2);
            list.AddRange(_data);
            this.data = list.ToArray();
            this.msg = _data;
        }
    }
}



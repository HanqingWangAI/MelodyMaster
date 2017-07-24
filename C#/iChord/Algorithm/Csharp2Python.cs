using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using Microsoft.Scripting.Hosting;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Net;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace iChord
{
    class Csharp2Python
    {
        public string Python { set; get; }
        public string MyPythonApp { set; get; }

        public IPAddress IP { set; get; }

        Socket clientSocket { set; get; }

        /// <summary>
        /// Init python intepreter and python script 
        /// </summary>
        /// <param name="python"></param>
        /// <param name="myPythonApp"></param>

        public static string GetPythonPath()
        {
            string pathVariable = Environment.GetEnvironmentVariable("Path");
            if (pathVariable != null)
            {
                string[] allPaths = pathVariable.Split(';');
                foreach (var path in allPaths)
                {
                    string pythonPathFromEnv = path + "\\python.exe";
                    if (File.Exists(pythonPathFromEnv))
                        return pythonPathFromEnv;
                }
            }
            return null;
        }

        public Csharp2Python(string python = @"D:\Programs\Python27", string myPythonApp = @".\..\..\..\..\src\utils\RNN.py")
        {
            //python = System.Environment.GetEnvironmentVariable("Python");
            Python = GetPythonPath();
            MyPythonApp = myPythonApp;
        }

        public Csharp2Python(string ip,Socket _socket)
        {
            //设定服务器IP地址  
            IP = IPAddress.Parse(ip);
            // clientSocket = 
            try
            {
                clientSocket.Connect(new IPEndPoint(IP, 10010)); //配置服务器IP与端口  
                Console.WriteLine("Connect Successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Connect the Server Error!");
                Console.WriteLine(ex.Message);
            }
        }

        public string run(string score)
        {
            //create new process start info
            ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(Python);

            //make sure we can read the output from stdout
            myProcessStartInfo.UseShellExecute = false;
            myProcessStartInfo.RedirectStandardOutput = true;
            myProcessStartInfo.CreateNoWindow = true;

            //start python app with 3 arguments
            //1st argument is pointer to itself, 2nd and 3rd are actual arguments we want to send
            //sys.argv[0] in python is a pointer to itself.
            score = score.Replace(" ", "\" \"");
            string currentExecutePath = Directory.GetCurrentDirectory().Replace(" ", "\" \"");
            myProcessStartInfo.Arguments = MyPythonApp + " " + score + " " + currentExecutePath;

            //create a new process and start it
            Process myProcess = new Process();
            myProcess.StartInfo = myProcessStartInfo;
            
            myProcess.Start();

            StreamReader myStreamReader = myProcess.StandardOutput;
            string myString = myStreamReader.ReadToEnd();

            myProcess.WaitForExit();
            myProcess.Dispose();
            return myString;
        }


        private static byte[] result = new byte[1024];

        public async Task<string> connectServer(string score)
        {
            string res = "";
            
            ////Console.WriteLine("===============================\nStep into function");
            ////通过 clientSocket 发送数据  
            //try
            //{
            //    // Thread.Sleep(50);    //等待1秒钟  
            //    int sendSize = Encoding.UTF8.GetByteCount(score);
            //    byte[] sendSizeIn256 = BitConverter.GetBytes(Convert.ToInt16(sendSize));
            //    byte[] sendMessage = Encoding.UTF8.GetBytes(score);
            //    byte[] sendMessageByte = new byte[sendSizeIn256.Length + sendMessage.Length];

            //    Buffer.BlockCopy(sendSizeIn256, 0, sendMessageByte, 0, sendSizeIn256.Length);
            //    Buffer.BlockCopy(sendMessage, 0, sendMessageByte, sendSizeIn256.Length, sendMessage.Length);
                
            //    clientSocket.Send(sendMessageByte);
            //    Console.WriteLine(string.Format("Send Message to Server: {0}", Encoding.UTF8.GetString(sendMessage)));
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine("Error!!!"+ex.Message);
            //    clientSocket.Shutdown(SocketShutdown.Both);
            //    clientSocket.Close();
            //}
            //Console.WriteLine("Before receiving.");

            ////Thread.Sleep(500); //等待1秒钟
            ////通过clientSocket接收数据
            //string res = "";
            ////ConcurrentQueue<byte> queue = new ConcurrentQueue<byte>();
            ////int receiveLength = clientSocket.Receive(result);
            ////Console.WriteLine("Receive Message from Server: {0}", Encoding.UTF8.GetString(result, 0, receiveLength));
            ////string res = Encoding.ASCII.GetString(result, 0, receiveLength);

            ////try
            ////{
            ////    //Console.WriteLine("Step into try");
            ////    byte[] receiveMessage = new byte[1024];
                
            ////    int receivedLen = clientSocket.Receive(receiveMessage);

            ////    //Console.WriteLine(Encoding.UTF8.GetString(receiveMessage));
            ////    //Console.WriteLine("After receiving.");
            ////    //for (int i = 0; i< receivedLen; i++)
            ////    //{
            ////    //    queue.Enqueue(receiveMessage[i]);
            ////    //}
            ////    //Console.WriteLine("============------=======");

            ////    byte[] receiveSizeByte = new byte[4];

            ////    //while (!queue.TryDequeue(out receiveSizeByte[0])) ;
            ////    //while (!queue.TryDequeue(out receiveSizeByte[1])) ;

            ////    int receiveSizeInt = BitConverter.ToInt32(receiveSizeByte, 0);

            ////    receiveSizeInt -= receivedLen - 2;
            ////    //Console.WriteLine("Before while");
            ////    while(receiveSizeInt > 0)
            ////    {
            ////        receivedLen = clientSocket.Receive(receiveMessage);
            ////        //SocketAsyncEventArgs msg = new SocketAsyncEventArgs();
            ////        //msg.Buffer = receiveMessage;
            ////        //receivedLen = await clientSocket.ReceiveAsync(receiveMessage);
            ////        foreach (byte b in receiveMessage)
            ////        {
            ////            queue.Enqueue(b);
            ////        }
            ////        receiveSizeInt -= receivedLen;
            ////    }
            ////    Console.WriteLine("After while");
            ////    byte messageByte;
            ////    while (!queue.IsEmpty)
            ////    {
            ////        bool flag = queue.TryDequeue(out messageByte);
            ////        byte[] message = new byte[1];
            ////        message[0] = messageByte;
            ////        string tmpMessage = Encoding.UTF8.GetString(message);
            ////        res += tmpMessage;
            ////    }
            ////    Console.WriteLine("Queue is empty");
            ////
            ////}
            ////catch (Exception ex)
            ////{
            ////    Console.WriteLine("Error"+ex.Message);
            ////}
            ////Console.WriteLine(res);
            
            return res;
        }
    }
}

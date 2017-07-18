using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleMidiPlayer.Midi;
using System.Threading;

using System.Runtime.InteropServices;
using Windows;
using System.Windows;

namespace iChord
{
    class MidiPlay
    {

        public static int dely = 24;
        /// <summary>
        /// 音调偏移
        /// </summary>
        private static int key_move = 0;
        /// <summary>
        /// 全局移调函数
        /// </summary>
        /// <param name="value"></param>
        public static void changekey(int value)
        {
            key_move += value;
        }
        /// <summary>
        /// 全局节拍
        /// </summary>
        public static int beat = 100;
        /// <summary>
        /// 全局节拍控制函数
        /// </summary>
        /// <param name="节拍"></param>
        public static void changeBeat(int value)
        {
            beat = value;
        }
        /// <summary>
        /// 全局音量
        /// </summary>
        private static int volume = 100;
        /// <summary>
        /// 全局音量控制函数
        /// </summary>
        /// <param name="全局音量"></param>
        public static void changeVolume(int value)
        {
            if (value > 100)
                volume = 100;
            else if (value < 0)
                volume = 0;
            else
                volume = value;
        }
        /// <summary>
        /// MIDI设备复位函数
        /// </summary>
        /// <param name="MIDI设备"></param>
        public static void Reset(MidiDevice device)
        {
            device.Reset();
        }
        /// <summary>
        /// MIDI设备初始化函数
        /// </summary>
        /// <returns></returns>
        public static MidiDevice playInitialization()
        {
            MidiDevice midi = new MidiDevice();
            return midi;
        }


        public static void playMidi(MidiDevice device, string note)
        {
           
        }

        /// <summary>
        /// 播放一小节函数，供NewPlay函数调用（四轨道重载）
        /// 默认主旋律音量127=max 和弦轨音量100
        /// </summary>
        /// <param name="MIDI设备"></param>
        /// <param name="主旋律"></param>
        /// <param name="主音色"></param>
        /// <param name="和弦一"></param>
        /// <param name="音色一"></param>
        /// <param name="和弦二"></param>
        /// <param name="音色二"></param>
        public static void playMidi(MidiDevice device, string note, int n0, string chord, int n1, string music, int n2)
        {
            playThread_right(device, note, n0, 0, 127);
            playThread_left(device, chord, n1, 1, 127);
            playThread_left(device, music, n2, 2, 127);
        }
        /// <summary>
        /// 播放一小节函数，供NewPlay函数调用（四轨道重载）
        /// 默认主旋律音量127=max 和弦轨音量100
        /// </summary>
        /// <param name="MIDI设备"></param>
        /// <param name="主旋律"></param>
        /// <param name="主音色"></param>
        /// <param name="和弦一"></param>
        /// <param name="音色一"></param>
        /// <param name="和弦二"></param>
        /// <param name="音色二"></param>
        /// <param name="和弦三"></param>
        /// <param name="音色三"></param>
        public static void playMidi(MidiDevice device, string note0, int n0, string note1, int n1, string note2, int n2, string note3, int n3)
        {
            playThread_right(device, note0, n0, 0, 127);
            playThread_left(device, note1, n1, 1, 127);
            playThread_left(device, note2, n2, 2, 127);
            playThread_left(device, note3, n3, 3, 127);
        }

        /// <summary>
        /// 主旋律播放单音的方法
        /// </summary>
        /// <param name="MIDI设备"></param>
        /// <param name="乐谱"></param>
        /// <param name="音色"></param>
        /// <param name="MIDI频道"></param>
        /// <param name="音量"></param>
        public static void playBasic_right(MidiDevice Device, string note, int trmbre, int channel, int playvolume)
        {
            // 1.  A3+31999 C4+41999 C4+41999 C4+41999 C3+51999 C5050999
            // 2.  8位一个音，1音名；2八度；3升降＋－0；4音长；5附点1、0；6－8无意义的9占位；

            if (note != null)
            {
                //处理轨道音色问题
                Device.ChangeProgram(channel, trmbre, 0);

                //定义变量
                int lenth = note.Length;
                int location;
                double speed;

                for (int i = 0; i < lenth - 5; i += 8)
                {
                    //处理几个八度问题 第二位
                    location = 60;
                    location += 12 * (int)(note[i + 1] - 53);
                    //处理八度以内问题 第一位
                    switch (note[i])
                    {
                        case 'C':
                        case 'c': location += 0; break;

                        case 'D':
                        case 'd': location += 2; break;

                        case 'E':
                        case 'e': location += 4; break;
                        case 'F':
                        case 'f': location += 5; break;

                        case 'G':
                        case 'g': location += 7; break;

                        case 'A':
                        case 'a': location += 9; break;
                        case 'B':
                        case 'b': location += 11; break;
                        default: location = -1; break;
                    }
                    //处理升降问题 + - 0 第三位
                    if(location>-1)
                    {
                        switch (note[i + 2])
                        {
                            case '+': location += 1; break;
                            case '-': location -= 1; break;
                            default: break;
                        }
                    }
                    

                    //处理音符越界问题 第一二三位合起来
                    if (location > -1)
                    {
                        location += key_move;
                    }
                    if (location < -1)
                    {
                        location = -1;
                    }

                    //处理音符时长问题 第四位
                    speed = (60000 / beat / 16 * (int)Math.Pow(2, (int)note[i + 3] - 48));

                    //处理附点问题  第五位
                    if (note[i + 4] == '1')
                    {
                        speed *= 1.5;
                    }
                    Console.WriteLine("主旋律播放："
                        + note[i] + note[i + 1] + note[i + 2] + note[i + 3]
                        + note[i + 4] + note[i + 5] + note[i + 6] + note[i + 7]
                        + "=" + location + "\t播放时间=" + speed);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainWindow.InterfaceForMidi.keyDownColor_right(location);
                    }), null);
                    Device.Note_On(channel, location, (int)((double)volume / 100 * playvolume));
                    Thread.Sleep((int)speed);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainWindow.InterfaceForMidi.keyUpColor(location);
                    }), null);
                    Device.Note_Off(channel, location, (int)((double)volume / 100 * playvolume));
                }
            }
        }
        /// <summary>
        /// 和弦播放单音的方法
        /// </summary>
        /// <param name="MIDI设备"></param>
        /// <param name="乐谱"></param>
        /// <param name="音色"></param>
        /// <param name="MIDI频道"></param>
        /// <param name="音量"></param>
        public static void playBasic_left(MidiDevice Device, string note, int trmbre, int channel, int playvolume)
        {
            // 1.  A3+31999 C4+41999 C4+41999 C4+41999 C3+51999 C5050999
            // 2.  8位一个音，1音名；2八度；3升降＋－0；4音长；5附点1、0；6－8无意义的9占位；

            if (note != null)
            {
                //处理轨道音色问题
                Device.ChangeProgram(channel, trmbre, 0);

                //定义变量
                int lenth = note.Length;
                int location;
                double speed;

                for (int i = 0; i < lenth - 5; i += 8)
                {
                    //处理几个八度问题 第二位
                    location = 60;
                    location += 12 * (int)(note[i + 1] - 53);
                    //处理八度以内问题 第一位
                    switch (note[i])
                    {
                        case 'C':
                        case 'c': location += 0; break;

                        case 'D':
                        case 'd': location += 2; break;

                        case 'E':
                        case 'e': location += 4; break;
                        case 'F':
                        case 'f': location += 5; break;

                        case 'G':
                        case 'g': location += 7; break;

                        case 'A':
                        case 'a': location += 9; break;
                        case 'B':
                        case 'b': location += 11; break;
                        default: location = -1; break;
                    }
                    //处理升降问题 + - 0 第三位
                    switch (note[i + 2])
                    {
                        case '+': location += 1; break;
                        case '-': location -= 1; break;
                        default: break;
                    }

                    //处理音符越界问题 第一二三位合起来
                    if (location != -1)
                    {
                        location += key_move;
                    }
                    if (location < -1)
                    {
                        location = -1;
                    }

                    //处理音符时长问题 第四位
                    speed = (60000 / beat / 16 * (int)Math.Pow(2, (int)note[i + 3] - 48));

                    //处理附点问题  第五位
                    if (note[i + 4] == '1')
                    {
                        speed *= 1.5;
                    }
                    Console.WriteLine("和弦轨播放："
                        + note[i] + note[i + 1] + note[i + 2] + note[i + 3]
                        + note[i + 4] + note[i + 5] + note[i + 6] + note[i + 7]
                        + "=" + location + "\t播放时间=" + speed);
                   Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainWindow.InterfaceForMidi.keyDownColor_left(location);
                    }), null);
                    Device.Note_On(channel, location, (int)((double)volume / 100 * playvolume));
                    Thread.Sleep((int)speed);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MainWindow.InterfaceForMidi.keyUpColor(location);
                    }), null);
                    Device.Note_Off(channel, location, (int)((double)volume / 100 * playvolume));
                }
            }
        }


        /// <summary>
        /// 小节对齐播放/控制函数（三轨道重载）
        /// 每小节之间用，隔开
        /// </summary>
        /// <param name="MIDI设备"></param>
        /// <param name="主旋律轨"></param>
        /// <param name="主音色"></param>
        /// <param name="和弦一轨"></param>
        /// <param name="音色一"></param>
        /// <param name="和弦二轨"></param>
        /// <param name="音色二"></param>
        public static void NewPlay(MidiDevice Device, string inf1, int n1, string inf2, int n2, string inf3, int n3)
        {
            string[] note1 = inf1.Split(',');
            string[] note2 = inf2.Split(',');
            string[] note3 = inf3.Split(',');

            int index = note1.Length;
            for (int i = 0; i < index; i++)
            {
                Console.WriteLine("主旋律：note1[" + i + "]=" + note1[i]);
                Console.WriteLine("和弦：note2[" + i + "]=" + note2[i]);
                Console.WriteLine("和弦：note3[" + i + "]=" + note3[i]);

                double sleeptime = 0;
                double littletime;
                int lenth = note1[i].Length;
                for (int j = 0; j < lenth - 5; j += 8)
                {

                    littletime = (60000 / beat / 16 * (int)Math.Pow(2, (int)note1[i][j + 3] - 48));
                    if (note1[i][j + 4] == '1')
                    {
                        littletime *= 1.5;
                    }
                    sleeptime += littletime;
                }
                Console.WriteLine("小节播放时间：" + (int)sleeptime);
                Device.Reset();
                playMidi(Device, note1[i], n1, note2[i], n2, note3[i], n3);
                Thread.Sleep((int)sleeptime + dely);
                Device.Reset();
            }
        }

        /// <summary>
        /// 小节对齐播放/控制函数（四轨道重载）
        /// 每小节之间用，隔开
        /// </summary>
        /// <param name="MIDI设备"></param>
        /// <param name="主旋律轨"></param>
        /// <param name="主音色"></param>
        /// <param name="和弦一轨"></param>
        /// <param name="音色一"></param>
        /// <param name="和弦二轨"></param>
        /// <param name="音色二"></param>
        /// <param name="和弦三轨"></param>
        /// <param name="音色三"></param>
        public static void NewPlay(MidiDevice Device, string inf1, int n1, string inf2, int n2, string inf3, int n3, string inf4, int n4)
        {

            string[] note1 = inf1.Split(',');
            string[] note2 = inf2.Split(',');
            string[] note3 = inf3.Split(',');
            string[] note4 = inf4.Split(',');
            int index = note1.Length;
            for (int i = 0; i < index; i++)
            {
                Console.WriteLine("主旋律：note1[" + i + "]=" + note1[i]);
                Console.WriteLine("和弦：note2[" + i + "]=" + note2[i]);
                Console.WriteLine("和弦：note3[" + i + "]=" + note3[i]);
                Console.WriteLine("和弦：note4[" + i + "]=" + note4[i]);
                double sleeptime = 0;
                double littletime;
                int lenth = note1[i].Length;
                for (int j = 0; j < lenth - 5; j += 8)
                {

                    littletime = (60000 / beat / 16 * (int)Math.Pow(2, (int)note1[i][j + 3] - 48));
                    if (note1[i][j + 4] == '1')
                    {
                        littletime *= 1.5;
                    }

                    sleeptime += littletime;
                }
                Console.WriteLine("小节播放时间：" + (int)sleeptime);
                Device.Reset();
                playMidi(Device, note1[i], n1, note2[i], n2, note3[i], n3, note4[i], n4);
                Thread.Sleep((int)sleeptime + dely);
                Device.Reset();
            }
        }


        //实时播放
        public static void RealPlay(MidiDevice Device, string inf1, int n1, string inf2, int n2, string inf3, int n3, string inf4, int n4)
        {

            string[] note1 = inf1.Split(',');
            string[] note2 = inf2.Split(',');
            string[] note3 = inf3.Split(',');
            string[] note4 = inf4.Split(',');
            int index = note1.Length;
            for (int i = 0; i < index; i++)
            {
                Console.WriteLine("主旋律：note1[" + i + "]=" + note1[i]);
                Console.WriteLine("和弦：note2[" + i + "]=" + note2[i]);
                Console.WriteLine("和弦：note3[" + i + "]=" + note3[i]);
                Console.WriteLine("和弦：note4[" + i + "]=" + note4[i]);
                double sleeptime = 0;
                double littletime;
                int lenth = note1[i].Length;
                for (int j = 0; j < lenth - 5; j += 8)
                {

                    littletime = (60000 / beat / 16 * (int)Math.Pow(2, (int)note1[i][j + 3] - 48));
                    if (note1[i][j + 4] == '1')
                    {
                        littletime *= 1.5;
                    }

                    sleeptime += littletime;
                }
                Console.WriteLine("小节播放时间：" + (int)sleeptime);
                Device.Reset();
                playMidi(Device, note1[i], n1, note2[i], n2, note3[i], n3, note4[i], n4);
                Thread.Sleep((int)sleeptime + dely);
                Device.Reset();
            }
        }



        /// <summary>
        /// 主旋律播放的多线程启动方法
        /// </summary>
        /// <param name="MIDI设备"></param>
        /// <param name="乐谱"></param>
        /// <param name="音色"></param>
        /// <param name="MIDI频道"></param>
        /// <param name="音量"></param>
        public static void playThread_right(MidiDevice Device, string note, int trmbre, int channel, int playvolume)
        {
            MutilVariable bus = new MutilVariable();
            bus.setdevice(Device);
            bus.setnote(note);
            bus.setchannel(channel);
            bus.settrmbre(trmbre);
            Thread t = new Thread(new ThreadStart(bus.run_right));
            bus.setplayvolume(playvolume);
            t.IsBackground = true;
            t.Start();
        }
        /// <summary>
        /// 和弦播放的多线程启动方法
        /// </summary>
        /// <param name="MIDI设备"></param>
        /// <param name="乐谱"></param>
        /// <param name="音色"></param>
        /// <param name="MIDI频道"></param>
        /// <param name="音量"></param>
        public static void playThread_left(MidiDevice Device, string note, int trmbre, int channel, int playvolume)
        {
            MutilVariable bus = new MutilVariable();
            bus.setdevice(Device);
            bus.setnote(note);
            bus.setchannel(channel);
            bus.settrmbre(trmbre);
            Thread t = new Thread(new ThreadStart(bus.run_left));
            bus.setplayvolume(playvolume);
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        /// BrandNew方法为了用单独的线程启动播放  不卡界面线程（四轨道重载）
        /// </summary>
        /// <param name="MIDI设备"></param>
        /// <param name="主旋律轨"></param>
        /// <param name="主音色"></param>
        /// <param name="和弦一轨"></param>
        /// <param name="音色一"></param>
        /// <param name="和弦二轨"></param>
        /// <param name="音色二"></param>
        /// <param name="和弦三轨"></param>
        /// <param name="音色三"></param>
        public static void BrandNew(MidiDevice Device, string inf1 = "", int n1 = 0, string inf2 = "", int n2 = 0, string inf3 ="", int n3 = 0, string inf4="", int n4=0)
        {
            MutilThread engine = new MutilThread();
            engine.setdevice(Device);
            engine.setinf1(inf1);
            engine.setinf2(inf2);
            engine.setinf3(inf3);
            engine.setinf4(inf4);
            engine.setn1(n1);
            engine.setn2(n2);
            engine.setn3(n3);
            engine.setn4(n4);
            Thread t = new Thread(new ThreadStart(engine.run));
            t.IsBackground = true;
            t.Start();
        }

    }
    /// <summary>
    /// 为了NewPlay方法写的多线程启动类
    /// 目的在于不卡播放界面
    /// </summary>
    class MutilThread
    {
        private MidiDevice device;
        public void setdevice(MidiDevice value)
        {
            this.device = value;
        }
        private string inf1 = null;
        public void setinf1(string value)
        {
            this.inf1 = value;
        }
        private string inf2 = null;
        public void setinf2(string value)
        {
            this.inf2 = value;
        }
        private string inf3 = null;
        public void setinf3(string value)
        {
            this.inf3 = value;
        }
        private string inf4 = null;
        public void setinf4(string value)
        {
            this.inf4 = value;
        }
        private int n1 = 2;
        public void setn1(int value)
        {
            n1 = value;
        }
        private int n2 = 2;
        public void setn2(int value)
        {
            n2 = value;
        }
        private int n3 = 2;
        public void setn3(int value)
        {
            n3 = value;
        }
        private int n4 = 2;
        public void setn4(int value)
        {
            n4 = value;
        }

        public void run()
        {
            //以下的Play如果移植到源工程需要改为MidiPlay
            if (inf4 != null)
                MidiPlay.NewPlay(device, inf1, n1, inf2, n2, inf3, n3, inf4, n4);
            else if (inf4 == null && inf3 != null)
                MidiPlay.NewPlay(device, inf1, n1, inf2, n2, inf3, n3);

        }

    }

    /// <summary>
    /// 为了playBasic_right/playBasic_left方法写的多线程启动类
    /// 目的在于多参数启动多线程
    /// </summary>
    class MutilVariable
    {
        private MidiDevice device;
        public void setdevice(MidiDevice value)
        {
            this.device = value;
        }

        private string note;
        public void setnote(string value)
        {
            this.note = value;
        }
        private int trmbre;
        public void settrmbre(int value)
        {
            this.trmbre = value;
        }
        private int channel;
        public void setchannel(int value)
        {
            this.channel = value;
        }
        private int playvolume;
        public void setplayvolume(int value)
        {
            this.playvolume = value;
        }
        //以下的Play如果移植到源工程需要改为MidiPlay
        public void run_right()
        {
            MidiPlay.playBasic_right(device, note, trmbre, channel, playvolume);
        }
        public void run_left()
        {
            MidiPlay.playBasic_left(device, note, trmbre, channel, playvolume);
        }

    }
}

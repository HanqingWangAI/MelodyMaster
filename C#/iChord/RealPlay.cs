using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleMidiPlayer.Midi;
using System.Windows;
using System.Threading;
namespace iChord
{
    public partial class MainWindow : Window
    {
        public void realTimePlay()
        {
            MutileThreadForRealPlay rp = new MutileThreadForRealPlay();
            rp.setdevice(myMidiDevice);
            rp.setn1(timbreTrack[0]);
            rp.setn2(timbreTrack[1]);
            rp.setn3(timbreTrack[2]);
            rp.setn4(timbreTrack[3]);
            rp.setscore1(textBlock_main.Text.Replace(" ", ""));
            rp.setscore2(textBlock_main2.Text.Trim());
            Thread t = new Thread(new ThreadStart(rp.run));
            t.IsBackground = true;
            t.Start();
        }


        public void realTimePlayContext(string score1,string score2)
        {
            string[] track = new string[instrumentN];
            //Application.Current.Dispatcher.Invoke(new Action(() =>
            //{
               
            //}), null);
            //string score0 = textBlock_main.Text.Replace(" ", "");// 去掉曲谱里的空格
            string[] score0_seq = score1.Split(',');
            string[] chord_seq = score2.Split(' ');

            try
            {
                NoteInput.inputCheck(score1.Replace(",", ""));
                winInputCheck();

                if (muteMainScore)
                {
                    for (int i = 0; i < score0_seq.Length; i++)
                        score0_seq[i] = NoteInput.muteNote(score0_seq[i]);
                }

                string resultChordSeq = score2;
                // MessageBox.Show(textBlock_main2.Text);

                for (int i = 0; i < score0_seq.Length; i++)
                {
                    if ("" == score0_seq[i])
                        break;
                    track[0] = score0_seq[i];
                    Console.WriteLine("主旋律=" + track[0]);

                    if (i < chord_seq.Length)
                    {
                        track[1] = chordTrack[0].translateChordToNote(chord_seq[i]);
                        track[2] = chordTrack[1].translateChordToNote(chord_seq[i]);
                        track[3] = chordTrack[2].translateChordToNote(chord_seq[i]);
                    }
                    Console.WriteLine("第二轨=" + track[1]);
                    Console.WriteLine("第三轨=" + track[2]);
                    Console.WriteLine("第四轨=" + track[3]);
                    //MidiPlay.BrandNew(myMidiDevice, track[0], timbreTrack[0], track[1], timbreTrack[1], track[2], timbreTrack[2], track[3], timbreTrack[3]);
                    //Thread.Sleep(3000);
                    /*
                    string testAns = "";
                    foreach (string x in track)
                        testAns += x + '\n';
                    MessageBox.Show(testAns);//实时演奏播放函数 
                    */
                    //  MidiPlay.BrandNew(myMidiDevice, track[0], timbreTrack[0], track[1], timbreTrack[1], track[2], timbreTrack[2], track[3], timbreTrack[3]);
                    //fjc
                    double sleeptime = 0;
                    double littletime;
                    int lenth = track[0].Length;
                    for (int j = 0; j < lenth - 5; j += 8)
                    {

                        littletime = (60000 / MidiPlay.beat / 16 * (int)Math.Pow(2, (int)track[0][j + 3] - 48));
                        if (track[0][j + 4] == '1')
                        {
                            littletime *= 1.5;
                        }

                        sleeptime += littletime;
                    }

                    Console.WriteLine("小节播放时间：" + (int)sleeptime);
                    myMidiDevice.Reset();
                    MidiPlay.playMidi(myMidiDevice, track[0], timbreTrack[0], track[1], timbreTrack[1], track[2], timbreTrack[2], track[3], timbreTrack[3]);
                    Thread.Sleep((int)sleeptime + MidiPlay.dely);
                    myMidiDevice.Reset();
                }
                //MidiPlay.BrandNew(myMidiDevice, track[0], timbreTrack[0], track[1], timbreTrack[1], track[2], timbreTrack[2], track[3], timbreTrack[3]);
            }
            catch (UserInputException )
            {
                MessageBox.Show("请输入音符");
                initAllInputWin();
            }
        }
        /// <summary>
        /// 控制4个轨道的变化
        /// </summary>
        /// <param name="pattentId">轨道节奏型选择（1-6）根据combobx元素个数确定</param>
        /// <param name="trackId">伴奏轨道号（0-2）</param>
        public void setChordPattent(int pattentId, int trackId)
        {
            // chordComboxChange(2, chordTrack[2]);
            chordComboxChange(pattentId, chordTrack[trackId]);
        }
    }


    class MutileThreadForRealPlay
    {
        private MidiDevice device;
        public void setdevice(MidiDevice value)
        {
            this.device = value;
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
        private int n4 = -1;
        public void setn4(int value)
        {
            n4 = value;
        }
        private string score1 = null;

        public void setscore1(string value)
        {
            score1 = value;
        }
        private string score2 = null;
        public void setscore2(string value)
        {
            score2 = value;
        }

        public void run()
        {

            MainWindow.InterfaceForMidi.realTimePlayContext(score1,score2);

        }
    }
}

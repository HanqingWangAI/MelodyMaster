using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleMidiPlayer.Midi;
using System.Threading;

using System.Runtime.InteropServices;
using Windows;
namespace iChord
{
    class MidiPlay
    {
        //private static double beat = 120;
        private static double beat = 120;//真实速度反比于beat
        public static void changeBeat(int value)
        {
            beat = value;
        }

        private static int volume = 100 ;
        public static void changeVolume(int value)
        {
            if (value > 100)
                volume = 100;
            else if (value < 0)
                volume = 0 ;
            else
                volume = value;
        }

        public static MidiDevice playInitialization()
        {
            MidiDevice midi = new MidiDevice();
            return midi;
        }
        public static void playMidi(MidiDevice device, string note, string chord, string music)
        {
            playThread(device, note, 2, 0);
            playThread(device, chord, 2, 1);
            playThread(device, music, 2, 2);
        }
        public static void playMidi(MidiDevice device, string note, int n0, string chord, int n1, string music, int n2)
        {
            playThread(device, note, n0, 0);
            playThread(device, chord, n1, 1);
            playThread(device, music, n2, 2);
        }

        public static void playMidi(MidiDevice device, string note, string chord)
        {

            playThread(device, note, 2, 0);
            playThread(device, chord, 2, 1);
        }
        public static void playMidi(MidiDevice device, string note, int n0, string chord, int n1)
        {

            playThread(device, note, n0, 0);
            playThread(device, chord, n1, 1);
        }

        public static void playMidi(MidiDevice device, string note)
        {
            playThread(device, note, 2, 0);
        }
        public static void playMidi(MidiDevice device, string note, int n0)
        {
            playThread(device, note, n0, 0);
        }
        
        public static void playBasic(MidiDevice Device, string note, int trmbre, int channel)
        {

            Device.ChangeProgram(channel, trmbre, 0);
            int lenth = note.Length;
            int location;
            int speed;
            for (int i = 0; i < lenth-2; i += 3)
            {
                location = 60;
                location += 12 * (int)(note[i + 1] - 53);

                switch (note[i])
                {
                    case 'C':
                    case 'c': location += 0; break;
                    case '!': location += 1; break;
                    case 'D':
                    case 'd': location += 2; break;
                    case '#': location += 3; break;
                    case 'E':
                    case 'e': location += 4; break;
                    case 'F':
                    case 'f': location += 5; break;
                    case '%': location += 6; break;
                    case 'G':
                    case 'g': location += 7; break;
                    case '*': location += 8; break;
                    case 'A':
                    case 'a': location += 9; break;
                    case 'B':
                    case 'b': location += 11; break;
                    default: location = -1; break;
                }

                //speed = 2 * int2Pow(note[i + 2] - 44);
               // speed = (int)(60000 / beat / 16 * (int)Math.Pow(2, (int)note[i + 2] - 48)); 
               speed = (int) ( beat * int2Pow ( note[i + 2] - 48 ) ) ;
                Device.Note_On(channel, location, 100);
                Thread.Sleep(speed);
                Device.Note_Off(channel, location, 100);
            }
        }

        public static void playThread(MidiDevice Device, string note, int trmbre, int channel)
        {
            MutilVariable bus = new MutilVariable();
            bus.setdevice(Device);
            bus.setnote(note);
            bus.setchannel(channel);
            bus.settrmbre(trmbre);
            Thread t = new Thread(new ThreadStart(bus.run));
            //t.IsBackground = false;
            t.Start();
        }

        public static int noteToNum(string str)
        {
            char[] a = str.ToArray();
            char first = a[0];
            int second = a[1] - '0';
            int location = 0;
            switch (first)
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
            }
            int result = 12 * second + location;
            return result;
        }

        public static int int2Pow(int n)
        {
            int result = 1;
            for (int i = 1; i <= n; i++)
                result *= 2;
            return result;
        }

    }

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
        public void run()
        {
            MidiPlay.playBasic(device, note, trmbre, channel);
        }
      
    }
}

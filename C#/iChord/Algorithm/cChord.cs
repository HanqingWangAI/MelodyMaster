using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChord
{
    //自定义和弦的信息，本类为和弦计算的基本依靠。
    public class cChord : IComparable
    {
        private int note1;
        private int note2;
        private int note3;
        private int note4;
        private int chordID;
        public int Note1 { get { return note1; } set { note1 = value; } }
        public int Note2 { get { return note2; } set { note2 = value; } }
        public int Note3 { get { return note3; } set { note3 = value; } }
        public int Note4 { get { return note4; } set { note4 = value; } }
        public int ChordID { get { return chordID; } set { chordID = value; } }

        public static int chordN = 1;
        public int freq;
        public String name;
        public int counter;
        public int priority;
        public int duration;
        public double Sp;

        public cChord(int a, int b, int c, String name, int counter, int priority)
        {
            this.Note1 = a;
            this.Note2 = b;
            this.Note3 = c;
            this.name = name;
            this.counter = counter;
            this.priority = priority;
            this.ChordID = chordN++;
        }
        public cChord(int a, int b, int c, int d, String name, int counter, int priority)
        {
            this.Note1 = a;
            this.Note2 = b;
            this.Note3 = c;
            this.note4 = d;
            this.name = name;
            this.counter = counter;
            this.priority = priority;
        }
        public cChord()
        {
            this.Note1 = 1;
            this.Note2 = 1;
            this.Note3 = 1;
            this.name = "C";
            this.counter = 0;
            this.priority = 0;
        }

        //定义比较规则
        public int CompareTo(object obj)
        {
            cChord p = obj as cChord;
            if (p == null)
            {
                throw new NotImplementedException();
            }
            if (this.counter < p.counter) // More counter is better
                return 1;
            else if (this.counter == p.counter)
            {

                /*
                       if (this.freq > p.freq)// less freqency
                           return 1;
                       else if (this.freq == p.freq)
                       {
                           if (this.priority < p.priority) // higher priority
                               return 1;
                           else
                               return -1;
                       }
                       else
                           return -1;
                   }
                   else 
                   return -1;
               */
                if (this.priority < p.priority) // higher priority
                    return 1;
                else
                    return -1;
            }
            else
                return -1;

        }
    }
}

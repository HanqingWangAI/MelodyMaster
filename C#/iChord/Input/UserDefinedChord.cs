using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleMidiPlayer.Midi;
namespace iChord
{
    //A3+31999
    public class UserDefinedChord
    {
        private const int nChord = 7;
        public string[] userChord = new string[nChord];
        private string inputChord = "C";
       
        public UserDefinedChord()
        {
            stdPianoChord1();
        }
        public string InputChord
        {
            get { return inputChord; }
            set { inputChord = value; }
        }

        public void setUserChord(string[] userChord)
        {
            this.userChord = userChord;
        }

        public string translateChordToNote(string chord)//得到不含空格的字幕和弦对应的 可以直接使用的乐谱A23C33
        {
            string ansStr = "";
            switch (chord)
            {
                case "C":
                    ansStr = userChord[0];
                    break;
                case "Dm":
                    ansStr = userChord[1];
                    break;
                case "Em":
                    ansStr = userChord[2];
                    break;
                case "F":
                    ansStr = userChord[3];
                    break;
                case "G":
                    ansStr = userChord[4];
                    break;
                case "Am":
                    ansStr = userChord[5];
                    break;
                case "G7":
                    ansStr = userChord[6];
                    break;
            }
            return ansStr.Replace(" ","");//去除里面的空格
        }

        public string splitedChordNote(string chordSeq)//空格为分隔符
        {
            string backStr="";
            string[] handleChord = chordSeq.Split(new char[] { ' ' });
            foreach (string x in handleChord)
            {
                if(x!= null && x!="")
                {
                    string str = translateChordToNote(x);
                    if (str != null)
                        backStr += (str + ",");
                }
               
            }
            return backStr;
        }


        public string[] translateUserChord(string[] str)
        {
            int i = 0;
            foreach(string x in str)
            {
                str[i++] = x.Replace(" ", "");
            }
            return str;
        }
        public string adjustOctave(string chord, string melody)
        {
            int lowestMelody = 9, lowestChord = 9;
            string newChord = "";
            for (int i=0; i< melody.Length; i++)
            {
                if (i % 4 == 1)
                    lowestMelody = Math.Min(lowestMelody, melody[i] - '0');
            }

            for (int i = 0; i < chord.Length; i++)
            {
                if (i % 4 == 1)
                    lowestChord = Math.Min(lowestChord, chord[i] - '0');
            }

            int change = lowestMelody - lowestChord - 2;//控制两者相差一个八度。
            for (int i = 0; i < chord.Length; i++)
            {

                if (i % 4 == 1)//改变音符八度（1八度）
                {
                    char t = (char)(chord[i] + change);
                    if (t < '0')
                        t = '0';
                    newChord += t;
                   
                }
                else
                    newChord += chord[i];
            }
            return newChord;
        }
        public void adjustForOctave(string chordS, string melodyS)
        {
            string newC = "";
            string[] a = melodyS.Split(',');
            string[] b = chordS.Split(',');
            for(int i=0; i<a.Length; i++)
            {
                newC += adjustOctave(a[i], b[i]);
            }
            chordS = newC;
        }


        #region 默认和弦     
        public void stdPianoChord1()
        {
            string[] b =
            {
                //A3+30999
                "C4040999 E4040999 G4040999 C5040999",
                "D4040999 F4040999 A4040999 D5040999",
                "E4040999 G4040999 B4040999 E5040999",
                "F4040999 A4040999 C5040999 F5040999",
                "G3040999 B3040999 D4040999 G4040999",
                "A3040999 C4040999 E4040999 A4040999",
                "G3040999 B3040999 D4040999 F4040999",
            };
            
            int j = 0;
            foreach (string strB in b)
            {
                char[] t = strB.ToArray();
                for (int i = 0; i < t.Length; i++)
                {
                    if (i % 4 == 2)//改变音符长短（2倍）
                        //t[i]++;
                    if (i % 4 == 1)//改变音符八度（1八度）
                        {
                            t[i]--;
                            t[i]--;
                        }
                        
                }

                b[j] = new string(t);
                j++;
            }
            
            userChord = b;
        }

        public void stdPianoChord2()
        {
            string[] b =
           {
              "C4040999 E4030999 G4040999 C4030999 G4030999 C5030999",
              "D4040999 F4030999 A4040999 D4030999 A4030999 D5030999",
              "E4040999 G4030999 B4040999 E4030999 B4030999 E5030999",
              "F4040999 A4030999 C4040999 D4030999 C4030999 F5030999",
              "G4040999 B4030999 D4040999 E4030999 D4030999 G5030999",
              "A4040999 C4030999 E4040999 A4030999 C4030999 A5030999",
              "G4040999 B4030999 D4040999 B4030999 D4030999 F5030999",
          };
            /**/
            int j = 0;
            foreach (string strB in b)
            {
                char[] t = strB.ToArray();
                for (int i = 0; i < t.Length; i++)
                {
                    if (i % 4 == 2)//改变音符长短（2倍）
                        //t[i]++;
                    if (i % 4 == 1)//改变音符八度（1八度）
                        t[i]--;
                }

                b[j] = new string(t);
                j++;
            }
            
            userChord = b;
        }

        public void stdPianoChord3()
        {
            string[] b =
           {
              "C4050999 C4050999",
              "D4050999 D4050999",
              "E4050999 E4050999",
              "F4050999 F4050999",
              "G3050999 G3050999",
              "A3050999 A3050999",
              "G3050999 B3050999",
          };

            userChord = b;
        }

        public void stdPianoChord4()
        {
            string[] b =
           {
              "",
              "",
              "",
              "",
              "",
              "",
              "",
          };
            userChord = b;
        }

        public void stdPianoChord5()//proculsive
        {
            string[] b =
           {
                "C4030999 E4030999 G4030999 C5030999 C4030999 E4030999 G4030999 C50309999",
                "D4030999 F4030999 A4030999 D5030999 D4030999 F4030999 A4030999 D5030999",
                "E4030999 G4030999 B4030999 E5030999 E4030999 G4030999 B4030999 E5030999",
                "F4030999 A4030999 C5030999 F5030999 F4030999 A4030999 C5030999 F5030999",
                "G3030999 B3030999 D4030999 G4030999 G3030999 B3030999 D4030999 G4030999",
                "A3030999 C4030999 E4030999 A4030999 A3030999 C4030999 E4030999 A4030999",
                "G3030999 B3030999 D4030999 F4040999 G3030999 B3030999 D4030999 F4040999",
          };
            userChord = b;
        }

        public void stdPianoChord6()// naughty
        {
            string[] b =
           {
                "C4030999 Z4030999 E4030999 Z5030999 G4030999 Z5030999 C5040999 Z5030999",
                "D4030999 Z4030999 F4030999 Z5030999 A4030999 Z5030999 D5040999 Z5030999",
                "E4030999 Z4030999 G4030999 Z5030999 B4030999 Z5030999 E5040999 Z5030999",
                "F4030999 Z4030999 A4030999 Z5030999 C4030999 Z5030999 F5040999 Z5030999",
                "G3030999 Z4030999 B3030999 Z5030999 D4030999 Z5030999 G4040999 Z5030999",
                "A3030999 Z4030999 C4030999 Z5030999 Z5030999 E4030999 A4040999 Z5030999",
                "G3030999 Z4030999 B3030999 Z5030999 D4030999 Z5030999 F4040999 Z5030999",
          };
            /*
             string[] b =
           {
                "Z43 C43 Z53 E43 Z53 G43 C54",
                "Z43 D44 F43 Z53 A43 Z53 D53",
                "E43 Z44 G43 Z53 B43 Z53 E53",
                "F43 Z44 A43 Z53 C43 Z53 F53",
                "G43 Z44 B43 Z53 D43 Z53 G53",
                "A43 Z44 C43 Z53 E43 Z53 A53",
                "G43 Z44 B43 Z53 D43 Z53 F53",
          };
          */
            userChord = b;
        }
        #endregion
    }
}
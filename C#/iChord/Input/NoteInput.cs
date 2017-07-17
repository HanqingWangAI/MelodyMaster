using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iChord
{
    //A3+31999
    class NoteInput
    {
        public static int octove;
        public static char bias;
        public static int duration;
        public static int dot;
        public static string others = "999";
        public static int timeOfBar;
        public static char seperator = ' ';
        public static int noteLength = 9;
        #region keyName
        public static String[] keyName = new String[]
        {
            "C2","D2","E2","F2","G2","A2","B2",
            "C3","D3","E3","F3","G3","A3","B3"
        };
        #endregion

        static NoteInput()
        {
            octove = 4;
            bias = '0';
            duration = 4;
            dot = 0;
            timeOfBar = 4 * int2Pow(4);//定义4个4分音符为一个小节
        }
        
        public static String input(int x)
        {
            if (x==0)
                return "Z" + octove + bias + duration + dot + others + seperator;
            string outStr = "";
            string note = "";
            int oc = octove + (x - 1) / 7;
            switch (x % 7)
            {
                case 1:
                    note += "C"; break;
                case 2:
                    note += "D"; break;
                case 3:
                    note += "E"; break;
                case 4:
                    note += "F"; break;
                case 5:
                    note += "G"; break;
                case 6:
                    note += "A"; break;
                case 0:
                    note += "B"; break;
            }
            outStr = note + oc + bias + duration + dot + others;
            return outStr + seperator;
        }

        public static string devideBar(string inputStr)
        {
            string resultStr = inputStr;
            int currentTime = 0;
            int changeOfLength = 0;

            char[] a = inputStr.ToCharArray();
            int len = inputStr.Length;
            int theTimeInfo = 3;//0,1,2,3位置是时长
            for (int i = theTimeInfo; i < len; i += noteLength)
            {
                currentTime += (int) ( int2Pow(a[i] - '0')*(  (double)(a[i+1]-'0')/2 + 1) );
                if (currentTime >= timeOfBar)
                {
                    currentTime = 0;
                    resultStr = resultStr.Insert(i+(noteLength-theTimeInfo+ changeOfLength),",");
                    changeOfLength++;
                }
            }
            return resultStr;
        }

        public static int int2Pow(int n)
        {
            int result = 1;
            for (int i = 1; i <= n; i++)
                result *= 2;
            return result;
        }


        public static void inputCheck(string inStr)
        {
           // inStr = inStr.Replace(" ", "");
            
            char[] inChar = inStr.ToCharArray();
            int len = inChar.Length;
            bool isOK = true;
            for(int i = 0; i< len; i++)//A23 B33类似的代码
            {
                //A3+31999
                switch ( i%(noteLength-1) )
                {
                    case 0:
                        isOK = isNote(inChar[i]); break;
                    case 1:
                        isOK = isOctave(inChar[i]); break;
                    case 2:
                        isOK = isBias(inChar[i]); break;
                    case 3:
                        isOK = isDuration(inChar[i]); break;
                    case 4:
                        isOK = isDot(inChar[i]); break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        break;
                }
                if (!isOK)
                    throw new UserInputException("Wrong Input!");
            }
                
        }

        public static bool isNote(char x)
        {
            switch(x)
            {
                case 'C':
                case 'c': return true; 
               
                case 'D':
                case 'd': return true;

                case 'E':
                case 'e': return true;
                case 'F':
                case 'f': return true;

                case 'G':
                case 'g': return true;

                case 'A':
                case 'a': return true;
                case 'B':
                case 'b': return true;
                case 'Z':
                case 'z':return true;
                default: return false;
            }
        }
        public static bool isOctave(char x)
        {
            x -= '0';
            if (x >= 0 && x <= 9)
                return true;
            else
                return false;
        }

        public static bool isDuration(char x)
        {
            x -= '0';
            return (x >= 0 && x <= 9);
        }
        public static bool isDot(int x)
        {
            return (x == '1' || x == '0');
        }
        public static bool isBias(char x)
        {
            return (x == '+' || x == '-' || x=='0');
        }

        public static string muteNote(string str)
        {
            string retureStr = "";
            char[] a = str.ToCharArray();
            foreach(char x in a)
            {
                char addStr = x;
                if (x <= 'Z' && x >= 'A')
                    addStr = 'Z';
                retureStr += addStr;
            }
            return retureStr;
        }
    }
}

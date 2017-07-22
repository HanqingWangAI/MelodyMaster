using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace iChord
{
    /*和弦核心算法，通过接受用户输入的音符，自动处理分析，能够结合音符特点，乐曲特点，音符长短等信息进行处理得到正确，和协的和弦。
    *
    * 因为本类涉及许多音乐知识，也有一些数学方法，更是本程序的核心，不做过多复杂的解释。由于时间原因，本算法主要以单段乐句为单位
    * 进行分析，当然会存在一些错误和不和谐，今后会进一步完善本算法，形成一个更为精准，基于整首乐曲的和弦算法。
    * 
    */

    class chordAlgorithm
    {
        const int totalChord = 7;//Total chord +1
        const int auxChordNum = 5;
        cChord[] myChord = new cChord[totalChord + 1];
        cChord[][] auxiliaryChord = new cChord[totalChord + 1][];
        private int musicStyle { get; set; }
        private int[,] chordRelationTable = new int[totalChord + 1, totalChord + 1]
        {
           {0, 0, 0, 0, 0, 0, 0, 0},
           {0, 5, 0, 7,10, 8, 8, 5},
           {0, 0, 5, 8, 0,10, 6, 6},
           {0, 6, 0, 5, 8, 0, 8, 6},
           {0, 8, 6, 8, 5, 9, 5, 8},
           {0,10, 0, 6, 8, 5, 5, 6},
           {0, 0, 8, 6, 6, 8, 5, 6},
           {0, 9, 8, 6, 6, 9, 6, 5},
        };


        public int musicBars = 0;
        public int totalBars = 0;

        public int preChordID = 5;//赋值属和弦
        public string preChord = "G";
        public int MusicStyle { get { return musicStyle; } set { musicStyle = value; } }
        public int[,] ChordRelationTable { get { return chordRelationTable; } }
        public chordAlgorithm()
        {
            MusicStyle = 4;
            chordInit();
        }
        //初始化


        public void init()
        {
            counterInit();
            freqInit();
            preChord = "G";
            preChordID = 5;
        }
        public void chordInit()
        {
            myChord[1] = new cChord(1, 3, 5, "C", 0, 7);
            myChord[2] = new cChord(2, 4, 6, "Dm", 0, 2);
            myChord[3] = new cChord(3, 5, 7, "Em", 0, 3);
            myChord[4] = new cChord(4, 6, 1, "F", 0, 5);
            myChord[5] = new cChord(5, 7, 2, "G", 0, 6);
            myChord[6] = new cChord(6, 1, 3, "Am", 0, 4);
            myChord[7] = new cChord(7, 2, 4, "G7", 0, 1);
            //cChord(int a, int b, int c, String name, int counter, int priority)
        }
        public void auxChordInit()
        {
            auxiliaryChord[1][1] = new cChord(1, 4, 5, "Csus4", 0, 3);
            auxiliaryChord[1][2] = new cChord(5, 1, 3, "C6", 0, 2);
            auxiliaryChord[1][3] = new cChord(3, 5, 1, "C4", 0, 1);

        }
        public void counterInit()
        {
            for (int i = 1; i <= 7; i++)
                myChord[i].counter = 0;
        }
        public void freqInit()
        {
            for (int i = 1; i <= 7; i++)
                myChord[i].freq = 0;
        }

        public int checkChordTable(cChord[] chordInput)
        {
            int n = preChordID, m = 1;// m is the result chordID
            //首先需要满足chordtable，其次选出counter最优，再选择table值最优。
            int j = 1;
            int tableValue = 0;
            while (j <= totalChord)
            {
                tableValue = ChordRelationTable[n, chordInput[j].ChordID];
                //检查counter一样的情况下选tableValue最大的
                //到下一个索引（m）
                if (chordInput[j].counter != chordInput[j + 1].counter)
                {
                    m = j;
                }
                else
                {
                    while (j + 1 <= totalChord && chordInput[j].counter == chordInput[j + 1].counter)//counter一样时
                    {
                        if (ChordRelationTable[n, chordInput[j + 1].ChordID] > ChordRelationTable[n, chordInput[m].ChordID])
                        {
                            m = j + 1;
                        }
                        j++;
                    }
                }
                if (ChordRelationTable[n, chordInput[m].ChordID] > 0)
                {
                    break;
                }
                else
                {
                    // Console.WriteLine("n and chordID is " + n + " " + chordInput[m].ChordID );
                    //Console.WriteLine("j is 0 and j is " + j);
                    j++;
                }

            }
            return m;
        }


        public void checkInput(char[] input)
        {
            int n = input.Length;
            for (int i = 0; i < n; i++)
            {
                switch (input[i])
                {
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                        break;
                    default:
                        throw new ProjectException("Wrong Input!");
                }
            }
        }

        public void collectMusicInfo()
        {

        }

        public void chordCounter(string inputStr)
        {
            char[] input = inputStr.ToCharArray();
            checkInput(input);
            int n = input.Length;
            for (int i = 0; i < n; i++)
            {
                for (int j = 1; j <= totalChord; j++)
                {
                    if (input[i] - '0' == myChord[j].Note1)
                    {
                        myChord[j].counter++;
                    }
                    if (input[i] - '0' == myChord[j].Note2)
                    {
                        myChord[j].counter++;
                    }
                    if (input[i] - '0' == myChord[j].Note3)
                    {
                        myChord[j].counter++;
                    }
                    if (input[i] - '0' == myChord[j].Note4)
                    {
                        myChord[j].counter++;
                    }
                }
            }
        }
        //综合算法，将音符用数学进行处理。
        public String chordFeedback(String inputStr)  //throws ProjectException
        {
            int selectedChordID;
            string selectedChord;
            chordCounter(inputStr);

            // chordSort(myChord);
            optimizedChordSort(myChord);
            selectedChordID = checkChordTable(myChord);
            //myChord[selectedChordID].freq++;
           // printInfo(); // 输出此次进行处理的所有数据信息
                         //Console.WriteLine("PreChord is" + preChord);
            counterInit(); //renew the counter for note analysis
            musicBars++;// counter the bars
            if (musicBars % MusicStyle == 0)
                freqInit();

            selectedChord = myChord[selectedChordID].name;
            preChord = selectedChord;
            preChordID = myChord[selectedChordID].ChordID;

            return selectedChord;
        }

        public String firstChord(String inputStr)
        {
            chordCounter(inputStr);
            optimizedChordSort(myChord);
            preChord = myChord[1].name;
            preChordID = myChord[1].ChordID;
            return myChord[1].name;
        }

        public String lastChord(String inputStr)
        {
            string x = "unsolved";
            return x;
        }

        //利用到了排序
        public void optimizedChordSort(cChord[] a)
        {
            Array.Sort(a);
        }

        public bool checkInput(string str)
        {
            int tag = str.Length;
            char[] t = str.ToCharArray();
            for (int i = 0; i < tag; i++)
            {
                switch (t[i])
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                        break;
                    default:
                        throw new ProjectException("Sorry!No such Command!");
                }
            }
            return true;
        }
        /*
  public void printChordRelationTable()
  {
      for (int i = 1; i <= totalChord; i++)
      {
          for (int j = 1; j <= totalChord; j++)
          {
               Console.Write(chordRelationTable[i, j] + " ");   
          }
          Console.WriteLine();
      }
  }
  */
        public string originTohandle(string str)
        {
            string result = "";
            char[] s = str.ToArray();
            int len = str.Length;
            for (int i = 0; i < len; i++)
            {
                char temp = s[i];
                switch (temp)
                {
                    case 'C':
                        result += '1';
                        break;
                    case 'D':
                        result += '2';
                        break;
                    case 'E':
                        result += '3';
                        break;
                    case 'F':
                        result += '4';
                        break;
                    case 'G':
                        result += '5';
                        break;
                    case 'A':
                        result += '6';
                        break;
                    case 'B':
                        result += '7';
                        break;
                    default:
                        break;
                }
            }
            return result;
        }
        public string rnnChordGenertor(string inStr)//','分隔开音符传递进来
        {
            string outStr = "";
            //outStr = cnnFunction();
            var RNN = new Csharp2Python();
            outStr = RNN.run(inStr);
            // outStr = RNN.connectPython(inStr);
            
            //if (splitedStr[i] != "")//防止出现  最后一个和弦（split函数多余出来的）对应的旋律是空的
            //    outStr += chordFeedback(originTohandle(splitedStr[i++])) + " ";

            int i = 0;
            //最后两个和弦为 G C
            if (i > 3)//有4个和弦时
            {
                int len = outStr.Length, k = 0, j;
                for (j = len - 1; j > 0;)
                {
                    if (k > 1) break;
                    if (outStr[j] == ' ')
                    {
                        k++;
                        j--;
                    }
                    while (outStr[j] != ' ')
                        j--;
                }
                outStr = outStr.Substring(0, j);
                outStr += " G C ";
            }

            return outStr;
        }
        public string multiChordGenertor(string inStr)//','分隔开音符传递进来
        {
            string outStr = "";
            string[] splitedStr = inStr.Split(',');
            int nOfChord = splitedStr.Length;
            int i;
            for (i = 0; i < splitedStr.Length - 1 ; i++)//将最后两个和弦留出来
            {
                outStr += chordFeedback(originTohandle(splitedStr[i])) + " ";
            }
            if (splitedStr[i] != "")//防止出现  最后一个和弦（split函数多余出来的）对应的旋律是空的
                outStr += chordFeedback(originTohandle(splitedStr[i++])) + " ";
            //最后两个和弦为 G C
            if (i > 3)//有4个和弦时
            {
                int len = outStr.Length, k=0, j;
                for(j= len-1; j>0;)
                {
                    if (k > 1) break;
                    if(outStr[j] == ' ')
                    {
                        k++;
                        j--;
                    }
                    while (outStr[j] != ' ')
                        j--;
                }
                outStr = outStr.Substring(0,j);
                outStr += " G C ";
            }

            return outStr;
        }



        #region PrintInfo

        public void printInfo()
        {
            Console.WriteLine("Basic info:");
            for (int i = 1; i <= 7; i++)
                Console.Write(myChord[i].name + " ");
            Console.WriteLine();

            Console.WriteLine("Name:");
            for (int i = 1; i <= 7; i++)
                Console.Write(myChord[i].name + " ");
            Console.WriteLine();

            Console.WriteLine("Counter:");
            for (int i = 1; i <= 7; i++)
                Console.Write(myChord[i].counter + " ");
            Console.WriteLine();
            /*
                        Console.WriteLine("Frequency:");
                        for (int i = 1; i <= 7; i++)
                            Console.Write(myChord[i].freq + " ");
                        Console.WriteLine();
                        */
        }

        #endregion
    }
}

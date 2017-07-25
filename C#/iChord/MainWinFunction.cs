using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Xml;
using System.IO;
using SimpleMidiPlayer.Midi;
using System.Windows.Markup;

namespace iChord
{

    #region function
    public partial class MainWindow : Window
    {

        public void defineUserChord(string inputChord, int instrumentID)
        {
            string[] splitedChord = inputChord.Split(new Char[] { ',' });
            chordTrack[instrumentID].userChord = splitedChord;
        }

        public void pianoKeyDown(int xKey)
        {
            string send = NoteInput.input(xKey);
            if (editSingleNote)
            {
                inputMainMelodyChange(singleNoteId, send);
                editSingleNote = false;
            }
            else
            {
                this.textBlock_main.Text += send;
                addMelody(send.Trim());//加一个音
            }
            keyDown(send);

        }
        public void pianoKeyUp(int xKey)
        {
            string send = NoteInput.input(xKey);
            send.Substring(0, send.Length - 1);
        }
        public void chordAndnoteFeedBack()
        {
            textBlock_main.Text += textBlock_main.Text;
            string str = textBlock_main.Text;
            str = myAlgorithm.originTohandle(str);
            if (str != null)
                textBlock_main2.Text += " " + myAlgorithm.chordFeedback(str);
            textBlock_main.Text = "";
        }

        //public void change


        public void mainPlay()
        {
            string[] track = new string[instrumentN];
            string score0 = textBlock_main.Text.Replace(" ", "").Replace(",", "");// 去掉曲谱里的逗号和空格
            try
            {
                NoteInput.inputCheck(score0);
                winInputCheck();
                track[0] = textBlock_main.Text.Replace(" ", "");
                if (muteMainScore)
                {
                    track[0] = NoteInput.muteNote(track[0]);
                }

                string resultChordSeq = textBlock_main2.Text.Trim();
                // MessageBox.Show(textBlock_main2.Text);
                track[1] = chordTrack[0].splitedChordNote(resultChordSeq);
                track[2] = chordTrack[1].splitedChordNote(resultChordSeq);
                track[3] = chordTrack[2].splitedChordNote(resultChordSeq);

                /*
                for (int i = 0; i < instrumentN; i++)
                {
                    if (track[i].Last() == ',')
                        track[i] = track[i].Substring(0, track[i].Length - 1);

                    chordTrack[i].adjustForOctave(track[i], track[0]);//调整八度关系
                    //MessageBox.Show(track[i]);
                }
                */
                /*
                foreach(string x in track)
                {
                    MessageBox.Show(x);
                }
                */
               // if( ! ( textBlock_main.Text==null && textBlock_main2.Text== null) )
                MidiPlay.BrandNew(myMidiDevice, track[0], timbreTrack[0], track[1], timbreTrack[1], track[2], timbreTrack[2], track[3], timbreTrack[3]);
            }
            catch (UserInputException )
            {
                MessageBox.Show("请输入音符");
                initAllInputWin();
            }
        }

        public void chordComboxChange(int x, UserDefinedChord uC)
        {
            switch (x)
            {
                case 1:
                    uC.stdPianoChord1(); break;
                case 2:
                    uC.stdPianoChord2(); break;
                case 3:
                    uC.stdPianoChord3(); break;
                case 4:
                    uC.stdPianoChord4(); break;
                case 5:
                    uC.stdPianoChord5(); break;
                case 6:
                    uC.stdPianoChord6(); break;
            }
        }

        public void inputDelete()
        {
            int len = textBlock_main.Text.Length;
            //string feedStr = "";
            if (textBlock_main.Text != null && len - LengthOfeachNote >= 0)
            {
                textBlock_main.Text = textBlock_main.Text.Replace(",", "");
                len = textBlock_main.Text.Length;
                textBlock_main.Text = textBlock_main.Text.Substring(0, len - LengthOfeachNote);
                clearAll();

                if (textBlock_main.Text != "")
                    setMelody(textBlock_main.Text);
            }

        }

        public int randomTimbre()
        {
            Random rd = new Random();
            int x = rd.Next(1, 50);
            return x;
        }

        public void timbreChange(int trackNum, int timbreID)
        {
            timbreTrack[trackNum] = timbreID;
        }
        public void timbreChangeFor0(int trackNum, int timbreID)
        {
            muteMainScore = false;
            timbreTrack[trackNum] = timbreID;
        }

        public void initChordTrack()
        {
            for (int i = 0; i < instrumentN; i++)
                chordTrack[i] = new UserDefinedChord();
        }

        public void initMidi()
        {
            for (int i = 0; i < instrumentN; i++)
                timbreTrack[i] = 2;
        }
        public void initTimbre()
        {

        }
        public void initMusicInfo()
        {
            myAlgorithm.init();
        }
        public void initAllInputWin()
        {
            textBlock_main.Text = textBlock_main2.Text = "";
        }
        public void mainInit()
        {
            initChordTrack();
            initMidi();
        }
        public void keyDown(string str)
        {
            //string s = str + "5";
            MidiPlay.BrandNew(myMidiDevice, str);

        }

        public void winInputCheck()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (textBlock_main.Text == "" && textBlock_main2.Text == "")
                    throw new UserInputException("You input nothing!");
            }), null);
           
        }

        public void winShow(string chord, string melody)
        {
            clearAll();
            setMelody(melody);
            setChord(chord);
        }

        public async Task smartChord()
        {
            string myStr = inputMainMelody = textBlock_main.Text.Replace(",", "");
            textBlock_main.Text = NoteInput.devideBar(myStr);
            myAlgorithm.init();//初始化后重新算一遍
            string str = myAlgorithm.rnnChordGenertor(textBlock_main.Text);//RNN Function
            //string str = myAlgorithm.multiChordGenertor(textBlock_main.Text);//old Function
            Console.WriteLine(str);
            if (!String.IsNullOrEmpty(str) && str.Last() != ' ')
                str += " ";
            textBlock_main2.Text = inputMainChord = str;
        }

        public void keyDownColor_right(int x)
        {
            SolidColorBrush keyColor = new SolidColorBrush(Color.FromRgb(170, 220, 240));
            switch (x)
            {
                case 48: white_piano0.Background = keyColor; break;
                case 50: white_piano1.Background = keyColor; break;
                case 52: white_piano2.Background = keyColor; break;
                case 53: white_piano3.Background = keyColor; break;
                case 55: white_piano4.Background = keyColor; break;
                case 57: white_piano5.Background = keyColor; break;
                case 59: white_piano6.Background = keyColor; break;

                case 60: white_piano7.Background = keyColor; break;
                case 62: white_piano8.Background = keyColor; break;
                case 64: white_piano9.Background = keyColor; break;
                case 65: white_piano10.Background = keyColor; break;
                case 67: white_piano11.Background = keyColor; break;
                case 69: white_piano12.Background = keyColor; break;
                case 71: white_piano13.Background = keyColor; break;

                case 72: white_piano14.Background = keyColor; break;
                case 74: white_piano15.Background = keyColor; break;
                case 76: white_piano16.Background = keyColor; break;
                case 77: white_piano17.Background = keyColor; break;
                case 79: white_piano18.Background = keyColor; break;
                case 81: white_piano19.Background = keyColor; break;
                case 83: white_piano20.Background = keyColor; break;

                case 84: white_piano21.Background = keyColor; break;
                default: break;

            }
        }
        public void keyDownColor_left(int x)
        {
            SolidColorBrush keyColor = new SolidColorBrush(Color.FromRgb(100, 169, 251));
            switch (x)
            {
                case 48: white_piano0.Background = keyColor; break;
                case 50: white_piano1.Background = keyColor; break;
                case 52: white_piano2.Background = keyColor; break;
                case 53: white_piano3.Background = keyColor; break;
                case 55: white_piano4.Background = keyColor; break;
                case 57: white_piano5.Background = keyColor; break;
                case 59: white_piano6.Background = keyColor; break;

                case 60: white_piano7.Background = keyColor; break;
                case 62: white_piano8.Background = keyColor; break;
                case 64: white_piano9.Background = keyColor; break;
                case 65: white_piano10.Background = keyColor; break;
                case 67: white_piano11.Background = keyColor; break;
                case 69: white_piano12.Background = keyColor; break;
                case 71: white_piano13.Background = keyColor; break;

                case 72: white_piano14.Background = keyColor; break;
                case 74: white_piano15.Background = keyColor; break;
                case 76: white_piano16.Background = keyColor; break;
                case 77: white_piano17.Background = keyColor; break;
                case 79: white_piano18.Background = keyColor; break;
                case 81: white_piano19.Background = keyColor; break;
                case 83: white_piano20.Background = keyColor; break;

                case 84: white_piano21.Background = keyColor; break;
                default: break;

            }
        }

        public void keyUpColor(int x)
        {
            SolidColorBrush keyColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            switch (x)
            {
                case 48: white_piano0.Background = keyColor; break;
                case 50: white_piano1.Background = keyColor; break;
                case 52: white_piano2.Background = keyColor; break;
                case 53: white_piano3.Background = keyColor; break;
                case 55: white_piano4.Background = keyColor; break;
                case 57: white_piano5.Background = keyColor; break;
                case 59: white_piano6.Background = keyColor; break;

                case 60: white_piano7.Background = keyColor; break;
                case 62: white_piano8.Background = keyColor; break;
                case 64: white_piano9.Background = keyColor; break;
                case 65: white_piano10.Background = keyColor; break;
                case 67: white_piano11.Background = keyColor; break;
                case 69: white_piano12.Background = keyColor; break;
                case 71: white_piano13.Background = keyColor; break;

                case 72: white_piano14.Background = keyColor; break;
                case 74: white_piano15.Background = keyColor; break;
                case 76: white_piano16.Background = keyColor; break;
                case 77: white_piano17.Background = keyColor; break;
                case 79: white_piano18.Background = keyColor; break;
                case 81: white_piano19.Background = keyColor; break;
                case 83: white_piano20.Background = keyColor; break;

                case 84: white_piano21.Background = keyColor; break;
                default: break;
            }
        }

        public void inputMainMelodyChange(int x, string insertStr)//旋律轨道音符改变，x为音符序号，inserStr为要修改的字符串
        {
            if (inputMainMelody == null) inputMainMelody = textBlock_main.Text;
            inputMainMelody = inputMainMelody.Remove(x * LengthOfeachNote, LengthOfeachNote).Insert(x * LengthOfeachNote, insertStr);
            textBlock_main.Text = NoteInput.devideBar(inputMainMelody);
            clearAll();
            setMelody(textBlock_main.Text);
           
        }
        public void inputMainMelodyChange(int x)
        {
            editSingleNote = true;
            singleNoteId = x;
        }

        public void inputMainChordChange(int x, string insertStr)//和弦轨道音符改变，x为和弦序号，inserStr为要修改的字符串
        {
            insertStr += " ";
            if (insertStr == null) inputMainChord = textBlock_main2.Text;
            int iStart = 0, iEndCount = 0, cnt=0;
            foreach (char c in inputMainChord)
            {
                if (cnt == x)
                    break;
                if (c == ' ')
                    cnt++;
                iStart++;
            }

            for (int i = iStart; i < inputMainChord.Length; i++)
            {
                if (inputMainChord[i] != ' ')
                    iEndCount++;
                else
                    break;
            }
            iEndCount++;//删掉空格
            //MessageBox.Show("Maininput:" + inputMainChord + "textBlock:" + textBlock_main2.Text + 1);
            if (inputMainChord.Last() != ' ') iEndCount--;
            inputMainChord = inputMainChord.Remove(iStart, iEndCount).Insert(iStart, insertStr);
            textBlock_main2.Text = inputMainChord;
            //MessageBox.Show(inputMainChord);
        }

        public void noteDurationChang(int x, string urlStr )
        {
            NoteInput.duration = x;
            ImageBrush[] defaultBrush = new ImageBrush[5];
            for(int i=0; i<5; i++)
                defaultBrush[i] = new ImageBrush();

            defaultBrush[0].ImageSource = new BitmapImage(new Uri("Icons/Note/note-black-1_16.png", UriKind.Relative));
            label2_Duration.Background = defaultBrush[0];
            defaultBrush[1].ImageSource = new BitmapImage(new Uri("Icons/Note/note-black-1_8.png", UriKind.Relative));
            label3_Duration.Background = defaultBrush[1];
            defaultBrush[2].ImageSource = new BitmapImage(new Uri("Icons/Note/note-black-1_4.png", UriKind.Relative));
            label4_Duration.Background = defaultBrush[2];
            defaultBrush[3].ImageSource = new BitmapImage(new Uri("Icons/Note/note-black-1_2.png", UriKind.Relative));
            label5_Duration.Background = defaultBrush[3];
            defaultBrush[4].ImageSource = new BitmapImage(new Uri("Icons/Note/note-black-1_1.png", UriKind.Relative));
            label6_Duration.Background = defaultBrush[4];

            ImageBrush my = new ImageBrush();
            my.ImageSource = new BitmapImage(new Uri(urlStr, UriKind.Relative));
            switch(x)
            {
                case 2:
                    label2_Duration.Background = my; break;
                case 3:
                    label3_Duration.Background = my; break;
                case 4:
                    label4_Duration.Background = my; break;
                case 5:
                    label5_Duration.Background = my; break;
                case 6:
                    label6_Duration.Background = my; break;
            }
          }


    }
    #endregion
}

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
    #region Windows Demonstration
    public partial class MainWindow : Window
    {
        #region const
        const int MAX = 100;
        const int pr = 5;
        const int MAXCNT_MELODY = pr * 16 + pr - 1;
        const int MAXCNT_CHORD = 2 * pr - 1;
        const double width = 320;//default
        const double melodyheight = 27;//default旋律图片高度
        const double chordheight = 27;//default
        const double height = 27;
        const double upheight = 6;//旋律图片上压缩距离
        const double downheight = 0;
        const int chordfontsize = 15;//和弦字母大小
        Color highlight = Color.FromRgb(1, 1, 1);
        #endregion

        #region locationflag
        private int[] cnt_melody = new int[MAX];
        private int[] cnt_chord = new int[MAX];
        private int melodyloc;
        private int chordloc;
        private int sploc;
        private int hlmelody1 = 0;//locate high light melody location
        private int hlmelody2 = 0;
        public int thlmelody;//location of the high light melody for transferring
        public int thlchord;
        private int hlchord1;//locate high light chord location
        private int hlchord2;
        private int chordchoice=0;
        #endregion

        Label[,] chordhl = new Label[MAX, MAXCNT_CHORD];
        Label[,] melodyhl = new Label[MAX, MAXCNT_MELODY];
        ComboBox[,] chordcb = new ComboBox[MAX, MAXCNT_CHORD];//quantity:maxValue of the chord labels
        Label[,] melodylabel = new Label[MAX, MAXCNT_MELODY];//quantity:maxValue of the melody labels
        ImageBrush[] imagepic = new ImageBrush[1000];//enought quantity for presentation

        private StackPanel[] msp;
        private StackPanel[] csp;
        private StackPanel[] blanksp;
        private Grid[] mg = new Grid[MAX];
        private Grid[] cg = new Grid[MAX];

        private string curstr;

        private void initOthers()
        {
            initData();
            msp = new StackPanel[MAX];
            csp = new StackPanel[MAX];
            blanksp = new StackPanel[2 * MAX];
            for (int i = 0; i < MAX; i++)
            {
                msp[i] = new StackPanel();
                msp[i].Height = height;
                csp[i] = new StackPanel();
                csp[i].Height = height;
                blanksp[2 * i] = new StackPanel();
                blanksp[2 * i].Height = upheight;
                blanksp[2 * i + 1] = new StackPanel();
                blanksp[2 * i + 1].Height = downheight;
            }
            for (int i = 0; i < MAX; i++)
                for (int j = 0; j < MAXCNT_MELODY; j++)
                {
                    melodylabel[i, j] = new Label();
                    melodylabel[i, j].HorizontalContentAlignment = HorizontalAlignment.Center;
                    melodylabel[i, j].VerticalContentAlignment = VerticalAlignment.Center;
                    melodylabel[i, j].Height = melodyheight;
                    melodylabel[i, j].HorizontalAlignment = HorizontalAlignment.Stretch;
                    melodylabel[i, j].VerticalAlignment = VerticalAlignment.Stretch;
                    melodylabel[i, j].Margin = new Thickness(0, 0, 0, 0);
                    melodyhl[i, j] = new Label();
                    melodyhl[i, j].MouseDown += melodyhl_MouseDown;
                    melodyhl[i, j].Background = new SolidColorBrush(highlight);
                    melodyhl[i, j].Opacity = 0;
                }
            for (int i = 0; i < MAX; i++)
                for (int j = 0; j < MAXCNT_CHORD; j++)
                {
                    chordcb[i, j] = new ComboBox();
                    chordcb[i, j].HorizontalContentAlignment = HorizontalAlignment.Left;
                    chordcb[i, j].VerticalContentAlignment = VerticalAlignment.Center;
                    chordcb[i, j].Height = chordheight;
                    chordcb[i, j].HorizontalAlignment = HorizontalAlignment.Stretch;
                    chordcb[i, j].VerticalAlignment = VerticalAlignment.Stretch;
                    chordcb[i, j].Margin = new Thickness(0, 0, 0, 0);
                    chordcb[i, j].FontSize = chordfontsize;
                    chordcb[i, j].MouseEnter += chordcb_MouseEnter;
                    chordcb[i, j].MouseLeave += chordcb_MouseLeave;
                    //chordcb[i, j].ContextMenuClosing += chordcb_ContextMenuClosing;
                    //chordcb[i, j].SelectionChanged += chordcb_SelectionChanged;

                    /*
                    chordcb[i, j].Background=new SolidColorBrush(Color.FromRgb(189,189,189));
                    chordcb[i, j].MouseDown += chordcb_MouseDown;
                    chordcb[i, j].Foreground = new SolidColorBrush(Color.FromRgb(207, 207, 207));
                    chordcb[i, j].BorderThickness = new Thickness(0, 0, 0, 0);
                    chordcb[i, j].MouseEnter += chordcb_MouseEnter;
                    */
                    chordcb[i, j].Style = Resources["ComboBox-ChordShow"] as Style;
                    //chordcb[i, j].Style = Resources["ComboBox"] as Style;

                    chordcb[i, j].Items.Add("C");
                    chordcb[i, j].Items.Add("Dm");
                    chordcb[i, j].Items.Add("Em");
                    chordcb[i, j].Items.Add("F");
                    chordcb[i, j].Items.Add("G");
                    chordcb[i, j].Items.Add("Am");
                    chordcb[i, j].Items.Add("G7");
                    chordcb[i, j].SelectedIndex = 0;
                }
            for (int i = 0; i < 1000; i++)
            {
                imagepic[i] = new ImageBrush();
                imagepic[i].ImageSource = new BitmapImage(new Uri(String.Format("images/{0}.png", i), UriKind.Relative));
                //index first 22 for basic melody pic.22 for 8. 22 for 16;then 66 for - then 67 for 0 68 for 0/ 69 for 0// 70 for _ 71 for blank 72 for |
            }
            addNewSP();
            //disableAll();
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void fileinit()
        {
            //文件存储功能初始化函数

            if (!Directory.Exists("MusicLib"))
            {
                DirectoryInfo d = Directory.CreateDirectory("MusicLib");
            }
            string[] Files;
            Files = Directory.GetFiles("MusicLib");
            for (int i = 0; i < Files.Length; i++)
            {
                if (Files[i].Substring(Files[i].Length - 4) == ".icd")
                {
                    int j = 1 + Files[i].LastIndexOf('\\');
                    int len = Files[i].Length;
                    listView.Items.Add(Files[i].Substring(j, len - 4 - j));

                }
            }
        }
        private void initData()
        {
            for (int i = 0; i < MAX; i++)
            {
                cnt_melody[i] = 0;
                cnt_chord[i] = 0;
            }
            sploc = 0;
            melodyloc = 0;
            chordloc = 0;
            curstr = "0";
        }
        private void melodyhl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            melodyhl[hlmelody1, hlmelody2].Opacity = 0;
            for (int i = 0; i <= melodyloc; i++)
                for (int j = 0; j < min(cnt_melody[i], MAXCNT_MELODY); j++)
                {
                    if (melodyhl[i, j].Equals((Label)sender))
                    {
                        hlmelody1 = i;
                        hlmelody2 = j;
                    }
                }
            melodyhl[hlmelody1, hlmelody2].Opacity = 0.5;
            //melodylabel[hlmelody1, hlmelody2].BorderBrush = Brushes.Yellow;
            //melodylabel[hlmelody1, hlmelody2].BorderThickness = new Thickness(1, 1, 1, 1);
            //MessageBox.Show(String.Format("{0},{1}", hlmelody1, hlmelody2));
            thlmelody = countTransm();

            inputMainMelodyChange(thlmelody);
        }
        private void chordcb_MouseEnter(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i <= chordloc; i++)
                for (int j = 0; j < min(cnt_chord[i],MAXCNT_CHORD); j++)
                {
                    if (chordcb[i, j].Equals((ComboBox)sender))
                    {
                        hlchord1 = i;
                        hlchord2 = j;
                    }
                }
            chordchoice = chordcb[hlchord1, hlchord2].SelectedIndex;
        }
        private void chordcb_MouseLeave(object sender,RoutedEventArgs e)
        {
            int tmpchoice = chordcb[hlchord1, hlchord2].SelectedIndex;
            if (tmpchoice != chordchoice)
            {
                //MessageBox.Show(String.Format("{0},{1}", hlchord1, hlchord2));
                string tmpstr = null;
                chordchoice = tmpchoice;
                switch (tmpchoice)
                {
                    case 0:
                        tmpstr = "C";
                        break;
                    case 1:
                        tmpstr = "Dm";
                        break;
                    case 2:
                        tmpstr = "Em";
                        break;
                    case 3:
                        tmpstr = "F";
                        break;
                    case 4:
                        tmpstr = "G";
                        break;
                    case 5:
                        tmpstr = "Am";
                        break;
                    case 6:
                        tmpstr = "G7";
                        break;
                }
                thlchord = countTransc();
                //MessageBox.Show(String.Format("{0}", thlchord));
               // MessageBox.Show(String.Format("{0}" + tmpstr, thlchord));
                inputMainChordChange(thlchord, tmpstr);
            }
        }
        private void chordcb_ContextMenuClosing(object sender,ContextMenuEventArgs e)
        {
            int tmpchoice = chordcb[hlchord1, hlchord2].SelectedIndex;
            if (tmpchoice != chordchoice)
            {
                //MessageBox.Show(String.Format("{0},{1}", hlchord1, hlchord2));
                string tmpstr=null;
                chordchoice = tmpchoice;
                switch(tmpchoice)
                {
                    case 0:
                        tmpstr = "C";
                        break;
                    case 1:
                        tmpstr = "Dm";
                        break;
                    case 2:
                        tmpstr = "Em";
                        break;
                    case 3:
                        tmpstr = "F";
                        break;
                    case 4:
                        tmpstr = "G";
                        break;
                    case 5:
                        tmpstr = "Am";
                        break;
                    case 6:
                        tmpstr = "G7";
                        break;
                }
                thlchord = countTransc();
                //MessageBox.Show(String.Format("{0}",thlchord));
                inputMainChordChange(thlchord, tmpstr);
            }

        }
        private void chordcb_MouseDown(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i <= chordloc; i++)
                for (int j = 0; j < cnt_chord[chordloc]; j++)
                {
                    if (melodyhl[i, j].Equals((ComboBox)sender))
                    {
                        hlchord1 = i;
                        hlchord2 = j;
                    }
                }
            chordcb[hlchord1, hlchord2].Background = new SolidColorBrush(Color.FromRgb(133, 133, 133));
        }
        /*
        private void chordcb_SelectionChanged(object sender ,SelectionChangedEventArgs e)
        {
            for (int i = 0; i <= chordloc; i++) 
                for(int j=0;j< min(cnt_chord[i], MAXCNT_CHORD);j++)
                {
                    if(chordcb[i,j].Equals((ComboBox)sender))
                    {
                        hlchord1 = i;
                        hlchord2 = j;
                    }
                }
            MessageBox.Show(String.Format("{0},{1}", hlchord1, hlchord2));
        }
        */
        private int countTransm()
        {
            inputMainMelody = textBlock_main.Text.Replace(",","");
            string tmp =string.Copy(inputMainMelody);
            string[] strSplitted = tmp.Trim().Split(new char[] { ' ' });
            int len = strSplitted.Length;
            int time = pr * 16 * hlmelody1 + hlmelody2;
            int cur = 0;
            int loc = 0;
            for(int i=0;i< pr;i++)
            {
                if (hlmelody2 > i * 17 + 16) time--;
            }
            for (;cur < time; loc++)
            {
                switch (strSplitted[loc].ElementAt(3))
                {
                    case '2':
                        cur += 1;
                        break;
                    case '3':
                        cur += 2;
                        break;
                    case '4':
                        cur += 4;
                        break;
                    case '5':
                        cur += 8;
                        break;
                    case '6':
                        cur += 16;
                        break;
                }
            }
            //MessageBox.Show(String.Format("{0}", loc));
            return loc;
        }
        private int countTransc()
        {
            int loc=hlchord1*pr+hlchord2/2;
            return loc;
        }
        /* ???
        [System.Runtime.InteropServices.DllImport("user32.dll ")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int wndproc);
        [System.Runtime.InteropServices.DllImport("user32.dll ")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        public const int GWL_STYLE = -16;
        public const int WS_DISABLED = 0x8000000;

        public static void SetControlEnabled(Control c, bool enabled)
        {
            if (enabled)
            { SetWindowLong(c.Handle, GWL_STYLE, (~WS_DISABLED) & GetWindowLong(c.Handle, GWL_STYLE)); }
            else
            { SetWindowLong(c.Handle, GWL_STYLE, WS_DISABLED + GetWindowLong(c.Handle, GWL_STYLE)); }
        }
        */
        private void disableAll()
        {
            foreach (UIElement element in mainWindow.Children)
            {
                element.IsEnabled = false;
            }
        }
        private bool addNewSP()
        {
            //upblank
            mainsp.Children.Add(blanksp[sploc * 2]);
            //melodypart
            mg[sploc] = new Grid();
            ColumnDefinition[] tmpcd = new ColumnDefinition[MAXCNT_MELODY];
            for (int i = 0; i < MAXCNT_MELODY; i++)
            {
                tmpcd[i] = new ColumnDefinition();
                mg[sploc].ColumnDefinitions.Add(tmpcd[i]);
            }
            for (int i = 16; i < MAXCNT_MELODY; i += 17)
            {
                melodylabel[sploc, i].Background = imagepic[72];
                mg[sploc].Children.Add(melodylabel[sploc, i]);
                Grid.SetColumn(melodylabel[sploc, i], i);
                melodylabel[sploc, i].Visibility = Visibility.Hidden;
            }
            msp[sploc].Children.Add(mg[sploc]);
            mainsp.Children.Add(msp[sploc]);
            //downblank
            mainsp.Children.Add(blanksp[sploc * 2 + 1]);
            //chordpart
            cg[sploc] = new Grid();
            cg[sploc].Background = new SolidColorBrush(Color.FromRgb(189, 189, 189));
            //cg[sploc].Background = new SolidColorBrush(Color.FromRgb(229, 229,229));
            tmpcd = new ColumnDefinition[MAXCNT_CHORD];
            for (int i = 0; i < MAXCNT_CHORD; i++)
            {
                tmpcd[i] = new ColumnDefinition();
                if (i % 2 == 0) tmpcd[i].Width = new GridLength(16, GridUnitType.Star);
                else tmpcd[i].Width = new GridLength(1, GridUnitType.Star);
                cg[sploc].ColumnDefinitions.Add(tmpcd[i]);
            }
            csp[sploc].Children.Add(cg[sploc]);
            mainsp.Children.Add(csp[sploc]);
            sploc++;
            return true;
        }
        private bool judgeNewSP()
        {
            if (chordloc > sploc - 1 || melodyloc > sploc - 1)
            {
                addNewSP();
            }
            return true;
        }
        private Grid getMelodyGrid()
        {
            Grid tmpg = new Grid();
            ColumnDefinition[] tmpcd = new ColumnDefinition[MAXCNT_MELODY];
            for (int i = 0; i < MAXCNT_MELODY; i++)
            {
                tmpcd[i] = new ColumnDefinition();
                tmpg.ColumnDefinitions.Add(tmpcd[i]);
            }
            return tmpg;
        }
        private Grid getChordGrid()
        {
            Grid tmpg = new Grid();
            ColumnDefinition[] tmpcd = new ColumnDefinition[MAXCNT_CHORD];
            for (int i = 0; i < MAXCNT_CHORD; i++)
            {
                tmpcd[i] = new ColumnDefinition();
                tmpg.ColumnDefinitions.Add(tmpcd[i]);
            }
            return tmpg;
        }
        public void setChord(string str)//str和弦，空格分开。
        {
            string[] strSplitted = str.Split(new char[] { ' ' });
            int len = strSplitted.Length;
            for (int i = 0; i < len; i++)
            {
                addChord(strSplitted[i]);
            }
        }
        public void setMelody(string str)
        {
            str = str.Replace(",", "").Trim();
            string[] strSplitted = str.Split(new char[] { ' ' });
            int len = strSplitted.Length;
            for (int i = 0; i < len; i++)
            {
                addMelody(strSplitted[i]);
            }
        }
        private void addChord(string str)
        {
            switch (str)
            {
                case "C":
                    chordcb[chordloc, cnt_chord[chordloc]].SelectedIndex = 0;
                    break;
                case "Dm":
                    chordcb[chordloc, cnt_chord[chordloc]].SelectedIndex = 1;
                    break;
                case "Em":
                    chordcb[chordloc, cnt_chord[chordloc]].SelectedIndex = 2;
                    break;
                case "F":
                    chordcb[chordloc, cnt_chord[chordloc]].SelectedIndex = 3;
                    break;
                case "G":
                    chordcb[chordloc, cnt_chord[chordloc]].SelectedIndex = 4;
                    break;
                case "Am":
                    chordcb[chordloc, cnt_chord[chordloc]].SelectedIndex = 5;
                    break;
                case "G7":
                    chordcb[chordloc, cnt_chord[chordloc]].SelectedIndex = 6;
                    break;
            }
            cg[chordloc].Children.Add(chordcb[chordloc, cnt_chord[chordloc]]);
            Grid.SetColumn(chordcb[chordloc, cnt_chord[chordloc]], cnt_chord[chordloc]);
            cnt_chord[chordloc] += 2;
            if (cnt_chord[chordloc] >= MAXCNT_CHORD)
                chordloc++;
            judgeNewSP();
        }
        private void audiJudgeLine()
        {
            if (cnt_melody[melodyloc] % 17 == 16)
            {
                cnt_melody[melodyloc]++;
            }

        }
        private void audiAddMelody(bool flag)
        {
            mg[melodyloc].Children.Add(melodylabel[melodyloc, cnt_melody[melodyloc]]);
            Grid.SetColumn(melodylabel[melodyloc, cnt_melody[melodyloc]], cnt_melody[melodyloc]);
            if (!flag) return;//no action for them
            mg[melodyloc].Children.Add(melodyhl[melodyloc, cnt_melody[melodyloc]]);
            Grid.SetColumn(melodyhl[melodyloc, cnt_melody[melodyloc]], cnt_melody[melodyloc]);
        }
        private void audiJudgeMelody(int num)
        {
            switch (num)
            {
                case 2:
                    cnt_melody[melodyloc]++;
                    break;
                case 3:
                    for (int i = 0; i < 2; i++)
                        audiJudgeLine();
                    break;
                case 4:
                    for (int i = 0; i < 4; i++)
                        audiJudgeLine();
                    cnt_melody[melodyloc] += 4;
                    break;
            }
            for (int i = 16; i < MAXCNT_MELODY; i += 17)
            {
                if (cnt_melody[melodyloc] > i)
                    melodylabel[melodyloc, i].Visibility = Visibility.Visible;
            }
            if (cnt_melody[melodyloc] >= MAXCNT_MELODY)
            {
                int tmp = cnt_melody[melodyloc] - MAXCNT_MELODY;
                cnt_melody[melodyloc] = MAXCNT_MELODY;
                melodyloc++;
                cnt_melody[melodyloc] += tmp;
            }
            judgeNewSP();
        }
        private void addMelody(string str)
        {
            curstr += " ";
            curstr += str;
            switch (str.ElementAt(0))
            {
                case 'C':
                    switch (str.ElementAt(1))
                    {
                        case '4':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[44];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[22];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[0];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[0];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[0];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '5':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[51];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[29];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[7];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[7];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[7];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '6':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[58];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[36];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[14];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[14];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[14];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '7':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[65];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[43];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[21];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[21];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[21];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                    }
                    break;
                case 'D':
                    switch (str.ElementAt(1))
                    {
                        case '4':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[45];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[23];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[1];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[1];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[1];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '5':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[52];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[30];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[8];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[8];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[8];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '6':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[59];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[37];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[15];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[15];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[15];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                    }
                    break;
                case 'E':
                    switch (str.ElementAt(1))
                    {
                        case '4':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[46];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[24];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[2];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[2];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[2];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '5':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[53];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[31];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[9];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[9];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[9];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '6':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[60];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[38];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[16];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[16];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[16];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                    }
                    break;
                case 'F':
                    switch (str.ElementAt(1))
                    {
                        case '4':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[47];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[25];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[3];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[3];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[3];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '5':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[54];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[32];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[10];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[10];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[10];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '6':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[61];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[39];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[17];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[17];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[17];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                    }
                    break;
                case 'G':
                    switch (str.ElementAt(1))
                    {
                        case '4':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[48];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[26];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[4];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[4];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[4];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '5':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[55];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[33];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[11];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[11];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[11];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '6':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[62];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[40];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[18];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[18];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[18];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                    }
                    break;
                case 'A':
                    switch (str.ElementAt(1))
                    {
                        case '4':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[49];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[27];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[5];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[5];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[5];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '5':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[56];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[34];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[12];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[12];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[12];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '6':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[63];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[41];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[19];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[19];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[19];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                    }
                    break;
                case 'B':
                    switch (str.ElementAt(1))
                    {
                        case '4':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[50];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[28];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[6];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[6];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[6];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '5':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[57];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[35];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[13];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[13];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[13];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                        case '6':
                            switch (str.ElementAt(3))
                            {
                                case '2':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[64];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2);
                                    break;
                                case '3':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[42];
                                    audiAddMelody(true);
                                    audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                                    break;
                                case '4':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[20];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    break;
                                case '5':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[20];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                                case '6':
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[20];
                                    audiAddMelody(true);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[66];
                                    audiAddMelody(false);
                                    audiJudgeMelody(4);
                                    break;
                            }
                            break;
                    }
                    break;
                case 'Z':
                    switch (str.ElementAt(3))
                    {
                        case '2':
                            audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[69];
                            audiAddMelody(true);
                            audiJudgeMelody(2);
                            break;
                        case '3':
                            audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[68];
                            audiAddMelody(true);
                            audiJudgeMelody(2); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[70]; audiAddMelody(false); audiJudgeMelody(2);
                            break;
                        case '4':
                            audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[67];
                            audiAddMelody(true);
                            audiJudgeMelody(4);
                            break;
                        case '5':
                            audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[67];
                            audiAddMelody(true);
                            audiJudgeMelody(4);
                            audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[67];
                            audiAddMelody(false);
                            audiJudgeMelody(4);
                            break;
                        case '6':
                            audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[67];
                            audiAddMelody(true);
                            audiJudgeMelody(4);
                            audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[67];
                            audiAddMelody(false);
                            audiJudgeMelody(4);
                            audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[67];
                            audiAddMelody(false);
                            audiJudgeMelody(4);
                            audiJudgeLine(); melodylabel[melodyloc, cnt_melody[melodyloc]].Background = imagepic[67];
                            audiAddMelody(false);
                            audiJudgeMelody(4);
                            break;
                    }
                    break;
            }
        }
        private int min(int a, int b)
        {
            return a < b ? a : b;
        }
        private bool clearAll()
        {
            mainsp.Children.Clear();
            for(int i=0;i< sploc;i++)
            {
                mg[i].Children.Clear();
                cg[i].Children.Clear();
            }
            initOthers();
            return true;
        }
        private void judgeDel()
        {
            if (cnt_melody[melodyloc] == 0)
                melodyloc--;
        }
        private void audiDelMelody(char num)
        {
            switch (num)
            {
                case '0':
                    throw new Exception("no melody!");
                case '2':
                    judgeDel();
                    cnt_melody[melodyloc]--;
                    msp[melodyloc].Children.Remove(melodylabel[melodyloc, cnt_melody[melodyloc]]);
                    break;
                case '3':
                    for (int i = 0; i < 2; i++)
                    {
                        judgeDel();
                        cnt_melody[melodyloc]--;
                        msp[melodyloc].Children.Remove(melodylabel[melodyloc, cnt_melody[melodyloc]]);
                    }
                    break;
                case '4':
                    for (int i = 0; i < 4; i++)
                    {
                        judgeDel();
                        cnt_melody[melodyloc]--;
                    }
                    msp[melodyloc].Children.Remove(melodylabel[melodyloc, cnt_melody[melodyloc]]);
                    break;
                case '5':
                    for (int i = 0; i < 4; i++)
                    {
                        judgeDel();
                        cnt_melody[melodyloc]--;
                    }
                    msp[melodyloc].Children.Remove(melodylabel[melodyloc, cnt_melody[melodyloc]]);
                    for (int i = 0; i < 4; i++)
                    {
                        judgeDel();
                        cnt_melody[melodyloc]--;
                    }
                    msp[melodyloc].Children.Remove(melodylabel[melodyloc, cnt_melody[melodyloc]]);
                    break;
                case '6':
                    for (int i = 0; i < 4; i++)
                    {
                        judgeDel();
                        cnt_melody[melodyloc]--;
                    }
                    msp[melodyloc].Children.Remove(melodylabel[melodyloc, cnt_melody[melodyloc]]);
                    for (int i = 0; i < 4; i++)
                    {
                        judgeDel();
                        cnt_melody[melodyloc]--;
                    }
                    msp[melodyloc].Children.Remove(melodylabel[melodyloc, cnt_melody[melodyloc]]);
                    for (int i = 0; i < 4; i++)
                    {
                        judgeDel();
                        cnt_melody[melodyloc]--;
                    }
                    msp[melodyloc].Children.Remove(melodylabel[melodyloc, cnt_melody[melodyloc]]);
                    for (int i = 0; i < 4; i++)
                    {
                        judgeDel();
                        cnt_melody[melodyloc]--;
                    }
                    msp[melodyloc].Children.Remove(melodylabel[melodyloc, cnt_melody[melodyloc]]);
                    break;

            }
        }
        private void delMelody()
        {
            char tmp = curstr.ElementAt(curstr.Length - 1);
            curstr = curstr.Substring(0, curstr.Length - 4);
            audiDelMelody(tmp);
        }
    }
    #endregion


}

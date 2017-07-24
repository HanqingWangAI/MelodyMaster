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
using TCPLib;

namespace iChord
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow InterfaceForMidi;
        public static Client client;
        public MainWindow()
        {
            client = new Client("10.172.150.34", 10010);
            client.Start();
            mainInit();
            InitializeComponent();
            initOthers();
            fileinit();
            InterfaceForMidi = this;

            combobox_Chord c = new combobox_Chord();

            //c.Content
        }
        bool muteMainScore = false;//是否静音
        chordAlgorithm myAlgorithm = new chordAlgorithm();//算法
        const int LengthOfeachNote = 8+1;//音符长度（包括空格）
        const int instrumentN = 4;//总的乐器数目
        UserDefinedChord[] chordTrack = new UserDefinedChord[instrumentN];//用户定义和弦（暂时没用）
        bool editSingleNote = false;//修改单音状态
        MidiDevice myMidiDevice = MidiPlay.playInitialization();//建立一个midi设备
        int singleNoteId;//第几个音需要被修改（根据鼠标点击）
        string inputMainMelody, inputMainChord;//标准音符编码：音符空格隔开，最后一个音符带空格。 和弦空格隔开，最后一个和弦带空格
        int[] timbreTrack = new int[instrumentN];//音色选择
        int[] defaultInstrument = { 0, 20, 99, 25, 40, 73, 68, 111 };//默认的combobox的音色
        int toneId = 0;
        string[] toneOfMusic = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };//上方移调功能显示的文字
        #region keybotton definition
        private void button_piano21_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(22);
        }
        private void button_piano20_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(21);
        }
        private void button_piano19_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(20);
        }
        private void button_piano18_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(19);
        }
        private void button_piano17_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(18);
        }
        private void button_piano16_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(17);
        }
        private void button_piano15_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(16);
        }
        private void button_piano14_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(15);
        }
        private void button_piano13_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(14);
        }
        private void button_piano12_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(13);
        }
        private void button_piano11_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(12);
        }
        private void button_piano10_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(11);
        }
        private void button_piano9_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(10);
        }
        private void button_piano8_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(9);
        }
        private void button_piano7_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(8);
        }
        private void button_piano6_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(7);
        }
        private void button_piano5_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(6);
        }
        private void button_piano4_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(5);
        }
        private void button_piano3_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(4);
        }
        private void button_piano2_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(3);
        }
        private void button_piano1_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(2);
        }
        private void button_piano0_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(1);
        }
        #endregion

        #region appBar
        private void appBarButton_Delete_Click(object sender, RoutedEventArgs e)
        {
            inputDelete();
        }

        private void appBarButton_Accept_Click(object sender, RoutedEventArgs e)
        {
            textBlock_main.Text = NoteInput.devideBar(textBlock_main.Text);
            //textBox_inputNote.Text = "";//是否清空输入栏
            textBlock_main2.Text = myAlgorithm.multiChordGenertor(textBlock_main.Text);
        }

        private async void appBarButton_Smart_Click(object sender, RoutedEventArgs e)
        {
            await smartChord();
            //显示和弦，和旋律。
            string melody = textBlock_main.Text;
            string chord = textBlock_main2.Text.Trim();
            textBlock_main.Text = melody;
            textBlock_main2.Text = chord;

            winShow(chord, melody);
        }

        private void appBarButton_Click(object sender, RoutedEventArgs e)//new 新文件
        {
            textBlock_main.Text = "";
            textBlock_main2.Text = "";
            upWin_Name.Text = "新曲目";
            myAlgorithm.init();
            clearAll();
        }

        private void appBarButton_start_Click(object sender, RoutedEventArgs e)
        {
            mainPlay();
        }
        #endregion

        #region Duration
        private void radioButton_Note0_Checked(object sender, RoutedEventArgs e)
        {
            noteDurationChang(2, "Icons/Note/note-white-1_16.png");
        }
        private void radioButton_Note1_Checked(object sender, RoutedEventArgs e)
        {
            noteDurationChang(3, "Icons/Note/note-white-1_8.png");
        }
        private void radioButton_Note2_Checked(object sender, RoutedEventArgs e)
        {
            noteDurationChang(4, "Icons/Note/note-white-1_4.png");
        }
        private void radioButton_Note3_Checked(object sender, RoutedEventArgs e)
        {
            noteDurationChang(5, "Icons/Note/note-white-1_2.png");
        }
        private void radioButton_Note4_Checked(object sender, RoutedEventArgs e)
        {
            noteDurationChang(6, "Icons/Note/note-white-1_1.png");
        }
        #endregion

        #region ChordTrack Combobox
        private void box1_Chord1_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(1, chordTrack[0]);
        }
        private void box1_Chord2_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(2, chordTrack[0]);
        }
        private void box1_Chord3_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(3, chordTrack[0]);
        }
        private void box1_Chord4_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(4, chordTrack[0]);
        }
        private void box1_Chord5_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(5, chordTrack[0]);
        }
        private void box1_Chord6_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(6, chordTrack[0]);
        }

        private void box2_Chord1_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(1, chordTrack[1]);
        }
        private void box2_Chord2_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(2, chordTrack[1]);
        }
        private void box2_Chord3_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(3, chordTrack[1]);
        }
        private void box2_Chord4_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(4, chordTrack[1]);
        }
        private void box2_Chord5_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(5, chordTrack[1]);
        }
        private void box2_Chord6_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(6, chordTrack[1]);
        }

        private void box3_Chord1_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(1, chordTrack[2]);
        }
        private void box3_Chord2_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(2, chordTrack[2]);
        }
        private void box3_Chord3_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(3, chordTrack[2]);
        }
        private void box3_Chord4_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(4, chordTrack[2]);
        }
        private void box3_Chord5_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(5, chordTrack[2]);
        }
        private void box3_Chord6_Selected(object sender, RoutedEventArgs e)
        {
            chordComboxChange(6, chordTrack[2]);
        }
        #endregion

        #region toneChange
        private void box0_ToneChange1_Checked(object sender, RoutedEventArgs e)
        {
            timbreChangeFor0(0, defaultInstrument[0]);
        }
        private void box0_ToneChange2_Checked(object sender, RoutedEventArgs e)
        {
            timbreChangeFor0(0, defaultInstrument[1]);
        }
        private void box0_ToneChange3_Checked(object sender, RoutedEventArgs e)
        {
            timbreChangeFor0(0, defaultInstrument[2]);
        }
        private void box0_ToneChange4_Checked(object sender, RoutedEventArgs e)
        {
            timbreChangeFor0(0, defaultInstrument[3]);
        }
        private void box0_ToneChange5_Checked(object sender, RoutedEventArgs e)
        {
            timbreChangeFor0(0, randomTimbre());
        }
        private void box0_ToneChange7_Checked(object sender, RoutedEventArgs e)
        {
            timbreChangeFor0(0, defaultInstrument[4]);
        }
        private void box0_ToneChange8_Checked(object sender, RoutedEventArgs e)
        {
            timbreChangeFor0(0, defaultInstrument[5]);
        }
        private void box0_ToneChange9_Checked(object sender, RoutedEventArgs e)
        {
            timbreChangeFor0(0, defaultInstrument[6]);
        }
        private void box0_ToneChange10_Checked(object sender, RoutedEventArgs e)
        {
            timbreChangeFor0(0, defaultInstrument[7]);
        }



        private void box1_ToneChange1_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(1, defaultInstrument[0]);
        }
        private void box1_ToneChange2_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(1, defaultInstrument[1]);
        }
        private void box1_ToneChange3_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(1, defaultInstrument[2]);
        }
        private void box1_ToneChange4_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(1, defaultInstrument[3]);
        }
        private void box1_ToneChange5_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(1, randomTimbre());
        }
        private void box1_ToneChange6_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(1, defaultInstrument[4]);
        }
        private void box1_ToneChange7_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(1, defaultInstrument[5]);
        }
        private void box1_ToneChange8_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(1, defaultInstrument[6]);
        }
        private void box1_ToneChange9_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(1, defaultInstrument[7]);
        }



        private void box2_ToneChange1_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(2, defaultInstrument[0]);
        }
        private void box2_ToneChange2_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(2, defaultInstrument[1]);
        }
        private void box2_ToneChange3_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(2, defaultInstrument[2]);
        }
        private void box2_ToneChange4_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(2, defaultInstrument[3]);
        }
        private void box2_ToneChange5_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(2, randomTimbre());
        }
        private void box2_ToneChange6_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(2, defaultInstrument[4]);
        }
        private void box2_ToneChange7_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(2, defaultInstrument[5]);
        }
        private void box2_ToneChange8_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(2, defaultInstrument[6]);
        }
        private void box2_ToneChange9_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(2, defaultInstrument[7]);
        }





        private void box3_ToneChange1_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(3, defaultInstrument[0]);
        }
        private void box3_ToneChange2_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(3, defaultInstrument[1]);
        }
        private void box3_ToneChange3_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(3, defaultInstrument[2]);
        }
        private void box3_ToneChange4_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(3, defaultInstrument[3]);
        }
        private void box3_ToneChange5_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(3, randomTimbre());
        }
        private void box3_ToneChange6_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(3, defaultInstrument[4]);
        }
        private void box3_ToneChange7_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(3, defaultInstrument[5]);
        }
        private void box3_ToneChange8_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(3, defaultInstrument[6]);
        }
        private void box3_ToneChange9_Checked(object sender, RoutedEventArgs e)
        {
            timbreChange(3, defaultInstrument[7]);
        }

        #endregion

        private void white_piano0_MouseUp(object sender, MouseButtonEventArgs e)
        {
            keyDown("C1");
        }

        private void white_piano0_MouseDown(object sender, MouseButtonEventArgs e)
        {
            keyDown("C1");
        }
        #region slider event
        private void slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int x = (int)slider2.Value;
            textBlock_Beat.Text = upWin_Speed.Text = x.ToString();
            x += 20;
            MidiPlay.changeBeat(x);
        }

        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int x = (int)slider1.Value;
            MidiPlay.changeVolume(x);
            textBlock_Volume.Text = x.ToString();
        }


        #endregion
        private void Button_Rest_Click(object sender, RoutedEventArgs e)
        {
            pianoKeyDown(0);
        }



        private void appBarButton_piano3_Click(object sender, RoutedEventArgs e)
        {
            myMidiDevice.Close();
        }

        private void box0_ToneChange6_Checked(object sender, RoutedEventArgs e)
        {
            muteMainScore = true;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            mainWindow.Children.Remove(canvas_UserGuidance);
            // white_piano0.RaiseEvent(new RoutedEventArgs(Button.MouseEnterEvent));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void button_InputBoxCancel_Click(object sender, RoutedEventArgs e)
        {
            inputBoxForFile.Visibility = Visibility.Hidden;
            keyBoardFocus = 1;
        }

        private void button_FileSave_Click_1(object sender, RoutedEventArgs e)
        {
            RenameBox.Text = "";
            inputBoxForFile.Visibility = Visibility.Visible;
            keyBoardFocus = 0;
        }



        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            button_FileRead.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        private void button_DownTone_Click(object sender, RoutedEventArgs e)
        {
            //if(toneId<=12)
            MidiPlay.changekey(-1);
            toneId = toneId  - 1;
            if (toneId < 0) toneId += 12;
            upWin_Tone.Text = toneOfMusic[toneId % 12];

        }

        private void black_piano0_Click(object sender, RoutedEventArgs e)
        {
            inputMainMelodyChange(0, "hahahhaha");
            inputMainChordChange(0, "CMM");
        }

        private void appBarButton_start_GotFocus(object sender, RoutedEventArgs e)
        {
            /*
            ImageBrush imagepic= new ImageBrush();
            imagepic.ImageSource = new BitmapImage(new Uri("Icons/button_stop.png", UriKind.Relative));
            appBarButton_start.Background = imagepic;
            */
        }

        private void label2_Duration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            radioButton_Note0.RaiseEvent(new RoutedEventArgs(ComboBoxItem.SelectedEvent));
        }

        private void label3_Duration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            radioButton_Note1.RaiseEvent(new RoutedEventArgs(ComboBoxItem.SelectedEvent));
        }

        private void label4_Duration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            radioButton_Note2.RaiseEvent(new RoutedEventArgs(ComboBoxItem.SelectedEvent));
        }

        private void label5_Duration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            radioButton_Note3.RaiseEvent(new RoutedEventArgs(ComboBoxItem.SelectedEvent));
        }

        private void label6_Duration_MouseDown(object sender, MouseButtonEventArgs e)
        {
            radioButton_Note4.RaiseEvent(new RoutedEventArgs(ComboBoxItem.SelectedEvent));
        }

        private void comboBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MessageBox.Show("ASDSDSDS");
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show("ASDSDSDS");
        }

        private void toggleButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)toggleButton.IsChecked)
                box0_ToneChange6.RaiseEvent(new RoutedEventArgs(ComboBoxItem.SelectedEvent));
            else
                box0_ToneChange1.RaiseEvent(new RoutedEventArgs(ComboBoxItem.SelectedEvent));
        }

        private void button_Click_1(object sender, RoutedEventArgs e)//TEst button
        {
            // realTimePlay();

            Window_RealPlay wd2 = new Window_RealPlay();
            // wd2.textBlock.Text = textBlock_main.Text;
            wd2.Show();
            


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try {
                MainWindow.client.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void button_UpTone_Click(object sender, RoutedEventArgs e)
        {
            MidiPlay.changekey(1);
            toneId = toneId + 1;
            upWin_Tone.Text = toneOfMusic[toneId % 12];
            keyDownColor_right(60);
        }
    }

    //    1. A3+30999
    #region KeyBoardFunction


    public partial class MainWindow : Window
    {
        public static int keyBoardFocus = 1;

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(keyBoardFocus == 1)
            {
                if (e.Key == Key.D1)
                    key_Duration(0);
                if (e.Key == Key.D2)
                    key_Duration(1);
                if (e.Key == Key.D3)
                    key_Duration(2);
                if (e.Key == Key.D4)
                    key_Duration(3);
                if (e.Key == Key.D5)
                    key_Duration(4);
                if (e.Key == Key.Delete || e.Key == Key.Back)
                    inputDelete();
                //播放快捷键
                // if (e.Key == Key.Space)
                //   appBarButton_start.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                if (e.Key == Key.Enter)
                    smartChord();
                //键盘快捷键
                // if (false == textBox_inputNote.Focusable)
                switch (e.Key)
                {
                    case Key.A:
                        pianoKeyDown(15); break;
                    case Key.S:
                        pianoKeyDown(16); break;
                    case Key.D:
                        pianoKeyDown(17); break;
                    case Key.F:
                        pianoKeyDown(18); break;
                    case Key.G:
                        pianoKeyDown(19); break;
                    case Key.H:
                        pianoKeyDown(20); break;
                    case Key.J:
                        pianoKeyDown(21); break;
                    case Key.K:
                        pianoKeyDown(22); break;
                    case Key.Z:
                        pianoKeyDown(0); break;

                }
            }
          
        }

        private void key_Duration(int x)
        {
            NoteInput.duration = x + 2;
            combobox_Duration.SelectedIndex = x;
        }



    }
    #endregion


    #region
    //The definition of button
    /// <summary>
    /// FButton.xaml 的交互逻辑
    /// </summary>

    public partial class FButton : Button
    {
        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(FButton), new PropertyMetadata(Brushes.DarkBlue));
        /// <summary>
        /// 鼠标按下背景样式
        /// </summary>
        public Brush PressedBackground
        {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
        }

        public static readonly DependencyProperty PressedForegroundProperty =
            DependencyProperty.Register("PressedForeground", typeof(Brush), typeof(FButton), new PropertyMetadata(Brushes.White));
        /// <summary>
        /// 鼠标按下前景样式（图标、文字）
        /// </summary>
        public Brush PressedForeground
        {
            get { return (Brush)GetValue(PressedForegroundProperty); }
            set { SetValue(PressedForegroundProperty, value); }
        }

        public static readonly DependencyProperty MouseOverBackgroundProperty =
            DependencyProperty.Register("MouseOverBackground", typeof(Brush), typeof(FButton), new PropertyMetadata(Brushes.RoyalBlue));
        /// <summary>
        /// 鼠标进入背景样式
        /// </summary>
        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        public static readonly DependencyProperty MouseOverForegroundProperty =
            DependencyProperty.Register("MouseOverForeground", typeof(Brush), typeof(FButton), new PropertyMetadata(Brushes.White));
        /// <summary>
        /// 鼠标进入前景样式
        /// </summary>
        public Brush MouseOverForeground
        {
            get { return (Brush)GetValue(MouseOverForegroundProperty); }
            set { SetValue(MouseOverForegroundProperty, value); }
        }

        public static readonly DependencyProperty FIconProperty =
            DependencyProperty.Register("FIcon", typeof(string), typeof(FButton), new PropertyMetadata("\ue604"));
        /// <summary>
        /// 按钮字体图标编码
        /// </summary>
        public string FIcon
        {
            get { return (string)GetValue(FIconProperty); }
            set { SetValue(FIconProperty, value); }
        }

        public static readonly DependencyProperty FIconSizeProperty =
            DependencyProperty.Register("FIconSize", typeof(int), typeof(FButton), new PropertyMetadata(20));
        /// <summary>
        /// 按钮字体图标大小
        /// </summary>
        public int FIconSize
        {
            get { return (int)GetValue(FIconSizeProperty); }
            set { SetValue(FIconSizeProperty, value); }
        }

        public static readonly DependencyProperty FIconMarginProperty = DependencyProperty.Register(
            "FIconMargin", typeof(Thickness), typeof(FButton), new PropertyMetadata(new Thickness(0, 1, 3, 1)));
        /// <summary>
        /// 字体图标间距
        /// </summary>
        public Thickness FIconMargin
        {
            get { return (Thickness)GetValue(FIconMarginProperty); }
            set { SetValue(FIconMarginProperty, value); }
        }

        public static readonly DependencyProperty AllowsAnimationProperty = DependencyProperty.Register(
            "AllowsAnimation", typeof(bool), typeof(FButton), new PropertyMetadata(true));
        /// <summary>
        /// 是否启用Ficon动画
        /// </summary>
        public bool AllowsAnimation
        {
            get { return (bool)GetValue(AllowsAnimationProperty); }
            set { SetValue(AllowsAnimationProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(FButton), new PropertyMetadata(new CornerRadius(2)));
        /// <summary>
        /// 按钮圆角大小,左上，右上，右下，左下
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty ContentDecorationsProperty = DependencyProperty.Register(
            "ContentDecorations", typeof(TextDecorationCollection), typeof(FButton), new PropertyMetadata(null));
        public TextDecorationCollection ContentDecorations
        {
            get { return (TextDecorationCollection)GetValue(ContentDecorationsProperty); }
            set { SetValue(ContentDecorationsProperty, value); }
        }

        static FButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FButton), new FrameworkPropertyMetadata(typeof(FButton)));
        }
    }
    #endregion

}
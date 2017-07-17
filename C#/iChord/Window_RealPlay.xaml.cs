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
using System.Windows.Shapes;

namespace iChord
{
    /// <summary>
    /// Window_RealPlay.xaml 的交互逻辑
    /// </summary>
    public partial class Window_RealPlay : Window
    {
        private const int MAXCOLUME = 6;
        private const int MAXROW = 4;
        private int NumberI, NumberJ;
        Button[,] buttonSeq = new Button[MAXCOLUME, MAXROW];

        public Window_RealPlay()
        {
            InitializeComponent();
            initRealPlayWin();
            initOtherComponent();
        }

        public void initOtherComponent()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                //同步左边的combobox/音色
                comboBox_ToneChange0.SelectedIndex = 
                MainWindow.InterfaceForMidi.comboBox_ToneChange0.SelectedIndex;

                comboBox_ToneChange1.SelectedIndex =
                MainWindow.InterfaceForMidi.comboBox_ToneChange1.SelectedIndex;

                comboBox_ToneChange2.SelectedIndex =
                MainWindow.InterfaceForMidi.comboBox_ToneChange2.SelectedIndex;

                comboBox_ToneChange3.SelectedIndex =
                MainWindow.InterfaceForMidi.comboBox_ToneChange3.SelectedIndex;

                //同步左边的第二排combobox，节奏型
            }), null);

        }

        private void buttonSeq_Clicked(object sender, RoutedEventArgs e)
        {
            Button myBtn = (Button)sender;
            string x = myBtn.Content.ToString();
            int i = x[0]-'0', j = x[1]-'0';

           for(int k = 0; k< MAXCOLUME; k++)
            {
                buttonSeq[k, j+1].Background = new SolidColorBrush(Color.FromRgb(221, 221, 221));//第j行所有的按钮
            }

            myBtn.Background = new SolidColorBrush(Color.FromRgb(131, 131, 131));//选中的按钮变成的颜色
           // myBtn.Foreground = myBtn.Background;//隐藏字体

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.InterfaceForMidi.setChordPattent(i, j);
               // MessageBox.Show(i + " " + j);//查看是否正确传入了i，j参数到setChordPattent中
            }), null);

        }

        public void initRealPlayWin()
        {
            //public void test() { }
            for(int i=0; i<MAXCOLUME; i++)
            {
                for (int j = 1; j<MAXROW; j++)
                {
                    NumberI = i+1; NumberJ = j-1;//1-6中风格，0-2轨道

                    buttonSeq[i, j] = new Button();
                    buttonSeq[i, j].Click += buttonSeq_Clicked;
                    gridForRealPlay.Children.Add(buttonSeq[i, j]);
                    Grid.SetColumn(buttonSeq[i, j], i);
                    Grid.SetRow(buttonSeq[i, j], j);

                    buttonSeq[i, j].Content = NumberI + "" +  NumberJ;
                    buttonSeq[i, j].Foreground = buttonSeq[i, j].Background;//隐藏字体
                }
            }
               
        }

        private void box00_ToneChange1_Selected(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                   // MainWindow.InterfaceForMidi.comboBox_ToneChange0.RaiseEvent;
            }), null);
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
           
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (MainWindow.InterfaceForMidi.textBlock_main.Text != "" &&
               MainWindow.InterfaceForMidi.textBlock_main.Text != null)
                    MainWindow.InterfaceForMidi.realTimePlay();
            }), null);

            //MainWindow.;
        }



    }
}

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
    public partial class MainWindow : Window
    {

        private void button_InputBoxSave_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("MusicLib/" + RenameBox.Text + ".icd"))
            {
                if (MessageBox.Show("文件" + RenameBox.Text + ".icd已存在\n\r是否要覆盖原文件？",
                    "确认覆盖已有文件？", MessageBoxButton.OKCancel, MessageBoxImage.Warning)
                    == MessageBoxResult.OK)
                {
                    FileStream tempStream = new FileStream("MusicLib/" + RenameBox.Text + ".icd", FileMode.Create);
                    StreamWriter writer = new StreamWriter(tempStream);
                    writer.WriteLine(textBlock_main.Text);
                    writer.WriteLine(textBlock_main2.Text);
                  

                    writer.Close();
                    tempStream.Close();
                    //MessageBox.Show("文件已覆盖！\n文件名为" + RenameBox.Text + ".icd");
                    //listView.Items.Add(RenameBox.Text);
                    upWin_Name.Text = RenameBox.Text;
                    RenameBox.Text = "";
                    inputBoxForFile.Visibility = Visibility.Hidden;
                    keyBoardFocus = 1;
                }

            }
            else
            {
                FileStream tempStream = new FileStream("MusicLib/" + RenameBox.Text + ".icd", FileMode.Create);
                StreamWriter writer = new StreamWriter(tempStream);
                writer.WriteLine(textBlock_main.Text);
                writer.WriteLine(textBlock_main2.Text);
                writer.Close();
                tempStream.Close();
                MessageBox.Show("信息已经保存！\n文件名为" + RenameBox.Text + ".icd");
                listView.Items.Add(RenameBox.Text);
                upWin_Name.Text = RenameBox.Text;
                RenameBox.Text = "";
                inputBoxForFile.Visibility = Visibility.Hidden;
                keyBoardFocus = 1;
            }
        }
        private void button_FileRead_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count == 0) ;
            // MessageBox.Show("未选中任何文件！");
            else if (listView.SelectedItems.Count > 1)
                MessageBox.Show("选中的的文件过多！");
            else
            {
                StreamReader Read = new StreamReader("MusicLib/" + listView.SelectedItem.ToString() + ".icd", Encoding.Default);
                textBlock_main.Text = Read.ReadLine();
                textBlock_main2.Text = Read.ReadLine();
                Read.Close();
                //MessageBox.Show(listView.SelectedItem.ToString() + ".icd文件已打开！");
                clearAll();
                setMelody(textBlock_main.Text);
               
                setChord(textBlock_main2.Text);
                //appBarButton_chordCreate2.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                upWin_Name.Text = listView.SelectedItem.ToString();

                inputMainChord = textBlock_main2.Text;
                inputMainMelody = textBlock_main.Text;
            }


        }

        private void button_FileDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count == 1)
            {
                if (File.Exists("MusicLib/" + listView.SelectedItem.ToString() + ".icd"))
                {
                    if (MessageBox.Show("确认从本地删除" + listView.SelectedItem.ToString()
                        + ".icd" + "文件？\r\n(无法恢复，请谨慎操作)", "确认删除文件？",
                        MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                    {
                        File.Delete("MusicLib/" + listView.SelectedItem.ToString() + ".icd");
                        // MessageBox.Show("已删除" + listView.SelectedItem.ToString() + ".icd");
                        //while (listView.SelectedItems.Count != 0)
                        listView.Items.Remove(listView.SelectedItem);
                        upWin_Name.Text = "新曲目";
                        clearAll();
                    }

                }
                else
                {
                    MessageBox.Show("文件不存在！");
                }
            }

        }
    }
}

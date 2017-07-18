using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iChord;
namespace SimpleMidiPlayer.Midi
{
    /// <summary>
    /// The base class for all MIDI device exception classes.
    /// </summary>
    public class MidiDeviceException : Exception
    {
        public string ExceptionName;//错误名称
        public string ExceptionSource;//错误来源
        public string ExceptionMessage;//错误具体的描述

    }

    public class ExeceptionEventArgs : EventArgs//必须从EventArgs继承.
    {
        public ExeceptionEventArgs ExceptionMessage;

        public ExeceptionEventArgs() { }//默认构造

        //构造并给一个值给成员 ExeceptionMessage 以便参数传递
        public ExeceptionEventArgs(ExeceptionEventArgs m)
        {
            ExceptionMessage = m;
        }
    }

    //定义错误委托声明(参数就是上面刚刚申明的)
    public delegate void ExeceptionEventHandle(ExeceptionEventArgs e);

    //主要的东西来了,这就是错误处理主程序
    //我写的比较简单,大家可以自由发挥
    public class Exeception
    {
        //委托事件实例,就是那个delegate建出来的,但实例时,必须用event
        //建了这个,就会出现类似OnClick+=那种闪电类型,嘿嘿
        public event ExeceptionEventHandle OnExeception;

        //处理主函数
        public void ThrowExeception(ExeceptionEventArgs m)
        {
            //这里省略,你可以写一些自己的基本代码
            //....比如你可以建一个错误列表等等,随你
            Console.WriteLine("Exeception Happen!");
            if (m != null && OnExeception != null)//检查错误消息是否为空&错误事件是否为空
            {
                //调用刚才建立的委托实例.
                OnExeception(new ExeceptionEventArgs(m));
            }
            else
            {//如果传来的错误消息有问题,忽略(我在这故意这么写的)}

            }
        }
    }
}

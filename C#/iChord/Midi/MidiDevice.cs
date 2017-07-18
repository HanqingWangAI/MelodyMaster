using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMidiPlayer.Midi
{
    public class MidiDevice : OutputDeviceBase
    {
       
        #region 设备相关函数
        /// <summary>
        /// 使用第一个设备
        /// </summary>
        public MidiDevice()
            : base(0)
        {
        }
        #endregion

        #region 音乐相关函数
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="iStatus"></param>
        /// <param name="iChannel"></param>
        /// <param name="iKey"></param>
        /// <param name="volume"></param>
        private void Send(int iStatus, int iChannel, int iKey, int volume)
        {
            midiOutShortMsg(hndle, iStatus | iChannel | (iKey << 8) | (volume << 16));
        }
        /// <summary>
        /// 键盘按下，默认为第一通道
        /// </summary>
        /// <param name="iKey"></param>
        /// <param name="volume"></param>
        //Note_On(频道，音调，音量)
        public void Note_On(int iChannel, int iKey, int volume)
        {
            Send(0x90, iChannel, iKey, volume);
        }   
        public void Note_Off(int iChannel, int iKey, int volume)
        {
            Send(0x80, iChannel, iKey, volume);
        }
       
        
        /// <summary>
        /// 改变音色（乐器）
        /// </summary>
        /// <param name="timbre">音色序号（0-127）</param>
        public void ChangeProgram(int iChannel, int timbre, int iData2)
        {
            Send(0xC0, iChannel, timbre, iData2);
        }

        #endregion

    }



}


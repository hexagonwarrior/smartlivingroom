using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

using System.Threading;
using System.IO;
using System.Runtime.InteropServices;


namespace smartlivingroom
{
    public partial class Form1 : Form
    {
        [DllImport("winmm.dll", SetLastError = true)]
        static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        private readonly List<string> ON = new List<string>() { "打", "开", "把"};
        private readonly List<string> OFF = new List<string>() { "关", "观", "官"};
        private readonly List<string> TV = new List<string>() { "电", "视", "机", "击", "鸡", "看" };
        private readonly List<string> LAMP = new List<string>() { "台", "灯" };
            

        // const
        private const int LAMP_X = 13;
        private const int LAMP_Y = 268;
        private const int TV_X = 149;
        private const int TV_Y = 100;
        private const int STEREO_X = 247;
        private const int STEREO_Y = 380;
        private string AUDIO_FILE = "temp/cmd.wav";

        enum STATE
        {
            INVALID,
            TV_ON,
            TV_OFF,
            LAMP_ON,
            LAMP_OFF
        };

        private string API_KEY = "O0zl0RyfKcwcqBjOBoPxDLAK";
        private string SECRET_KEY = "gg0P2WzFzdM2EIazqh3RqdF5HRDqihKb";

        // global
        private Image livingroom;
        private WaveInEvent waveIn;
        private WaveFileWriter writer;
        private Baidu.Aip.Speech.Asr client;

        private STATE lamp_state = STATE.LAMP_OFF;
        private STATE tv_state = STATE.TV_OFF;

        public Form1()
        {
            InitializeComponent();
            livingroom = Image.FromFile("image/livingroom.jpeg");
            pictureBox1.Image = livingroom;

            
            client = new Baidu.Aip.Speech.Asr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;

            ThreadStart ts = new ThreadStart(record_thread);
            Thread t = new Thread(ts);
            t.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void lamp_button_Click(object sender, EventArgs e)
        {
            if (lamp_state == STATE.LAMP_OFF)
            {
                // recognize("audio/LAMP_ON.wav");
                set(STATE.LAMP_ON);
            }
            else
            {
                // recognize("audio/LAMP_OFF.wav");
                set(STATE.LAMP_OFF);
            }
        }

        private void tv_button_Click(object sender, EventArgs e)
        {
            if (tv_state == STATE.TV_OFF)
            {
                // recognize("audio/TV_ON.wav");
                set(STATE.TV_ON);
            }
            else
            {
                // recognize("audio/TV_OFF.wav");
                set(STATE.TV_OFF);
            }
        }

        private void set(STATE state)
        {
            Image img;
            Graphics g = Graphics.FromImage(livingroom);

            switch (state)
            { 
                case STATE.LAMP_ON:
                    img = Image.FromFile("image/lamp_on.jpeg");
                    g.DrawImage(img, LAMP_X, LAMP_Y);
                    lamp_state = state;
                    break;
                case STATE.LAMP_OFF:
                    img = Image.FromFile("image/lamp_off.jpeg");
                    g.DrawImage(img, LAMP_X, LAMP_Y);
                    lamp_state = state;
                    break;
                case STATE.TV_ON:
                    img = Image.FromFile("image/tv_on.jpeg");
                    g.DrawImage(img, TV_X, TV_Y, img.Width, img.Height);

                    img = Image.FromFile("image/stereo_on.jpeg");
                    g.DrawImage(img, STEREO_X, STEREO_Y, img.Width, img.Height);

                    tv_state = state;
                    break;
                case STATE.TV_OFF:
                    img = Image.FromFile("image/tv_off.jpeg");
                    g.DrawImage(img, TV_X, TV_Y);

                    img = Image.FromFile("image/stereo_off.jpeg");
                    g.DrawImage(img, STEREO_X, STEREO_Y, img.Width, img.Height);

                    tv_state = state;
                    break;
                default:
                    break;
            }

            pictureBox1.Image = livingroom;
            make_sound(state);
        }

        // 声音按键录制，按record开始，按stop结束
        private void record_button_Click(object sender, EventArgs e)
        {
            waveIn = new WaveInEvent();
            writer = new WaveFileWriter(AUDIO_FILE, waveIn.WaveFormat);

            waveIn.DataAvailable += (s, a) =>
            {
                writer.Write(a.Buffer, 0, a.BytesRecorded);
                writer.Flush();
            };

            waveIn.RecordingStopped += (s, a) =>
            {
                // writer.Close();
                writer.Dispose();
                //writer = null;
                waveIn.Dispose();
            };

            waveIn.StartRecording();
        }

        private void stop_button_Click(object sender, EventArgs e)
        {
            waveIn.StopRecording();
        }

        // 声音循环录制
        private void record_thread()
        {
            WaveInEvent rwaveIn;
            WaveFileWriter rwriter;
            Console.WriteLine("record_thread start ...");
            int i = 1;
            
            while (true) {
                StringBuilder filename = new StringBuilder();
                filename.Append("temp/cmd");
                filename.Append(i.ToString());
                filename.Append(".wav");

                rwaveIn = new WaveInEvent();
                rwriter = new WaveFileWriter(filename.ToString(), rwaveIn.WaveFormat);

                rwaveIn.DataAvailable += (s, a) =>
                {
                    rwriter.Write(a.Buffer, 0, a.BytesRecorded);
                };

                rwaveIn.RecordingStopped += (s, a) =>
                {
                    rwriter.Close();
                };

                try
                {
                    rwaveIn.StartRecording();
                    Thread.Sleep(4000);
                    rwaveIn.StopRecording();
                }
                catch (Exception ex)
                {
 
                }

                // 识别
                recognize(filename.ToString());
                

                Console.WriteLine("Record round " + (i++));
                //Thread.Sleep(1000);
            }
        }


        // 调用百度语音接口对语音文件进行识别
        private void recognize(string filename)
        {
            try
            {
                var data = File.ReadAllBytes(filename);
                var options = new Dictionary<string, object>();
                options.Add("普通话", 1573);
                string cmd = null;

                client.Timeout = 60000;
                var result = client.Recognize(data, "wav", 16000, options);
                // Console.WriteLine(result);
            
                cmd = result["result"][0].ToString();
                Console.WriteLine(cmd);

                STATE instruct = proc_cmd(cmd);
                set(instruct);
            }
            catch (Exception ex) 
            { 
                
            }
        }


        // 匹配识别结果，生成控制指令
        private STATE proc_cmd(string cmd)
        {
            STATE instruct = STATE.INVALID;

            if (cmd != null)
            {
                int state = -1; // invalid

                // 匹配ON的关键字
                foreach (string key in ON)
                {
                    if (cmd.IndexOf(key) > -1)
                    {
                        state = 1; // on
                        break;
                    }
                }

                // 匹配OFF的关键字
                foreach (string key in OFF)
                {
                    if (cmd.IndexOf(key) > -1)
                    {
                        state = 0; // off
                        break;
                    }
                }

                // 匹配电视的关键字
                foreach (string key in TV)
                {
                    if (state == 0)
                    {
                        instruct = STATE.TV_OFF;
                        break;
                    }
                    else if (state == 1)
                    {
                        instruct = STATE.TV_ON;
                        break;
                    }
                }

                // 匹配台灯的关键字
                foreach (string key in LAMP)
                {
                    if (state == 0)
                    {
                        instruct = STATE.LAMP_OFF;
                        break;
                    }
                    else if (state == 1)
                    {
                        instruct = STATE.LAMP_ON;
                        break;
                    }
                }
            }
            return instruct;
        }

        private void make_sound(object p)
        {
            if ((STATE)p == STATE.TV_ON)
            {
                play_mp3("sound/TV_ON.mp3");
            }
            else if ((STATE)p == STATE.TV_OFF)
            {
                play_mp3("sound/TV_OFF.mp3");
            }
            else if ((STATE)p == STATE.LAMP_ON)
            {
                play_mp3("sound/LAMP_ON.mp3");
            }
            else if ((STATE)p == STATE.LAMP_OFF)
            {
                play_mp3("sound/LAMP_OFF.mp3");
            }
        }

        //语音播报
        private void play_mp3(string text)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("open ");
            sb.Append(text);
            sb.Append(" alias temp_alias");
            // 播放音频文件
            mciSendString(sb.ToString(), null, 0, IntPtr.Zero);
            mciSendString("play temp_alias", null, 0, IntPtr.Zero);
            // 等待播放结束
            StringBuilder strReturn = new StringBuilder(64);
            do
            {
                mciSendString("status temp_alias mode", strReturn, 64, IntPtr.Zero);
            } while (!strReturn.ToString().Contains("stopped"));
            // 关闭音频文件
            mciSendString("close temp_alias", null, 0, IntPtr.Zero);
        }

    }
}

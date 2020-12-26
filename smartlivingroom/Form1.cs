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
        private readonly List<string> ON = new List<string>() { "打", "开", "把", "看", "大" };
        private readonly List<string> OFF = new List<string>() { "关", "闭", "观", "官", "管", "不", "安", "利" };

        // const
        private const int LAMP_X = 13;
        private const int LAMP_Y = 268;
        private const int TV_X = 149;
        private const int TV_Y = 100;
        private const int STEREO_X = 247;
        private const int STEREO_Y = 380;
        private const int LEFT_X = 149;
        private const int LEFT_Y = 378;
        private const int RIGHT_X = 413;
        private const int RIGHT_Y = 378;

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
        
        private bool playing_sound = false;
        public Form1()
        {
            InitializeComponent();
            livingroom = Image.FromFile("image/livingroom.jpeg");
            pictureBox1.Image = livingroom;

            
            client = new Baidu.Aip.Speech.Asr(API_KEY, SECRET_KEY);
            client.Timeout = 60000;

            string synthesisVoiceFile = "sound/tv_on.mp3";
            Synthesis("已经为您打开了家庭影院", synthesisVoiceFile);

            synthesisVoiceFile = "sound/tv_off.mp3";
            Synthesis("已经为您关闭了家庭影院", synthesisVoiceFile);

            synthesisVoiceFile = "sound/lamp_on.mp3";
            Synthesis("已经为您打开了台灯", synthesisVoiceFile);

            synthesisVoiceFile = "sound/lamp_off.mp3";
            Synthesis("已经为您关闭了台灯", synthesisVoiceFile);

            ThreadStart ts = new ThreadStart(record_thread);
            Thread t = new Thread(ts);
            t.Start();

            ShowImage(STATE.LAMP_ON);
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
                ShowImage(STATE.LAMP_ON);

                PlayCommandVoice(STATE.LAMP_ON);
            }
            else
            {
                // recognize("audio/LAMP_OFF.wav");
                ShowImage(STATE.LAMP_OFF);

                PlayCommandVoice(STATE.LAMP_OFF);
            }

            
        }

        private void tv_button_Click(object sender, EventArgs e)
        {
            if (tv_state == STATE.TV_OFF)
            {
                // recognize("audio/TV_ON.wav");
                ShowImage(STATE.TV_ON);

                PlayCommandVoice(STATE.TV_ON);
            }
            else
            {
                // recognize("audio/TV_OFF.wav");
                ShowImage(STATE.TV_OFF);

                PlayCommandVoice(STATE.TV_OFF);
            }
        }

        private void ShowImage(STATE state)
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

                    img = Image.FromFile("image/lamp_off.jpeg");
                    g.DrawImage(img, LAMP_X, LAMP_Y);

                    img = Image.FromFile("image/left.jpeg");
                    g.DrawImage(img, LEFT_X, LEFT_Y);

                    img = Image.FromFile("image/right.jpeg");
                    g.DrawImage(img, RIGHT_X, RIGHT_Y);

                    tv_state = state;
                    break;
                case STATE.TV_OFF:

                    livingroom = Image.FromFile("image/livingroom.jpeg");
                    g = Graphics.FromImage(livingroom);

                    img = Image.FromFile("image/lamp_on.jpeg");
                    g.DrawImage(img, LAMP_X, LAMP_Y);

                    tv_state = state;
                    break;
                default:
                    break;
            }

            pictureBox1.Image = livingroom;
        }

        // 声音按键录制，按record开始，按stop结束
        private void record_button_Click(object sender, EventArgs e)
        {
            record_button.Enabled = false;
            stop_button.Enabled = true;

            waveIn = new WaveInEvent();
            writer = new WaveFileWriter("temp/cmd.wav", waveIn.WaveFormat);

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

            record_button.Enabled = true;
            stop_button.Enabled = false;
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
                    if (playing_sound == true)
                    {
                        Thread.Sleep(4000); // 如果在播报中，暂停录音
                    }
                    else
                    {
                        rwaveIn.StartRecording();
                        Thread.Sleep(4000);
                        rwaveIn.StopRecording();
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show(ex.Message);
                }

                // 识别
                recognize(filename.ToString());
                

                Console.WriteLine("Record round " + (i++));
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

                cmd = result["result"][0].ToString();
                Console.WriteLine(cmd);

                var state = IdentifyCommand(cmd);

                ShowImage(state);
                PlayCommandVoice(state);
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }
        }


        // 匹配识别结果，生成控制指令
        private STATE IdentifyCommand(string cmd)
        {
            if (cmd != null)
            {
                // 匹配ON的关键字
                foreach (string key in ON)
                {
                    if (cmd.IndexOf(key) > -1)
                    {
                        return STATE.TV_ON;
                    }
                }

                // 匹配OFF的关键字
                foreach (string key in OFF)
                {
                    if (cmd.IndexOf(key) > -1)
                    {
                        return STATE.TV_OFF;
                    }
                }
            }

            return STATE.INVALID;
        }

        private void PlayCommandVoice(object p)
        {
            playing_sound = true;
            if ((STATE)p == STATE.TV_ON)
            {
                play_mp3("sound/tv_on.mp3");
                // play_mp3("sound/madmachine.mp3");
            }
            else if ((STATE)p == STATE.TV_OFF)
            {
                play_mp3("sound/tv_off.mp3");
            }
            else if ((STATE)p == STATE.LAMP_ON)
            {
                play_mp3("sound/lamp_on.mp3");
            }
            else if ((STATE)p == STATE.LAMP_OFF)
            {
                play_mp3("sound/lamp_off.mp3");
            }
            playing_sound = false;
        }

        //语音播报
        private void play_mp3(string text)
        {
            using (BackgroundWorker worker = new BackgroundWorker())
            {
                WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();
                worker.DoWork += (s, e) =>
                {
                    try {
                        wplayer.URL = text;
                        wplayer.controls.play();
                        Thread.Sleep(500);
                    }
                    catch
                    {
                        
                    }
                };

                worker.RunWorkerAsync();
            }
        }

        public void Synthesis(string words, string voiceFilePath)
        {
            var client = new Baidu.Aip.Speech.Tts(API_KEY, SECRET_KEY);
            client.Timeout = 60000;

            var option = new Dictionary<string, object>()
            {
                {"spd", 5}, // 语速
                {"vol", 7}, // 音量
                {"per", 0},  // 发音人-度小美
                {"aue", 3} //synthesised audio format is MP3
            };

            var result = client.Synthesis(words, option);

            if (result.ErrorCode == 0)
            {
                File.WriteAllBytes(voiceFilePath, result.Data);
            }
            else
            {
                throw new Exception(result.ErrorMsg);
            }
        }
    }
}

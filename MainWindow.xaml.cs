using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;

namespace AnimeLoupe2x
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public delegate void UpdateLabelContentEventHandler(Label label, string data);
        public event UpdateLabelContentEventHandler UpdateLabelContentEvent = null;

        public delegate bool GetCheckBoxIsCheckedEventHandler(CheckBox check_box);
        public event GetCheckBoxIsCheckedEventHandler GetCheckBoxIsCheckedContentEvent = null;

        public delegate void UpdateTextBoxEventHandler(TextBox text_box, string data);
        public event UpdateTextBoxEventHandler UpdateTextBoxContentEvent = null;

        Commander commands;
        PathManager paths;
        bool cancel_flag;

        void ConvertTaskMethod(bool test_run = false, float scale = 0.0f)
        {
            this.Dispatcher.Invoke(UpdateTextBoxContentEvent, OutputLogTextbox, "処理(´・ω・｀)はじまり");
            string audio_temp = @"audio.aac";

            var com1 = new LoadExecJob();

            // 動画のFPSなどの情報を取得する
            commands.GetVideoInfoString(paths.InputFile);
            com1.SetOutputFunc(GetVideoInfoOutputFunc);
            com1.RunFFmpegAndJoin(commands.command, commands.option);

            // 動画から音声を切り出し
            if ((bool)this.Dispatcher.Invoke(GetCheckBoxIsCheckedContentEvent, Video2AudioCheckBox) || test_run)
            {
                if (cancel_flag) return;
                commands.MakeSepAudioString(paths.InputFile, paths.GetTempAudioDir() + audio_temp);
                com1.SetOutputFunc(Video2AudioOutput);
                com1.RunFFmpegAndJoin(commands.command, commands.option);
            }

            // 動画から画像を切り出し
            if ((bool)this.Dispatcher.Invoke(GetCheckBoxIsCheckedContentEvent, Video2ImageCheckBox) || test_run)
            {
                if (cancel_flag) return;
                commands.MakeVideo2ImageString(paths.InputFile, paths.GetTempImageDir() + "image_%08d.png");
                com1.SetOutputFunc(Video2ImageOutput);
                com1.RunFFmpegAndJoin(commands.command, commands.option);
            }

            /* waifu */
            if ((bool)this.Dispatcher.Invoke(GetCheckBoxIsCheckedContentEvent, ConvertCheckBox))
            {
                commands.ci_scale = 1.50f; // 1280x720
                //commands.ci_scale = 2.00f; // 

                if (scale != 0.0f) commands.ci_scale = scale;

                var delta_list = new List<string>();
                string[] pre_temp_files = Directory.GetFiles(paths.GetTempImageDir(), "*.png");
                foreach (string name in pre_temp_files)
                {
                    delta_list.Add(System.IO.Path.GetFileName(name));
                }

                string[] conv_temp_files = Directory.GetFiles(paths.GetTempConvertDir(), "*.png");
                foreach (string name in conv_temp_files)
                {
                    delta_list.Remove(System.IO.Path.GetFileName(name));
                    //Console.WriteLine(System.IO.Path.GetFileName(name));
                }

                int total_img = delta_list.Count;
                Console.WriteLine("total:"+ total_img.ToString());
                int num = 0;
                foreach (string img in delta_list)
                {
                    string pre_image = Path.GetFileName(img);
                    commands.MakeAnime4KString(paths.GetTempImageDir() + pre_image, paths.GetTempConvertDir() + pre_image);
                    //commands.MakeWaifu2xString();
                    //Console.WriteLine(commands.option);
                    com1.SetOutputFunc(ConvertOutput);
                    com1.Run(commands.command, commands.option, false);
                    com1.Join();

                    this.Dispatcher.Invoke(UpdateLabelContentEvent, ConvertLabel, num.ToString() + "/" + total_img.ToString());
                    
                    num += 1;
                    if (cancel_flag) return;
                    
                }
                
            }

            // 画像から動画を作成
            if ((bool)this.Dispatcher.Invoke(GetCheckBoxIsCheckedContentEvent, Image2VideoCheckBox) || test_run)
            {
                if (cancel_flag) return;
                commands.MakeImage2VideoString(paths.GetTempConvertDir() + "image_%08d.png", paths.GetTempVideoDir() + "video.avi");
                if (test_run) commands.MakeImage2VideoString(paths.GetTempImageDir() + "image_%08d.png", paths.GetTempVideoDir() + "video.avi");
                com1.SetOutputFunc(Image2VideoOutput);
                com1.RunFFmpegAndJoin(commands.command, commands.option);
            }

            // 動画と音声を結合
            if ((bool)this.Dispatcher.Invoke(GetCheckBoxIsCheckedContentEvent, CompAudioCheckBox) || test_run)
            {
                if (cancel_flag) return;
                commands.MakeComAudioString(paths.GetTempVideoDir() + "video.avi", paths.GetTempAudioDir() + audio_temp, paths.OutputFile);
                com1.SetOutputFunc(CompAudioOutput);
                com1.RunFFmpegAndJoin(commands.command, commands.option);
            }

            this.Dispatcher.Invoke(UpdateTextBoxContentEvent, OutputLogTextbox, "処理(´・ω・｀)おわり");
        }

        /* 各命令のコンソール出力 */
        void GetVideoInfoOutputFunc(string out_txt)
        {
            if (out_txt.IndexOf("fps") > 0)
            {
                commands.vi_fps = out_txt.Substring(out_txt.IndexOf("fps") - 6, 5);
            }
            if (out_txt.IndexOf("bitrate:") > 0)
            {
                string bitrate_tmp = out_txt.Substring(out_txt.IndexOf("bitrate:") + 9, 6);
                commands.vi_bitrate = bitrate_tmp.Replace(" ", "");
            }
            if (out_txt.IndexOf("time=") > 0)
            {
                string time_tmp = out_txt.Substring(out_txt.IndexOf("time=") + 5, 8);
            }

            //this.Dispatcher.Invoke(UpdateLabelContentEvent, Video2ImageLabel, out_txt);
            this.Dispatcher.Invoke(UpdateTextBoxContentEvent, OutputLogTextbox, out_txt);
        }

        void Video2AudioOutput(string out_txt)
        {
            this.Dispatcher.Invoke(UpdateTextBoxContentEvent, OutputLogTextbox, out_txt);

            if (out_txt.IndexOf("time=") > 0)
            {
                string time_tmp = out_txt.Substring(out_txt.IndexOf("time=") + 5, 8);
                this.Dispatcher.Invoke(UpdateLabelContentEvent, Video2AudioLabel, time_tmp);
            }
        }

        void Video2ImageOutput(string out_txt)
        {
            this.Dispatcher.Invoke(UpdateTextBoxContentEvent, OutputLogTextbox, out_txt);

            if (out_txt.IndexOf("time=") > 0)
            {
                string time_tmp = out_txt.Substring(out_txt.IndexOf("time=") + 5, 8);
                this.Dispatcher.Invoke(UpdateLabelContentEvent, Video2ImageLabel, time_tmp);
            }
        }

        void ConvertOutput(string out_txt)
        {
            //Console.WriteLine(out_txt);
            //this.Dispatcher.Invoke(UpdateLabelContentEvent, ConvertLabel, out_txt);
        }

        void Image2VideoOutput(string out_txt)
        {
            this.Dispatcher.Invoke(UpdateTextBoxContentEvent, OutputLogTextbox, out_txt);

            if (out_txt.IndexOf("time=") > 0)
            {
                string time_tmp = out_txt.Substring(out_txt.IndexOf("time=") + 5, 8);
                this.Dispatcher.Invoke(UpdateLabelContentEvent, Image2VideoLabel, time_tmp);
            }
        }

        void CompAudioOutput(string out_txt)
        {
            this.Dispatcher.Invoke(UpdateTextBoxContentEvent, OutputLogTextbox, out_txt);

            if (out_txt.IndexOf("time=") > 0)
            {
                string time_tmp = out_txt.Substring(out_txt.IndexOf("time=") + 5, 8);
                this.Dispatcher.Invoke(UpdateLabelContentEvent, CompAudioLabel, time_tmp);
            }
        }

        /* デリゲート */
        void event_DataReceived(Label label, string data)
        {
            label.Content = data;
        }

        bool event_CheckBoxIsChecked(CheckBox check_box)
        {
            return (bool)check_box.IsChecked;
        }

        void event_DataReceived2(TextBox text_box, string data)
        {
            text_box.AppendText(data+ "\n");
            text_box.ScrollToEnd();
        }

        /* ボタンUI関連 */
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (TempFolderTextBox.Text == "") TempFolderTextBox.Text = Directory.GetCurrentDirectory();
            if (InputPathTextBox.Text == "") return;
            if (OutputPathTextBox.Text == "") OutputPathTextBox.Text = System.IO.Path.GetDirectoryName(InputPathTextBox.Text)+"[conv]"+ System.IO.Path.GetFileName(InputPathTextBox.Text);

            paths.InitTempDirectory(TempFolderTextBox.Text, false);
            paths.InputFile = InputPathTextBox.Text;
            paths.OutputFile = OutputPathTextBox.Text;

            cancel_flag = false;

            float scale_tmp = 0.0f;
            bool scale_result = float.TryParse(ScaleTextbox.Text, out scale_tmp);

            Task task = Task.Run(() => {
                ConvertTaskMethod(false, scale_tmp);
            });
        }

        private void TestRunButton_Click(object sender, RoutedEventArgs e)
        {
            if (TempFolderTextBox.Text == "") TempFolderTextBox.Text = Directory.GetCurrentDirectory();
            if (InputPathTextBox.Text == "") return;
            if (OutputPathTextBox.Text == "") OutputPathTextBox.Text = System.IO.Path.GetDirectoryName(InputPathTextBox.Text) + "[conv]" + System.IO.Path.GetFileName(InputPathTextBox.Text);

            paths.InitTempDirectory(TempFolderTextBox.Text, true);
            paths.InputFile = InputPathTextBox.Text;
            paths.OutputFile = OutputPathTextBox.Text;

            cancel_flag = false;

            Task task = Task.Run(() => {
                ConvertTaskMethod(true);
            });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            cancel_flag = true;
        }

        /* ロードとセーブ関連 */
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TempFolderTextBox.Text = Properties.Settings.Default.TempFolderPathSetting;
            InputPathTextBox.Text = Properties.Settings.Default.InputFolderPathSetting;
            OutputPathTextBox.Text = Properties.Settings.Default.OutputFolderPathSetting;

            Video2ImageCheckBox.IsChecked = Properties.Settings.Default.Video2ImageCheckBoxSetting;
            Video2AudioCheckBox.IsChecked = Properties.Settings.Default.Video2AudioCheckBoxSetting;
            ConvertCheckBox.IsChecked = Properties.Settings.Default.ConvertCheckBoxSetting;
            Image2VideoCheckBox.IsChecked = Properties.Settings.Default.Image2VideoCheckBoxSetting;
            CompAudioCheckBox.IsChecked = Properties.Settings.Default.CompAudioCheckBoxSetting;
            ScaleTextbox.Text = Properties.Settings.Default.WaifuScaleSetting.ToString();

            paths = new PathManager();
            commands = new Commander(paths.GetFFmpegPath(), paths.GetWaifu2xPath(), paths.GetAnime4KPath() );

            cancel_flag = false;

            UpdateLabelContentEvent = new UpdateLabelContentEventHandler(event_DataReceived);
            GetCheckBoxIsCheckedContentEvent = new GetCheckBoxIsCheckedEventHandler(event_CheckBoxIsChecked);
            UpdateTextBoxContentEvent = new UpdateTextBoxEventHandler(event_DataReceived2);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.TempFolderPathSetting = TempFolderTextBox.Text;
            Properties.Settings.Default.InputFolderPathSetting = InputPathTextBox.Text;
            Properties.Settings.Default.OutputFolderPathSetting = OutputPathTextBox.Text;

            Properties.Settings.Default.Video2ImageCheckBoxSetting = (bool)Video2ImageCheckBox.IsChecked;
            Properties.Settings.Default.Video2AudioCheckBoxSetting = (bool)Video2AudioCheckBox.IsChecked;
            Properties.Settings.Default.ConvertCheckBoxSetting = (bool)ConvertCheckBox.IsChecked;
            Properties.Settings.Default.Image2VideoCheckBoxSetting = (bool)Image2VideoCheckBox.IsChecked;
            Properties.Settings.Default.CompAudioCheckBoxSetting = (bool)CompAudioCheckBox.IsChecked;
            Properties.Settings.Default.WaifuScaleSetting = float.Parse(ScaleTextbox.Text);

            Properties.Settings.Default.Save();
        }


    }
}

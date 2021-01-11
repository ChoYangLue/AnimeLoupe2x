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

        Commander commands;
        PathManager paths;
        bool cancel_flag;

        void ConvertTaskMethod(bool test_run = false)
        {
            Console.WriteLine("すごく重い処理その1(´・ω・｀)はじまり");

            var com1 = new LoadExecJob();

            // 動画のFPSなどの情報を取得する
            commands.GetVideoInfoString(paths.InputFile);
            com1.SetOutputFunc(GetVideoInfoOutputFunc);
            com1.RunFFmpegAndJoin(commands.command, commands.option);

            // 動画から音声を切り出し
            if ((bool)this.Dispatcher.Invoke(GetCheckBoxIsCheckedContentEvent, Video2AudioCheckBox) || test_run)
            {
                commands.MakeSepAudioString(paths.InputFile, paths.GetTempAudioDir() + @"audio.wav");
                com1.SetOutputFunc(Video2AudioOutput);
                com1.RunFFmpegAndJoin(commands.command, commands.option);
                if (cancel_flag) return;
            }

            // 動画から画像を切り出し
            if ((bool)this.Dispatcher.Invoke(GetCheckBoxIsCheckedContentEvent, Video2ImageCheckBox) || test_run)
            {
                commands.MakeVideo2ImageString(paths.InputFile, paths.GetTempImageDir() + "image_%08d.png");
                com1.SetOutputFunc(Video2ImageOutput);
                com1.RunFFmpegAndJoin(commands.command, commands.option);
                if (cancel_flag) return;
            }

            /* waifu */
            if ((bool)this.Dispatcher.Invoke(GetCheckBoxIsCheckedContentEvent, ConvertCheckBox))
            {
                string[] pre_images = Directory.GetFiles(paths.GetTempImageDir(), "*.png");
                foreach (string pre_image in pre_images)
                {
                    Console.WriteLine(pre_image);
                    commands.MakeAnime4KString(paths.GetTempImageDir() + pre_image, paths.GetTempConvertDir() + pre_image);
                    //commands.MakeWaifu2xString();
                    com1.SetOutputFunc(ConvertOutput);
                    com1.Run(commands.command, commands.option);
                    com1.Join();
                    if (cancel_flag) return;
                }
                
            }

            // 画像から動画を作成
            if ((bool)this.Dispatcher.Invoke(GetCheckBoxIsCheckedContentEvent, Image2VideoCheckBox) || test_run)
            {
                commands.MakeImage2VideoString(paths.GetTempConvertDir() + "image_%08d.png", paths.GetTempVideoDir() + "video.avi");
                if (test_run) commands.MakeImage2VideoString(paths.GetTempImageDir() + "image_%08d.png", paths.GetTempVideoDir() + "video.avi");
                com1.SetOutputFunc(Image2VideoOutput);
                com1.RunFFmpegAndJoin(commands.command, commands.option);
                if (cancel_flag) return;
            }

            // 動画と音声を結合
            if ((bool)this.Dispatcher.Invoke(GetCheckBoxIsCheckedContentEvent, CompAudioCheckBox) || test_run)
            {
                commands.MakeComAudioString(paths.GetTempVideoDir() + "video.avi", paths.GetTempAudioDir() + @"audio.wav", paths.OutputFile);
                com1.SetOutputFunc(CompAudioOutput);
                com1.RunFFmpegAndJoin(commands.command, commands.option);
                if (cancel_flag) return;
            }

            Console.WriteLine("すごく重い処理その1(´・ω・｀)おわり");
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

            this.Dispatcher.Invoke(UpdateLabelContentEvent, Video2ImageLabel, out_txt);
        }

        void Video2AudioOutput(string out_txt)
        {
            //Console.WriteLine(out_txt);
            this.Dispatcher.Invoke(UpdateLabelContentEvent, Video2AudioLabel, out_txt);
        }

        void Video2ImageOutput(string out_txt)
        {
            Console.WriteLine(out_txt);

            if (out_txt.IndexOf("frame=") >= 0)
            {
                string sererere = out_txt.Substring(out_txt.IndexOf("frame=") + 7, 5);
                this.Dispatcher.Invoke(UpdateLabelContentEvent, Video2ImageLabel, sererere);
            }
        }

        void ConvertOutput(string out_txt)
        {
            //Console.WriteLine(out_txt);
            this.Dispatcher.Invoke(UpdateLabelContentEvent, ConvertLabel, out_txt);
        }

        void Image2VideoOutput(string out_txt)
        {
            Console.WriteLine(out_txt);

            if (out_txt.IndexOf("frame=") >= 0)
            {
                string sererere = out_txt.Substring(out_txt.IndexOf("frame=") + 7, 5);
                this.Dispatcher.Invoke(UpdateLabelContentEvent, Image2VideoLabel, sererere);
            }
        }

        void CompAudioOutput(string out_txt)
        {
            //Console.WriteLine(out_txt);
            this.Dispatcher.Invoke(UpdateLabelContentEvent, CompAudioLabel, out_txt);
        }

        /* デリゲート */
        void event_DataReceived(Label label, string data)
        {
            label.Content = data;
            Console.WriteLine(data);// ［出力］ウィンドウに出力
        }

        bool event_CheckBoxIsChecked(CheckBox check_box)
        {
            return (bool)check_box.IsChecked;
        }

        /* ボタンUI関連 */
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (TempFolderTextBox.Text == "") TempFolderTextBox.Text = Directory.GetCurrentDirectory();
            if (InputPathTextBox.Text == "") return;
            if (OutputPathTextBox.Text == "") OutputPathTextBox.Text = System.IO.Path.GetDirectoryName(InputPathTextBox.Text)+"[conv]"+ System.IO.Path.GetFileName(InputPathTextBox.Text);

            paths.InitTempDirectory(TempFolderTextBox.Text);
            paths.InputFile = InputPathTextBox.Text;
            paths.OutputFile = OutputPathTextBox.Text;

            cancel_flag = false;

            Task task = Task.Run(() => {
                ConvertTaskMethod();
            });
        }

        private void TestRunButton_Click(object sender, RoutedEventArgs e)
        {
            if (TempFolderTextBox.Text == "") TempFolderTextBox.Text = Directory.GetCurrentDirectory();
            if (InputPathTextBox.Text == "") return;
            if (OutputPathTextBox.Text == "") OutputPathTextBox.Text = System.IO.Path.GetDirectoryName(InputPathTextBox.Text) + "[conv]" + System.IO.Path.GetFileName(InputPathTextBox.Text);

            paths.InitTempDirectory(TempFolderTextBox.Text);
            paths.InputFile = InputPathTextBox.Text;
            paths.OutputFile = OutputPathTextBox.Text;

            Task task = Task.Run(() => {
                ConvertTaskMethod(true);
            });
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

            paths = new PathManager();
            commands = new Commander(paths.GetFFmpegPath(), paths.GetWaifu2xPath(), paths.GetAnime4KPath() );

            cancel_flag = false;

            UpdateLabelContentEvent = new UpdateLabelContentEventHandler(event_DataReceived);

            GetCheckBoxIsCheckedContentEvent = new GetCheckBoxIsCheckedEventHandler(event_CheckBoxIsChecked);
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

            Properties.Settings.Default.Save();
        }


    }
}

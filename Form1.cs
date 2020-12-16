using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace AnimeLoupe2x
{
    public partial class AnimeLoupe2x : Form
    {
        public AnimeLoupe2x()
        {
            InitializeComponent();
        }


		//ラベル操作のデリゲート
		public delegate void MyEventHandler(string data);
		public event MyEventHandler myEvent = null;
		Process process = null;
		bool exec_com_flag = true;

        public struct Command
        {
            public string command;
            public string option;
            public void log()
            {
                Console.WriteLine("command: " + command);
                Console.WriteLine("option: " + option);
            }
        }
        string FFmpegPath = Directory.GetCurrentDirectory()+@"\Lib\ffmpeg\";
        string Waifu2xPath = Directory.GetCurrentDirectory() + @"\Lib\waifu2x-caffe\";
        string Anime4KPath = Directory.GetCurrentDirectory() + @"\Lib\Anime4KCPP_CLI\";

        string TEMP_AUDIO_FILE_NAME = "audio1.wav";
        string TEMP_VIDEO_FILE_NAME = "video_output.avi";
        string TEMP_BASE_DIR = Directory.GetCurrentDirectory() + @"\temp\";

        public struct VideoInfo
        {
            public string fps;
            public string bitrate;

            public void log()
            {
                Console.WriteLine("video info");
                Console.WriteLine("fps: "+fps);
                Console.WriteLine("bitrate: "+bitrate);
            }
        }

        public struct TempFileInfo
        {
            public string baseTempPath;
            public string imageTempPath;
            public string convertTempPath;
            public string audioTempPath;
            public string videoTempPath;
        }

        public struct ConvertInfo
        {
            public float scale;
            public string mode;
            public string process;
            public int noise_level;
            public string y;
        }

        public bool CancelFlag;

		private void myProcess_Exited(object sender, System.EventArgs e)
		{
			this.Invoke(myEvent, "end");
			exec_com_flag = true;
		}

		void event_DataReceived(string data)
		{
			LogText.AppendText(data + "\r\n");
			Console.WriteLine(data);// ［出力］ウィンドウに出力
		}

		void p_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
		{
			this.Invoke(myEvent, e.Data);
		}

		void CommandThreadFunc()
		{
			// 現在のマネージドスレッドの識別子を表示
			Console.WriteLine("スレッドID] => {0}", Thread.CurrentThread.ManagedThreadId);
            CancelFlag = false;

            if (inputPath == null)
			{
				Console.WriteLine("file input is none.");
				return;
			}
			if (outputPath == null)
			{
				Console.WriteLine("file output is none.");
				return;
			}

            string inputFilePath = inputPath.Text;
			string outputFilePath = outputPath.Text;

            TempFileInfo temp_file_info = new TempFileInfo();
            InitalizeTempDir(ref temp_file_info);

            Command meirei;

            VideoInfo video_info = new VideoInfo();
            GetVideoInfo(inputFilePath, ref video_info);
            video_info.log();

            if (V2ICheckBox.Checked)
			{
				meirei = MakeVideo2ImageString(inputFilePath, temp_file_info.imageTempPath + "image_%08d.png");
				ExecFFmpegCommand(meirei);
			}
			if (V2ACheckBox.Checked)
			{
				meirei = MakeSepAudioString(inputFilePath, temp_file_info.audioTempPath + TEMP_AUDIO_FILE_NAME);
				ExecFFmpegCommand(meirei);
			}
			if (Waifu2xCheckBox.Checked)
			{
                var delta_list = new List<string>();
                string[] pre_temp_files = Directory.GetFiles(temp_file_info.imageTempPath, "*.png");
                this.Invoke(myEvent, "init files");
                foreach (string name in pre_temp_files)
                {
                    delta_list.Add(System.IO.Path.GetFileName(name));
                }

                string[] conv_temp_files = Directory.GetFiles(temp_file_info.convertTempPath, "*.png");
                foreach (string name in conv_temp_files)
                {
                    delta_list.Remove(System.IO.Path.GetFileName(name));
                    //Console.WriteLine(System.IO.Path.GetFileName(name));
                }
                this.Invoke(myEvent, "convert file count="+delta_list.Count().ToString() );

                ConvertInfo c_info = new ConvertInfo();
                c_info.mode = "noise_scale";
                c_info.process = "cudnn";
                c_info.scale = 2.25f; // 720x480
                //c_info.scale = 1.50f; // 1280x720
                if (SizeRateTextBox.Text != "") c_info.scale = float.Parse(SizeRateTextBox.Text);
                c_info.noise_level = 2;
                c_info.y = "upconv_7_anime_style_art_rgb";

                
                int temp_file_count = 0;
                foreach (string file_name in delta_list)
                {
                    if (CancelFlag) return;

                    meirei = MakeAnime4KString(temp_file_info.imageTempPath + file_name, temp_file_info.convertTempPath + file_name, c_info);
                    if (Waifu2xRadio.Checked)
                    {
                        meirei = MakeWaifu2xString(temp_file_info.imageTempPath + file_name, temp_file_info.convertTempPath + file_name, c_info);
                        this.Invoke(myEvent, "convert engine is waifu2x");
                    }
                    ExecFFmpegCommand(meirei);
                    this.Invoke(myEvent, "converted: "+ file_name +" ("+ temp_file_count.ToString()+"/" +delta_list.Count.ToString()+")" );
                    temp_file_count += 1;
                }

            }
			if (I2VCheckBox.Checked)
			{
				meirei = MakeImage2VideoString(temp_file_info.convertTempPath + "image_%08d.png", temp_file_info.videoTempPath + TEMP_VIDEO_FILE_NAME, video_info);
                ExecFFmpegCommand(meirei);
			}
			if (AddAudioCheckBox.Checked)
			{
				meirei = MakeComAudioString(temp_file_info.videoTempPath + TEMP_VIDEO_FILE_NAME, temp_file_info.audioTempPath + TEMP_AUDIO_FILE_NAME, outputFilePath);
				ExecFFmpegCommand(meirei);
			}

            if (ConvertTestCheckBox.Checked)
            {
                string image_file_format = ".png";

                var com1 = new LoadExecJob();
                com1.SetOutputFunc(test_func);

                meirei = MakeVideo2ImageString(inputFilePath, temp_file_info.imageTempPath + "image_%08d"+ image_file_format);
                com1.RunFFmpegAndJoin(meirei.command, meirei.option);

                meirei = MakeSepAudioString(inputFilePath, temp_file_info.audioTempPath + TEMP_AUDIO_FILE_NAME);
                com1.RunFFmpegAndJoin(meirei.command, meirei.option);

                meirei = MakeImage2VideoString(temp_file_info.imageTempPath + "image_%08d" + image_file_format, temp_file_info.videoTempPath + TEMP_VIDEO_FILE_NAME, video_info);
                com1.RunFFmpegAndJoin(meirei.command, meirei.option);

                meirei = MakeComAudioString(temp_file_info.videoTempPath + TEMP_VIDEO_FILE_NAME, temp_file_info.audioTempPath + TEMP_AUDIO_FILE_NAME, outputFilePath);
                com1.RunFFmpegAndJoin(meirei.command, meirei.option);
            }


            this.Invoke(myEvent, "END command");
        }

        void test_func(string out_txt)
        {
            this.Invoke(myEvent, out_txt);
        }


        Command MakeSepAudioString(string videoPath, string audioPath)
		{
			Command ret_val = new Command();
			ret_val.command = FFmpegPath + "ffmpeg.exe";
			ret_val.option = "-i "+videoPath + " -acodec copy " + audioPath;
			return ret_val;
		}

		Command MakeComAudioString(string baseVideoPath, string audioPath, string outVideoPath)
		{
			Command ret_val = new Command();
			ret_val.command = FFmpegPath + "ffmpeg.exe";
			//ret_val.option = @"-i F:\Program\C#\AnimeLoupe2x\temp\video_output.avi -i F:\Program\C#\AnimeLoupe2x\temp\audio.aac -c:v copy F:\Program\C#\AnimeLoupe2x\temp\output1.mp4";
			ret_val.option = @"-i "+ baseVideoPath + " -i "+audioPath+" -c copy -map 0:v:0 -map 1:a:0 " + "\"" + outVideoPath + "\"";
			return ret_val;
		}

		Command MakeVideo2ImageString(string videoPath, string tempPath)
		{
			Command ret_val = new Command();
			ret_val.command = FFmpegPath + "ffmpeg.exe";
			ret_val.option = @"-i "+ "\"" + videoPath + "\"" + " -vcodec png " +tempPath;
			return ret_val;
		}

		Command MakeImage2VideoString(string imagePath, string videoPath, VideoInfo vi)
		{
			Command ret_val = new Command();
			ret_val.command = FFmpegPath + "ffmpeg.exe";
			//ret_val.option = @"-framerate " + vi.fps + @" -i "+ imagePath + " -vcodec libx264 -q 0 -pix_fmt yuv420p "+"-b "+ vi.bitrate + " -r "+vi.fps+" "+ "\"" + videoPath+"\"";
            //ret_val.option = @"-framerate " + vi.fps + @" -i "+ imagePath + " -vcodec libx264 -crf 0 -pix_fmt yuv420p" + " -r "+vi.fps+" " +videoPath;
            ret_val.option = @"-framerate " + vi.fps + @" -i "+ imagePath + " -vcodec h264_nvenc -crf 0 -pix_fmt yuv420p" + " -r "+vi.fps+" " +videoPath;
            return ret_val;
		}

		Command MakeWaifu2xString(string inputFile, string outputFile, ConvertInfo info)
		{
			Command ret_val = new Command();
			ret_val.command = Waifu2xPath + "waifu2x-caffe-cui.exe";
			ret_val.option = "-i "+ inputFile + @" -o "+ outputFile+ " -m "+info.mode+" -s "+info.scale.ToString("0.00") + " -n "+info.noise_level.ToString()+" -p "+info.process+ " -y "+info.y;
			return ret_val;
		}
        
        Command MakeAnime4KString(string inputFile, string outputFile, ConvertInfo info)
        {
            Command ret_val = new Command();
            ret_val.command = Anime4KPath + "Anime4KCPP_CLI.exe";
            ret_val.option = "-i " + inputFile + @" -o " + outputFile + " -z " + info.scale.ToString("0.000") + " -q -a";
            return ret_val;
        }

		int ExecFFmpegCommand(Command meirei)
		{
            meirei.log();
			Process proc = new Process();
			proc.StartInfo.FileName = meirei.command;
			proc.StartInfo.Arguments = meirei.option;

			proc.StartInfo.RedirectStandardError = true; // 標準出力をリダイレクト
			proc.StartInfo.UseShellExecute = false; // シェル機能オフ
			proc.StartInfo.CreateNoWindow = true; // コマンドプロンプトを非表示
			if (!proc.Start())
			{
				Console.WriteLine("Error starting");
				return -1;
			}
			StreamReader reader = proc.StandardError;
			string line;
			while ((line = reader.ReadLine()) != null)
			{
                /*
                int indxc = line.IndexOf("frame=");
                if (indxc > 0)
                {

                }
                */
                this.Invoke(myEvent, line);
            }
			proc.Close();
			
			return 0;
		}

        int GetVideoInfo(string videoPath, ref VideoInfo vi)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = FFmpegPath + "ffmpeg.exe";
            proc.StartInfo.Arguments = "-i " + videoPath;

            proc.StartInfo.RedirectStandardError = true; // 標準出力をリダイレクト
            proc.StartInfo.UseShellExecute = false; // シェル機能オフ
            proc.StartInfo.CreateNoWindow = true; // コマンドプロンプトを非表示
            if (!proc.Start())
            {
                Console.WriteLine("Error starting");
                return -1;
            }
            StreamReader reader = proc.StandardError;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                this.Invoke(myEvent, line);
                if (line.IndexOf("fps") > 0)
                {
                    vi.fps = line.Substring(line.IndexOf("fps") - 6, 5);
                }
                if (line.IndexOf("bitrate:") > 0)
                {
                    string bitrate_tmp = line.Substring(line.IndexOf("bitrate:") + 9, 6);
                    vi.bitrate = bitrate_tmp.Replace(" ", "");
                } 

            }
            proc.Close();

            return 0;
        }

        void UpdateTempDirPath()
        {
            if (TempFilePathText.Text == "")
            {
                TEMP_BASE_DIR = Directory.GetCurrentDirectory() + @"\temp\";
                return;
            }

            if (!Directory.Exists(TempFilePathText.Text))
            {
                return;
            }
            TEMP_BASE_DIR = TEMP_BASE_DIR = TempFilePathText.Text + @"\temp\";
        }

        void InitalizeTempDir(ref TempFileInfo tfi)
        {
            UpdateTempDirPath();

            tfi.imageTempPath = TEMP_BASE_DIR + @"image\";
            tfi.convertTempPath = TEMP_BASE_DIR + @"convert\";
            tfi.audioTempPath = TEMP_BASE_DIR + @"audio\";
            tfi.videoTempPath = TEMP_BASE_DIR + @"video\";

            if (Directory.Exists(TEMP_BASE_DIR) == false)
            {
                Directory.CreateDirectory(TEMP_BASE_DIR);
                Directory.CreateDirectory(tfi.imageTempPath);
                Directory.CreateDirectory(tfi.convertTempPath);
                Directory.CreateDirectory(tfi.audioTempPath);
                Directory.CreateDirectory(tfi.videoTempPath);
            }

        }

        public static void DeleteTempDir(string targetDirectoryPath)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            //ディレクトリ以外の全ファイルを削除
            string[] filePaths = Directory.GetFiles(targetDirectoryPath);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            //ディレクトリの中のディレクトリも再帰的に削除
            string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
            foreach (string directoryPath in directoryPaths)
            {
                DeleteTempDir(directoryPath);
            }

            //中が空になったらディレクトリ自身も削除
            Directory.Delete(targetDirectoryPath, false);
        }

        int ExecWaifu2xCommand()
		{
			
        // List<string> str
        for (int i = 0; i < 1; i++)
        {
	        while (exec_com_flag == false)
	        {
		        Console.WriteLine("waiting");
		        Thread.Sleep(1000);
	        }

	        process = new Process();

	        process.StartInfo.FileName = @"F:\Program\C#\AnimeLoupe2x\ffmpeg.exe";
	        process.StartInfo.Arguments = @"-i F:\Program\C#\AnimeLoupe2x\input.mp4 -vcodec png F:\Program\C#\AnimeLoupe2x\temp\image_%05d.png";

	        process.StartInfo.UseShellExecute = false; // シェル機能オフ
	        process.StartInfo.CreateNoWindow = true; // コマンドプロンプトを非表示
	        process.StartInfo.RedirectStandardOutput = true; // 標準出力をリダイレクト
	        process.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
	        process.StartInfo.Arguments = @"-i F:\Program\C#\AnimeLoupe2x\input.mp4 -vcodec png F:\Program\C#\AnimeLoupe2x\temp\image_%05d.png";
	        process.EnableRaisingEvents = true;
	        process.Exited += new EventHandler(myProcess_Exited);

	        process.Start();
	        process.BeginOutputReadLine();
	        exec_com_flag = false;
        }
			return 0;
		}

        /* 設定の読み込みと保存 */
		private void AnimeLoupe2x_Load(object sender, EventArgs e)
		{
			inputPath.Text = Properties.Settings.Default.input_path;
			outputPath.Text = Properties.Settings.Default.output_path;

			V2ICheckBox.Checked = Properties.Settings.Default.video2image_flag;
			V2ACheckBox.Checked = Properties.Settings.Default.video2audio_flag;
			Waifu2xCheckBox.Checked = Properties.Settings.Default.waifu2x_flag;
			I2VCheckBox.Checked = Properties.Settings.Default.image2video_flag;
			AddAudioCheckBox.Checked = Properties.Settings.Default.add_audio_flag;
            ConvertTestCheckBox.Checked = Properties.Settings.Default.convert_test_flag;

            SizeRateTextBox.Text = Properties.Settings.Default.output_size_rate;
            TempFilePathText.Text = Properties.Settings.Default.temp_dir_path;
        }

		private void AnimeLoupe2x_FormClosing(object sender, FormClosingEventArgs e)
		{
			Properties.Settings.Default.input_path = inputPath.Text;
			Properties.Settings.Default.output_path = outputPath.Text;

			Properties.Settings.Default.video2image_flag = V2ICheckBox.Checked;
			Properties.Settings.Default.video2audio_flag = V2ACheckBox.Checked;
			Properties.Settings.Default.waifu2x_flag = Waifu2xCheckBox.Checked;
			Properties.Settings.Default.image2video_flag = I2VCheckBox.Checked;
			Properties.Settings.Default.add_audio_flag = AddAudioCheckBox.Checked;
            Properties.Settings.Default.convert_test_flag = ConvertTestCheckBox.Checked;

            Properties.Settings.Default.output_size_rate = SizeRateTextBox.Text;
            Properties.Settings.Default.temp_dir_path = TempFilePathText.Text;

            // ファイルに保存
            Properties.Settings.Default.Save();
        }

        /* ボタンクリック関連 */
        private void InputPathButton_Click(object sender, EventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = System.IO.Path.GetFileName(inputPath.Text);
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            if (inputPath.Text != "") ofd.InitialDirectory = System.IO.Path.GetDirectoryName(inputPath.Text);
            //[ファイルの種類]に表示される選択肢を指定する
            //指定しないとすべてのファイルが表示される
            ofd.Filter = "mp4ファイル(*.mp4)|*.mp4|すべてのファイル(*.*)|*.*";
            //[ファイルの種類]ではじめに選択されるものを指定する
            //2番目の「すべてのファイル」が選択されているようにする
            ofd.FilterIndex = 2;
            //タイトルを設定する
            ofd.Title = "開くファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            ofd.RestoreDirectory = true;
            //存在しないファイルの名前が指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckFileExists = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            ofd.CheckPathExists = true;

            //ダイアログを表示する
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                //OKボタンがクリックされたとき、選択されたファイル名を表示する
                Console.WriteLine(ofd.FileName);
                inputPath.Text = ofd.FileName;
                outputPath.Text = System.IO.Path.GetDirectoryName(ofd.FileName)+"\\" + System.IO.Path.GetFileNameWithoutExtension(ofd.FileName) + "[convert]" + System.IO.Path.GetExtension(ofd.FileName);
            }
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            myEvent = new MyEventHandler(event_DataReceived);

            Console.WriteLine(System.Environment.CurrentDirectory);

            Task task = Task.Run(() => {
                CommandThreadFunc();
            });

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            CancelFlag = true;
            this.Invoke(myEvent, "command is canceled!");
        }

        private void ClearTempButton_Click(object sender, EventArgs e)
        {
            UpdateTempDirPath();
            DeleteTempDir(TEMP_BASE_DIR);
        }
    }
}

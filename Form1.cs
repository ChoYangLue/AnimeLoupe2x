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

        private void RunButton_Click(object sender, EventArgs e)
        {
			myEvent = new MyEventHandler(event_DataReceived);

			//System.Environment.CurrentDirectory = @"C:\Users\";
			Console.WriteLine(System.Environment.CurrentDirectory);

			Task task = Task.Run(() => {
				CommandThreadFunc();
			});

		}

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

            string dirpath = Directory.GetCurrentDirectory();
            Console.WriteLine(dirpath);

            string inputFilePath = inputPath.Text;
			string outputFilePath = outputPath.Text;

            string baseTempPath = Directory.GetCurrentDirectory() + @"\temp\";
            string imageTempPath = baseTempPath+@"image\";
            string audioTempPath = baseTempPath+@"audio\";
            string videoTempPath = baseTempPath+@"video\";

            if (Directory.Exists(baseTempPath) == false)
            {
                Directory.CreateDirectory(baseTempPath);
                Directory.CreateDirectory(imageTempPath);
                Directory.CreateDirectory(audioTempPath);
                Directory.CreateDirectory(videoTempPath);
            }


            Command meirei;

            VideoInfo video_info = new VideoInfo();
            GetVideoInfo(inputFilePath, ref video_info);
            video_info.log();

            if (V2ICheckBox.Checked)
			{
				meirei = MakeVideo2ImageString(inputFilePath, imageTempPath+ "image_%08d.png");
				ExecFFmpegCommand(meirei);
			}
			if (V2ACheckBox.Checked)
			{
				meirei = MakeSepAudioString(inputFilePath, audioTempPath + "audio1.aac");
				ExecFFmpegCommand(meirei);
			}
			if (Waifu2xCheckBox.Checked)
			{
				meirei = MakeWaifu2xString();
			}
			if (I2VCheckBox.Checked)
			{
				meirei = MakeImage2VideoString(imageTempPath+ "image_%08d.png", videoTempPath+ "video_output.avi", video_info);
				ExecFFmpegCommand(meirei);
			}
			if (AddAudioCheckBox.Checked)
			{
				meirei = MakeComAudioString(videoTempPath + "video_output.avi", audioTempPath + "audio1.aac", outputFilePath);
				ExecFFmpegCommand(meirei);
			}


		}


        Command MakeSepAudioString(string videoPath, string audioPath)
		{
			Command ret_val = new Command();
			ret_val.command = FFmpegPath + "ffmpeg.exe";
			ret_val.option = "-i "+ videoPath + " -vn -acodec copy "+ audioPath;
			return ret_val;
		}

		Command MakeComAudioString(string baseVideoPath, string audioPath, string outVideoPath)
		{
			Command ret_val = new Command();
			ret_val.command = FFmpegPath + "ffmpeg.exe";
			//ret_val.option = @"-i F:\Program\C#\AnimeLoupe2x\temp\video_output.avi -i F:\Program\C#\AnimeLoupe2x\temp\audio.aac -c:v copy F:\Program\C#\AnimeLoupe2x\temp\output1.mp4";
			ret_val.option = @"-i "+ baseVideoPath + " -i "+audioPath+" -c copy -map 0:v:0 -map 1:a:0 "+outVideoPath;
			return ret_val;
		}

		Command MakeVideo2ImageString(string videoPath, string tempPath)
		{
			Command ret_val = new Command();
			ret_val.command = FFmpegPath + "ffmpeg.exe";
			ret_val.option = @"-i "+ videoPath + " -vcodec png "+tempPath;
			//ret_val.option = @"-i F:\Program\C#\AnimeLoupe2x\input.mp4";
			return ret_val;
		}

		Command MakeImage2VideoString(string imagePath, string videoPath, VideoInfo vi)
		{
			Command ret_val = new Command();
			ret_val.command = FFmpegPath + "ffmpeg.exe";
			ret_val.option = @"-framerate " + vi.fps + @" -i "+ imagePath + " -vcodec libx264 -pix_fmt yuv420p "+"-b "+ vi.bitrate + " -r "+vi.fps+" "+ videoPath;
			return ret_val;
		}

		Command MakeWaifu2xString()
		{
			Command ret_val = new Command();
			ret_val.command = Waifu2xPath + "waifu2x-caffe-cui.exe";
			ret_val.option = @"-i F:\Program\C#\AnimeLoupe2x\temp\image_%08d.png -vcodec mjpeg -q 0 F:\Program\C#\AnimeLoupe2x\temp\video_output.avi";
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
				this.Invoke(myEvent, line);

			}
			proc.Close();
			this.Invoke(myEvent, "END command");
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

		private void AnimeLoupe2x_Load(object sender, EventArgs e)
		{
			inputPath.Text = Properties.Settings.Default.input_path;
			outputPath.Text = Properties.Settings.Default.output_path;

			V2ICheckBox.Checked = Properties.Settings.Default.video2image_flag;
			V2ACheckBox.Checked = Properties.Settings.Default.video2audio_flag;
			Waifu2xCheckBox.Checked = Properties.Settings.Default.waifu2x_flag;
			I2VCheckBox.Checked = Properties.Settings.Default.image2video_flag;
			AddAudioCheckBox.Checked = Properties.Settings.Default.add_audio_flag;
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

            // ファイルに保存
            Properties.Settings.Default.Save();
        }

        private void InputPathButton_Click(object sender, EventArgs e)
        {
            //OpenFileDialogクラスのインスタンスを作成
            OpenFileDialog ofd = new OpenFileDialog();

            //はじめのファイル名を指定する
            //はじめに「ファイル名」で表示される文字列を指定する
            ofd.FileName = System.IO.Path.GetFileName(inputPath.Text);
            //はじめに表示されるフォルダを指定する
            //指定しない（空の文字列）の時は、現在のディレクトリが表示される
            ofd.InitialDirectory = System.IO.Path.GetDirectoryName(inputPath.Text);
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
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace AnimeLoupe2x
{
    class Commander
    {
		/* 全コマンド共通 */
		private string FFmpegPath;
		private string Waifu2xPath;
		private string Anime4KPath;

		/* 各コマンド固有 */
		public string command;
        public string option;

		/* 動画情報 */
		public float ci_scale;
		public string ci_mode;
		public string ci_process;
		public int ci_noise_level;
		public string ci_y;
		public string vi_fps;
		public string vi_bitrate;

		public Commander(string ffmpeg_path, string waifu2x_path, string anime4k_path)
        {
			FFmpegPath = ffmpeg_path;
			Waifu2xPath = waifu2x_path;
			Anime4KPath = anime4k_path;

			ci_scale = 0.0f;
			ci_mode = "";
			ci_process = "";
			ci_noise_level = 0;
			ci_y = "";
			vi_fps = "";
			vi_bitrate = "";
		}

		public void MakeSepAudioString(string videoPath, string audioPath)
		{
			command = FFmpegPath + "ffmpeg.exe";
			option = "-i " + videoPath + " -acodec copy " + audioPath;
		}

		public void MakeComAudioString(string baseVideoPath, string audioPath, string outVideoPath)
		{
			command = FFmpegPath + "ffmpeg.exe";
			//option = @"-i F:\Program\C#\AnimeLoupe2x\temp\video_output.avi -i F:\Program\C#\AnimeLoupe2x\temp\audio.aac -c:v copy F:\Program\C#\AnimeLoupe2x\temp\output1.mp4";
			option = @"-i " + baseVideoPath + " -i " + audioPath + " -c copy -map 0:v:0 -map 1:a:0 " + "\"" + outVideoPath + "\"";
		}

		public void MakeVideo2ImageString(string videoPath, string tempPath)
		{
			command = FFmpegPath + "ffmpeg.exe";
			option = @"-i " + "\"" + videoPath + "\"" + " -vcodec png " + tempPath;
		}

		public void MakeImage2VideoString(string imagePath, string videoPath)
		{
			command = FFmpegPath + "ffmpeg.exe";
			//option = @"-framerate " + vi.fps + @" -i "+ imagePath + " -vcodec libx264 -q 0 -pix_fmt yuv420p "+"-b "+ vi.bitrate + " -r "+vi.fps+" "+ "\"" + videoPath+"\"";
			//option = @"-framerate " + vi.fps + @" -i "+ imagePath + " -vcodec libx264 -crf 0 -pix_fmt yuv420p" + " -r "+vi.fps+" " +videoPath;
			//option = @"-framerate " + vi_fps + @" -i " + imagePath + " -vcodec h264_nvenc -crf 2 -qp 0 -pix_fmt yuv420p" + " -r " + vi_fps + " " + videoPath;
			option = @"-framerate " + vi_fps + @" -i " + imagePath + " -vcodec libx265 -crf 2 -qp 0 -pix_fmt yuv420p" + " -r " + vi_fps + " " + videoPath;
		}

		public void MakeWaifu2xString(string inputFile, string outputFile)
		{
			command = Waifu2xPath + "waifu2x-caffe-cui.exe";
			option = "-i " + inputFile + @" -o " + outputFile + " -m " + ci_mode + " -s " + ci_scale.ToString("0.00") + " -n " + ci_noise_level.ToString() + " -p " + ci_process + " -y " + ci_y;
		}

		public void MakeAnime4KString(string inputFile, string outputFile)
		{
			command = Anime4KPath + "Anime4KCPP_CLI.exe";
			option = "-i " + inputFile + @" -o " + outputFile + " -z " + ci_scale.ToString("0.000") + " -q -a";
		}

		public void GetVideoInfoString(string inputFile)
        {
			command = FFmpegPath + "ffmpeg.exe";
			option = "-i " + inputFile;
		}

		public void log()
		{
			Console.WriteLine("command: " + command);
			Console.WriteLine("option: " + option);
		}
	}
}

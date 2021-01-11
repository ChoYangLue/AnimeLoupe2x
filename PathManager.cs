using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AnimeLoupe2x
{
    class PathManager
    {
        /* ライブラリパス */
        private static string FFmpegPath = Directory.GetCurrentDirectory() + @"\Lib\ffmpeg\";
        private static string Waifu2xPath = Directory.GetCurrentDirectory() + @"\Lib\waifu2x-caffe\";
        private static string Anime4KPath = Directory.GetCurrentDirectory() + @"\Lib\Anime4KCPP_CLI\";

        /* Tempディレクトリパス */
        private string TempImageDir = Directory.GetCurrentDirectory() + @"\temp\image\";
        private string TempConvertDir = Directory.GetCurrentDirectory() + @"\temp\convert\";
        private string TempAudioDir = Directory.GetCurrentDirectory() + @"\temp\audio\";
        private string TempVideoDir = Directory.GetCurrentDirectory() + @"\temp\video\";

        /* ユーザ指定パス */
        public string InputFile = "";
        public string OutputFile = "";

        public void CreateTempDir(string targetDirectoryPath)
        {
            Directory.CreateDirectory(targetDirectoryPath + @"image\");
            TempImageDir = targetDirectoryPath + @"image\";

            Directory.CreateDirectory(targetDirectoryPath + @"convert\");
            TempConvertDir = targetDirectoryPath + @"convert\";

            Directory.CreateDirectory(targetDirectoryPath + @"audio\");
            TempAudioDir = targetDirectoryPath + @"audio\";

            Directory.CreateDirectory(targetDirectoryPath + @"video\");
            TempVideoDir = targetDirectoryPath + @"video\";
        }

        public void DeleteTempDir(string targetDirectoryPath)
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

        public void InitTempDirectory(string root_dir_path)
        {
            // hoge/hoge

            string temp_dir_root = root_dir_path + @"\temp\";

            if (Directory.Exists(temp_dir_root))
            {
                DeleteTempDir(temp_dir_root);
            }

            if (Directory.Exists(temp_dir_root) == false)
            {
                Directory.CreateDirectory(temp_dir_root);
                CreateTempDir(temp_dir_root);
            }
        }


        /* ゲッター */
        public string GetFFmpegPath()
        {
            return FFmpegPath;
        }

        public string GetWaifu2xPath()
        {
            return Waifu2xPath;
        }

        public string GetAnime4KPath()
        {
            return Anime4KPath;
        }

        public string GetTempImageDir()
        {
            return TempImageDir;
        }
        public string GetTempConvertDir()
        {
            return TempConvertDir;
        }
        public string GetTempAudioDir()
        {
            return TempAudioDir;
        }
        public string GetTempVideoDir()
        {
            return TempVideoDir;
        }
    }
}

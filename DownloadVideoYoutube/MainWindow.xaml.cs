using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Xabe;
using Xabe.FFmpeg;
using System.Diagnostics;

namespace DownloadVideoYoutube
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        YoutubeClient youtube;

        StreamManifest streamManifest;

        public string Link { get; set; }

        public string LinkID { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _ = DownloadAsync();
        }

        private async Task GetInfo()
        {
            if(image1.Source != null)
            {
                image1.Source = null;
            }

            if(comboBox1.Items.Count > 0)
            {
                comboBox1.Items.Clear();
            }

            textBoxStatus.Text = "Начинаю загружать данные о видео. Ожидайте!";

            Link = textbox1.Text;

            LinkID = Link.Substring(Link.IndexOf('=') + 1);

            youtube = new YoutubeClient();

            var video = await youtube.Videos.GetAsync(Link);

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(LinkID);

            var streamInfo = streamManifest
            .GetVideoOnly()
            .Where(s => s.Container == Container.Mp4)
            .GetAllVideoQualityLabels();

            foreach (var item in streamInfo)
            {
                comboBox1.Items.Add(item);
            }

            textBoxTitle.Text = video.Title;
            textBoxAuthor.Text = video.Author;

            textbox2.Text = video.Description;

            textBoxStatus.Text = "Данные успешно загруженны!";

            var img = Bitmap.FromStream(new MemoryStream(new WebClient().DownloadData($"https://img.youtube.com/vi/" + LinkID +  "/maxresdefault.jpg")));

            byte[] arr = (byte[])(new ImageConverter()).ConvertTo(img, typeof(byte[]));

            image1.Source = byteArrayToImage(arr);

        }

        private BitmapImage byteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                stream.Write(byteArrayIn, 0, byteArrayIn.Length);
                stream.Position = 0;
                System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                BitmapImage returnImage = new BitmapImage();
                returnImage.BeginInit();
                MemoryStream ms = new MemoryStream();
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                returnImage.StreamSource = ms;
                returnImage.EndInit();

                return returnImage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DownloadAsync()
        {
            if (comboBox1.Text != "")
            {
                SaveFileDialog saveFile = new SaveFileDialog();

                saveFile.ShowDialog();

                if (saveFile.FileName != "")
                {
                    youtube = new YoutubeClient();

                    streamManifest = await youtube.Videos.Streams.GetManifestAsync(LinkID);

                    switch(comboBox1.Text)
                    {
                        case "1080p":
                            _ = DownloadHightQualityAsync(VideoQuality.High1080, saveFile.FileName);
                            break; 
                        case "1080p60":
                            _ = DownloadHightQualityAsync(VideoQuality.High1080, saveFile.FileName);
                            break;
                        case "1080p30":
                            _ = DownloadHightQualityAsync(VideoQuality.High1080, saveFile.FileName);
                            break;
                        case "2160p60 HDR":
                            _ = DownloadHightQualityAsync(VideoQuality.High2160, saveFile.FileName);
                            break;
                        case "2160p60":
                            _ = DownloadHightQualityAsync(VideoQuality.High2160, saveFile.FileName);
                            break;
                        case "2160p":
                            _ = DownloadHightQualityAsync(VideoQuality.High2160, saveFile.FileName);
                            break;
                        case "4320p60 HDR":
                            _ = DownloadHightQualityAsync(VideoQuality.High4320, saveFile.FileName);
                            break;
                        case "4320p60":
                            _ = DownloadHightQualityAsync(VideoQuality.High4320, saveFile.FileName);
                            break;
                        case "4320p":
                            _ = DownloadHightQualityAsync(VideoQuality.High4320, saveFile.FileName);
                            break;
                        case "1440p":
                            _ = DownloadHightQualityAsync(VideoQuality.High1440, saveFile.FileName);
                            break;
                        case "1440p60 HDR":
                            _ = DownloadHightQualityAsync(VideoQuality.High1440, saveFile.FileName);
                            break;
                        case "1440p60":
                            _ = DownloadHightQualityAsync(VideoQuality.High1440, saveFile.FileName);
                            break;
                        case "720p":
                            _ = DownloadMediumQualityAsync(VideoQuality.High720,saveFile.FileName);
                            break;
                        case "720p60":
                            _ = DownloadMediumQualityAsync(VideoQuality.High720,saveFile.FileName);
                            break;
                        case "720p30":
                            _ = DownloadMediumQualityAsync(VideoQuality.High720,saveFile.FileName);
                            break;
                        case "480p":
                            _ = DownloadMediumQualityAsync(VideoQuality.Medium480,saveFile.FileName);
                            break;
                        case "360p":
                            _ = DownloadMediumQualityAsync(VideoQuality.Medium360,saveFile.FileName);
                            break;
                        case "240p":
                            _ = DownloadMediumQualityAsync(VideoQuality.Low240,saveFile.FileName);
                            break;
                        case "144p":
                            _ = DownloadMediumQualityAsync(VideoQuality.Low144,saveFile.FileName);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public async Task DownloadMediumQualityAsync(VideoQuality quality,string saveFile)
        {
            var streamInfo = streamManifest.GetMuxed().Where(t => t.VideoQuality == quality).WithHighestVideoQuality();

            if (streamInfo != null)
            {
                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{saveFile}.{streamInfo.Container}");
            }
        }

        public async Task DownloadHightQualityAsync(VideoQuality quality, string saveFile)
        {
            System.Windows.MessageBox.Show("Укажите путь к файлу FFMPEG.EXE");

            OpenFileDialog openFile = new OpenFileDialog();

            openFile.Filter = "EXE file(*.exe)|*.exe";

            openFile.ShowDialog();

            textBoxStatus.Text = "Начинаю загрузку видео! Это может быть долго, ожидайте!";

            var streamInfo = streamManifest.GetVideoOnly().Where(s => s.VideoQuality == quality).WithHighestVideoQuality();

            var streamInfo1 = streamManifest.GetAudioOnly().WithHighestBitrate();

            if (streamInfo != null)
            {
                await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{saveFile}.mp4");
            }

            if (streamInfo != null)
            {
                await youtube.Videos.Streams.DownloadAsync(streamInfo1, $"{saveFile}.mp3");
            }

            string args = $"/c {openFile.FileName} -i \"{saveFile}.mp4\" -i \"{saveFile}.mp3\" -shortest {saveFile}_{quality.ToString()}.mp4";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.FileName = "cmd.exe";
            startInfo.WorkingDirectory = saveFile + ".mp4";
            startInfo.Arguments = args;

            using (Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.WaitForExit();
            }

            File.Delete(saveFile + ".mp4");
            File.Delete(saveFile + ".mp3");

            textBoxStatus.Text = $"Видео успешно скачано, путь к файлу - > {saveFile}_{quality.ToString()}.mp4";
        }

        public async Task DownloadAudioAsync()
        {
            SaveFileDialog saveFile = new SaveFileDialog();

            var youtube = new YoutubeClient();

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(LinkID);

            var streamInfo = streamManifest.GetAudioOnly().WithHighestBitrate();

            saveFile.Filter = $"mp3 file (*.mp3)|*.mp3";

            saveFile.ShowDialog();

            if(saveFile.FileName != "")
            {
                if(streamInfo != null)
                {
                    var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                    await youtube.Videos.Streams.DownloadAsync(streamInfo, $"{saveFile.FileName}.mp3");
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _ = GetInfo();
        }

       
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();

            saveFile.Filter = "Jpg file (*.jpg)|*.jpg|JPEG file (*.jpeg)|*.jpeg";

            saveFile.ShowDialog();

            if (saveFile.FileName != "")
            {
                var img = Bitmap.FromStream(new MemoryStream(new WebClient().DownloadData($"https://img.youtube.com/vi/" + LinkID + "/maxresdefault.jpg")));

                img.Save(saveFile.FileName);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            _ = DownloadAudioAsync();
        }
    }
}

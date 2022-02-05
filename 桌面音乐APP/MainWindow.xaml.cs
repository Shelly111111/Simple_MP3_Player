using System;
using System.IO;
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
using System.Windows.Threading;

namespace 桌面音乐APP
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        bool flag = true;
        public enum CycleMode{Single,List,Random};
        private List<Button> btnMusics = new List<Button>();//定义一个按钮集合
        public DispatcherTimer ShowTimer = new DispatcherTimer();//设置计时器
        /// <summary>
        /// 加载窗口程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DirectoryInfo folder = new DirectoryInfo(Environment.CurrentDirectory + @"\MP3");
            foreach(FileInfo file in folder.GetFiles("*.mp3"))
            {
                Button btnMusic = new Button();
                btnMusic.Content = file.Name;//设置显示文本
                btnMusic.Width = lstSongList.Width - 15;//设置按钮宽度
                btnMusic.HorizontalContentAlignment = HorizontalAlignment.Left;//设置文本对齐方式为左对齐
                btnMusic.HorizontalAlignment = HorizontalAlignment.Stretch;//设置按钮对齐方式为两端对齐
                btnMusic.VerticalAlignment = VerticalAlignment.Stretch;//设置按钮对齐方式为上下对齐
                btnMusic.Background = Brushes.Transparent;//设置按钮背景颜色为透明
                btnMusic.BorderBrush = null;//设置按钮的边框为空
                btnMusic.Click += BtnMusic_Click;
                btnMusics.Add(btnMusic);//将按钮添加到btnMusics泛型里
                lstSongList.Items.Add(btnMusic);//将按钮添加到音乐菜单里
            }
            musicPlayer.Loaded += MusicPlayer_Loaded;//音乐加载
            musicPlayer.MediaEnded += MusicPlayer_MediaEnded;//音乐结束并重新开始
            musicPlayer.Unloaded += MusicPlayer_Unloaded;//音乐未加载成功，停止播放
            //Progress_Slider.AddHandler(Slider.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Progress_Slider_MouseLeftButtonUp), true);
        }
        /// <summary>
        /// 音乐按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMusic_Click(object sender, RoutedEventArgs e)
        {
            musicPlayer.Stop();//当前播放停止
            musicPlayer.Source = new Uri(Environment.CurrentDirectory + @"\MP3\" + ((Button)sender).Content);//导入音乐地址
            Lb_SongName.Content = System.IO.Path.GetFileNameWithoutExtension(musicPlayer.Source.ToString());//标签显示获得的音乐名称
            if(flag==true)
                musicPlayer.Play();//开始播放
            Vol_Slider.Value = musicPlayer.Volume * 10;//将当前音量按比例赋值给音量条
        }
        /// <summary>
        /// 暂停/播放按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            if(flag==true)
            {
                musicPlayer.Pause();//音乐暂停
                btnPlay.Style = Resources["ButtonStyle1"] as Style;//将btnPlay按钮的风格改为暂停时的按钮风格
                flag = false;
            }
            else
            {
                musicPlayer.Play();//音乐播放
                btnPlay.Style = Resources["RoundButton"] as Style;//将按钮风格改为播放风格
                flag = true;
            }
        }

        private void MusicPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            MusicPlayer_Loaded(sender, e);
        }
        /// <summary>
        /// 音乐未加载成功事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MusicPlayer_Unloaded(object sender, RoutedEventArgs e)
        {
            (sender as MediaElement).Stop();
        }
        /// <summary>
        /// 音乐循环播放事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MusicPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            switch (Convert.ToInt32(btnCycleMode.Content))
            {
                case 0://单曲循环
                    (sender as MediaElement).Stop();
                    (sender as MediaElement).Play();
                    break;
                case 1://列表循环
                    musicPlayer.Stop();
                    for (int i = 0; i < btnMusics.Count; i++)
                    {
                        if (btnMusics[i].Content.ToString() == System.IO.Path.GetFileName(musicPlayer.Source.ToString()))
                        {
                            BtnMusic_Click(btnMusics[(i + 1) % btnMusics.Count], new RoutedEventArgs());
                            break;
                        }
                    }
                    break;
                case 2://随机循环
                    musicPlayer.Stop();
                    Random random = new Random();
                    int m = random.Next(btnMusics.Count);
                    BtnMusic_Click(btnMusics[m], new RoutedEventArgs());
                    break;
            }
        }
        /// <summary>
        /// 音乐加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MusicPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            if (btnMusics.Count > 0)
            {
                musicPlayer.Source = new Uri(Environment.CurrentDirectory + @"\MP3\" + btnMusics[0].Content);//如果当前音乐菜单里的音乐数大于0，则从第0首自动开始播放
                Lb_SongName.Content = System.IO.Path.GetFileNameWithoutExtension(musicPlayer.Source.ToString());//将当前播放音乐的名称显示到音乐名标签上
                (sender as MediaElement).Play();//音乐自动播放
                Vol_Slider.Value = musicPlayer.Volume * 10;//获取音量至音乐条上
                ShowTimer.Tick += ShowTimer_Tick;
                ShowTimer.Interval = TimeSpan.FromSeconds(0.1);
                ShowTimer.Start();
            }
            else
                MessageBox.Show("MP3文件夹下无MP3音频文件");//否则报错
        }
        /// <summary>
        /// 计时器更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowTimer_Tick(object sender, EventArgs e)
        {
            Progress_Slider.Value = musicPlayer.Position.TotalSeconds;//进度条读取音乐的当前位置所代表的秒数
            if (musicPlayer.NaturalDuration.HasTimeSpan == true)
            {
                Lb_Time.Content = musicPlayer.Position.Minutes + ":" + musicPlayer.Position.Seconds
                    + '/' + musicPlayer.NaturalDuration.TimeSpan.Minutes + ":" + musicPlayer.NaturalDuration.TimeSpan.Seconds;//时间标签显示时间
                Progress_Slider.Maximum = musicPlayer.NaturalDuration.TimeSpan.TotalSeconds;//进度条最大值读取为音乐自然播放时间长度
            }
        }
        /// <summary>
        /// 音量条更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Vol_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            musicPlayer.Volume = Vol_Slider.Value / 10;//根据音量条当前的值更改音乐的音量
        }
        /// <summary>
        /// 歌曲菜单单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCash_Click(object sender, RoutedEventArgs e)
        {
            if (lstSongList.Visibility == Visibility.Visible)//如果菜单已显示，则隐藏
            {
                btnExit.Visibility = Visibility.Hidden;
                btnMini.Visibility = Visibility.Hidden;
                lstSongList.Visibility = Visibility.Hidden;
                btnCash.Style = Resources["RoundButton_MusicCash"] as Style;
            }
            else
            {
                lstSongList.Visibility = Visibility.Visible;
                btnMini.Visibility = Visibility.Visible;
                btnExit.Visibility = Visibility.Visible;
                btnCash.Style = Resources["RoundButton_MusicCash_Close"] as Style;
            }
        }
        /// <summary>
        /// 上一曲单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            musicPlayer.Stop();//播放停止
            switch (Convert.ToInt32(btnCycleMode.Content))
            {
                case 0: musicPlayer.Play(); break;
                case 1:
                    for (int i = 0; i < btnMusics.Count; i++)
                    {
                        if (btnMusics[i].Content.ToString() == System.IO.Path.GetFileName(musicPlayer.Source.ToString()))//得到当前音乐在btnMusics中的下标
                        {
                            try
                            {
                                BtnMusic_Click(btnMusics[i - 1], new RoutedEventArgs());//单击上一曲的按钮
                                break;
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                BtnMusic_Click(btnMusics[(i - 1) + btnMusics.Count], new RoutedEventArgs());//如果下标-1后为负，则单击最后一曲音乐
                                break;
                            }
                        }
                    }
                    break;
                case 2:
                    Random random = new Random();
                    int m = random.Next(btnMusics.Count);
                    BtnMusic_Click(btnMusics[m], new RoutedEventArgs());
                    break;
            }
        }
        /// <summary>
        /// 下一曲单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            musicPlayer.Stop();
            switch(Convert.ToInt32(btnCycleMode.Content))
            {
                case 0:musicPlayer.Play();break;
                case 1:
                    for (int i = 0; i < btnMusics.Count; i++)
                    {
                        if (btnMusics[i].Content.ToString() == System.IO.Path.GetFileName(musicPlayer.Source.ToString()))
                        {
                            BtnMusic_Click(btnMusics[(i + 1) % btnMusics.Count], new RoutedEventArgs());
                            break;
                        }
                    }
                    break;
                case 2:
                    Random random = new Random();
                    int m = random.Next(btnMusics.Count);
                    BtnMusic_Click(btnMusics[m], new RoutedEventArgs());
                    break;
            }
        }
        /// <summary>
        /// 循环模式切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCycleMode_Click(object sender, RoutedEventArgs e)
        {
            switch(Convert.ToInt32(btnCycleMode.Content))
            {
                case 0:btnCycleMode.Content = CycleMode.List;//由单曲循环切换为列表循环
                    btnCycleMode.Style = Resources["RoundButton_ListMode"] as Style;
                    break;
                case 1:btnCycleMode.Content = CycleMode.Random;//由列表循环切换为随机循环
                    btnCycleMode.Style = Resources["RoundButton_RandomMode"] as Style;
                    break;
                case 2:btnCycleMode.Content = CycleMode.Single;//由随机循环切换为单曲循环
                    btnCycleMode.Style = Resources["RoundButton_CycleMode"] as Style;
                    break;
            }
        }
        /// <summary>
        /// 最小化窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMini_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// 程序退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        /// <summary>
        /// 进度条鼠标拖拽完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Progress_Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            musicPlayer.Position = TimeSpan.FromSeconds(Progress_Slider.Value);
            ShowTimer.Start();
        }
        /// <summary>
        /// 进度条鼠标拖拽开始事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Progress_Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            ShowTimer.Stop();
        }
        /// <summary>
        /// 拖动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}

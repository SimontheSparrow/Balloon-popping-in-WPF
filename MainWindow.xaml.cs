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
using System.Windows.Threading;
namespace Balloon_popping_WPF
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DispatcherTimer gameTimer = new DispatcherTimer();

        int speed = 3, intervals=90, balloonSkins, i , missedBalloons, score;

        bool gameIsActive;

        Random rand = new Random();

        List<Rectangle> itemRemover = new List<Rectangle>();

        ImageBrush backgroundImage = new ImageBrush();

        MediaPlayer soundPlayer = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Tick += TimerEvent;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            backgroundImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/background-Image.jpg"));
            myCanvas.Background = backgroundImage;
            RestartGame();
        }

        private void TimerEvent(object sender, EventArgs e)
        {
            scoreText.Content = "Score: " + score;
            intervals -= 10;
            if (intervals<1)
            {
                ImageBrush balloonImage = new ImageBrush();
                balloonSkins++;
                if (balloonSkins>5)
                {
                    balloonSkins = 1;
                }
                balloonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/balloon"+ balloonSkins + ".png"));

                Rectangle newBalloon = new Rectangle
                {
                    Tag = "balloon",
                    Width = 50,
                    Height = 70,
                    Fill = balloonImage
                };

                Canvas.SetLeft(newBalloon, rand.Next(50, (int)Application.Current.MainWindow.Width - 50));
                Canvas.SetTop(newBalloon, rand.Next((int)Application.Current.MainWindow.Height + 50, 800));
                myCanvas.Children.Add(newBalloon);

                intervals = rand.Next(90, 150);
            }
            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag=="balloon")
                {
                    i = rand.Next(-5, 5);
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - (i * -1));
                    Canvas.SetTop(x, Canvas.GetTop(x) - speed);
                }
                if (Canvas.GetTop(x)<20)
                {
                    itemRemover.Add(x);
                    missedBalloons++;
                }
               
            }
            foreach (Rectangle y in itemRemover)
            {
                myCanvas.Children.Remove(y);
            }
            if (missedBalloons>10)
            {
                gameIsActive = false;
                gameTimer.Stop();
                MessageBox.Show("Game over!!! Click to restart.");
                RestartGame();
            }
        }

        private void PopBalloon(object sender, MouseButtonEventArgs e)
        {
            if (gameIsActive)
            {
                if (e.OriginalSource is Rectangle)
                {
                    Rectangle activeRectangle = (Rectangle)e.OriginalSource;
                    soundPlayer.Open(new Uri("../../images/pop_sound.mp3", UriKind.RelativeOrAbsolute));
                    soundPlayer.Play();
                    myCanvas.Children.Remove(activeRectangle);
                    score++;
                }
            }
        }

        private void StartGame()
        {
            gameTimer.Start();
            score = 0;
            missedBalloons = 0;
            intervals = 90;
            gameIsActive = true;
        }
        
        private void RestartGame()
        {
            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                itemRemover.Add(x);
            }
            foreach (Rectangle y in itemRemover)
            {
                myCanvas.Children.Remove(y);
            }
            itemRemover.Clear();
            StartGame();
        }


    }
}

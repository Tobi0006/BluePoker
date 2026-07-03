using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BluePoker
{
    public partial class MainWindow : Window
    {
        private int currentIndex = 0;
        private Image[] slides;
        private string[] sources = { "landing1.png", "landing4.png", "landing3.png" };
        private string[] titles = {
            "PREMIUM POKER EXPERIENCE",
            "HIGH-STAKES ACTION",
            "JOIN THE ELITE"
        };
        private string[] subtitles = {
            "Play with the world's best poker legends",
            "Feel the thrill of every hand, every raise, every win",
            "Sign up today and claim your seat at the table"
        };

        public MainWindow()
        {
            InitializeComponent();
            slides = new Image[] { Slide1, Slide2, Slide3 };
            Loaded += (s, e) =>
            {
                ApplySizes();
                StartCarousel();
            };
        }

        private void TitleBar_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CarouselBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplySizes();
        }

        private void ApplySizes()
        {
            double w = CarouselBorder.ActualWidth;
            double h = CarouselBorder.ActualHeight;
            if (w <= 0 || h <= 0) return;

            foreach (var img in slides)
            {
                img.Width = w;
                img.Height = h;
                Canvas.SetLeft(img, 0);
                Canvas.SetTop(img, 0);
            }
        }

        private void StartCarousel()
        {
            UpdateImageSource(slides[0], sources[0]);
            currentIndex = 0;
            UpdateDots(0);

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
            timer.Tick += (s, e) => NextSlide();
            timer.Start();
        }

        private void NextSlide()
        {
            int nextIndex = (currentIndex + 1) % sources.Length;

            double w = CarouselCanvas.ActualWidth;
            if (w <= 0) w = CarouselBorder.ActualWidth;
            if (w <= 0) w = 800;

            var nextSlide = slides[nextIndex];
            var currSlide = slides[currentIndex];

            UpdateImageSource(nextSlide, sources[nextIndex]);
            nextSlide.Visibility = Visibility.Visible;
            Canvas.SetLeft(nextSlide, w);

            var animOut = new DoubleAnimation(0, -w, TimeSpan.FromSeconds(0.8));
            animOut.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut };

            var animIn = new DoubleAnimation(w, 0, TimeSpan.FromSeconds(0.8));
            animIn.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut };

            animOut.Completed += (a, e) =>
            {
                currSlide.Visibility = Visibility.Collapsed;
                Canvas.SetLeft(currSlide, 0);
            };

            currSlide.BeginAnimation(Canvas.LeftProperty, animOut);
            nextSlide.BeginAnimation(Canvas.LeftProperty, animIn);

            currentIndex = nextIndex;
            UpdateDots(currentIndex);
            UpdateTitles(currentIndex);
        }

        private void UpdateImageSource(Image img, string src)
        {
            img.Source = new BitmapImage(new Uri(src, UriKind.Relative));
        }

        private void UpdateDots(int index)
        {
            Dot1.Fill = index == 0 ? (Brush)FindResource("ElectricBrush") : Brushes.White;
            Dot2.Fill = index == 1 ? (Brush)FindResource("ElectricBrush") : Brushes.White;
            Dot3.Fill = index == 2 ? (Brush)FindResource("ElectricBrush") : Brushes.White;
        }

        private void UpdateTitles(int index)
        {
            SlideTitle.Text = titles[index];
            SlideSubtitle.Text = subtitles[index];
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailInput.Text.Trim();
            string password = PasswordInput.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                LoginError.Text = "Please fill in all fields.";
                LoginError.Visibility = Visibility.Visible;
                return;
            }

            // Hide previous error
            LoginError.Visibility = Visibility.Collapsed;

            // Show loading state
            SubmitButton.IsEnabled = false;
            SubmitButton.Content = "LOADING...";

            // Random delay between 1.0 and 2.0 seconds
            var rng = new Random();
            int delayMs = rng.Next(1000, 2001);
            await Task.Delay(delayMs);

            // Show error
            LoginError.Text = "Something went wrong";
            LoginError.Visibility = Visibility.Visible;

            // Restore button
            SubmitButton.Content = "LOGIN";
            SubmitButton.IsEnabled = true;
        }
    }
}
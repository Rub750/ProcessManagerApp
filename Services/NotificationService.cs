using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ProcessManagerApp.Services
{
    public class NotificationService
    {
        private readonly Window _ownerWindow;
        private readonly Dispatcher _dispatcher;

        public NotificationService(Window ownerWindow)
        {
            _ownerWindow = ownerWindow ?? throw new ArgumentNullException(nameof(ownerWindow));
            _dispatcher = _ownerWindow.Dispatcher;
        }

        public void ShowNotification(string message, NotificationType type = NotificationType.Info, int durationMs = 3000)
        {
            _dispatcher.Invoke(() =>
            {
                try
                {
                    var notification = new Grid
                    {
                        Background = GetBackgroundBrush(type),
                        CornerRadius = new CornerRadius(5),
                        Margin = new Thickness(10),
                        Padding = new Thickness(15, 10, 15, 10),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top
                    };

                    var textBlock = new TextBlock
                    {
                        Text = message,
                        Foreground = GetForegroundBrush(type),
                        FontSize = 14,
                        TextWrapping = TextWrapping.Wrap,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    notification.Children.Add(textBlock);

                    // Add to owner window
                    var container = new Grid
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };

                    container.Children.Add(notification);
                    _ownerWindow.Content = container;

                    // Remove after duration
                    var timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromMilliseconds(durationMs)
                    };
                    timer.Tick += (s, e) =>
                    {
                        timer.Stop();
                        _dispatcher.Invoke(() =>
                        {
                            try
                            {
                                container.Children.Remove(notification);
                            }
                            catch { }
                        });
                    };
                    timer.Start();
                }
                catch { }
            });
        }

        private Brush GetBackgroundBrush(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => new SolidColorBrush(Colors.LightGreen),
                NotificationType.Warning => new SolidColorBrush(Colors.Gold),
                NotificationType.Error => new SolidColorBrush(Colors.LightCoral),
                _ => new SolidColorBrush(Colors.SteelBlue)
            };
        }

        private Brush GetForegroundBrush(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => Brushes.DarkGreen,
                NotificationType.Warning => Brushes.DarkGoldenrod,
                NotificationType.Error => Brushes.DarkRed,
                _ => Brushes.White
            };
        }
    }

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }
}

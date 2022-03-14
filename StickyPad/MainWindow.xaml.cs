using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using WindowPlacementHelper;
using static StickyPad.Properties.Settings;
using static StickyPad.WindowBackground;

namespace StickyPad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static Storyboard storyboard;

        static bool Tab1Selected = false;
        static bool SettingsOpen = false;

        #region Main Entry Point

        public MainWindow()
        {
            SingleInstance.Check();

            if (Default.UpgradeRequired)
            {
                Default.Upgrade();
                Default.UpgradeRequired = false;
                Default.Save();
            }

            InitializeComponent();

            LoadSettings();

            OpacitySlider.ValueChanged += OpacitySlider_ValueChanged;
        }

        #endregion Main Entry Point

        #region Render Window

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            WindowPlacement.SetWindowByResolution(window, false);

            storyboard = FindResource("ToolbarFadeOutAni") as Storyboard;
            storyboard.Begin();

            EnableBlurEffect(window);

            SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            WindowPlacement.SetWindowByResolution(window, false);
        }

        #endregion Render Window

        #region App Settings

        private void LoadSettings()
        {
            if (Default.UseAccentWindow)
            {
                WindowsAccentCheckBox.IsChecked = true;
            }
            if (Default.UseAccentBorder)
            {
                BorderAccentCheckBox.IsChecked = true;
            }

            SetWindowColor(Default.UseAccentWindow);

            SetBorderColor(Default.UseAccentBorder);

            SetToolBarColor();

            NoteTab1.FontSize = Default.TabFontSize;
            NoteTab2.FontSize = Default.TabFontSize;

            foreach (Control c in FontSizePanel.Children)
            {
                if (c.GetType() == typeof(Button) && c.Name.Contains(Default.TabFontSize.ToString()))
                {
                    c.Background = Brushes.Red;
                }
            }

            SolidColorBrush ForeColorBrush = new SolidColorBrush(Default.ForeColor);
            window.Foreground = ForeColorBrush;
            NoteTab1.Foreground = ForeColorBrush;
            NoteTab2.Foreground = ForeColorBrush;

            SolidColorBrush CaretBrush = new SolidColorBrush(Default.TextCaret);
            NoteTab1.CaretBrush = CaretBrush;
            NoteTab2.CaretBrush = CaretBrush;

            OpacitySlider.Value = Default.WindowOpacity / 0.1;

            if (Default.NoteTab1Contents.Length > 0)
            {
                NoteTab1.Text = Default.NoteTab1Contents;
            }
            if (Default.NoteTab2Contents.Length > 0)
            {
                NoteTab2.Text = Default.NoteTab2Contents;
            }

            if (Default.Tab1IsActive)
            {
                Grid2.Children.Remove(NoteTab2);
                TabButtonImage.ToolTip = "Tab 1 Selected";
            }
            else
            {
                Grid2.Children.Remove(NoteTab1);
                TabButtonImage.ToolTip = "Tab 2 Selected";
            }

            string ColorSet = Default.BackColor.ToString();
            if (ColorSet == "#FFFFF2AB" || ColorSet == "#FFF3F3F3")
            {
                DarkForeground();
            }
            else
            {
                LightForeground();
            }

            Grid2.Children.Remove(SettingsPanel);

            SystemParameters.StaticPropertyChanged += WindowGlassBrush_Changed;

            WindowsAccentCheckBox.Checked += WindowsAccentCheckBox_Checked;
            WindowsAccentCheckBox.Unchecked += WindowsAccentCheckBox_Unchecked;

            BorderAccentCheckBox.Checked += BorderAccentCheckBox_Checked;
            BorderAccentCheckBox.Unchecked += BorderAccentCheckBox_Unchecked;

            MouseUp += MainWindow_MouseUp;

            Left = Default.LocationX;
            Top = Default.LocationY;
            Width = Default.WindowWidth;
            Height = Default.WindowHeight;
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            WindowPlacement.SetWindowByResolution(window, true);
        }

        private void SaveSettings_Close()
        {
            SystemParameters.StaticPropertyChanged -= WindowGlassBrush_Changed;

            WindowsAccentCheckBox.Checked -= WindowsAccentCheckBox_Checked;
            WindowsAccentCheckBox.Unchecked -= WindowsAccentCheckBox_Unchecked;

            BorderAccentCheckBox.Checked -= BorderAccentCheckBox_Checked;
            BorderAccentCheckBox.Unchecked -= BorderAccentCheckBox_Unchecked;

            OpacitySlider.ValueChanged -= OpacitySlider_ValueChanged;

            SaveSettings();

            Environment.Exit(0);
        }

        private void SaveSettings()
        {
            Default.NoteTab1Contents = NoteTab1.Text;

            Default.NoteTab2Contents = NoteTab2.Text;

            WindowPlacement.SetWindowByResolution(window, true);

            if (WindowsAccentCheckBox.IsChecked == true)
            {
                Default.UseAccentWindow = true;
            }
            else
            {
                Default.UseAccentWindow = false;

                Default.BackColor = ((SolidColorBrush)this.window.Background).Color;
            }

            Default.ForeColor = ((SolidColorBrush)NoteTab1.Foreground).Color;

            Default.TextCaret = ((SolidColorBrush)NoteTab1.CaretBrush).Color;

            Default.WindowOpacity = this.window.Background.Opacity;

            Default.TabFontSize = NoteTab1.FontSize;

            if (BorderAccentCheckBox.IsChecked == true)
            {
                Default.UseAccentBorder = true;
            }
            else
            {
                Default.UseAccentBorder = false;
            }

            if (Grid2.Children.Contains(NoteTab1))
            {
                Default.Tab1IsActive = true;
            }
            else
            {
                Default.Tab1IsActive = false;
            }

            Default.LocationX = Left;
            Default.LocationY = Top;
            Default.WindowWidth = Width;
            Default.WindowHeight = Height;
            Default.Save();

            Default.Save();
        }

        #endregion App Settings

        #region Events

        private void WindowsAccentCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetWindowColor(true);

            Default.UseAccentWindow = true;
        }

        private void WindowsAccentCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetWindowColor(false);

            Default.UseAccentWindow = false;
        }

        private void BorderAccentCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SetBorderColor(true);

            Default.UseAccentBorder = true;
        }

        private void BorderAccentCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SetBorderColor(false);

            Default.UseAccentBorder = false;
        }

        private void WindowGlassBrush_Changed(object sender, EventArgs e)
        {
            if (WindowsAccentCheckBox.IsChecked == true)
            {
                SetWindowColor(true);
            }
            else
            {
                SetWindowColor(false);
            }

            if (BorderAccentCheckBox.IsChecked == true)
            {
                SetBorderColor(true);
            }
            else
            {
                SetBorderColor(false);
            }
        }

        private void ToolBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
            e.Handled = true;
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            SaveSettings_Close();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            storyboard = FindResource("ToolbarFadeOutAni") as Storyboard;
            storyboard.Begin();

            if (Default.NoteTab1Contents != NoteTab1.Text || Default.NoteTab2Contents != NoteTab2.Text)
            {
                SaveSettings();
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            storyboard = FindResource("ToolbarFadeInAni") as Storyboard;
            storyboard.Begin();

            if (IsActive)
            {
                if (Grid2.Children.Contains(NoteTab1))
                {
                    NoteTab1.Focus();
                }
                else
                {
                    NoteTab2.Focus();
                }
            }
        }

        #endregion Events

        #region Toolbar

        private void SetToolBarColor()
        {
            var darkBrush = ColorHelper.ColorShade(Default.BackColor, false);

            ToolBar.Background = darkBrush;
            ToolBar.Background.Opacity = 0.2;

            SettingsPanel.Background = darkBrush;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsOpen)
            {
                SetDockPanel();
            }

            SaveSettings_Close();
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            SetDockPanel();
        }

        private void TabButton_Click(object sender, RoutedEventArgs e)
        {
            if (Grid2.Children.Contains(NoteTab1))
            {
                Grid2.Children.Add(NoteTab2);
                Grid2.Children.Remove(NoteTab1);

                NoteTab2.Focus();
                TabButtonImage.ToolTip = "Tab 2 Selected";
            }
            else
            {
                Grid2.Children.Add(NoteTab1);
                Grid2.Children.Remove(NoteTab2);

                NoteTab1.Focus();
                TabButtonImage.ToolTip = "Tab 1 Selected";
            }
        }

        #endregion Toolbar

        #region Settings Panel

        private void SetDockPanel()
        {
            if (Grid2.Children.Contains(NoteTab1))
            {
                Grid2.Children.Remove(NoteTab1);
                Tab1Selected = true;
            }
            else if (Tab1Selected)
            {
                Grid2.Children.Add(NoteTab1);
            }
            else if (Grid2.Children.Contains(NoteTab2))
            {
                Grid2.Children.Remove(NoteTab2);
                Tab1Selected = false;
            }
            else if (!Tab1Selected)
            {
                Grid2.Children.Add(NoteTab2);
            }

            if (Grid2.Children.Contains(SettingsPanel))
            {
                TabButton.IsEnabled = true;
                Grid2.Children.Remove(SettingsPanel);
                SettingsOpen = false;
            }
            else
            {
                TabButton.IsEnabled = false;
                Grid2.Children.Add(SettingsPanel);
                SettingsOpen = true;
            }
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            window.Background.Opacity = OpacitySlider.Value * 0.1;

            Default.WindowOpacity = window.Background.Opacity;
        }

        private void SetDarkMode(Button button)
        {
            LightForeground();

            Default.BackColor = ((SolidColorBrush)button.Background).Color;

            SolidColorBrush brush = new SolidColorBrush(Default.BackColor);
            window.Background = brush;
            window.Background.Opacity = Default.WindowOpacity;

            SetToolBarColor();

            SolidColorBrush windowColor = new SolidColorBrush(Default.BackColor);

            foreach (Control c in BackColorBar.Children)
            {
                if (c.GetType() == typeof(Button))
                {
                    if (c.Background.ToString() == windowColor.ToString())
                    {
                        c.Width = 32;
                        c.Height = 32;
                    }
                    else
                    {
                        c.Width = 24;
                        c.Height = 24;
                    }
                }
            }
        }

        private void SetLightMode(System.Windows.Controls.Button button)
        {
            DarkForeground();

            Default.BackColor = ((SolidColorBrush)button.Background).Color;

            SolidColorBrush brush = new SolidColorBrush(Default.BackColor);
            window.Background = brush;
            window.Background.Opacity = Default.WindowOpacity;

            SetToolBarColor();
        }

        private void RedBackButton_Click(object sender, RoutedEventArgs e)
        {
            SetDarkMode(RedBackButton);
        }

        private void BlueBackButton_Click(object sender, RoutedEventArgs e)
        {
            SetDarkMode(BlueBackButton);
        }

        private void GreenBackButton_Click(object sender, RoutedEventArgs e)
        {
            SetDarkMode(GreenBackButton);
        }

        private void YellowBackButton_Click(object sender, RoutedEventArgs e)
        {
            SetLightMode(YellowBackButton);
        }

        private void BlackBackButton_Click(object sender, RoutedEventArgs e)
        {
            SetDarkMode(BlackBackButton);
        }

        private void GrayBackButton_Click(object sender, RoutedEventArgs e)
        {
            SetDarkMode(GrayBackButton);
        }

        private void WhiteBackButton_Click(object sender, RoutedEventArgs e)
        {
            SetLightMode(WhiteBackButton);
        }

        private void SetFontProperties(Button button, double size)
        {
            foreach (Control c in FontSizePanel.Children)
            {
                if (c.GetType() == typeof(Button))
                {
                    c.Background = Brushes.Transparent;
                }
            }

            button.Background = Brushes.Red;

            NoteTab1.FontSize = size;
            NoteTab2.FontSize = size;
        }

        private void FontSizeButton12_Click(object sender, RoutedEventArgs e)
        {
            SetFontProperties(FontSizeButton12, 12);
        }

        private void FontSizeButton13_Click(object sender, RoutedEventArgs e)
        {
            SetFontProperties(FontSizeButton13, 13);
        }

        private void FontSizeButton14_Click(object sender, RoutedEventArgs e)
        {
            SetFontProperties(FontSizeButton14, 14);
        }

        private void FontSizeButton15_Click(object sender, RoutedEventArgs e)
        {
            SetFontProperties(FontSizeButton15, 15);
        }

        private void FontSizeButton16_Click(object sender, RoutedEventArgs e)
        {
            SetFontProperties(FontSizeButton16, 16);
        }

        private void LightForeground(bool loading = false)
        {
            if (!loading)
            {
                Default.ForeColor = Color.FromRgb(245, 245, 245);

                SolidColorBrush brush = new SolidColorBrush(Default.ForeColor);

                SetForeGround(brush);
            }

            CloseButtonImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/CLOSE.png", UriKind.Absolute));

            OptionsButtonImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/OPTIONS.png", UriKind.Absolute));

            TabButtonImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/NEW_TAB.png", UriKind.Absolute));
        }

        private void DarkForeground(bool loading = false)
        {
            if (!loading)
            {
                Default.ForeColor = Color.FromRgb(45, 45, 45);

                SolidColorBrush brush = new SolidColorBrush(Default.ForeColor);

                SetForeGround(brush);
            }

            CloseButtonImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/CLOSE - Dark.png", UriKind.Absolute));

            OptionsButtonImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/OPTIONS - Dark.png", UriKind.Absolute));

            TabButtonImage.Source = new BitmapImage(new Uri(@"pack://application:,,,/Resources/NEW_TAB - Dark.png", UriKind.Absolute));
        }

        private void SetForeGround(SolidColorBrush brush)
        {
            window.Foreground = brush;
            NoteTab1.Foreground = brush;
            NoteTab2.Foreground = brush;
            NoteTab1.CaretBrush = brush;
            NoteTab2.CaretBrush = brush;

            // Settings Controls
            OpacityLabel.Foreground = brush;
            BackgroundLabel.Foreground = brush;
            FontSizeLabel.Foreground = brush;
            WindowsAccentCheckBox.Foreground = brush;
            BorderAccentCheckBox.Foreground = brush;

            FontSizeButton12.Foreground = brush;
            FontSizeButton13.Foreground = brush;
            FontSizeButton14.Foreground = brush;
            FontSizeButton15.Foreground = brush;
            FontSizeButton16.Foreground = brush;
        }

        #endregion Settings Panel

        #region Windows Accent Color

        private void SetWindowColor(bool UseAccentColor)
        {
            SolidColorBrush windowColor = ColorHelper.ColorShade(SystemParameters.WindowGlassColor, true);
            if (UseAccentColor)
            {
                LightForeground();

                SettingsPanel.Children.Remove(BackColorBar);

                WindowsAccentCheckBox.IsChecked = true;

                window.Background = windowColor;
                window.Background.Opacity = Default.WindowOpacity;

                var darkBrush = ColorHelper.ColorShade(SystemParameters.WindowGlassColor, false);

                NoteTab1.SelectionBrush = darkBrush;
                NoteTab2.SelectionBrush = darkBrush;
            }
            else
            {
                try
                {
                    string ColorSet = Default.BackColor.ToString();
                    if (ColorSet == "#FFFFF2AB" || ColorSet == "#FFF3F3F3")
                    {
                        DarkForeground();
                    }
                    else
                    {
                        LightForeground();
                    }

                    if (!SettingsPanel.Children.Contains(BackColorBar))
                    {
                        SettingsPanel.Children.Add(BackColorBar);
                    }

                    windowColor = new SolidColorBrush(Default.BackColor);
                    window.Background = windowColor;

                    var tabColor = ColorHelper.ColorShade(windowColor.Color, true);

                    NoteTab1.SelectionBrush = tabColor;
                    NoteTab2.SelectionBrush = tabColor;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    throw;
                }
            }

            window.Background.Opacity = Default.WindowOpacity;

            foreach (Control c in BackColorBar.Children)
            {
                if (c.GetType() == typeof(Button))
                {
                    if (c.Background.ToString() == windowColor.ToString())
                    {
                        c.Width = 32;
                        c.Height = 32;
                    }
                }
            }
        }

        private void SetBorderColor(bool UseAccentColor)
        {
            if (UseAccentColor)
            {
                window.BorderBrush = SystemParameters.WindowGlassBrush;
            }
            else
            {
                SolidColorBrush borderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                window.BorderBrush = borderBrush;
            }
        }

        #endregion Windows Accent Color
    }
}

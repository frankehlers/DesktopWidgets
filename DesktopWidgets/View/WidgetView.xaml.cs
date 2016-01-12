﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using DesktopWidgets.Classes;
using DesktopWidgets.Helpers;
using DesktopWidgets.ViewModelBase;

namespace DesktopWidgets.View
{
    /// <summary>
    ///     Interaction logic for View.xaml
    /// </summary>
    public partial class WidgetView : Window
    {
        private readonly MouseChecker _mouseChecker;
        public readonly WidgetSettingsBase Settings;
        public readonly UserControl UserControl;
        private DispatcherTimer _introTimer;
        private DispatcherTimer _onTopForceTimer;
        public bool IsRefreshRequired;

        public WidgetView(WidgetId id, WidgetViewModelBase viewModel, UserControl userControl)
        {
            InitializeComponent();
            HideOpacity();
            Visibility = Visibility.Visible;
            Id = id;
            Settings = id.GetSettings();
            ViewModel = viewModel;
            UserControl = userControl;

            userControl.Style = (Style) FindResource("UserControlStyle");
            MainContentContainer.Content = userControl;
            var contextMenu = (ContextMenu)
                (userControl.TryFindResource("WidgetContextMenu") ?? TryFindResource("WidgetContextMenu"));
            contextMenu.DataContext = ViewModel;
            MainContentContainer.ContextMenu = contextMenu;
            userControl.MouseDown += Widget_OnMouseDown;

            MainContentContainer.ScrollToHorizontalOffset(Settings.ScrollHorizontalOffset);
            MainContentContainer.ScrollToVerticalOffset(Settings.ScrollVerticalOffset);

            var frameTop = userControl.TryFindResource("FrameTop") as Grid;
            if (frameTop != null)
            {
                FrameContainerTop.Child = frameTop;
                FrameContainerTop.Visibility = Visibility.Visible;
            }
            var frameBottom = userControl.TryFindResource("FrameBottom") as Grid;
            if (frameBottom != null)
            {
                FrameContainerBottom.Child = frameBottom;
                FrameContainerBottom.Visibility = Visibility.Visible;
            }
            var frameLeft = userControl.TryFindResource("FrameLeft") as Grid;
            if (frameLeft != null)
            {
                FrameContainerLeft.Child = frameLeft;
                FrameContainerLeft.Visibility = Visibility.Visible;
            }
            var frameRight = userControl.TryFindResource("FrameRight") as Grid;
            if (frameRight != null)
            {
                FrameContainerRight.Child = frameRight;
                FrameContainerRight.Visibility = Visibility.Visible;
            }

            _mouseChecker = new MouseChecker(this, Settings);
            UpdateUi(true);
            IsRefreshRequired = true;

            if (!App.Arguments.Contains("-systemstartup"))
                _mouseChecker.QueueIntro = true;

            _mouseChecker.Start();
        }

        public WidgetViewModelBase ViewModel { get; set; }

        public bool IsContextMenuOpen => ViewModel.IsContextMenuOpen;

        public WidgetId Id { get; }
        public bool AnimationRunning { get; set; } = false;

        public new bool IsVisible => !(Opacity < 1);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            var widgetSrc = HwndSource.FromHwnd(hwnd);

            widgetSrc?.AddHook(WndProc);

            if (Settings.Unclickable)
                new Win32App(hwnd).SetWindowExTransparent();
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == SingleInstanceHelper.WM_SHOWAPP)
                ShowIntro();

            return IntPtr.Zero;
        }

        public void ShowIntro(int duration = -1, bool reversable = false, bool activate = false,
            bool hideOnFinish = true,
            Action finishAction = null)
        {
            if (IsRefreshRequired || Settings.OpenMode == OpenMode.AlwaysOpen || !Settings.ShowIntro)
                return;

            if (_introTimer == null)
            {
                _introTimer = new DispatcherTimer();
                _introTimer.Tick += delegate
                {
                    _introTimer.Stop();
                    if (hideOnFinish)
                        _mouseChecker.Hide(checkHideStatus: true);
                    finishAction?.Invoke();
                };
            }

            _introTimer.Stop();
            _introTimer.Interval = TimeSpan.FromMilliseconds(duration == -1 ? Settings.IntroDuration : duration);
            if (_mouseChecker.KeepOpenForIntro && reversable)
            {
                _mouseChecker.KeepOpenForIntro = false;
            }
            else
            {
                _mouseChecker.KeepOpenForIntro = true;
                if (duration != 0)
                {
                    _introTimer.Start();
                    if (activate)
                        Activate();
                }
            }
        }

        public void ShowUI()
        {
            if (Settings.OpenMode != OpenMode.AlwaysOpen)
                _mouseChecker.Show();
        }

        public void HideUI()
        {
            if (Settings.OpenMode != OpenMode.AlwaysOpen)
                _mouseChecker.Hide();
        }

        public void UpdateUi(bool resetContext = true, bool updateNonUi = true, bool? isDocked = null,
            HorizontalAlignment? dockHorizontalAlignment = null,
            VerticalAlignment? dockVerticalAlignment = null)
        {
            if (!IsVisible)
                Refresh(resetContext, updateNonUi, false);
            else
                this.Animate(AnimationMode.Hide, null, () => Refresh(resetContext, updateNonUi, true), isDocked,
                    dockHorizontalAlignment,
                    dockVerticalAlignment);
        }

        private void Refresh(bool resetContext, bool updateNonUi, bool showIntro)
        {
            if (resetContext)
            {
                //UpdateLayout();
                DataContext = null;
                //UpdateLayout();
                DataContext = ViewModel;
            }
            UpdateLayout();
            ViewModel.UpdateSize();
            UpdateLayout();
            ViewModel.UpdatePosition();
            UpdateLayout();
            ViewModel.UpdatePosition();
            UpdateLayout();
            if (updateNonUi)
            {
                UpdateTimers();
                ViewModel.ReloadHotKeys();
            }
            IsRefreshRequired = false;
            if (showIntro && !_mouseChecker.KeepOpenForIntro)
                ShowIntro();
        }

        private void UpdateTimers()
        {
            _mouseChecker.UpdateIntervals();
            if (_mouseChecker.IsRunning)
            {
                _mouseChecker.Stop();
                _mouseChecker.Start();
            }
            if (Settings.ForceOnTop && Settings.ForceOnTopInterval > 0)
            {
                if (_onTopForceTimer == null)
                {
                    _onTopForceTimer = new DispatcherTimer();
                    _onTopForceTimer.Tick += delegate
                    {
                        Settings.OnTop = false;
                        Settings.OnTop = true;
                    };
                }
                _onTopForceTimer.Interval = TimeSpan.FromMilliseconds(Settings.ForceOnTopInterval);
                if (_onTopForceTimer.IsEnabled)
                {
                    _onTopForceTimer.Stop();
                    _onTopForceTimer.Start();
                }
            }
        }

        private void WidgetView_OnClosing(object sender, CancelEventArgs e)
        {
            _mouseChecker.Stop();
            HotkeyStore.RemoveHotkey(Id.Guid);
            ViewModel = null;
            Settings.ScrollHorizontalOffset = MainContentContainer.ContentHorizontalOffset;
            Settings.ScrollVerticalOffset = MainContentContainer.ContentVerticalOffset;
        }

        private void WidgetView_OnLocationChanged(object sender, EventArgs e)
        {
            if (Settings.SnapToScreenEdges && !Settings.IsDocked)
                this.Snap(true);
        }

        private void Widget_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && !Settings.IsDocked &&
                Settings.DragToMove)
                DragMove();
        }

        private void Titlebar_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && !Settings.IsDocked &&
                Settings.DragTitlebarToMove)
                DragMove();
        }

        public void ShowOpacity()
        {
            Opacity = 1;
        }

        public void HideOpacity()
        {
            Opacity = 0;
        }

        private void btnMenu_OnClick(object sender, RoutedEventArgs e)
        {
            MainContentContainer.ContextMenu.IsOpen = true;
        }

        private void btnReload_OnClick(object sender, RoutedEventArgs e)
        {
            Id.ToggleEnable();
            Id.ToggleEnable();
        }
    }
}
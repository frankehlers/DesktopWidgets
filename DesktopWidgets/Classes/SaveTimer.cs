﻿using DesktopWidgets.Helpers;
using System;
using System.Threading;
using System.Windows.Threading;

namespace DesktopWidgets.Classes
{
    public class SaveTimer
    {
        private readonly DispatcherTimer _autoSaveTimer;
        private readonly DispatcherTimer _timer;

        public SaveTimer(TimeSpan waitTime, TimeSpan autoSaveInterval)
        {
            _timer = new DispatcherTimer { Interval = waitTime };
            _timer.Tick += Timer_OnTick;

            if (autoSaveInterval.TotalSeconds > 0)
            {
                _autoSaveTimer = new DispatcherTimer { Interval = autoSaveInterval };
                _autoSaveTimer.Tick += Timer_OnTick;
                _autoSaveTimer.Start();
            }
        }

        private void Timer_OnTick(object sender, EventArgs eventArgs)
        {
            ThreadPool.QueueUserWorkItem(delegate { SettingsHelper.SaveSettings(); }, null);
        }

        public void DelaySave()
        {
            _timer.Stop();
            _timer.Start();
        }

        public void Stop()
        {
            _timer?.Stop();
            _autoSaveTimer?.Stop();
        }
    }
}
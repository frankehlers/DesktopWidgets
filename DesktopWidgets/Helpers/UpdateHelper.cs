﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using DesktopWidgets.Classes;
using DesktopWidgets.Properties;
using DesktopWidgets.Windows;

namespace DesktopWidgets.Helpers
{
    internal class UpdateHelper
    {
        public static bool IsUpdateable => ApplicationDeployment.IsNetworkDeployed;
        private static Version ForgetUpdateVersion => Settings.Default.ForgetUpdateVersion ?? new Version(0, 0, 0, 0);
        public static bool IsUpdateDay => Settings.Default.UpdateDays.Contains(DateTime.Today.DayOfWeek.ToString());

        public static void CheckForUpdatesAsync(bool auto)
        {
            if ((auto && !IsUpdateDay) || FullScreenHelper.DoesAnyMonitorHaveFullscreenApp())
                return;

            UpdateCheckInfo updateInfo = null;
            var bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                    updateInfo = ApplicationDeployment.CurrentDeployment.CheckForDetailedUpdate(false);
            };
            bw.RunWorkerCompleted += (sender, args) => CheckForUpdates(updateInfo, auto);
            bw.RunWorkerAsync();
        }

        private static void CheckForUpdates(UpdateCheckInfo info, bool auto)
        {
            try
            {
                if (!ApplicationDeployment.IsNetworkDeployed)
                {
                    if (!auto)
                        Popup.Show(
                            "This application was not installed via ClickOnce and cannot be updated automatically.");
                    return;
                }

                if (info == null)
                {
                    if (!auto)
                        Popup.Show(
                            "An error occurred while trying to check for updates.");
                    return;
                }

                Settings.Default.LastUpdateCheck = DateTime.Now;
                if (info.UpdateAvailable && !(Settings.Default.UpdateBranch != UpdateBranch.Beta &&
                                              info.AvailableVersion.Minor == Settings.Default.BetaVersion) &&
                    !(AssemblyInfo.Version.Major != ForgetUpdateVersion.Major &&
                      ForgetUpdateVersion.Major == info.AvailableVersion.Major))
                {
                    var ad = ApplicationDeployment.CurrentDeployment;
                    ad.UpdateCompleted += delegate
                    {
                        var args = new List<string>();
                        if (auto && Settings.Default.AutoUpdate)
                            args.Add("updatingsilent");
                        else
                            args.Add("updating");
                        AppHelper.RestartApplication(args);
                    };
                    if (auto && Settings.Default.AutoUpdate)
                    {
                        try
                        {
                            ad.UpdateAsync();
                        }
                        catch
                        {
                            // ignored
                        }
                        return;
                    }
                    if (auto && info.AvailableVersion == ForgetUpdateVersion)
                        return;
                    App.UpdateScheduler?.Stop();

                    var updateDialog = new UpdatePrompt(info.AvailableVersion, info.IsUpdateRequired);
                    updateDialog.ShowDialog();

                    switch (updateDialog.SelectedUpdateMode)
                    {
                        case UpdatePrompt.UpdateMode.RemindNever:
                            Settings.Default.ForgetUpdateVersion = info.AvailableVersion;
                            break;
                        case UpdatePrompt.UpdateMode.UpdateNow:
                            try
                            {
                                ad.UpdateAsync();
                            }
                            catch (DeploymentDownloadException dde)
                            {
                                Popup.Show(
                                    "Cannot download the latest version of this application.\n\nPlease check your network connection, or try again later.\n\nError: " +
                                    dde);
                            }
                            break;
                    }

                    App.UpdateScheduler?.Start();
                }
                else
                {
                    if (!auto)
                        Popup.Show(
                            $"You have the latest version. ({AssemblyInfo.Version})");
                }
            }
            catch (Exception)
            {
                if (!auto)
                    throw;
            }
        }
    }
}
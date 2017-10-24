﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using DesktopWidgets.Classes;
using DesktopWidgets.Properties;
using DesktopWidgets.WidgetBase;
using DesktopWidgets.WidgetBase.Settings;
using DesktopWidgets.Windows;
using Newtonsoft.Json;

namespace DesktopWidgets.Helpers
{
    public static class SettingsHelper
    {
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public static readonly JsonSerializerSettings JsonSerializerSettingsAllTypeHandling = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            MissingMemberHandling = MissingMemberHandling.Ignore
        };

        public static object CloneObject(object obj)
        {
            return
                JsonConvert.DeserializeObject(
                    JsonConvert.SerializeObject(obj, JsonSerializerSettingsAllTypeHandling),
                    JsonSerializerSettingsAllTypeHandling);
        }

        public static void UpgradeSettings()
        {
            // Upgrade settings from old version.
            if (Settings.Default.MustUpgrade)
            {
                Settings.Default.Upgrade();
                Settings.Default.MustUpgrade = false;
                Settings.Default.Save();
            }
        }

        private static void LoadWidgetsDataFromSettings()
        {
            App.WidgetsSettingsStore =
                JsonConvert.DeserializeObject<WidgetsSettingsStore>(Settings.Default.Widgets, JsonSerializerSettings) ??
                new WidgetsSettingsStore
                {
                    Widgets = new ObservableCollection<WidgetSettingsBase>()
                };
            App.WidgetsSettingsStore.Widgets.CollectionChanged += (sender, args) => App.SaveTimer.DelaySave();
            App.WidgetsSettingsStore.EventActionPairs.CollectionChanged += (sender, args) => App.SaveTimer.DelaySave();
        }

        private static void SaveWidgetsDataToSettings()
        {
            Settings.Default.Widgets = JsonConvert.SerializeObject(App.WidgetsSettingsStore, JsonSerializerSettings);
        }

        public static void LoadSettings()
        {
            LoadWidgetsDataFromSettings();
        }

        public static void SaveSettings()
        {
            SaveWidgetsDataToSettings();

            Settings.Default.Save();

            if (Settings.Default.BackupInterval.TotalSeconds > 0 && DateTime.Now - Settings.Default.BackupInterval > Settings.Default.LastBackupDateTime)
            {
                Backup();
            }
        }

        public static void ResetSettings(bool msg = true, bool refresh = true)
        {
            if (msg && Popup.Show(
                "Are you sure you want to reset ALL settings (including widgets)?\n\nThis cannot be undone.",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
            {
                return;
            }
            Settings.Default.Reset();
            Settings.Default.MustUpgrade = false;
            Settings.Default.Widgets = string.Empty;
            LoadWidgetsDataFromSettings();
            if (refresh)
            {
                WidgetHelper.LoadWidgetViews();
            }
            if (msg)
            {
                Popup.Show("All settings have been restored to default.");
            }
        }

        public static void ImportData()
        {
            var dialog = new InputBox("Import Widgets");
            dialog.ShowDialog();
            if (dialog.Cancelled == false &&
                Popup.Show(
                    "Are you sure you want to overwrite ALL widgets, events, and actions?",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                try
                {
                    // Test import data before overwriting existing data.
                    foreach (
                        var id in
                            JsonConvert.DeserializeObject<WidgetsSettingsStore>(dialog.InputData, JsonSerializerSettings)
                                .Widgets.Select(x => x.Identifier.Guid))
                    {
                    }
                }
                catch
                {
                    Popup.Show(
                        "Import failed.\nData may be corrupt.\n\nNo changes have been made.",
                        image: MessageBoxImage.Error);
                    return;
                }
                Settings.Default.Widgets = dialog.InputData;
                Settings.Default.Save();
                LoadWidgetsDataFromSettings();
                WidgetHelper.LoadWidgetViews();
            }
        }

        public static string GetExportedData()
        {
            return JsonConvert.SerializeObject(App.WidgetsSettingsStore, JsonSerializerSettings);
        }

        public static void ExportData()
        {
            var dialog = new InputBox("Export Widgets", GetExportedData());
            dialog.ShowDialog();
        }

        private static void Backup()
        {
            Settings.Default.LastBackupDateTime = DateTime.Now;
            var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Resources.AppName);
            var filename = $"backup-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.json";
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, filename), GetExportedData());
        }
    }
}
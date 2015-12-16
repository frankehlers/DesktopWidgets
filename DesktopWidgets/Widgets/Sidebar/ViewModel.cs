﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DesktopWidgets.Classes;
using DesktopWidgets.Helpers;
using DesktopWidgets.ViewModelBase;
using GalaSoft.MvvmLight.Command;
using NHotkey;
using NHotkey.Wpf;
using DataFormats = System.Windows.Forms.DataFormats;

namespace DesktopWidgets.Widgets.Sidebar
{
    public class ViewModel : WidgetViewModelBase
    {
        public ViewModel(WidgetId id) : base(id)
        {
            Settings = id.GetSettings() as Settings;
            if (Settings == null)
                return;
            IconCache = new Dictionary<string, ImageSource>();

            Drop = new RelayCommand<DragEventArgs>(DropExecute);
            Refresh = new RelayCommand(RefreshExecute);
            NewShortcut = new RelayCommand(NewShortcutExecute);
            ManageShortcut = new RelayCommand(ManageShortcutsExecute);

            ShortcutFocus = new RelayCommand<Shortcut>(ShortcutFocusExecute);

            ShortcutEdit = new RelayCommand(ShortcutEditExecute);
            ShortcutMoveUp = new RelayCommand(ShortcutMoveUpExecute);
            ShortcutMoveDown = new RelayCommand(ShortcutMoveDownExecute);
            ShortcutRemove = new RelayCommand(ShortcutRemoveExecute);
            ShortcutOpenFolder = new RelayCommand(ShortcutOpenFolderExecute);
            ShortcutExecute = new RelayCommand<Shortcut>(ShortcutExecuteExecute);

            if (Settings.DefaultShortcutsMode != DefaultShortcutsMode.DontChange)
            {
                Settings.Shortcuts =
                    new ObservableCollection<Shortcut>(ShortcutHelper.GetDefaultShortcuts(Settings.DefaultShortcutsMode));
                Settings.DefaultShortcutsMode = DefaultShortcutsMode.DontChange;
            }
            ReloadShortcutHotkeys();
        }

        public override void UpdateUi()
        {
            base.UpdateUi();
            ReloadShortcutHotkeys();
        }

        private void ReloadShortcutHotkeys()
        {
            if (Settings?.Shortcuts == null)
                return;
            foreach (var shortcut in Settings.Shortcuts.Where(x => x.HotKey != Key.None))
                ReloadShortcutHotKey(shortcut);
        }

        public void ReloadShortcutHotKey(Shortcut shortcut)
        {
            HotkeyManager.Current.AddOrReplace($"Shortcut\\{shortcut.Guid}\\Execute", shortcut.HotKey,
                shortcut.HotKeyModifiers, OnShortcutHotKey);
        }

        private void OnShortcutHotKey(object sender, HotkeyEventArgs e)
        {
            var nameSplit = e.Name.Split('\\');
            if (nameSplit.Length >= 1 && nameSplit[0] == "Shortcut")
            {
                if (nameSplit.Length >= 3 && nameSplit[2] == "Execute")
                {
                    var shortcut = Settings.Shortcuts.FirstOrDefault(x => x.Guid.ToString() == nameSplit[1]);
                    if(shortcut != null)
                        this.Execute(shortcut, false);
                }
            }
            e.Handled = true;
        }

        public Settings Settings { get; }

        public Dictionary<string, ImageSource> IconCache { get; set; }

        public ICommand Refresh { get; set; }
        public ICommand Drop { get; set; }

        public ICommand ShortcutFocus { get; set; }

        public ICommand ShortcutEdit { get; set; }
        public ICommand ShortcutMoveUp { get; set; }
        public ICommand ShortcutMoveDown { get; set; }
        public ICommand ShortcutRemove { get; set; }
        public ICommand ShortcutOpenFolder { get; set; }
        public ICommand NewShortcut { get; set; }
        public ICommand ManageShortcut { get; set; }
        public ICommand ShortcutExecute { get; set; }

        private Shortcut SelectedShortcut { get; set; }

        private void DropExecute(DragEventArgs e)
        {
            if (Settings.AllowDropFiles && e.Data.GetDataPresent(DataFormats.FileDrop))
                this.ProcessFiles((string[]) e.Data.GetData(DataFormats.FileDrop));
        }

        private void ShortcutFocusExecute(Shortcut shortcut)
        {
            SelectedShortcut = shortcut;
        }

        private void ShortcutExecuteExecute(Shortcut shortcut)
        {
            ShortcutFocusExecute(shortcut);
            this.Execute(SelectedShortcut, !Keyboard.Modifiers.HasFlag(ModifierKeys.Shift));
        }

        private void ShortcutEditExecute()
        {
            this.OpenProperties(SelectedShortcut);
        }

        private void ShortcutMoveUpExecute()
        {
            this.MoveUp(SelectedShortcut);
        }

        private void ShortcutMoveDownExecute()
        {
            this.MoveDown(SelectedShortcut);
        }

        private void ShortcutRemoveExecute()
        {
            this.Remove(SelectedShortcut, true);
        }

        private void ShortcutOpenFolderExecute()
        {
            SelectedShortcut.OpenFolder();
        }

        private void NewShortcutExecute()
        {
            this.New();
        }

        private void ManageShortcutsExecute()
        {
            var dialog = new ManageShortcuts(this);
            dialog.ShowDialog();
        }

        private void RefreshExecute()
        {
            this.ForceRefresh();
        }
    }
}
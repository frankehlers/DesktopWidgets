﻿using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using DesktopWidgets.Classes;

namespace DesktopWidgets.Actions
{
    internal class WriteFileAction : ActionBase
    {
        public FilePath FilePath { get; set; } = new FilePath();

        [DisplayName("Write Mode")]
        public FileWriteMode FileWriteMode { get; set; } = FileWriteMode.Append;

        [DisplayName("Write Text")]
        public string WriteText { get; set; } = "";

        [DisplayName("Use Clipboard Content")]
        public bool UseClipboard { get; set; } = false;

        [DisplayName("New Line Mode")]
        public InsertMode NewLineMode { get; set; } = InsertMode.None;

        protected override void ExecuteAction()
        {
            base.ExecuteAction();
            var writePath = FilePath.Path;
            if (string.IsNullOrWhiteSpace(writePath) || !File.Exists(writePath))
            {
                Popup.Show($"File \"{writePath}\" does not exist.", image: MessageBoxImage.Error);
                return;
            }
            var writeText = UseClipboard ? Clipboard.GetText() : WriteText;
            switch (NewLineMode)
            {
                case InsertMode.Prefix:
                    writeText = Environment.NewLine + writeText;
                    break;
                case InsertMode.Suffix:
                    writeText = writeText + Environment.NewLine;
                    break;
            }
            switch (FileWriteMode)
            {
                case FileWriteMode.Overwrite:
                    File.WriteAllText(writePath, writeText);
                    break;
                case FileWriteMode.Append:
                    File.AppendAllText(writePath, writeText);
                    break;
            }
        }
    }
}
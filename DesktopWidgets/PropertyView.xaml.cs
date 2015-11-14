﻿#region

using System.Windows;

#endregion

namespace DesktopWidgets
{
    /// <summary>
    ///     Interaction logic for PropertyView.xaml
    /// </summary>
    public partial class PropertyView : Window
    {
        public PropertyView(object view)
        {
            InitializeComponent();
            PropertyGrid.SelectedObject = view;
        }
    }
}
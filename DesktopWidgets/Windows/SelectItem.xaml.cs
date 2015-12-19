﻿using System.Collections.Generic;
using System.Windows;
using DesktopWidgets.ViewModel;

namespace DesktopWidgets.Windows
{
    /// <summary>
    ///     Interaction logic for SelectItem.xaml
    /// </summary>
    public partial class SelectItem : Window
    {
        private readonly SelectItemViewModel ViewModel;

        public SelectItem(IEnumerable<object> items)
        {
            InitializeComponent();
            ViewModel = new SelectItemViewModel(items);
            DataContext = ViewModel;
        }

        public object SelectedItem => ViewModel.SelectedItem;

        private void btnOK_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
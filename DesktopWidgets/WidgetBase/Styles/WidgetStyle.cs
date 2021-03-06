﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace DesktopWidgets.WidgetBase.Styles
{
    [ExpandableObject]
    [DisplayName("Style")]
    public class WidgetStyle : BorderStyleBase
    {
        public WidgetStyle()
        {
            FontSettings.FontSize = 14;
            Padding = new Thickness(3);
            TextColor = Colors.Black;
            BackgroundColor = Colors.White;
            BackgroundOpacity = 0.95;
            Width = double.NaN;
            Height = double.NaN;
            CornerRadius = new CornerRadius(5);
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        [Category("Size")]
        [DisplayName("Min Width (px)")]
        public double MinWidth { get; set; } = double.NaN;

        [Category("Size")]
        [DisplayName("Min Height (px)")]
        public double MinHeight { get; set; } = double.NaN;

        [Category("Size")]
        [DisplayName("Max Width (px)")]
        public double MaxWidth { get; set; } = double.NaN;

        [Category("Size")]
        [DisplayName("Max Height (px)")]
        public double MaxHeight { get; set; } = double.NaN;

        [Category("Animation")]
        [DisplayName("Ease")]
        public bool AnimationEase { get; set; } = true;

        [Category("Animation")]
        [DisplayName("Type")]
        public AnimationType AnimationType { get; set; } = AnimationType.Fade;

        [Category("Animation")]
        [DisplayName("Duration (ms)")]
        public int AnimationTime { get; set; } = 100;

        [Category("Background Image")]
        [DisplayName("Path")]
        public string BackgroundImagePath { get; set; }

        [Category("Background Image")]
        [DisplayName("Stretch")]
        public Stretch BackgroundImageStretch { get; set; } = Stretch.Fill;

        [DisplayName("Context Menu Enabled")]
        public bool ShowContextMenu { get; set; } = true;

        [DisplayName("Scrollbar Visibility")]
        public ScrollBarVisibility ScrollBarVisibility { get; set; } = ScrollBarVisibility.Auto;

        [Category("Frame")]
        [DisplayName("Top")]
        public bool ShowTopFrame { get; set; } = true;

        [Category("Frame")]
        [DisplayName("Bottom")]
        public bool ShowBottomFrame { get; set; } = true;

        [Category("Frame")]
        [DisplayName("Left")]
        public bool ShowLeftFrame { get; set; } = true;

        [Category("Frame")]
        [DisplayName("Right")]
        public bool ShowRightFrame { get; set; } = true;

        [Category("Frame")]
        [DisplayName("Padding")]
        public Thickness FramePadding { get; set; } = new Thickness(4);

        [Category("Border")]
        [DisplayName("Visible")]
        public bool BorderEnabled { get; set; } = true;

        [Category("Border")]
        [DisplayName("Color")]
        public Color BorderColor { get; set; } = Colors.Gray;

        [Category("Border")]
        [DisplayName("Opacity")]
        public double BorderOpacity { get; set; } = 0.25;

        [Category("Border")]
        [DisplayName("Thickness")]
        public Thickness BorderThickness { get; set; } = new Thickness(1);
    }
}
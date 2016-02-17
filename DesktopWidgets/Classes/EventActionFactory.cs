﻿using System.Collections.Generic;
using DesktopWidgets.Actions;
using DesktopWidgets.Events;

namespace DesktopWidgets.Classes
{
    public static class EventActionFactory
    {
        public static readonly List<string> AvailableEvents = new List<string>
        {
            "Hotkey",
            "Launch",
            "Foreground Fullscreen Changed",
            "Foreground Title Changed",
            "Widget Mouse Down",
            "Widget Mouse Up",
            "Widget Mouse Double Click",
            "Widget Enable",
            "Widget Disable",
            "Widget Reload",
            "Widget Intro",
            "Widget Intro End",
            "Widget Dismiss",
            "Widget Special Event"
        };

        public static readonly List<string> AvailableActions = new List<string>
        {
            "Open File",
            "Play Sound",
            "Show Popup",
            "Widget Enable",
            "Widget Disable",
            "Widget Reload",
            "Widget Toggle Enabled",
            "Widget Intro",
            "Widget Cancel Intro",
            "Widget Dismiss",
            "Widget Hide"
        };

        public static IEvent GetNewEventFromName(string name)
        {
            switch (name)
            {
                case "Widget Mouse Down":
                    return new WidgetMouseDownEvent();
                case "Widget Mouse Up":
                    return new WidgetMouseUpEvent();
                case "Widget Enable":
                    return new WidgetEnableEvent();
                case "Widget Disable":
                    return new WidgetDisableEvent();
                case "Widget Intro":
                    return new WidgetIntroEvent();
                case "Widget Dismiss":
                    return new WidgetDismissEvent();
                case "Widget Special Event":
                    return new WidgetSpecialEvent();
                case "Widget Intro End":
                    return new WidgetIntroEndEvent();
                case "Widget Mouse Double Click":
                    return new WidgetMouseDoubleClickEvent();
                case "Widget Reload":
                    return new WidgetReloadEvent();
                case "Foreground Fullscreen Changed":
                    return new ForegroundFullscreenChangedEvent();
                case "Foreground Title Changed":
                    return new ForegroundTitleChangedEvent();
                case "Launch":
                    return new LaunchEvent();
                case "Hotkey":
                    return new HotkeyEvent();
            }
            return null;
        }

        public static IAction GetNewActionFromName(string name)
        {
            switch (name)
            {
                case "Open File":
                    return new OpenFileAction();
                case "Play Sound":
                    return new PlaySoundAction();
                case "Widget Enable":
                    return new WidgetEnableAction();
                case "Widget Disable":
                    return new WidgetDisableAction();
                case "Widget Intro":
                    return new WidgetIntroAction();
                case "Widget Dismiss":
                    return new WidgetDismissAction();
                case "Widget Hide":
                    return new WidgetHideAction();
                case "Widget Toggle Enabled":
                    return new WidgetToggleEnabledAction();
                case "Widget Cancel Intro":
                    return new WidgetCancelIntroAction();
                case "Widget Reload":
                    return new WidgetReloadAction();
                case "Show Popup":
                    return new PopupAction();
            }
            return null;
        }

        public static string GetNameFromEvent(IEvent evnt)
        {
            if (evnt is WidgetMouseDownEvent)
                return "Widget Mouse Down";
            if (evnt is WidgetMouseUpEvent)
                return "Widget Mouse Up";
            if (evnt is WidgetEnableEvent)
                return "Widget Enable";
            if (evnt is WidgetDisableEvent)
                return "Widget Disable";
            if (evnt is WidgetIntroEvent)
                return "Widget Intro";
            if (evnt is WidgetDismissEvent)
                return "Widget Dismiss";
            if (evnt is WidgetSpecialEvent)
                return "Widget Special Event";
            if (evnt is WidgetIntroEndEvent)
                return "Widget Intro End";
            if (evnt is WidgetMouseDoubleClickEvent)
                return "Widget Mouse Double Click";
            if (evnt is WidgetReloadEvent)
                return "Widget Reload";
            if (evnt is ForegroundFullscreenChangedEvent)
                return "Foreground Fullscreen Changed";
            if (evnt is ForegroundTitleChangedEvent)
                return "Foreground Title Changed";
            if (evnt is LaunchEvent)
                return "Launch";
            if (evnt is HotkeyEvent)
                return "Hotkey";
            return null;
        }

        public static string GetNameFromAction(IAction action)
        {
            if (action is OpenFileAction)
                return "Open File";
            if (action is PlaySoundAction)
                return "Play Sound";
            if (action is WidgetEnableAction)
                return "Widget Enable";
            if (action is WidgetDisableAction)
                return "Widget Disable";
            if (action is WidgetIntroAction)
                return "Widget Intro";
            if (action is WidgetDismissAction)
                return "Widget Dismiss";
            if (action is WidgetHideAction)
                return "Widget Hide";
            if (action is WidgetToggleEnabledAction)
                return "Widget Toggle Enabled";
            if (action is WidgetCancelIntroAction)
                return "Widget Cancel Intro";
            if (action is WidgetReloadAction)
                return "Widget Reload";
            if (action is PopupAction)
                return "Show Popup";
            return null;
        }
    }
}
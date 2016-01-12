﻿using System;

namespace DesktopWidgets.Helpers
{
    public static class StringHelper
    {
        public static bool Contains(this string source, string value, bool caseInsensitive)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(value))
                return false;
            return caseInsensitive
                ? source.IndexOf(value, 0, StringComparison.CurrentCultureIgnoreCase) != -1
                : source.Contains(value);
        }
    }
}
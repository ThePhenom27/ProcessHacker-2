﻿/*
 * Process Hacker - 
 *   logging
 * 
 * Copyright (C) 2009 wj32
 * 
 * This file is part of Process Hacker.
 * 
 * Process Hacker is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Process Hacker is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Process Hacker.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public static class Logging
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern void OutputDebugString(string OutputString);

        public enum Importance : int
        {
            Information = 0,
            Warning,
            Error,
            Critical
        }

        [Conditional("DEBUG")]
        public static void Log(Importance importance, string message)
        {
            OutputDebugString("ProcessHacker: (" + importance.ToString() + ") " + message);
        }

        [Conditional("DEBUG")]
        public static void Log(Exception ex)
        {
            Log(Importance.Error, ex.ToString());
        }
    }
}

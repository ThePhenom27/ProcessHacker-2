﻿/*
 * Process Hacker
 * 
 * Copyright (C) 2008 wj32
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Runtime.InteropServices;

namespace ProcessHacker
{
    public partial class Win32
    {
        /// <summary>
        /// Represents a handle owned by another process.
        /// </summary>
        /// <typeparam name="THandle">A type derived from Win32Handle.</typeparam>
        public class RemoteHandle
        {
            private ProcessHandle _phandle;
            private int _handle;

            public RemoteHandle(ProcessHandle phandle, int handle)
            {
                _phandle = phandle;
                _handle = handle;
            }

            public int GetHandle(int rights)
            {
                int new_handle = 0;

                if (ZwDuplicateObject(_phandle.Handle, _handle,
                    Program.CurrentProcess, out new_handle,
                    (STANDARD_RIGHTS)rights, 0, 0) != 0)
                    throw new Exception("Could not duplicate token handle!");

                return new_handle;
            }
        }
    }
}

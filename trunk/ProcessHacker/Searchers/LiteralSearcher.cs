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
    public class LiteralSearcher : Searcher
    {
        public LiteralSearcher(int PID) : base(PID) { }

        public override void Search()
        {
            Results.Clear();

            byte[] text = (byte[])Params["text"];
            int handle = 0;
            int address = 0;
            Win32.MEMORY_BASIC_INFORMATION info = new Win32.MEMORY_BASIC_INFORMATION();
            int count = 0;

            bool opt_priv = (bool)Params["private"];
            bool opt_img = (bool)Params["image"];
            bool opt_map = (bool)Params["mapped"];

            bool nooverlap = (bool)Params["nooverlap"];

            if (text.Length == 0)
            {
                CallSearchFinished();
                return;
            }

            handle = Win32.OpenProcess(Win32.PROCESS_RIGHTS.PROCESS_VM_READ |
                Win32.PROCESS_RIGHTS.PROCESS_QUERY_INFORMATION, 0, PID);

            if (handle == 0)
            {
                CallSearchError("Could not open process: " + Win32.GetLastErrorMessage());
                return;
            }

            while (true)
            {
                if (!Win32.VirtualQueryEx(handle, address, ref info,
                    Marshal.SizeOf(typeof(Win32.MEMORY_BASIC_INFORMATION))))
                {
                    break;
                }
                else
                {
                    address += info.RegionSize;

                    // skip unreadable areas
                    if (info.Protect == Win32.MEMORY_PROTECTION.PAGE_ACCESS_DENIED)
                        continue;
                    if (info.State != Win32.MEMORY_STATE.MEM_COMMIT)
                        continue;

                    if ((!opt_priv) && (info.Type == Win32.MEMORY_TYPE.MEM_PRIVATE))
                        continue;

                    if ((!opt_img) && (info.Type == Win32.MEMORY_TYPE.MEM_IMAGE))
                        continue;

                    if ((!opt_map) && (info.Type == Win32.MEMORY_TYPE.MEM_MAPPED))
                        continue;

                    byte[] data = new byte[info.RegionSize];
                    int bytesRead = 0;

                    CallSearchProgressChanged(
                        String.Format("Searching 0x{0:x8} ({1} found)...", info.BaseAddress, count));

                    Win32.ReadProcessMemory(handle, info.BaseAddress, data, info.RegionSize, out bytesRead);

                    if (bytesRead == 0)
                        continue;

                    for (int i = 0; i < bytesRead; i++)
                    {
                        bool good = true;

                        for (int j = 0; j < text.Length; j++)
                        {
                            if (i + j > bytesRead - 1)
                                continue;

                            if (data[i + j] != text[j])
                            {
                                good = false;
                                break;
                            }
                        }

                        if (good)
                        {
                            Results.Add(new string[] { String.Format("0x{0:x8}", info.BaseAddress),
                                String.Format("0x{0:x8}", i), text.Length.ToString(), "" });

                            count++;

                            if (nooverlap)
                                i += text.Length - 1;
                        }
                    }

                    data = null;
                }
            }

            Win32.CloseHandle(handle);

            CallSearchFinished();
        }
    }
}

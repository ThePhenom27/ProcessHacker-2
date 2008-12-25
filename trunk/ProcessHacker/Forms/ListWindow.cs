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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProcessHacker
{
    public partial class ListWindow : Form
    {
        public ListWindow(List<KeyValuePair<string, string>> list)
        {
            InitializeComponent();

            foreach (KeyValuePair<string, string> kvp in list)
            {
                ListViewItem item = new ListViewItem();

                item.Text = kvp.Key;
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, kvp.Value));

                listView.Items.Add(item);
            }

            listView.ContextMenu = GenericViewMenu.GetMenu(listView);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

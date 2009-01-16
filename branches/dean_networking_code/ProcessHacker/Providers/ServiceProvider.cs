﻿/*
 * Process Hacker - 
 *   service provider
 * 
 * Copyright (C) 2008 wj32
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
using System.Drawing;
using System.Windows.Forms;

namespace ProcessHacker
{
    public struct ServiceItem
    {
        public Win32.ENUM_SERVICE_STATUS_PROCESS Status;
        public Win32.QUERY_SERVICE_CONFIG Config;
    }

    public class ServiceProvider : Provider<string, ServiceItem>
    {
        public ServiceProvider()
            : base()
        {      
            this.ProviderUpdate += new ProviderUpdateOnce(UpdateOnce);   
        }

        public void UpdateServiceConfig(string name, Win32.QUERY_SERVICE_CONFIG config)
        {
            ServiceItem item = Dictionary[name];

            Dictionary[name] = new ServiceItem()
            {
                Config = config,
                Status = item.Status
            };

            this.CallDictionaryModified(item, Dictionary[name]);
        }

        private void UpdateOnce()
        {
            Dictionary<string, Win32.ENUM_SERVICE_STATUS_PROCESS> newdictionary
                = Win32.EnumServices();

            // check for removed services
            foreach (string s in Dictionary.Keys)
            {
                if (!newdictionary.ContainsKey(s))
                {
                    ServiceItem service = Dictionary[s];

                    this.CallDictionaryRemoved(service);
                    Dictionary.Remove(s);
                }
            }

            // check for new services
            foreach (string s in newdictionary.Keys)
            {
                if (!Dictionary.ContainsKey(s))
                {
                    ServiceItem item = new ServiceItem();

                    item.Status = newdictionary[s];

                    try
                    {
                        item.Config = Win32.GetServiceConfig(s);
                    }
                    catch
                    { }

                    this.CallDictionaryAdded(item);
                    Dictionary.Add(s, item);
                }
            }

            // check for modified services
            foreach (ServiceItem service in Dictionary.Values)
            {
                ServiceItem newserviceitem = service;

                newserviceitem.Status = newdictionary[service.Status.ServiceName];
                newserviceitem.Config = service.Config;

                bool modified = false;

                if (service.Status.DisplayName != newserviceitem.Status.DisplayName)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.ControlsAccepted !=
                    newserviceitem.Status.ServiceStatusProcess.ControlsAccepted)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.CurrentState !=
                    newserviceitem.Status.ServiceStatusProcess.CurrentState)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.ProcessID !=
                    newserviceitem.Status.ServiceStatusProcess.ProcessID)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.ServiceFlags !=
                    newserviceitem.Status.ServiceStatusProcess.ServiceFlags)
                    modified = true;
                else if (service.Status.ServiceStatusProcess.ServiceType !=
                    newserviceitem.Status.ServiceStatusProcess.ServiceType)
                    modified = true;
                else if (service.Config.StartType !=
                    newserviceitem.Config.StartType)
                    modified = true;

                if (modified)
                {
                    this.CallDictionaryModified(service, newserviceitem);
                    Dictionary[service.Status.ServiceName] = newserviceitem;
                }         
            }
        }
    }
}

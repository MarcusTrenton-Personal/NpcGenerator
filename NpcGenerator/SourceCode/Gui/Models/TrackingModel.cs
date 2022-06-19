﻿/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<https://www.gnu.org/licenses/>.*/

using System;
using System.Collections.Generic;
using System.Text;

namespace NpcGenerator
{
    public class TrackingModel : ITrackingModel
    {
        public TrackingModel(IUserSettings userSettings, string userSettingsPath)
        {
            m_userSettings = userSettings;
            m_userSettingsPath = userSettingsPath;
        }

        public bool TrackingConsent 
        { 
            get
            {
                return m_userSettings.AnalyticsConsent;
            }

            set
            {
                m_userSettings.AnalyticsConsent = value;
                m_userSettings.Save(m_userSettingsPath);
            }
        }

        private readonly IUserSettings m_userSettings;
        private readonly string m_userSettingsPath;
    }
}

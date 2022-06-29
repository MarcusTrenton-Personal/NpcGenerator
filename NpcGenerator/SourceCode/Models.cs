/*Copyright(C) 2022 Marcus Trenton, marcus.trenton@gmail.com

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

using WpfServices;

namespace NpcGenerator
{
    public class Models : IModels
    {
        public Models(
            ILocalizationModel localization, 
            IAboutModel about, 
            INavigationModel navigation, 
            ITrackingModel tracking,
            INpcGeneratorModel npcGenerator)
        {
            Localization = localization;
            About = about;
            Navigation = navigation;
            Tracking = tracking;
            NpcGenerator = npcGenerator;
        }

        public ILocalizationModel Localization { get; set; }
        public IAboutModel About { get; set; }
        public INavigationModel Navigation { get; set; }
        public ITrackingModel Tracking { get; set; }
        public INpcGeneratorModel NpcGenerator { get; set; }
    }
}

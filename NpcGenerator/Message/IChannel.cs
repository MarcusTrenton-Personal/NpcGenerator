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

using System;

namespace NpcGenerator.Message
{
    public interface IChannel<T>
    {
        public delegate void Callback(object sender, T message);
        public void Subscribe(Callback callback);
        public void Unsubscribe(Callback callback);
        public void Publish(object sender, T message);
        public Type GetMessageType();
    }
}
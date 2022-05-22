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

namespace NpcGenerator
{
    //I don't want to create each one of these individually.
    //If anyone tells the message system to subscribe to event t, that event queue should exist then.
    public class MessageQueue<T> : IPublisher<T> //Is this a Publisher or a MessageQueue?
    {
        public void Subscribe(IPublisher<T>.Callback callback)
        {
            Callbacks += callback;
        }

        public void Unsubscribe(IPublisher<T>.Callback callback)
        {
            Callbacks -= callback;
        }
        
        public void Publish(object sender, T message)
        {
            Callbacks?.Invoke(sender, message);
        }

        public Type GetMessageType()
        {
            return typeof(T);
        }

        private event IPublisher<T>.Callback Callbacks;
    }
}
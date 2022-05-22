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
using System.Collections.Generic;

namespace NpcGenerator
{
    public class MessageCenter : IMessageCenter
    {
        public void Send<T>(object sender, T message)
        {
            Type type = typeof(T);
            object uncastQueue = null;
            if (m_queues.TryGetValue(type, out uncastQueue))
            {
                //This cast shouldn't fail. If it does, let it crash.
                MessageQueue<T> queue = (MessageQueue<T>)uncastQueue;
                queue.Publish(sender, message);
            }
        }

        public void Subscribe<T>(IPublisher<T>.Callback callback)
        {
            Type type = typeof(T);
            object uncastQueue = null;
            if(!m_queues.TryGetValue(type, out uncastQueue))
            {
                uncastQueue = new MessageQueue<T>();
                m_queues[type] = uncastQueue;
            }
            //This cast shouldn't fail. If it does, let it crash.
            MessageQueue<T> queue = (MessageQueue<T>)uncastQueue;
            queue.Subscribe(callback);
        }

        public void Unsubcribe<T>(IPublisher<T>.Callback callback)
        {
            Type type = typeof(T);
            object uncastQueue = null;
            if (!m_queues.TryGetValue(type, out uncastQueue))
            {
                throw new ArgumentException("Cannot unsubscribe to messages of type " + type + 
                    " that was not already subscribed");
            }
            //This cast shouldn't fail. If it does, let it crash.
            MessageQueue<T> queue = (MessageQueue<T>)uncastQueue;
            queue.Unsubscribe(callback);
        }

        private Dictionary<Type, object> m_queues = new Dictionary<Type, object>();
    }
}
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
along with this program. If not, see<https://www.gnu.org/licenses/>.*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Message;
using System;

namespace Tests
{
    [TestClass]
    public class ChannelTests
    {
        private class MockEvent
        {
            public int Number { get; set; }
            public string Text { get; set; }
        }

        [TestInitialize]
        public void CreateChannel()
        {
            m_channel = new Channel<MockEvent>();
        }

        [TestMethod]
        public void SingleSubscribe()
        {
            bool subscribeCalledback = false;

            m_channel.Subscribe((object sender, MockEvent message) => 
            {
                Assert.AreEqual(this, sender, "Message sender is not correct");
                Assert.AreEqual(3, message.Number, "Message has incorrect value");
                Assert.AreEqual("Test", message.Text, "Message has incorrect value");

                subscribeCalledback = true;
            });

            m_channel.Send(this, new MockEvent { Number = 3, Text = "Test" });

            Assert.IsTrue(subscribeCalledback, "Subscribe callback was not called.");
        }

        [TestMethod]
        public void MultipleSubscribe()
        {
            bool firstCallbackHappened = false;
            bool secondCallbackHappened = false;

            m_channel.Subscribe((object sender, MockEvent message) =>
            {
                firstCallbackHappened = true;
            });

            m_channel.Subscribe((object sender, MockEvent message) =>
            {
                secondCallbackHappened = true;
            });

            m_channel.Send(this, new MockEvent { Number = 3, Text = "Test" });

            Assert.IsTrue(firstCallbackHappened, "First subscribe callback was not called.");
            Assert.IsTrue(secondCallbackHappened, "Second subscribe callback was not called.");
        }

        [TestMethod]
        public void Unsubscribe()
        {
            bool subscribeCalledback = false;

            m_channel.Subscribe(Callback);
            m_channel.Unsubscribe(Callback);

            m_channel.Send(this, new MockEvent { Number = 3, Text = "Test" });

            Assert.IsFalse(subscribeCalledback, "Subscribe callback was called despite being unsubscribed.");

            void Callback(object sender, MockEvent message)
            {
                subscribeCalledback = true;
            }
        }

        [TestMethod]
        public void Resubscribe()
        {
            int callbackCount = 0;

            m_channel.Subscribe(Callback);
            m_channel.Unsubscribe(Callback);
            m_channel.Subscribe(Callback);

            m_channel.Send(this, new MockEvent { Number = 3, Text = "Test" });

            Assert.AreEqual(1, callbackCount, "Resubscribed callback is not called just once.");

            void Callback(object sender, MockEvent message)
            {
                callbackCount++;
            }
        }

        [TestMethod]
        public void GetMessageType()
        {
            Type type = m_channel.GetMessageType();
            Assert.AreEqual(typeof(MockEvent), type, "Message type is incorrect");
        }

        private Channel<MockEvent> m_channel;
    }
}

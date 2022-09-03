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
along with this program. If not, see <https://www.gnu.org/licenses/>.*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Services.Message;

namespace Tests
{
    [TestClass]
    public class MessagerTests
    {
        private class MockEvent1
        {
            public int Number { get; set; }
        }

        private class MockEvent2
        {
            public string Text { get; set; }
        }

        [TestInitialize]
        public void CreateMessenger()
        {
            m_messager = new Messager();
        }

        [TestMethod]
        public void SingleSubscribe()
        {
            bool subscribeCalledback = false;

            m_messager.Subscribe((object sender, MockEvent1 message) =>
            {
                Assert.AreEqual(this, sender, "Message sender is not correct");
                Assert.AreEqual(3, message.Number, "Message has incorrect value");

                subscribeCalledback = true;
            });

            m_messager.Send(this, new MockEvent1 { Number = 3 });

            Assert.IsTrue(subscribeCalledback, "Subscribe callback was not called.");
        }

        [TestMethod]
        public void MultipleSubscribeToSameEvent()
        {
            bool firstCallbackHappened = false;
            bool secondCallbackHappened = false;

            m_messager.Subscribe((object sender, MockEvent1 message) =>
            {
                firstCallbackHappened = true;
            });

            m_messager.Subscribe((object sender, MockEvent1 message) =>
            {
                secondCallbackHappened = true;
            });

            m_messager.Send(this, new MockEvent1 { Number = 3 });

            Assert.IsTrue(firstCallbackHappened, "First subscribe callback was not called.");
            Assert.IsTrue(secondCallbackHappened, "Second subscribe callback was not called.");
        }

        [TestMethod]
        public void MultipleSubscribeToDifferentEvents()
        {
            bool firstCallbackHappened = false;
            bool secondCallbackHappened = false;

            m_messager.Subscribe((object sender, MockEvent1 message) =>
            {
                firstCallbackHappened = true;

                Assert.AreEqual(this, sender, "Message sender is not correct");
                Assert.AreEqual(3, message.Number, "Message has incorrect value");
            });

            m_messager.Subscribe((object sender, MockEvent2 message) =>
            {
                secondCallbackHappened = true;

                Assert.AreEqual(this, sender, "Message sender is not correct");
                Assert.AreEqual("Test", message.Text, "Message has incorrect value");
            });

            m_messager.Send(this, new MockEvent1 { Number = 3 });
            m_messager.Send(this, new MockEvent2 { Text = "Test" });

            Assert.IsTrue(firstCallbackHappened, "First subscribe callback was not called.");
            Assert.IsTrue(secondCallbackHappened, "Second subscribe callback was not called.");
        }

        [TestMethod]
        public void Unsubscribe()
        {
            bool subscribeCalledback = false;

            m_messager.Subscribe<MockEvent1>(Callback);
            m_messager.Unsubscribe<MockEvent1>(Callback);

            m_messager.Send(this, new MockEvent1 { Number = 3 });

            Assert.IsFalse(subscribeCalledback, "Subscribe callback was called despite being unsubscribed.");

            void Callback(object sender, MockEvent1 message)
            {
                subscribeCalledback = true;
            }
        }

        [TestMethod]
        public void Resubscribe()
        {
            int callbackCount = 0;

            m_messager.Subscribe<MockEvent1>(Callback);
            m_messager.Unsubscribe<MockEvent1>(Callback);
            m_messager.Subscribe<MockEvent1>(Callback);

            m_messager.Send(this, new MockEvent1 { Number = 3 });

            Assert.AreEqual(1, callbackCount, "Resubscribed callback is not called just once.");

            void Callback(object sender, MockEvent1 message)
            {
                callbackCount++;
            }
        }

        private Messager m_messager;
    }
}

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
using System;
using WpfServices;

namespace Tests
{
    [TestClass]
    public class CommandHandlerTests
    {
        [TestMethod]
        public void ActionHappens()
        {
            bool callBackCalled = false;
            object originalParameter = 7;

            void Callback(object parameter)
            {
                callBackCalled = true;

                Assert.AreEqual(originalParameter, parameter, "Parameter is incorrect");
            }

            CommandHandler command = new CommandHandler(Callback, canExecute: null);

            command.Execute(originalParameter);
            Assert.IsTrue(callBackCalled, "Callback was never called");
        }

        [TestMethod]
        public void TrueCondition()
        {
            bool Condition(object parameter)
            {
                return true;
            }

            void NotUsedCallback(object parameter)
            {
            }

            CommandHandler command = new CommandHandler(NotUsedCallback, canExecute: Condition);

            bool canExecute = command.CanExecute(null);
            Assert.IsTrue(canExecute, "canExecute condition is false instead of true");
        }

        [TestMethod]
        public void FalseCondition()
        {
            bool Condition(object parameter)
            {
                return false;
            }

            void NotUsedCallback(object parameter)
            {
            }

            CommandHandler command = new CommandHandler(NotUsedCallback, canExecute: Condition);

            bool canExecute = command.CanExecute(null);
            Assert.IsFalse(canExecute, "canExecute condition is true instead of false");
        }

        [TestMethod]
        public void FalseConditionBlocksAction()
        {
            bool callBackCalled = false;

            void Callback(object parameter)
            {
                callBackCalled = true;
            }

            bool Condition(object parameter)
            {
                return false;
            }

            CommandHandler command = new CommandHandler(Callback, canExecute: Condition);

            command.Execute(null);
            Assert.IsFalse(callBackCalled, "Callback was called despite the condition failing");
        }

        [TestMethod]
        public void NullAction()
        {
            Exception exception = null;
            try
            {
                CommandHandler command = new CommandHandler(null, null);
            }
            catch(Exception e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception, "Creating a CommandHandler with a null action doesn't throw an exception");
        }
    }
}

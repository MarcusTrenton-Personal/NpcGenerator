/*Copyright(C) 2023 Marcus Trenton, marcus.trenton@gmail.com

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
using NpcGenerator;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Tests
{
    [TestClass]
    public class BinaryChoiceModalWithCancelTests
    {
        private Action m_emptyAction = delegate () { };

        [TestMethod]
        public void Option1ActionIsCalled()
        {
            bool action1Called = false;
            bool closedAfterAction = false;
            Exception uncaughtException = null;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    void Option1Action()
                    {
                        action1Called = true;
                    }

                    BinaryChoiceModalWithCancel modal = new BinaryChoiceModalWithCancel(
                        title: "Fate",
                        body: "Make a choice",
                        option1: "Cake",
                        option2: "Death",
                        option1Action: Option1Action,
                        option2Action: null,
                        cancelAction: null
                    );

                    //********* Test Window ********************
                    Button option1Button = (Button)modal.FindName("option1");
                    option1Button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                    closedAfterAction = !modal.IsLoaded;
                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
            Assert.IsTrue(action1Called, "Failed to call action");
            Assert.IsTrue(closedAfterAction, "Failed to close after action");
        }

        [TestMethod]
        public void Option2ActionIsCalled()
        {
            bool action2Called = false;
            bool closedAfterAction = false;
            Exception uncaughtException = null;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    void Option2Action()
                    {
                        action2Called = true;
                    }

                    BinaryChoiceModalWithCancel modal = new BinaryChoiceModalWithCancel(
                        title: "Fate",
                        body: "Make a choice",
                        option1: "Cake",
                        option2: "Death",
                        option1Action: m_emptyAction,
                        option2Action: Option2Action,
                        cancelAction: m_emptyAction
                    );

                    //********* Test Window ********************
                    Button option1Button = (Button)modal.FindName("option2");
                    option1Button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

                    closedAfterAction = !modal.IsLoaded;
                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
            Assert.IsTrue(action2Called, "Failed to call action");
            Assert.IsTrue(closedAfterAction, "Failed to close after action");
        }

        [TestMethod]
        public void CancelActionIsCalled()
        {
            bool cancelActionCalled = false;
            bool closedAfterAction = false;
            Exception uncaughtException = null;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    void CancelAction()
                    {
                        cancelActionCalled = true;
                    }

                    BinaryChoiceModalWithCancel modal = new BinaryChoiceModalWithCancel(
                        title: "Fate",
                        body: "Make a choice",
                        option1: "Cake",
                        option2: "Death",
                        option1Action: null,
                        option2Action: null,
                        cancelAction: CancelAction
                    );

                    //********* Test Window ********************
                    modal.Close();

                    closedAfterAction = !modal.IsLoaded;
                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
            Assert.IsTrue(cancelActionCalled, "Failed to call action");
            Assert.IsTrue(closedAfterAction, "Failed to close after action");
        }

        [TestMethod]
        public void TitleIsNull()
        {
            Exception uncaughtException = null;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    new BinaryChoiceModalWithCancel(
                        title: null,
                        body: "Make a choice",
                        option1: "Cake",
                        option2: "Death",
                        option1Action: m_emptyAction,
                        option2Action: m_emptyAction,
                        cancelAction: m_emptyAction
                    );
                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
        }

        [TestMethod]
        public void TitleIsEmpty()
        {
            Exception uncaughtException = null;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    new BinaryChoiceModalWithCancel(
                        title: string.Empty,
                        body: "Make a choice",
                        option1: "Cake",
                        option2: "Death",
                        option1Action: m_emptyAction,
                        option2Action: m_emptyAction,
                        cancelAction: m_emptyAction
                    );
                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
        }

        [TestMethod]
        public void BodyIsNull()
        {
            Exception uncaughtException = null;
            bool caughtExpectedException = false;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    try
                    {
                        new BinaryChoiceModalWithCancel(
                            title: "Fate",
                            body: null,
                            option1: "Cake",
                            option2: "Death",
                            option1Action: m_emptyAction,
                            option2Action: m_emptyAction,
                            cancelAction: m_emptyAction
                        );
                    }
                    catch (ArgumentNullException)
                    {
                        caughtExpectedException = true;
                    }
                    
                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
            Assert.IsTrue(caughtExpectedException, "Failed to catch expected exception");
        }

        [TestMethod]
        public void BodyIsEmpty()
        {
            Exception uncaughtException = null;
            bool caughtExpectedException = false;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    try
                    {
                        new BinaryChoiceModalWithCancel(
                            title: "Fate",
                            body: string.Empty,
                            option1: "Cake",
                            option2: "Death",
                            option1Action: m_emptyAction,
                            option2Action: m_emptyAction,
                            cancelAction: m_emptyAction
                        );
                    }
                    catch (ArgumentException)
                    {
                        caughtExpectedException = true;
                    }

                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
            Assert.IsTrue(caughtExpectedException, "Failed to catch expected exception");
        }

        [TestMethod]
        public void Option1IsNull()
        {
            Exception uncaughtException = null;
            bool caughtExpectedException = false;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    try
                    {
                        new BinaryChoiceModalWithCancel(
                            title: "Fate",
                            body: "Make a choice",
                            option1: null,
                            option2: "Death",
                            option1Action: m_emptyAction,
                            option2Action: m_emptyAction,
                            cancelAction: m_emptyAction
                        );
                    }
                    catch (ArgumentNullException)
                    {
                        caughtExpectedException = true;
                    }

                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
            Assert.IsTrue(caughtExpectedException, "Failed to catch expected exception");
        }

        [TestMethod]
        public void Option1IsEmpty()
        {
            Exception uncaughtException = null;
            bool caughtExpectedException = false;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    try
                    {
                        new BinaryChoiceModalWithCancel(
                            title: "Fate",
                            body: "Make a choice",
                            option1: string.Empty,
                            option2: "Death",
                            option1Action: m_emptyAction,
                            option2Action: m_emptyAction,
                            cancelAction: m_emptyAction
                        );
                    }
                    catch (ArgumentException)
                    {
                        caughtExpectedException = true;
                    }

                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
            Assert.IsTrue(caughtExpectedException, "Failed to catch expected exception");
        }

        [TestMethod]
        public void Option2IsNull()
        {
            Exception uncaughtException = null;
            bool caughtExpectedException = false;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    try
                    {
                        new BinaryChoiceModalWithCancel(
                            title: "Fate",
                            body: "Make a choice",
                            option1: "Cake",
                            option2: null,
                            option1Action: m_emptyAction,
                            option2Action: m_emptyAction,
                            cancelAction: m_emptyAction
                        );
                    }
                    catch (ArgumentNullException)
                    {
                        caughtExpectedException = true;
                    }

                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
            Assert.IsTrue(caughtExpectedException, "Failed to catch expected exception");
        }

        [TestMethod]
        public void Option2IsEmpty()
        {
            Exception uncaughtException = null;
            bool caughtExpectedException = false;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    try
                    {
                        new BinaryChoiceModalWithCancel(
                            title: "Fate",
                            body: "Make a choice",
                            option1: "Cake",
                            option2: string.Empty,
                            option1Action: m_emptyAction,
                            option2Action: m_emptyAction,
                            cancelAction: m_emptyAction
                        );
                    }
                    catch (ArgumentException)
                    {
                        caughtExpectedException = true;
                    }

                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
            Assert.IsTrue(caughtExpectedException, "Failed to catch expected exception");
        }

        [TestMethod]
        public void Option1ActionIsOptional()
        {
            Exception uncaughtException = null;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    BinaryChoiceModalWithCancel modal = new BinaryChoiceModalWithCancel(
                        title: "Fate",
                        body: "Make a choice",
                        option1: "Cake",
                        option2: "Death",
                        option1Action: null,
                        option2Action: m_emptyAction,
                        cancelAction: m_emptyAction
                    );
                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
        }

        [TestMethod]
        public void Option2ActionIsOptional()
        {
            Exception uncaughtException = null;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    BinaryChoiceModalWithCancel modal = new BinaryChoiceModalWithCancel(
                        title: "Fate",
                        body: "Make a choice",
                        option1: "Cake",
                        option2: "Death",
                        option1Action: m_emptyAction,
                        option2Action: null,
                        cancelAction: m_emptyAction
                    );
                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
        }

        [TestMethod]
        public void CancelActionActionIsOptional()
        {
            Exception uncaughtException = null;

            ThreadCreatingTests.StartInUiThread(delegate ()
            {
                try
                {
                    //********* Setup Variables ********************
                    BinaryChoiceModalWithCancel modal = new BinaryChoiceModalWithCancel(
                        title: "Fate",
                        body: "Make a choice",
                        option1: "Cake",
                        option2: "Death",
                        option1Action: m_emptyAction,
                        option2Action: m_emptyAction,
                        cancelAction: null
                    );
                }
                //Any uncaught exception in this thread will deadlock the parent thread, causing the test to abort instead of fail.
                //Therefore, every exception must be caught and explicitly marked as failure.
                catch (Exception e)
                {
                    uncaughtException = e;
                }
            });

            Assert.IsNull(uncaughtException, "Test failed from uncaught exception: " + uncaughtException ?? uncaughtException.ToString());
        }
    }
}

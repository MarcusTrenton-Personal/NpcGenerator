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
using Services;
using System;

namespace Tests
{
    [TestClass]
    public class LogicalAllTests
    {
        private class TrueExpression : ILogicalExpression
        {
            public bool Evaluate()
            {
                return true;
            }
        }

        private class FalseExpression : ILogicalExpression
        {
            public bool Evaluate()
            {
                return false;
            }
        }

        [TestMethod]
        public void EmptyExpression()
        {
            LogicalAll expression = new LogicalAll();

            bool threwException = false;
            try
            {
                bool result = expression.Evaluate();
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Empty Any expression evaluated when it should have thrown an exception");
        }

        [TestMethod]
        public void NullAddition()
        {
            LogicalAll expression = new LogicalAll();

            bool threwException = false;
            try
            {
                expression.Add(null);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Null addition to expression should have thrown an exception");
        }

        [TestMethod]
        public void SingleTrue()
        {
            LogicalAll expression = new LogicalAll();
            expression.Add(m_true);

            bool result = expression.Evaluate();

            Assert.IsTrue(result, "Solitary true operand in All expression should evaluate to true");
        }

        [TestMethod]
        public void SingleFalse()
        {
            LogicalAll expression = new LogicalAll();
            expression.Add(m_false);

            bool result = expression.Evaluate();

            Assert.IsFalse(result, "Solitary false operand in All expression should evaluate to false");
        }

        [TestMethod]
        public void DoubleTrue()
        {
            LogicalAll expression = new LogicalAll();
            expression.Add(m_true);
            expression.Add(m_true);

            bool result = expression.Evaluate();

            Assert.IsTrue(result, "Double true operand in All expression should evaluate to true");
        }

        [TestMethod]
        public void DoubleFalse()
        {
            LogicalAll expression = new LogicalAll();
            expression.Add(m_false);
            expression.Add(m_false);

            bool result = expression.Evaluate();

            Assert.IsFalse(result, "Double false operand in All expression should evaluate to false");
        }

        [TestMethod]
        public void TrueFalse()
        {
            LogicalAll expression = new LogicalAll();
            expression.Add(m_true);
            expression.Add(m_false);

            bool result = expression.Evaluate();

            Assert.IsFalse(result, "Having a single true operand in All expression should evaluate to false");
        }

        [TestMethod]
        public void FalseTrue()
        {
            LogicalAll expression = new LogicalAll();
            expression.Add(m_false);
            expression.Add(m_true);

            bool result = expression.Evaluate();

            Assert.IsFalse(result, "Having a single true operand in All expression should evaluate to false");
        }

        [TestMethod]
        public void FalseTrueFalse()
        {
            LogicalAll expression = new LogicalAll();
            expression.Add(m_false);
            expression.Add(m_true);
            expression.Add(m_false);

            bool result = expression.Evaluate();

            Assert.IsFalse(result, "Having a single true operand in All expression should evaluate to false");
        }

        [TestMethod]
        public void DirectRecursion()
        {
            LogicalAll expression = new LogicalAll();

            bool threwException = false;
            try
            {
                expression.Add(expression);
            }
            catch (Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Adding expression to itself should not be allowed due to infinite loop during evaluation.");
        }

        [TestMethod]
        public void IndirectRecursion()
        {
            LogicalAll expression0 = new LogicalAll();
            LogicalAll expression1 = new LogicalAll();

            expression0.Add(expression1);
            expression1.Add(expression0);

            bool threwException = false;
            try
            {
                bool result = expression0.Evaluate();
            }
            catch (InfiniteEvaluationLoopException)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException,
                "InfiniteLoopException should be throw during looping recusion rather than a slower StackOverflowException");
        }

        [TestMethod]
        public void Nested()
        {
            LogicalAll expression0 = new LogicalAll();
            LogicalAll expression1 = new LogicalAll();

            expression0.Add(expression1);
            expression1.Add(m_true);

            bool result = expression0.Evaluate();

            Assert.IsTrue(result, "Nested evaluation should not throw an exception but instead true");
        }

        private TrueExpression m_true = new TrueExpression();
        private FalseExpression m_false = new FalseExpression();
    }
}

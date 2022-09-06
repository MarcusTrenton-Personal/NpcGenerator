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
using Services;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class LogicalAnyTests
    {
        [TestMethod]
        public void EmptyExpression()
        {
            LogicalAny expression = new LogicalAny();

            bool threwException = false;
            try
            {
                bool result = expression.Evaluate();
            }
            catch(Exception)
            {
                threwException = true;
            }

            Assert.IsTrue(threwException, "Empty Any expression evaluated when it should have thrown an exception");
        }

        [TestMethod]
        public void NullAddition()
        {
            LogicalAny expression = new LogicalAny();

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
            LogicalAny expression = new LogicalAny();
            expression.Add(m_true);

            bool result = expression.Evaluate();

            Assert.IsTrue(result, "Solitary true operand in Any expression should evaluate to true");
        }

        [TestMethod]
        public void SingleFalse()
        {
            LogicalAny expression = new LogicalAny();
            expression.Add(m_false);

            bool result = expression.Evaluate();

            Assert.IsFalse(result, "Solitary false operand in Any expression should evaluate to false");
        }

        [TestMethod]
        public void DoubleTrue()
        {
            LogicalAny expression = new LogicalAny();
            expression.Add(m_true);
            expression.Add(m_true);

            bool result = expression.Evaluate();

            Assert.IsTrue(result, "Double true operand in Any expression should evaluate to true");
        }

        [TestMethod]
        public void DoubleFalse()
        {
            LogicalAny expression = new LogicalAny();
            expression.Add(m_false);
            expression.Add(m_false);

            bool result = expression.Evaluate();

            Assert.IsFalse(result, "Double false operand in Any expression should evaluate to false");
        }

        [TestMethod]
        public void TrueFalse()
        {
            LogicalAny expression = new LogicalAny();
            expression.Add(m_true);
            expression.Add(m_false);

            bool result = expression.Evaluate();

            Assert.IsTrue(result, "Having a single true operand in Any expression should evaluate to true");
        }

        [TestMethod]
        public void FalseTrue()
        {
            LogicalAny expression = new LogicalAny();
            expression.Add(m_false);
            expression.Add(m_true);

            bool result = expression.Evaluate();

            Assert.IsTrue(result, "Having a single true operand in Any expression should evaluate to true");
        }

        [TestMethod]
        public void FalseTrueFalse()
        {
            LogicalAny expression = new LogicalAny();
            expression.Add(m_false);
            expression.Add(m_true);
            expression.Add(m_false);

            bool result = expression.Evaluate();

            Assert.IsTrue(result, "Having a single true operand in Any expression should evaluate to true");
        }

        [TestMethod]
        public void DirectRecursion()
        {
            LogicalAny expression = new LogicalAny();
            
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
            LogicalAny expression0 = new LogicalAny();
            LogicalAny expression1 = new LogicalAny();

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
            LogicalAny expression0 = new LogicalAny();
            LogicalAny expression1 = new LogicalAny();

            expression0.Add(expression1);
            expression1.Add(m_true);

            bool result = expression0.Evaluate();

            Assert.IsTrue(result, "Nested evaluation should not throw an exception but instead true");
        }

        [TestMethod]
        public void SubExpressionsEmpty()
        {
            LogicalAny expression = new LogicalAny();

            IReadOnlyList<ILogicalExpression> subExpressions = expression.SubExpressions;

            bool isNullOrEmpty = ListUtil.IsNullOrEmpty(subExpressions);
            Assert.IsTrue(isNullOrEmpty, "Subexpression lists should be empty or null");
        }

        [TestMethod]
        public void SubExpressionsSingle()
        {
            LogicalAny expression = new LogicalAny();
            expression.Add(m_true);

            IReadOnlyList<ILogicalExpression> subExpressions = expression.SubExpressions;

            Assert.AreEqual(1, subExpressions.Count, "Wrong number of SubExpressions");
            Assert.IsTrue(subExpressions[0] is AlwaysTrue, "Wrong type of SubExpression");
        }

        [TestMethod]
        public void SubExpressionsMultiple()
        {
            LogicalAny expression = new LogicalAny();
            expression.Add(m_true);
            expression.Add(m_false);

            IReadOnlyList<ILogicalExpression> subExpressions = expression.SubExpressions;

            Assert.AreEqual(2, subExpressions.Count, "Wrong number of SubExpressions");

            int trueCount = 0;
            int falseCount = 0;
            foreach (ILogicalExpression subExpression in subExpressions)
            {
                if (subExpression is AlwaysTrue)
                {
                    trueCount++;
                }
                else if (subExpression is AlwaysFalse)
                {
                    falseCount++;
                }
            }
            Assert.AreEqual(1, trueCount, "Wrong number of AlwaysTrue SubExpressions");
            Assert.AreEqual(1, falseCount, "Wrong number of AlwaysFalse SubExpressions");
        }

        private AlwaysTrue m_true = new AlwaysTrue();
        private AlwaysFalse m_false = new AlwaysFalse();
    }
}

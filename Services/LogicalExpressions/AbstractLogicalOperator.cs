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

using System;
using System.Collections.Generic;

namespace Services
{
    public abstract class AbstractLogicalOperator : ILogicalOperator
    {
        public void Add(ILogicalExpression expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException("Cannot add null ILogicalExpression as it cannot be evaluated", nameof(expression));
            }
            if (expression == this)
            {
                throw new ArgumentNullException(GetType().Name + " cannot add self as it causes an infinite loop during evaluation", 
                    nameof(expression));
            }

            m_expressions.Add(expression);
        }

        public bool Evaluate()
        {
            if (m_expressions.Count == 0)
            {
                throw new InvalidOperationException("Cannot evaluate an empty Any expression");
            }
            if (m_isEvaluating)
            {
                throw new InfiniteEvaluationLoopException(GetType().Name + " infinite recursion loop detected.");
            }

            m_isEvaluating = true;
            bool result = EvaluateInternal(m_expressions);
            m_isEvaluating = false;
            return result;
        }

        public IReadOnlyList<ILogicalExpression> SubExpressions 
        {
            get
            {
                return m_expressions;
            }
        }

        private readonly List<ILogicalExpression> m_expressions = new List<ILogicalExpression>();
        private bool m_isEvaluating;

        abstract protected bool EvaluateInternal(IReadOnlyList<ILogicalExpression> expressions);
    }

    public class InfiniteEvaluationLoopException : InvalidOperationException
    {
        public InfiniteEvaluationLoopException(string message) : base(message)
        {
        }
    }
}

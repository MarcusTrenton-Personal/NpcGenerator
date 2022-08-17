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

using System;

namespace Services
{
    public class LogicalNot : ILogicalOperator
    {
        public void Add(ILogicalExpression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("Cannot add null ILogicalExpression as it cannot be evaluated", nameof(expression));
            }
            if (m_expression != null)
            {
                throw new InvalidOperationException("LogicalNot can only have a single expression, but another was attempted");
            }

            m_expression = expression;
        }

        public bool Evaluate()
        {
            if (m_expression == null)
            {
                throw new InvalidOperationException("Cannot evaluate an empty And expression");
            }

            bool subExpressionResult = m_expression.Evaluate();
            return !subExpressionResult;
        }

        private ILogicalExpression m_expression;
    }
}
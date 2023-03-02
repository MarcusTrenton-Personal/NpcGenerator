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

using Services;
using System;
using System.ComponentModel;
using System.Windows;

namespace NpcGenerator
{
    public partial class BinaryChoiceModalWithCancel : Window
    {
        public BinaryChoiceModalWithCancel(
            string title, string body, string option1, string option2, Action option1Action, Action option2Action, Action cancelAction)
        {
            //Title can empty/null
            ParamUtil.VerifyHasContent(nameof(body), body);
            ParamUtil.VerifyHasContent(nameof(option1), option1);
            ParamUtil.VerifyHasContent(nameof(option2), option2);
            //All actions are optional

            m_title = title;
            m_body = body;
            m_option1 = option1;
            m_option2 = option2;
            m_option1Action = option1Action;
            m_option2Action = option2Action;
            m_cancelAction = cancelAction;

            InitializeComponent();
        }

        public string TitleText
        {
            get
            {
                return m_title; 
            }
        }

        public string BodyText
        {
            get
            {
                return m_body;
            }
        }

        public string Option1Text
        {
            get
            {
                return m_option1;
            }
        }

        public string Option2Text
        {
            get
            {
                return m_option2;
            }
        }

        private void ClickOption1(object sender, RoutedEventArgs e)
        {
            m_optionClicked = true;
            m_option1Action?.Invoke();
            Close();
        }

        private void ClickOption2(object sender, RoutedEventArgs e)
        {
            m_optionClicked = true;
            m_option2Action?.Invoke();
            Close();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (!m_optionClicked)
            {
                m_cancelAction?.Invoke();
            }
        }

        private readonly string m_title;
        private readonly string m_body;
        private readonly string m_option1;
        private readonly string m_option2;
        private readonly Action m_option1Action;
        private readonly Action m_option2Action;
        private readonly Action m_cancelAction;

        private bool m_optionClicked = false;
    }
}

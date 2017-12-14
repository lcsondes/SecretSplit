/*
  Copyright © 2017 László Csöndes

  This file is part of Secret Splitter.

  Secret Splitter is free software: you can redistribute it and/or modify
  it under the terms of the GNU Affero General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  Secret Splitter is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
  GNU Affero General Public License for more details.

  You should have received a copy of the GNU Affero General Public License
  along with Secret Splitter. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Windows;
using System.Windows.Input;

namespace SecretSplit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ValidateNumbersOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !uint.TryParse(e.Text, out uint value);
        }

        private void SplitSecret_Click(object sender, RoutedEventArgs e)
        {
            var parts = SecretSharing.SplitSecret(textSecret.Text, int.Parse(textNeededCount.Text), int.Parse(textTotalCount.Text));
            textParts.Text = string.Join("\n", parts);
        }

        private void MergeSecret_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parts = textParts.Text.Trim().Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                textSecret.Text = SecretSharing.MergeSecret(parts);
            }
            catch(Exception ex)
            {
                textSecret.Text = $"{ex.GetType()}:{ex.Message}";
            }
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            textNeededCount.IsEnabled = false;
            textTotalCount.IsEnabled = false;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            textNeededCount.IsEnabled = true;
            textTotalCount.IsEnabled = true;
        }
    }
}

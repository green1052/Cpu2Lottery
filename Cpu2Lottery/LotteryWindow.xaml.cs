using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cpu2Lottery.Module;
using HtmlAgilityPack;

namespace Cpu2Lottery
{
    /// <summary>
    ///     LotteryWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LotteryWindow : Window
    {
        private readonly Client _client;
        private readonly Random _random = new Random(DateTime.Now.Millisecond + Application.Current.GetHashCode());

        public LotteryWindow(Client client)
        {
            InitializeComponent();

            _client = client;
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LoadAddress(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(addressBox.Text))
            {
                Utility.ShowErrorMessageBox("게시글 주소가 비어 있습니다.");
                return;
            }

            userListBox.Items.Clear();
            winnerListBox.Items.Clear();

            try
            {
                HtmlDocument htmlDocument = _client.GetHtml(addressBox.Text);
                HtmlNodeCollection nodes = htmlDocument.DocumentNode.SelectNodes("//table//td[@valign='top']");

                if (nodes == null) throw new Exception();

                foreach (HtmlNode node in nodes)
                {
                    string user = node.SelectSingleNode(".//a")?.InnerText.Trim();
                    string replyId = node.SelectSingleNode(".//span[@id]").GetAttributeValue<string>("id", null)
                        .Substring(5);

                    string txt = node.SelectSingleNode($"//textarea[@id='save_comment_{replyId}']")?.InnerText
                        .Trim();

                    if (user == null) continue;

                    if (!userListBox.Items.Contains(user) && (txt ?? string.Empty).Contains(replyFormatBox.Text))
                        userListBox.Items.Add(user);
                }

                userListCountLabel.Content = $"{userListBox.Items.Count:#,##0} 명";
            }
            catch
            {
                Utility.ShowErrorMessageBox("잘못된 주소 입니다.");
            }
        }

        private void OnlyNumber(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                e.Handled = false;
            else
                e.Handled = true;
        }

        private void PickWinner(object sender, RoutedEventArgs e)
        {
            winnerListBox.Items.Clear();

            int.TryParse(countBox.Text, out int maxPickTime);

            maxPickTime = maxPickTime > userListBox.Items.Count ? userListBox.Items.Count : maxPickTime;

            List<string> userList = userListBox.Items.OfType<string>().ToList();

            for (int i = 0; i < maxPickTime; i++)
            {
                string item = userList[_random.Next(0, userList.Count)];
                userList.Remove(item);

                winnerListBox.Items.Add(item);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete) return;

            ListBox listBox = sender as ListBox;

            if (listBox == null)
                return;

            foreach (string listBoxSelectedItem in listBox.SelectedItems.OfType<string>().ToArray())
                listBox.Items.Remove(listBoxSelectedItem);

            userListCountLabel.Content = $"{userListBox.Items.Count:#,##0} 명";
        }
    }
}
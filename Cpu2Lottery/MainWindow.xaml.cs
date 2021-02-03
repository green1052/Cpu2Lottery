using System.Windows;
using Cpu2Lottery.Module;

namespace Cpu2Lottery
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            if (idBox.Text.Length <= 0 || passwordBox.Password.Length <= 0) return;

            Client client = new Client("https://www.2cpu.co.kr/bbs/login_check.php", idBox.Text, passwordBox.Password);

            if (client.ErrorMessage != null)
            {
                Utility.ShowErrorMessageBox(client.ErrorMessage);
                return;
            }

            LotteryWindow lotteryWindow = new LotteryWindow(client);
            Hide();
            lotteryWindow.Show();
        }
    }
}
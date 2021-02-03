using System.Windows;

namespace Cpu2Lottery.Module
{
    public static class Utility
    {
        public static void ShowErrorMessageBox(string content)
        {
            MessageBox.Show(content, "2cpu 추첨기", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
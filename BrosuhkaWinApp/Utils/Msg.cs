using System.Windows;

namespace BrosuhkaWinApp.Utils
{
    /// <summary>
    /// Message box utils.
    /// </summary>
    public class Msg
    {
        /// <summary>
        /// Shows information dialog.
        /// </summary>
        /// <param name="msg">Message to show.</param>
        public static void ShowInfo(string msg)
        {
            MessageBox.Show(
                msg, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Shows error dialog.
        /// </summary>
        /// <param name="msg">Message to show.</param>
        public static void ShowError(string msg)
        {
            MessageBox.Show(
                msg, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Shows information dialog.
        /// </summary>
        /// <param name="msg">Message to show.</param>
        public static bool ShowQuestion(string msg)
        {
            return MessageBox.Show(
                msg, "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes;
        }
    }
}

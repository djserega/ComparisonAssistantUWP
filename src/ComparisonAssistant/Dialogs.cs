using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ComparisonAssistant
{
    internal static class Dialogs
    {
        internal static async void Show(string message)
        {
            ContentDialog contentDialog = new ContentDialog()
            {
                Title = "Помощник сравнения",
                Content = message,
                CloseButtonText = "ОК"
            };
            await contentDialog.ShowAsync();
        }

        internal static async void ShowPopups(string message)
        {
            await new Windows.UI.Popups.MessageDialog(message).ShowAsync();
        }
    }
}

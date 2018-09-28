using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace ComparisonAssistant.Views
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class PageBrowser : Page
    {
        private static ChangeContentFrameEvents _changeContentFrameEvents;

        public PageBrowser()
        {
            InitializeComponent();
        }

        public string Source { get; set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is MainWindowFrame objParameter)
            {
                if (objParameter.CountParameter > 0)
                {
                    Source = objParameter.Parameters[0] as string;
                    _changeContentFrameEvents = objParameter.Parameters[1] as ChangeContentFrameEvents;
                }
            }
        }

        private void ButtonBackToCommits_Click(object sender, RoutedEventArgs e)
        {
            _changeContentFrameEvents.ChangeContent(null, null);
        }
    }
}

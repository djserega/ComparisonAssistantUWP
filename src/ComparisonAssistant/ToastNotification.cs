using Microsoft.Toolkit.Uwp.Notifications;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace ComparisonAssistant
{
    internal class ToastNotification
    {
        private Windows.UI.Notifications.ToastNotification _toast;
        private readonly ToastNotifier _toastNotifier;

        public ToastNotification()
        {
            _toastNotifier = ToastNotificationManager.CreateToastNotifier();
        }

        internal uint SequenceNumber { get; private set; }

        internal string Title { get; set; }
        internal string Text { get; set; }
        internal string Group { get; private set; }
        internal string Tag { get; private set; }

        internal string ProgressValue { get => GetProgressValue(); }

        internal double ProgressValueDouble { get; set; }
        internal double ProgressValueDoubleMax { get; set; }
        internal string ProgressValueString { get; set; } = string.Empty;
        internal string ProgressStatus { get; set; }

        internal void InitialToast(string tag, string group)
        {
            SequenceNumber = 1;
            Tag = tag;
            Group = group;

            _toast = new Windows.UI.Notifications.ToastNotification(InitialToastContent().GetXml())
            {
                Tag = Tag,
                Group = Group
            };

            _toast.Data = new NotificationData();
            _toast.Data.Values["progressValue"] = ProgressValue;
            _toast.Data.Values["progressValueString"] = ProgressValueString;
            _toast.Data.Values["progressStatus"] = ProgressStatus;

            _toast.Data.SequenceNumber = SequenceNumber;
        }

        internal void Show()
        {
            _toastNotifier.Show(_toast);
        }

        internal void Update(string progressValue = null, string progressValueString = null)
        {
            NotificationData data = new NotificationData
            {
                SequenceNumber = SequenceNumber++
            };
            data.Values["progressValue"] = (string.IsNullOrEmpty(progressValue) ? ProgressValue : progressValue).Replace(",", ".");
            data.Values["progressValueString"] = string.IsNullOrEmpty(progressValueString) ? ProgressValueString : progressValueString;

            _toastNotifier.Update(data, Tag, Group);
        }

        internal void Hide()
        {
            _toastNotifier.Hide(_toast);
        }

        private ToastContent InitialToastContent()
        {
            return new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = Title
                            },
                            new AdaptiveProgressBar()
                            {
                                Title = Text,
                                Value = new BindableProgressBarValue("progressValue"),
                                ValueStringOverride = new BindableString("progressValueString"),
                                Status = new BindableString("progressStatus")
                            }
                        }
                    }
                }
            };
        }

        private string GetProgressValue()
        {
            if (ProgressValueDoubleMax > 0)
                return (ProgressValueDouble * 100 / ProgressValueDoubleMax / 100).ToString();
            else
                return "";
        }
    }
}

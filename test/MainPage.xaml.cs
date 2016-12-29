using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace test
{
    public sealed partial class MainPage : Page
    {
        public ApplicationDataContainer localSettings;
        ObservableCollection<string> images = new ObservableCollection<string>();

        public MainPage()
        {
            localSettings = ApplicationData.Current.LocalSettings;
            this.InitializeComponent();
        }

        private async void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var t1 = new TimeTrigger(15, false);
            var t2 = new SystemTrigger(SystemTriggerType.UserPresent, false);
            var t3 = new SystemTrigger(SystemTriggerType.InternetAvailable, false);
            await RegisterBackgroundTask("t1", "bt.t", t1);
            await RegisterBackgroundTask("t2", "bt.t", t2);
            await RegisterBackgroundTask("t3", "bt.t", t3);
            ((Button)sender).Content = "Done.";
        }

        public async Task<BackgroundTaskRegistration> RegisterBackgroundTask(string taskName, string taskEntryPoint, IBackgroundTrigger trigger)
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy ||
                backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == taskName)
                    {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger(trigger);
                //taskBuilder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                return taskBuilder.Register();
            }
            return null;
        }
    }
}

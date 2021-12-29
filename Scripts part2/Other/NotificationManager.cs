#if Android
using Unity.Notifications.Android;
#endif

namespace AnilTools
{
    public static class NotificationManager
    {

#if Android
        static AndroidNotificationChannel DefaultNotificationChannel = new AndroidNotificationChannel()
        {
            Id = "DefaultChanel",
            Name = "Default Chanel",
            Description = "main notificulation",
            Importance = Importance.Default
        };
#endif
        public static void SendNotificulation(string title, string text, int duraition = 0)
        {
#if Android
            AndroidNotificationCenter.RegisterNotificationChannel(DefaultNotificationChannel);

            AndroidNotification notification = new AndroidNotification()
            {
                Title = title,
                Text = text,
                SmallIcon = "default",
                LargeIcon = "default",
                FireTime = System.DateTime.Now.AddSeconds(duraition)
            };

            AndroidNotificationCenter.SendNotification(notification, "DefaultChanel");
#endif
        }

    }

}
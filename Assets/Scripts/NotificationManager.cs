using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using Unity.Notifications.Android;

public class NotificationManager : ScreenState
{
    int endHour = 21;
    int incrementMinutes = 10;

    private void Start()
    {
        var channel1 = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Silent Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        var channel2 = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Sound Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };

        AndroidNotificationCenter.RegisterNotificationChannel(channel1);
        AndroidNotificationCenter.RegisterNotificationChannel(channel2);

        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

    }

    public void SetNotifications()
    {

        List<System.DateTime> fireTimes = new List<System.DateTime>();
        var nextFireTime = System.DateTime.Now;
        while(nextFireTime.Hour < endHour)
        {
            fireTimes.Add(nextFireTime);
            nextFireTime = nextFireTime.AddMinutes(incrementMinutes);
        }

        for (int i = 0; i < fireTimes.Count; i++)
        {
            var notification = new AndroidNotification();
            notification.Title = (fireTimes.Count - i).ToString();
            notification.FireTime = fireTimes[i];
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
            //print((fireTimes.Count - i).ToString() + ", " + fireTimes[i]);
        }

    }

    public void CancelNotifications() => AndroidNotificationCenter.CancelAllNotifications();

    public void SetEndHour(string hour) => endHour = int.Parse(hour);
    public void SetIncrementMinutes(string minute) => incrementMinutes = int.Parse(minute);
}

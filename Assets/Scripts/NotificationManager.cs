using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using Unity.Notifications.Android;

public class NotificationManager : MonoBehaviour
{
    int endHour = 9;

    private void Start()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

    }

    public void SetNotifications()
    {
        //for (int i = 0; i < 6; i++)
        //{
        //    var notification = new AndroidNotification();
        //    notification.Title = (i * 5) + " seconds";
        //    //notification.Text = "Your Text";
        //    notification.FireTime = System.DateTime.Now.AddSeconds(i * 5);
        //    AndroidNotificationCenter.SendNotification(notification, "channel_id");
        //}

        var nextFireTime = System.DateTime.Now;
        while (nextFireTime.Hour < endHour)
        {
            var notification = new AndroidNotification();
            notification.Title = nextFireTime.Hour.ToString();
            print(nextFireTime.Hour.ToString());
            notification.FireTime = nextFireTime;
            AndroidNotificationCenter.SendNotification(notification, "channel_id");
            nextFireTime = nextFireTime.AddMinutes(10);
        }
    }

    public void CancelNotifications() => AndroidNotificationCenter.CancelAllNotifications();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using Unity.Notifications.Android;

public class NotificationManager : MonoBehaviour
{
    public int endHour = 21;

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

        List<System.DateTime> fireTimes = new List<System.DateTime>();
        var nextFireTime = System.DateTime.Now;
        while(nextFireTime.Hour < endHour)
        {
            fireTimes.Add(nextFireTime);
            nextFireTime = nextFireTime.AddMinutes(10);
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
}

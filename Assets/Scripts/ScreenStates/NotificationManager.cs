using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using Unity.Notifications.Android;
using TMPro;

public class NotificationManager : ScreenState
{
    [SerializeField] TMP_Dropdown channelSelecter;
    public string currentChannel;
    enum EChannelIDs
    {
        None, Sound, Silent,
    }

    public int duration;
    public int incrementMinutes;

    private void Start()
    {

        var channel0 = new AndroidNotificationChannel()
        {
            Id = ChannelIDs.ids[0],
            Name = ChannelIDs.ids[0],
            Importance = Importance.Default,
            Description = "yee",
        };
        var channel1 = new AndroidNotificationChannel()
        {
            Id = ChannelIDs.ids[1],
            Name = ChannelIDs.ids[1],
            Importance = Importance.Default,
            Description = "yeee",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel0);
        AndroidNotificationCenter.RegisterNotificationChannel(channel1);

        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

    }

    public void ChangeChannel(int index)
    {
        AndroidNotificationCenter.CancelAllNotifications();
        if (index == 0)
        {
            return;
        }

        List<System.DateTime> fireTimes = new List<System.DateTime>();

        var now = System.DateTime.Now;
        var nextFireTime = now;
        while (nextFireTime <= now.AddMinutes(duration))
        {
            fireTimes.Add(nextFireTime);
            nextFireTime = nextFireTime.AddMinutes(incrementMinutes);
        }

        for (int i = 0; i < fireTimes.Count; i++)
        {
            var notification = new AndroidNotification();
            notification.Title = (fireTimes[fireTimes.Count - 1] - fireTimes[i]).Minutes.ToString();
            notification.FireTime = fireTimes[i];
            AndroidNotificationCenter.SendNotification(notification, ChannelIDs.ids[index - 1]);
            //print(fireTimes[fireTimes.Count - 1] - fireTimes[i]);
        }
    }

    public void SetDuration(string minute) => duration = int.Parse(minute);
    public void SetIncrementMinutes(string minute) => incrementMinutes = int.Parse(minute);
}

public static class ChannelIDs
{
    public enum ID
    {
        Sound, Silent,
    }
    public static readonly string[] ids = { "Sound", "Silent" };
}

using System;
using Org.Chromium.Chrome.Browser.Hosted;
using Android.Content;
using System.Collections.Generic;

namespace Xamarin.Android.CustomChromeTabs
{

    public class HostedServiceConnection : Java.Lang.Object, IServiceConnection
    {
        public Action<ComponentName, global::Android.OS.IBinder> OnServiceConnectedHandler { get; set; }
        public Action<ComponentName> OnServiceDisconnectedHandler { get; set; }

        public void OnServiceConnected (ComponentName name, global::Android.OS.IBinder service)
        {
            var h = OnServiceConnectedHandler;
            if (h != null)
                h (name, service);
        }

        public void OnServiceDisconnected (ComponentName name)
        {
            var h = OnServiceDisconnectedHandler;
            if (h != null)
                h (name);
        }
    }
}

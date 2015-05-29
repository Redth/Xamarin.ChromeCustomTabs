using System;
using Org.Chromium.Chrome.Browser.Hosted;
using Android.Content;
using System.Collections.Generic;

namespace Xamarin.Android.CustomChromeTabs
{

    public class BrowserConnectionCallback : IBrowserConnectionCallbackStub
    {
        public delegate void UserNavigationDelegate (long sessionId, string url, global::Android.OS.Bundle extras);

        public UserNavigationDelegate Handler { get; set; }

        public override void OnUserNavigation (long sessionId, string url, global::Android.OS.Bundle extras)
        {
            var h = Handler;
            if (h != null)
                h (sessionId, url, extras);
        }
    }

}


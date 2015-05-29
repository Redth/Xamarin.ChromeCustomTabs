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

        #region IBrowserConnectionCallback implementation
        public override void OnUserNavigation (long sessionId, string url, global::Android.OS.Bundle extras)
        {
            var h = Handler;
            if (h != null)
                h (sessionId, url, extras);
        }
        #endregion
//        #region IInterface implementation
//        public global::Android.OS.IBinder AsBinder ()
//        {
//            return  this.AsBinder ();
//        }
//        #endregion
    }

}


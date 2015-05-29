using System;
using Android.App;
using Android.OS;
using Android.Content;

namespace Xamarin.Android.CustomChromeTabs
{
    [Service]
    public class KeepAliveService : Service
    {
        private static Binder binder = new Binder();

        public override IBinder OnBind (Intent intent) 
        {
            return binder;
        }
    }
}


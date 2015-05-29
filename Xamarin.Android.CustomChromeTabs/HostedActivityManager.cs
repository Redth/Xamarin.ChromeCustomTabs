using System;
using Org.Chromium.Chrome.Browser.Hosted;
using Android.Content;
using System.Collections.Generic;
using Android.App;
using Android.Runtime;

namespace Xamarin.Android.CustomChromeTabs
{
    public class HostedActivityManager 
    {
        internal const string CHROME_PACKAGE = "com.chrome.dev";
        const string CHROME_SERVICE_CLASS_NAME = "org.chromium.chrome.browser.hosted.ChromeConnectionService";
        const string EXTRA_HOSTED_SESSION_ID = "hosted:session_id";
        const string EXTRA_HOSTED_KEEP_ALIVE = "hosted:keep_alive";

        public Activity Context {
            get ; private set;
        }

        List<Action> serviceActions;

        IBrowserConnectionService connectionService;
        bool serviceConnected;

        bool shouldRebind;
        IServiceConnection connection;

        bool bindHasBeenCalled;
        BrowserConnectionCallback browserConnectionCallback;
        long sessionId;

        public delegate void UserNavigationDelegate (long sessionId, string url, global::Android.OS.Bundle extras);
        public event UserNavigationDelegate UserNavigation;

        public HostedActivityManager (Activity context) 
        {
            Context = context;
            serviceActions = new List<Action> ();
            sessionId = -1;
            connection = new HostedServiceConnection {
                OnServiceConnectedHandler = (name, service) => {
                    if (browserConnectionCallback == null) {
                        browserConnectionCallback = new BrowserConnectionCallback {
                            Handler = (sessionId, url, extras) => {
                                var evt = UserNavigation;
                                if (evt != null)
                                    evt (sessionId, url, extras);
                            }
                        };
                    }

                    //var svcBinder = service as BrowserConnectionServiceBinder;
                    connectionService = IBrowserConnectionServiceStub.AsInterface (service); //IBrowserConnectionService.Stub.AsInterface (service); // svcBinder.GetSvc (); // IBrowserConnectionService.AsInterface(service);
                    try {
                        if (browserConnectionCallback != null) 
                            connectionService.FinishSetup (browserConnectionCallback);
                        sessionId = connectionService.NewSession ();
                    } catch (global::Android.OS.RemoteException e) {
                        sessionId = -1;
                        connectionService = null;
                        return;
                    }
                    serviceConnected = true;
                    foreach (var a in serviceActions) {
                        Context.RunOnUiThread (a);
                    }
                    serviceActions.Clear ();                  
                },
                OnServiceDisconnectedHandler = (name) => {

                }
            };
        }

        public bool BindService ()
        {
            bindHasBeenCalled = true;
            shouldRebind = true;
            var intent = new Intent();
            intent.SetClassName (CHROME_PACKAGE, CHROME_SERVICE_CLASS_NAME);
            bool available;
            try {
                available = Context.BindService (
                    intent, connection, Bind.AutoCreate | Bind.WaivePriority);
            } catch (Java.Lang.SecurityException e) {
                return false;
            }
            return available;
        }

        public void UnbindService ()
        {
            shouldRebind = false;
            Context.UnbindService (connection);
        }

        public bool Warmup ()
        {
            enqueueAction(() => {
                try {
                    connectionService.Warmup (0);
                } catch (global::Android.OS.RemoteException e) {
                    // Nothing
                }
            });
            return true;
        }

        public bool MayLaunchUrl (string url, List<string> otherLikelyUrls) 
        {
            var otherLikelyBundles = new List<global::Android.OS.Bundle> ();

            if (otherLikelyUrls != null) {
                foreach (var otherUrl in otherLikelyUrls) {
                    var bundle = new global::Android.OS.Bundle ();
                    bundle.PutString ("url", otherUrl);
                    otherLikelyBundles.Add (bundle);
                }
            }
            enqueueAction (() => {
                try {
                    var ja = new global::Android.Runtime.JavaList (otherLikelyBundles);
                    connectionService.MayLaunchUrl (sessionId, url, null, ja);
                } catch (global::Android.OS.RemoteException e) {
                    // Nothing
                }
            });
            return true;
        }

        public void LoadUrl (string url, HostedUIBuilder uiBuilder) 
        {
            var intent = uiBuilder.GetIntent ();
            var startBundle = uiBuilder.GetStartBundle ();

            intent.SetData (global::Android.Net.Uri.Parse (url));
            intent.PutExtra (EXTRA_HOSTED_SESSION_ID, sessionId);

            var keepAliveIntent = new Intent ();
            keepAliveIntent.SetClassName (
                Context.PackageName, Java.Lang.Class.FromType (typeof(KeepAliveService)).CanonicalName);
            
            intent.PutExtra(EXTRA_HOSTED_KEEP_ALIVE, keepAliveIntent);
            // The service needs to be reachable to get a sessionID, which is
            // required to connect to the KeepAlive service. We don't want users to
            // have to bind to the service manually, so do it for them here.
            if (!bindHasBeenCalled) {
                BindService ();
            }
            enqueueAction (() => {
                // If bindService() has not been called before, the
                // sessionId is unknown up to this point.
                intent.PutExtra (EXTRA_HOSTED_SESSION_ID, sessionId);
                Context.StartActivity (intent, startBundle);
            });
        }

        void enqueueAction (Action action)
        {            
            if (serviceConnected)                
                Context.RunOnUiThread (action);
            else
                serviceActions.Add (action);           
        }
    }

}

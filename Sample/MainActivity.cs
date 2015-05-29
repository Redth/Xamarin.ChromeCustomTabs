using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics;
using Xamarin.Android.CustomChromeTabs;

namespace ChromeCustomTabs
{
    [Activity (Label = "ChromeCustomTabs", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Button button;
        Button warmup_button;
        Button may_launch_button;
        EditText edit;

        static HostedActivityManager hostedManager;

        protected override void OnCreate (Bundle bundle)
        {
            base.OnCreate (bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            edit = FindViewById<EditText> (Resource.Id.edit);
            button = FindViewById<Button> (Resource.Id.button);
            may_launch_button = FindViewById<Button> (Resource.Id.may_launch_button);
            warmup_button = FindViewById<Button> (Resource.Id.warmup_button);


            if (hostedManager == null) {
                hostedManager = new HostedActivityManager (this);
                hostedManager.UserNavigation += (sessionId, url, extras) => {
                    Console.WriteLine ("UserNavigation: {0}, {1}", sessionId, url);
                };
            }
            
            warmup_button.Click += (sender, e) => {

                // Prepare
                hostedManager.BindService ();
                hostedManager.Warmup ();

            };

            may_launch_button.Click += (sender, e) => {

                // Notify the web view we may load a url so it might want to preload it
                var url = edit.Text;
                hostedManager.MayLaunchUrl (url, null);
            };

            button.Click += (sender, e) => {
                var url = edit.Text;

                var uiBuilder = new HostedUIBuilder ();

                // Xamarin Blue!
                uiBuilder.SetToolbarColor (Color.Argb (255, 52, 152, 219));

                // Setup our menu and action bar items
                PrepareMenuItems(uiBuilder);
                PrepareActionButton(uiBuilder);

                // Customize animations
                uiBuilder.SetStartAnimations (this, Resource.Animation.slide_in_right, Resource.Animation.slide_out_left);
                uiBuilder.SetExitAnimations (this, Resource.Animation.slide_in_left, Resource.Animation.slide_out_right);

                // Load the url!
                hostedManager.LoadUrl (url, uiBuilder);
            };

            if (Intent != null && Intent.HasExtra ("hug")) {
                Toast.MakeText (this, "Aww, thanks!", ToastLength.Long).Show ();
            }
        }


        void PrepareMenuItems (HostedUIBuilder uiBuilder) 
        {
            var menuIntent = new Intent ();
            menuIntent.SetClass (ApplicationContext, typeof (MainActivity));
            menuIntent.PutExtra ("hug", true);

            // Optional animation configuration when the user clicks menu items.
            var menuBundle = ActivityOptions.MakeCustomAnimation (this, Android.Resource.Animation.SlideInLeft,
                Android.Resource.Animation.SlideOutRight).ToBundle ();
            var pi = PendingIntent.GetActivity (ApplicationContext, 0, menuIntent, 0, menuBundle);
            uiBuilder.AddMenuItem ("Hug a Monkey", pi);
        }

        void PrepareActionButton(HostedUIBuilder uiBuilder) 
        {
            // An example intent that sends an email.
            Intent actionIntent = new Intent(Intent.ActionSend);
            actionIntent.SetType("*/*");
            actionIntent.PutExtra (Intent.ExtraEmail, "support@xamarin.com");
            actionIntent.PutExtra (Intent.ExtraSubject, "Help me make awesome apps!");
            PendingIntent pi = PendingIntent.GetActivity (ApplicationContext, 0, actionIntent, 0);
            Bitmap icon = BitmapFactory.DecodeResource (Resources, Resource.Drawable.Icon);
            uiBuilder.SetActionButton (icon, pi);
        }

    }
}



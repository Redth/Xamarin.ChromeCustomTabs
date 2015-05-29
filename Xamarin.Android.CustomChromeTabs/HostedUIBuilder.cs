using System;
using Android.Content;
using System.Collections.Generic;
using Android.App;
using Android.Graphics;

namespace Xamarin.Android.CustomChromeTabs
{
    public class HostedUIBuilder
    {
        const string EXTRA_HOSTED_MODE = "com.android.chrome.append_task";
        const string EXTRA_HOSTED_EXIT_ANIMATION_BUNDLE = "hosted:exit_animation_bundle";
        const string EXTRA_HOSTED_TOOLBAR_COLOR = "hosted:toolbar_color";
        const string EXTRA_HOSTED_MENU_ITEMS = "hosted:menu_items";
        const string EXTRA_HOSTED_ACTION_BUTTON_BUNDLE = "hosted:action_button_bundle";
        const string KEY_HOSTED_ICON = "hosted:icon";
        const string KEY_HOSTED_MENU_TITLE = "hosted:menu_title";
        const string KEY_HOSTED_PENDING_INTENT = "hosted:pending_intent";

        Intent intent;
        global::Android.OS.Bundle startBundle;
        List<global::Android.OS.Bundle> menuItems;

        public HostedUIBuilder () 
        {
            intent = new Intent();
            startBundle = null;
            menuItems = new List<global::Android.OS.Bundle> ();
            intent.PutExtra (EXTRA_HOSTED_MODE, true);
            intent.SetPackage (HostedActivityManager.CHROME_PACKAGE);
            intent.SetAction (Intent.ActionView);
        }

        /**
     * Sets the toolbar color.
     *
     * @param color The color.
     */
        public HostedUIBuilder SetToolbarColor (int color)
        {
            intent.PutExtra (EXTRA_HOSTED_TOOLBAR_COLOR, color);
            return this;
        }

        /**
     * Adds a menu item.
     *
     * @param label Menu label.
     * @param pendingIntent Pending intent delivered when the menu item is clicked.
     */
        public HostedUIBuilder AddMenuItem (string label, PendingIntent pendingIntent) 
        {
            var bundle = new global::Android.OS.Bundle ();
            bundle.PutString (KEY_HOSTED_MENU_TITLE, label);
            bundle.PutParcelable (KEY_HOSTED_PENDING_INTENT, pendingIntent);
            menuItems.Add (bundle);
            return this;
        }

        /**
     * Set the action button.
     *
     * @param bitmap The icon.
     * @param pendingIntent pending intent delivered when the button is clicked.
     */
        public HostedUIBuilder SetActionButton (Bitmap bitmap, PendingIntent pendingIntent) 
        {
            var bundle = new global::Android.OS.Bundle ();
            bundle.PutParcelable (KEY_HOSTED_ICON, bitmap);
            bundle.PutParcelable (KEY_HOSTED_PENDING_INTENT, pendingIntent);
            intent.PutExtra (EXTRA_HOSTED_ACTION_BUTTON_BUNDLE, bundle);
            return this;
        }

        /**
     * Sets the start animations,
     *
     * @param context Application context.
     * @param enterResId Resource ID of the "enter" animation for the browser.
     * @param exitResId Resource ID of the "exit" animation for the application.
     */
        public HostedUIBuilder SetStartAnimations(Context context, int enterResId, int exitResId) 
        {
            startBundle = ActivityOptions.MakeCustomAnimation (context, enterResId, exitResId).ToBundle ();
            return this;
        }

        /**
     * Sets the exit animations,
     *
     * @param context Application context.
     * @param enterResId Resource ID of the "enter" animation for the application.
     * @param exitResId Resource ID of the "exit" animation for the browser.
     */
        public HostedUIBuilder SetExitAnimations(Context context, int enterResId, int exitResId) 
        {
            var bundle = ActivityOptions.MakeCustomAnimation (context, enterResId, exitResId).ToBundle ();
            intent.PutExtra (EXTRA_HOSTED_EXIT_ANIMATION_BUNDLE, bundle);
            return this;
        }

        internal Intent GetIntent () 
        {
            intent.PutParcelableArrayListExtra (EXTRA_HOSTED_MENU_ITEMS, menuItems.ToArray ());
            return intent;
        }

        internal global::Android.OS.Bundle GetStartBundle () 
        {
            return startBundle;
        }
    }
}


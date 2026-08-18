using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;
using CommunityToolkit.Maui.Core.Services;
using Waybon.App.Platforms.Android.Services;

namespace Waybon.App.Platforms.Android
{
    [
        Activity
        (
            Theme = "@style/Maui.MainTheme.NoActionBar",
            MainLauncher = true,
            LaunchMode = LaunchMode.SingleTop,
            ConfigurationChanges = ConfigChanges.ScreenSize |
                                   ConfigChanges.Orientation |
                                   ConfigChanges.UiMode |
                                   ConfigChanges.ScreenLayout |
                                   ConfigChanges.SmallestScreenSize |
                                   ConfigChanges.Density
        )
    ]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SupportFragmentManager.RegisterFragmentLifecycleCallbacks(new FragmentLifecycleManager(new ImmersiveDialogFragmentService()), false);
            ApplyImmersiveMode();
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus)
            {
                ApplyImmersiveMode();
            }
        }

        private void ApplyImmersiveMode()
        {
            if (Window == null)
            {
                return;
            }

            WindowCompat.SetDecorFitsSystemWindows(Window, false);

            var controller = WindowCompat.GetInsetsController(Window, Window.DecorView);
            if (controller != null)
            {
                controller.Hide(WindowInsetsCompat.Type.SystemBars());
                controller.SystemBarsBehavior = WindowInsetsControllerCompat.BehaviorShowTransientBarsBySwipe;
            }
        }
    }
}
using Android.Content;
using Android.OS;
using AndroidX.Fragment.App;
using CommunityToolkit.Maui.Core;

namespace Waybon.App.Platforms.Android.Services;

internal sealed class ImmersiveDialogFragmentService : IDialogFragmentService
{
    public void OnFragmentAttached(FragmentManager fm, Fragment f, Context context)
    {

    }

    public void OnFragmentCreated(FragmentManager fm, Fragment f, Bundle? savedInstanceState)
    {
        
    }

    public void OnFragmentDestroyed(FragmentManager fm, Fragment f)
    {
        
    }

    public void OnFragmentDetached(FragmentManager fm, Fragment f)
    {
        
    }

    public void OnFragmentPaused(FragmentManager fm, Fragment f)
    {
        
    }

    public void OnFragmentPreAttached(FragmentManager fm, Fragment f, Context context)
    {
        
    }

    public void OnFragmentPreCreated(FragmentManager fm, Fragment f, Bundle? savedInstanceState)
    {
        
    }

    public void OnFragmentResumed(FragmentManager fm, Fragment f)
    {
        
    }

    public void OnFragmentSaveInstanceState(FragmentManager fm, Fragment f, Bundle outState)
    {
        
    }

    public void OnFragmentStarted(FragmentManager fm, Fragment f)
    {
        if (f is not DialogFragment dialogFragment)
        {
            return;
        }

        if (dialogFragment.Dialog == null || dialogFragment.Dialog.Window == null)
        {
            return;
        }

        var window = dialogFragment.Dialog.Window;
        if (!OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            return;
        }

        if (!OperatingSystem.IsAndroidVersionAtLeast(35))
        {
            window.SetDecorFitsSystemWindows(false);
        }

        if (window.InsetsController != null)
        {
            window.InsetsController.Hide(global::Android.Views.WindowInsets.Type.SystemBars());
            window.InsetsController.SystemBarsBehavior = (int)global::Android.Views.WindowInsetsControllerBehavior.ShowTransientBarsBySwipe;
        }
    }

    public void OnFragmentStopped(FragmentManager fm, Fragment f)
    {
        
    }

    public void OnFragmentViewCreated(FragmentManager fm, Fragment f, global::Android.Views.View v, Bundle? savedInstanceState)
    {
        
    }

    public void OnFragmentViewDestroyed(FragmentManager fm, Fragment f)
    {
        
    }
}
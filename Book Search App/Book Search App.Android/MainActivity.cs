using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

using FFImageLoading.Config;
using System.Net.Http;
using FFImageLoading;

namespace Book_Search_App.Droid
{
    [Activity(Label = "Book_Search_App", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Configuration config = new Configuration();
            config.AllowUpscale = true;
            config.HttpClient = new HttpClient();
            ImageService.Instance.Initialize(config);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: false);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.InitImageViewHandler();
            
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
using Android.App;
using Android.OS;
using MTBrokerMobile.DependencyServiceInterfaces;
using MTBrokerMobile.Droid.DependencyServImpl;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(ExternalStorageService))]
namespace MTBrokerMobile.Droid.DependencyServImpl
{
    public class ExternalStorageService : IExternalStorageService
    {
        //Android.App.Application.Context.GetExternalFilesDir("")
        public string GetExternalStoragePath()
        {



            /*Application.Context.GetExternalFilesDir(null)*/

            //Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);

            //return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath;

            return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
        }

        public bool IsExternalStoragePathWritable() => Android.OS.Environment.MediaMounted.Equals(Android.OS.Environment.ExternalStorageState);
    }
}
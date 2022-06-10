namespace MTBrokerMobile.DependencyServiceInterfaces
{
    public interface IExternalStorageService
    {
        string GetExternalStoragePath();

        bool IsExternalStoragePathWritable();
    }
}

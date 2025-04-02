namespace CustomerApp.Services
{
    public interface Singleton<T> where T: Singleton<T>
    {
        public static abstract T Instance { get; }
    }
}

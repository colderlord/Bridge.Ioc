namespace Bridge.Ioc
{
    public interface ILazy<T> where T: class
    {
        T Value();
    }
}

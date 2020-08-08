namespace affolterNET.Data.Interfaces
{
    public interface IViewFactory
    {
        IViewBase Get<T>()
            where T : IViewBase;
    }
}
namespace affolterNET.Data.Interfaces
{
    public interface IViewBase
    {
        string GetSelectCommand(int maxCount = 1000, params string[] excludedColumns);
    }
}
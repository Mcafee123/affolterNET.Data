using System.Data.SqlClient;
using System.Reflection;
using affolterNET.Data.Interfaces.SessionHandler;
using DbUp.Engine.Output;

namespace affolterNET.Data.DbUp.Services;

public class HistoryLog: IUpgradeLog
{
    private readonly IHistorySaver _historySaver;
    private readonly SqlConnection _connection;

    public HistoryLog(IHistorySaver historySaver, string connString)
    {
        _historySaver = historySaver;
        _connection = new SqlConnection(connString);
    }
    
    public void WriteInformation(string format, params object[] args)
    {
        var text = format;
        if (format.StartsWith("Executing Database Server script '{0}'"))
        {
            if (args.Length < 1 || args.FirstOrDefault() == null)
            {
                throw new InvalidOperationException("filename not found");
            }

            var fileName = args.First().ToString();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new InvalidOperationException("first argument with fileName was empty");
            }

            var contents = ReadContents(fileName);
            _historySaver.SaveHistory(fileName, contents).GetAwaiter().GetResult();
        }
    }

    public void WriteError(string format, params object[] args)
    { }

    public void WriteWarning(string format, params object[] args)
    { }

    private string ReadContents(string fileName)
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly == null)
        {
            throw new InvalidOperationException("could not get entry assembly");
        }

        using var stream = assembly.GetManifestResourceStream(fileName);
        if (stream == null)
        {
            throw new InvalidOperationException($"file stream {fileName} was null");
        }
        using var reader = new StreamReader(stream);
        var result = reader.ReadToEnd();
        return result;
    }
}
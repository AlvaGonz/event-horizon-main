namespace Services.Localization
{
    public interface ILocalization
    {
        string GetString(string key);
        string GetString(string key, params object[] args);
        string GetText(string key);
        string GetText(string key, params object[] args);
        string Localize(string key);
    }
}

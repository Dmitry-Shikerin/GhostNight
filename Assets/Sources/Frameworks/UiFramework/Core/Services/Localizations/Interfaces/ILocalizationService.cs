namespace Sources.Frameworks.UiFramework.Core.Services.Localizations.Interfaces
{
    public interface ILocalizationService
    {
        void Translate();
        string GetText(string key);
    }
}
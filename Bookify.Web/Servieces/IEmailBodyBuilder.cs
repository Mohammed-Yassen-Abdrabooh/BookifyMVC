namespace Bookify.Web.Servieces
{
    public interface IEmailBodyBuilder
    {
        string GetEmailBody(string template, Dictionary<string, string> placeholders);
    }
}

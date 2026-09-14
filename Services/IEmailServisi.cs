namespace ProblemTalepTakipSistemiHalkbank.Services
{
    public interface IEmailServisi
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
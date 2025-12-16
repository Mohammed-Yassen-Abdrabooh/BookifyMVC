using Microsoft.AspNetCore.Identity.UI.Services;

namespace Bookify.Web.Tasks
{
    public class HangFireTasks
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IWhatsAppClient _whatsAppClient;
        private readonly IEmailBodyBuilder _emailBodyBuilder;
        private readonly IEmailSender _emailSender;

        public HangFireTasks(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment, IWhatsAppClient whatsAppClient, IEmailBodyBuilder emailBodyBuilder, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
            _whatsAppClient = whatsAppClient;
            _emailBodyBuilder = emailBodyBuilder;
            _emailSender = emailSender;
        }



        // This Method to Prepare Expiration Alert for Subscribers whose Subscription will be expired in 5 Days
        public async Task PrepareExpirationAlert()
        {



            var subscribers = _dbContext.Subscribers.Include(s => s.Subscriptions)
                                                    .Where(s => s.Subscriptions.Any() && s.Subscriptions.OrderByDescending(x => x.EndDate).First().EndDate.Date == DateTime.Today.AddDays(5))
                                                    .ToList();

            foreach (var subscriber in subscribers)
            {
                var endDate = subscriber.Subscriptions
                                        .OrderByDescending(x => x.EndDate)
                                        .First().EndDate
                                        .ToString("dd MMM, yyyy");
                //Send Welcome Email to Subscriber

                var placeholders = new Dictionary<string, string>()
                {
                    { "imageUrl" , "https://res.cloudinary.com/yassen-bookify/image/upload/v1765870839/Best_Before_ususq2.png" },
                    { "header" , $"Hello {subscriber.FirstName} {subscriber.LastName}" },
                    { "body" , $"Your subscription will be expired by {endDate} 😔" }
                };

                var body = _emailBodyBuilder.GetEmailBody(MailTemplates.Notification, placeholders);
                var email = _webHostEnvironment.IsDevelopment() ? "mohammedyassen.pc@gmail.com" : subscriber.Email;
                // Need To Sent Direct Not in Enqueued Method
                await _emailSender.SendEmailAsync(email, "Bookify Subscription Expiration", body);


                // Use Hangfire to Schedule Email in Background Job after 1 Minute
                //BackgroundJob.Schedule(() => _emailSender.SendEmailAsync(email, "Renew Subscription for Bookify", body),TimeSpan.FromMinutes(1));

                //Send Welcome Message to Subscriber using WhatsApp Cloud Api
                if (subscriber.HasWhatsApp)
                {
                    // use New Created Template with Parameter in Meta WhatsApp Cloud Api and How Send Variables in it, While using "WhatsAppApiClient" Package by Elhelaly
                    var components = new List<WhatsAppComponent>()
                    {
                        new WhatsAppComponent
                        {
                            Type = "body",
                            Parameters = new List<object>()
                            {
                                new WhatsAppTextParameter { Text = endDate }
                            }
                        }
                    };

                    var mobileNumber = _webHostEnvironment.IsDevelopment() ? "01094046114" : subscriber.MobileNumber;
                    await _whatsAppClient.SendMessage($"2{mobileNumber}", WhatsAppLanguageCode.English, WhatsAppTemplates.BookifyExpireSubscription, components);
                    // Use Hangfire to Send WhatsApp Message in
                    //BackgroundJob.Enqueue(() => _whatsAppClient.SendMessage($"2{mobileNumber}", WhatsAppLanguageCode.English, WhatsAppTemplates.BookifyRenewSubscription, null));

                }
            }
        }
    }
}

using System.Text;
using System.Net.Mail;
using System.Net;
using DKW.DynamicDnsUpdater.Interface;

namespace DKW.DynamicDnsUpdater.Notification
{
    public class EmailNotification : INotification
	{
		/// <summary>
		/// Simple Send email 
		/// </summary>
		/// <param name="body"></param>
		public void Send(String body)
		{
			var fromAddress = new MailAddress(ConfigHelper.FromEmail);
			var toAddress = new MailAddress(ConfigHelper.ToEmail);
            String password = ConfigHelper.Password;
            String subject = ConfigHelper.Subject;

			using (var smtp = new SmtpClient
			{
				Host = ConfigHelper.Host,
				Port = Convert.ToInt32(ConfigHelper.Port),
				EnableSsl = true,
				DeliveryMethod = SmtpDeliveryMethod.Network,
				Credentials = new NetworkCredential(fromAddress.Address, password),
				Timeout = 20000
			})
			{
				using (var message = new MailMessage(fromAddress, toAddress)
				{
					BodyEncoding = Encoding.UTF8,
					SubjectEncoding = Encoding.UTF8,
					IsBodyHtml = true,
					Subject = subject,
					Body = body

				})
				{
					smtp.Send(message);
				} // using MailMessage IDisposable
			} // using SmtpClient IDisposable
		}
	}
}

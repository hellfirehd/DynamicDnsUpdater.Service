using DKW.DynamicDnsUpdater.Configuration;
using DKW.DynamicDnsUpdater.Interface;
using DKW.DynamicDnsUpdater.Models;
using Microsoft.Extensions.Options;
using System.Timers;

namespace DKW.DynamicDnsUpdater;

public class Worker : BackgroundService
{
	private readonly IEnumerable<IDnsProvider> _dnsProviders;
	private readonly ILogger<Worker> _logger;
	private readonly INotification _notification;
	private readonly DynamicDnsUpdaterOptions _options;

	private readonly System.Timers.Timer _updateTimer;
	private readonly System.Timers.Timer _montiorTimer;

	public Worker(IEnumerable<IDnsProvider> dnsProviders, ILogger<Worker> logger, INotification notification, IOptions<DynamicDnsUpdaterOptions> options)
	{
		_dnsProviders = dnsProviders;
		_logger = logger;
		_notification = notification;
		_options = options.Value;

		_updateTimer = new(Convert.ToDouble(_options.UpdateIntervalInMinutes * 1000 * 60));
		_updateTimer.Elapsed += UpdateTimer_Elapsed;

		_montiorTimer = new(Convert.ToDouble(_options.MonitorStatusInMinutes * 1000 * 60));
		_montiorTimer.Elapsed += MontiorTimer_Elapsed;
	}

	private void MontiorTimer_Elapsed(Object? sender, ElapsedEventArgs e) => RunMonitor();

	private void UpdateTimer_Elapsed(Object? sender, ElapsedEventArgs e) => RunUpdate();

	public override void Dispose()
	{
		_updateTimer.Elapsed -= UpdateTimer_Elapsed;
		_montiorTimer.Elapsed -= MontiorTimer_Elapsed;

		base.Dispose();
	}

	protected override Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			// First time running it no waiting 
			RunUpdate();

			// Configure the Timers and handlers
			_updateTimer.Start();
			_montiorTimer.Start();

			_logger.LogInformation("Service has started.");
		}

		return Task.CompletedTask;
	}

	/// <summary>
	/// Loop through the config files and update the IP
	/// </summary>
	public void RunUpdate()
	{

		// Loop through and update all domains
		foreach (var domain in _options.Domains)
		{
			try
			{
				// Check if it meets the minimal update settings (some DNS update system blocks your update to prevent flooding if you update too frequtently)
				var isUpdatable = domain.LastUpdatedDateTime.HasValue
					&& (domain.LastUpdatedDateTime.Value.AddMinutes(domain.MinimalUpdateIntervalInMinutes) < DateTime.UtcNow);

				// Check if we should force update with the preset days in config
				bool isForceUpdate = domain.LastUpdatedDateTime.HasValue
					&& domain.LastUpdatedDateTime < DateTime.UtcNow.AddDays(-_options.ForceUpdateInDays);

				var currentIpAddress = GetCurrentIpAddress();

				// Check if able to get IP from IpChecker
				if (currentIpAddress != null)
				{
					var lastIpAddress = domain.LastIpAddress?.Trim() ?? String.Empty;
					// Compare to last IP with current Ip
					bool ipAddressChanged = String.IsNullOrWhiteSpace(lastIpAddress) || (currentIpAddress.Trim() != lastIpAddress);

					// Update the IP
					if ((isUpdatable && isForceUpdate) || (isUpdatable && ipAddressChanged))
					{
						var dnsProvider = _dnsProviders.FirstOrDefault(p => p.Type == domain.ProviderType);
						if (dnsProvider == null)
						{
							_logger.LogError($"No DNS provider found for {domain.ProviderType}");
							continue;
						}

						var updatedId = dnsProvider.UpdateDns(domain.DomainName, currentIpAddress);

						// Successful with ID return, update information in the xml
						if (updatedId != null)
						{
							if (isForceUpdate)
								_logger.LogInformation("{Category}: Domain={Domain} - FORCE UPDATED provider successfully from IP={LastIpAddress} to IP={CurrentIpAddress} with ID={UpdateId}, passed {ForceUpdateInDays} days",
									LogCategoryType.DNS_UPDATE, domain.DomainName, domain.LastIpAddress, currentIpAddress, updatedId, _options.ForceUpdateInDays);
							else
								_logger.LogInformation("{Category}: Domain={Domain} - UPDATED provider successfully from IP={LastIpAddress} to IP={CurrentIpAddress} with ID={UpdatedId}",
									LogCategoryType.DNS_UPDATE, domain.DomainName, domain.LastIpAddress, currentIpAddress, updatedId);

							// Update the model                                     
							domain.HistoricalIPAddress.Add(currentIpAddress);
							domain.LastIpAddress = currentIpAddress;
							domain.LastUpdatedDateTime = DateTime.UtcNow;
							domain.ChangeStatusID = updatedId;   // For monitoring inteval timer to use
							domain.LastUpdatedReason = (isForceUpdate ? UpdateReasonType.FORCED : UpdateReasonType.CHANGED);

							_logger.LogInformation("{Category}: Domain={DomainName} - UPDATED XML configuration from {1} to {2}, LastUpdatedDateTime={3}, Reason={4}",
								LogCategoryType.DNS_UPDATE, domain.DomainName, String.Join(",", domain.HistoricalIPAddress), domain.LastIpAddress, domain.LastUpdatedDateTime, domain.LastUpdatedReason);

						}
						else
						{
							_logger.LogInformation("{Category}: Domain={DomainName} - NOT UPDATED because of unknown error with Provider={DnsProvider}",
								LogCategoryType.DNS_UPDATE, domain.DomainName, domain.DnsProvider);
						}
					}
					else
					{
						if ((!isForceUpdate) && (!ipAddressChanged))
							_logger.LogInformation(String.Format("Domain={0} - NOT UPDATED because IP={1} has not been changed", domain.DomainName, currentIpAddress), Meta.Enum.LogCategoryType.DNS_UPDATE.ToString());

						if (!isUpdatable)
							_logger.LogInformation(String.Format("Domain={0} - NOT UPDATED because update is too often, mininal interval={1} minute(s)", domain.DomainName, domain.MinimalUpdateIntervalInMinutes), Meta.Enum.LogCategoryType.DNS_UPDATE.ToString());
					}
				} // else


			} // try
			catch (Exception ex)
			{
				_logger.LogInformation(String.Format("Domain={0} - FATAL ERROR, Exception={1}", domain.DomainName, ex.ToString()), Meta.Enum.LogCategoryType.DNS_UPDATE.ToString());
			}

		}

	} // method



	/// <summary>
	/// Monitor the DNS update status after submitted to the provider
	/// </summary>
	public void RunMonitor()
	{
		List<Domain> notificationDomainList = new();

		// Loop through and update all domains
		foreach (var domain in _options.Domains.Where(x => x.ChangeStatusID != null))
		{
			try
			{
				var dnsProvider = _dnsProviders.FirstOrDefault(p => p.Type == domain.ProviderType);
				if (dnsProvider == null)
				{
					_logger.LogError($"No DNS provider found for {domain.ProviderType}");
					continue;
				}

				if (dnsProvider.CheckUpdateStatus(domain.DomainName) == ChangeStatusType.INSYNC)
				{
					// Update config file and reset model
					_options.UpdateChangeStatusInformation(domain.DomainName, String.Empty); // ChangeStatusID = Empty = Insync
					domain.ChangeStatusID = null;
					notificationDomainList.Add(domain); // For notification

					_logger.LogInformation(String.Format("Domain={0} - XML configuration ChangeStatusType Updated to {1}", domain.DomainName, Meta.Enum.ChangeStatusType.INSYNC.ToString()), Meta.Enum.LogCategoryType.STATUS_MONITOR.ToString());
				}
				else
					_logger.LogInformation(String.Format("Domain={0} - ChangeStatus={1}", domain.DomainName, Meta.Enum.ChangeStatusType.PENDING.ToString()), Meta.Enum.LogCategoryType.STATUS_MONITOR.ToString());
			}
			catch (Exception ex)
			{
				_logger.LogInformation(String.Format("Domain={0} -  FATAL ERROR during check change status, Exception={1}", domain.DomainName, ex.ToString()), Meta.Enum.LogCategoryType.STATUS_MONITOR.ToString());
			}
		}

		// Group send only 1 email for all completed domains
		if (notificationDomainList.Count > 0)
			SendNotification(notificationDomainList);

	}

	/// <summary>
	/// Get Ip Address from IpCheckers (If one fails, automatically try next one)
	/// </summary>
	/// <returns></returns>
	private string GetCurrentIpAddress()
	{
		// Loop through the whole list, if one fails go to next one
		foreach (IpCheckerModel model in _options.IpCheckers)
		{
			String currentIpAddress;
			try
			{
				currentIpAddress = model.IpAddressChecker.GetCurrentIpAddress(model.IpCheckerUrl, model.Client);
				_logger.LogInformation("{Category}: IpChecker={IpCheckerUrl}, IP={currentIpAddress}",
					LogCategoryType.IP_CHECKER, model.IpCheckerUrl, currentIpAddress);
			}
			catch (Exception ex)
			{
				// Suppress the error, log it and continue to next one
				_logger.LogInformation(String.Format("Fail to get the IP from IpChecker={0}, Exception={1}", model.IpCheckerUrl, ex.ToString()), Meta.Enum.LogCategoryType.IP_CHECKER.ToString());
			}

			// Exit the loop and return the IP 
			if (currentIpAddress != null)
				break;

		}

		// All failed
		if (currentIpAddress == null)
		{
			_logger.LogInformation("FATAL ERROR, fail to get the IP from All IpCheckers.", Meta.Enum.LogCategoryType.IP_CHECKER.ToString());
			throw new InvalidOperationException();
		}

		return currentIpAddress;
	}


	/// <summary>
	/// Send notification
	/// </summary>
	/// <param name="domain"></param>
	/// <param name="ip"></param>
	private void SendNotification(List<Domain> domainModelList)
	{
		try
		{
			string body = "The following domains have been updated:<br/><br/>";
			foreach (Domain item in domainModelList)
			{
				body += String.Format("Domain={0}, IP from {1} to IP={2}, Local Time={3}, Reason={4}", item.DomainName, item.HistoricalIpAddress, item.LastIpAddress, item.LastUpdatedDateTime.ToLocalTime(), Enum.GetName(typeof(Meta.Enum.UpdateReasonType), item.LastUpdatedReason)) + "<br/>";
			}

			// Send email (only email is supported for now, can add Facebook or others in future)
			INotification notification = _container.Resolve<INotification>(Meta.Enum.NotificationType.EMAIL.ToString());
			notification.Send(body);

			_logger.LogInformation(String.Format("Notification has been sent successfully"), Meta.Enum.LogCategoryType.NOTIFICATION.ToString());
		}
		catch (Exception ex)
		{
			_logger.LogInformation(String.Format("Fail to send notification, Exception={0}.", ex.ToString()), Meta.Enum.LogCategoryType.NOTIFICATION.ToString());

		}

	}


}

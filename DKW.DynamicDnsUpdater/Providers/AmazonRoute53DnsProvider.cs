﻿using Amazon.Route53;
using Amazon.Route53.Model;
using Amazon.Runtime;
using DKW.DynamicDnsUpdater.Interface;

namespace DKW.DynamicDnsUpdater.Providers
{
    public class AmazonRoute53DnsProvider : IDnsProvider
	{
		private IAmazonRoute53? _route53Client = null;
		private String _accessID;
		private String _secretKey;
		private String _providerUrl;

		/// <summary>
		/// Get the Amazon Route 53 Client
		/// </summary>
		private IAmazonRoute53 AmazonRoute53Client
		{
			get
			{

				if (_accessID == null || _secretKey == null || _providerUrl == null)
					throw new Exception("AmazonRoute53DnsProvider cannot be initialized.");

				// Check if it has been created already
				if (_route53Client != null)
					return _route53Client;
				else
				{
					// Define credentials
					Amazon.Util.ProfileManager.RegisterProfile("DnsUpdaterName", _accessID, _secretKey);
					AWSCredentials credential = new StoredProfileAWSCredentials("DnsUpdaterName");

					AmazonRoute53Config config = new AmazonRoute53Config();
					config.ServiceURL = _providerUrl;

					// Create an Amazon Route 53 client object
					_route53Client = new AmazonRoute53Client(credential, config);
					return _route53Client;
				}

			}

		}


		/// <summary>
		/// Update Route 53 DNS record
		/// </summary>
		/// <param name="accessID"></param>
		/// <param name="secretKey"></param>
		/// <param name="providerUrl"></param>
		/// <param name="domainName"></param>
		/// <param name="hostZoneId"></param>
		/// <param name="newIPaddress"></param>
		/// <returns></returns>
		public String UpdateDns(String accessID, String secretKey, String providerUrl, String domainName, String hostZoneId, String newIPaddress)
		{

            String changeRequestId = null;

			// Assign parameters
			_accessID = accessID;
			_secretKey = secretKey;
			_providerUrl = providerUrl;

			// Create a resource record set change batch
			ResourceRecordSet recordSet = new ResourceRecordSet()
			{
				Name = domainName,
				TTL = 60,
				Type = RRType.A,
				ResourceRecords = new List<ResourceRecord> { new ResourceRecord { Value = newIPaddress } }
			};

			Change change1 = new Change()
			{
				ResourceRecordSet = recordSet,
				Action = ChangeAction.UPSERT        // Note: UPSERT is used
			};

			ChangeBatch changeBatch = new ChangeBatch()
			{
				Changes = new List<Change> { change1 }
			};

			// Update the zone's resource record sets
			ChangeResourceRecordSetsRequest recordsetRequest = new ChangeResourceRecordSetsRequest()
			{
				HostedZoneId = hostZoneId,
				ChangeBatch = changeBatch
			};

			ChangeResourceRecordSetsResponse recordsetResponse = AmazonRoute53Client.ChangeResourceRecordSets(recordsetRequest);
			changeRequestId = recordsetResponse.ChangeInfo.Id;

			return changeRequestId;

		} // method



		/// <summary>
		///  AmazonRoute53 takes sevveral minutes to propagate through all the DNS servers, Status is Pending after submit
		/// </summary>
		/// <param name="id"></param>
		public Meta.Enum.ChangeStatusType CheckUpdateStatus(String accessID, String secretKey, String providerUrl, String id)
		{
			if (String.IsNullOrEmpty(id))
				return Meta.Enum.ChangeStatusType.INSYNC;

			// Assign parameters
			_accessID = accessID;
			_secretKey = secretKey;
			_providerUrl = providerUrl;

			// Monitor the change status
			GetChangeRequest changeRequest = new GetChangeRequest(id);

			if (AmazonRoute53Client.GetChange(changeRequest).ChangeInfo.Status == ChangeStatus.PENDING)
				return Meta.Enum.ChangeStatusType.PENDING;
			else
				return Meta.Enum.ChangeStatusType.INSYNC;

		} // method


	}
}

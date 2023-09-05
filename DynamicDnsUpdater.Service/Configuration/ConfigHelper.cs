using System;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using DynamicDnsUpdater.Service.Encryption;
using DynamicDnsUpdater.Service.Models;

namespace DynamicDnsUpdater.Service.Configuration
{
	public class ConfigHelper
    {
        // Basic encryption key
        public const String EncryptionKey = "bsBg34asdfi28B9N3489taduiBC23sdJNKIJFadSIUsaaFUU1344IDdsfF535fhB";

        // Helper for App.config
        public static String XmlConfigFileName { get { return ConfigurationManager.AppSettings["XmlConfigFileName"]; } }

        public static String UpdateIntervalInMinutes { get { return ConfigurationManager.AppSettings["UpdateIntervalInMinutes"]; } }
        public static String MonitorStatusInMinutes { get { return ConfigurationManager.AppSettings["MonitorStatusInMinutes"]; } }
        public static String ClientTimeoutInMinutes { get { return ConfigurationManager.AppSettings["ClientTimeoutInMinutes"]; } }
        public static String ForceUpdateInDays { get { return ConfigurationManager.AppSettings["ForceUpdateInDays"]; } }

        public static String FromEmail { get { return ConfigurationManager.AppSettings["FromEmail"]; } }
        public static String ToEmail { get { return ConfigurationManager.AppSettings["ToEmail"]; } }

        public static String Password {
            get {
                if (EnablePasswordEncryption)
                    return Des3.Decrypt(ConfigurationManager.AppSettings["Password"], EncryptionKey);
                else
                    return ConfigurationManager.AppSettings["Password"];
            }
        }

        public static String Subject { get { return ConfigurationManager.AppSettings["Subject"]; } }
        public static String Host { get { return ConfigurationManager.AppSettings["Host"]; } }
        public static String Port { get { return ConfigurationManager.AppSettings["Port"]; } }
        public static Boolean EnablePasswordEncryption { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["EnablePasswordEncryption"]); } }

        /// <summary>
        /// Load Config from XML to objects
        /// </summary>
        /// <returns></returns>
        public static XmlConfig LoadConfig()
        {
            XmlConfig config = null;
            XmlSerializer ser = new XmlSerializer(typeof(XmlConfig));
            using (XmlReader reader = XmlReader.Create(AppDomain.CurrentDomain.BaseDirectory + XmlConfigFileName)) {
                config = (XmlConfig)ser.Deserialize(reader);
            }

            return config;
        }

        /// <summary>
        /// Update several domain info in XmlConfig using XPath  (Called by Update Dns)
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="dateTimeinUTC"></param>
        /// <returns></returns>
        public static Boolean UpdateDomainInformation(DomainModel domain)
        {
            // Use Xpath to update (debatable - not deserialize the xml?)
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + XmlConfigFileName);
            XmlNode root = xmlDocument.DocumentElement;

            // Find the matching domain name using Xpath and update the datetime
            XmlNode node = root.SelectSingleNode("//Domains/Domain[DomainName=\"" + domain.DomainName + "\"]");

            if (node != null) {
                node["LastIpAddress"].InnerText = domain.LastIpAddress;
                node["LastUpdatedDateTime"].InnerText = domain.LastUpdatedDateTime.ToString("o");    // UTC timestamp in ISO 8601 format
                node["HistoricalIPAddress"].InnerText = domain.HistoricalIpAddress;
                node["LastUpdatedReason"].InnerText = Enum.GetName(typeof(Meta.Enum.UpdateReasonType), domain.LastUpdatedReason);

                // Need to use this to fix carriage return problem if InnerText is an empty string
                XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
                using (XmlWriter writer = XmlWriter.Create(AppDomain.CurrentDomain.BaseDirectory + XmlConfigFileName, settings)) {
                    xmlDocument.Save(writer);
                }

                return true;
            } else
                return false;
        }

        /// <summary>
        /// Update ChangeStatus in XmlConfig using XPath (called by Monitor Status Change)
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="changeStatus"></param>
        /// <returns></returns>
        public static Boolean UpdateChangeStatusInformation(String domainName, String changeStatusId)
        {
            // Use Xpath to update (debatable - not deserialize the xml?)
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + XmlConfigFileName);
            XmlNode root = xmlDocument.DocumentElement;

            // Find the matching domain name using Xpath and update the datetime
            XmlNode node = root.SelectSingleNode("//Domains/Domain[DomainName=\"" + domainName + "\"]");

            if (node != null) {
                node["ChangeStatusID"].InnerText = changeStatusId;

                // Need to use this to fix carriage return problem if InnerText is an empty string
                XmlWriterSettings settings = new XmlWriterSettings { Indent = true };
                using (XmlWriter writer = XmlWriter.Create(AppDomain.CurrentDomain.BaseDirectory + XmlConfigFileName, settings)) {
                    xmlDocument.Save(writer);
                }

                return true;
            } else
                return false;
        }
    }
}

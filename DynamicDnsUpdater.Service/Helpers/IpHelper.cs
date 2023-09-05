using System.Text.RegularExpressions;

namespace DynamicDnsUpdater.Service.Helpers
{
	public class IpHelper
    {
        // Validate if this is a valid IPV4 address
        public static System.Boolean IpAddressV4Validator(System.String ipString)
        {
            // Using regular expression
            var pattern = @"^((^|\.)((1?[1-9]?|10|2[0-4])\d|25[0-5])){4}$";
            Regex regex = new Regex(pattern);

            if (regex.IsMatch(ipString))
                return true;
            else
                return false;
        }

    }
}

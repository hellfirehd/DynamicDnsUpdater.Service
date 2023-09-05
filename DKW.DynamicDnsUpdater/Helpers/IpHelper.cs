﻿using System.Text.RegularExpressions;

namespace DKW.DynamicDnsUpdater.Helpers
{
	public class IpHelper
	{
		// Validate if this is a valid IPV4 address
		public static bool IpAddressV4Validator(string ipString)
		{
			// Using regular expression
			string pattern = @"^((^|\.)((1?[1-9]?|10|2[0-4])\d|25[0-5])){4}$";
			Regex regex = new Regex(pattern);

			if (regex.IsMatch(ipString))
				return true;
			else
				return false;
		}

	}
}
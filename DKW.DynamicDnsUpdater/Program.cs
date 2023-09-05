using DKW.DynamicDnsUpdater.Configuration;
using DKW.DynamicDnsUpdater.Interface;
using System.Reflection;

namespace DKW.DynamicDnsUpdater;

public class Program
{
	public static void Main(String[] args)
	{
		IHost host = Host.CreateDefaultBuilder(args)
			.ConfigureServices((context, services) =>
			{

				services.Configure<DynamicDnsUpdaterOptions>(context.Configuration.GetSection(nameof(DynamicDnsUpdaterOptions)));

				services.Scan(scan =>
					scan.FromApplicationDependencies(a => AssemblyFilter(a))
						.AddClasses(x => x.AssignableTo<IDnsProvider>())
							.AsImplementedInterfaces().WithTransientLifetime()
						.AddClasses(x => x.AssignableTo<IIpAddressChecker>())
							.AsImplementedInterfaces().WithTransientLifetime()
						.AddClasses(x => x.AssignableTo<IClient>())
							.AsImplementedInterfaces().WithTransientLifetime()
				);

				services.AddHostedService<Worker>();
			})
			.Build();

		host.Run();
	}

	private static Boolean AssemblyFilter(Assembly a) => a.FullName?.StartsWith("DKW") == true;
}
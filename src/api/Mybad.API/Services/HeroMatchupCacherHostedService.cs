using Mybad.Core.Services;
using Mybad.Services.OpenDota.Cachers;
using System.Diagnostics;

namespace Mybad.API.Services;

public class HeroMatchupCacherHostedService : BackgroundService
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly ILogger<HeroMatchupCacherHostedService> _logger;
	private readonly HeroMatchupCacherStatus _status;
	private const int _timeoutS = 900;

	public HeroMatchupCacherHostedService(IServiceScopeFactory scopeFactory, ILogger<HeroMatchupCacherHostedService> logger, HeroMatchupCacherStatus status)
	{
		_scopeFactory = scopeFactory;
		_logger = logger;
		_status = status;
	}

	/// <inheritdoc />
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using var timer = new PeriodicTimer(TimeSpan.FromSeconds(_timeoutS));
		while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
		{
			if (_status.IsEnabled)
			{
				await DoWork();
			}
		}
	}

	/// <inheritdoc />
	public override async Task StopAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("{@Service} is stopping.", nameof(HeroMatchupCacherHostedService));

		await base.StopAsync(stoppingToken);
	}

	/// <summary>
	/// Creates scope, and calls service to update db with new info.
	/// </summary>
	private async Task DoWork()
	{
		using var scope = _scopeFactory.CreateScope();
		var cacher = scope.ServiceProvider.GetRequiredService<ODotaHeroMatchupCacher>();
		var notifier = scope.ServiceProvider.GetService<INotifier>();
		var sw = Stopwatch.StartNew();
		var opResult = false;

		try
		{
			_logger.LogInformation("{@Method} - Start service method {@m}.", nameof(HeroMatchupCacherHostedService), nameof(DoWork));
			await cacher.UpdateHeroMatchupsDatabase(minRank: 75);
			_logger.LogInformation("{@Method} - End service method {@m}.", nameof(HeroMatchupCacherHostedService), nameof(DoWork));
			opResult = true;
		}
		catch (Exception ex)
		{
			_logger.LogWarning("{@Service} - Exception while doing work - {@ex}", nameof(HeroMatchupCacherHostedService), ex.Message);
			if (notifier is not null)
			{
				await notifier.NotifyAsync(new NotifyMessage($"<b>[{DateTime.Now} UTC]</b> - UpdateHeroMatchup failed. Exception:\n{ex.Message}."));
			}
		}
		finally
		{
			sw.Stop();
			if (notifier is not null)
			{
				await notifier.NotifyAsync(new NotifyMessage(
					$"[{DateTime.Now} UTC] - UpdateHeroMatchup finished with success - {opResult.ToString().ToUpperInvariant()}.\nMatches in Db - {cacher.CachedMatchesCount}.\nTime elapsed - {sw.Elapsed.TotalSeconds} s."));
			}
		}
	}
}

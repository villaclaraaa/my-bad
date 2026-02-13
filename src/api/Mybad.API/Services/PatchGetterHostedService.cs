
using Mybad.Core.Services;
using System.Diagnostics;

namespace Mybad.API.Services
{
    public class PatchGetterHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly int _timeoutH = 24;
        public PatchGetterHostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork();
            using var timer = new PeriodicTimer(TimeSpan.FromHours(_timeoutH));
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWork();
            }
        }

        private async Task DoWork()
        {
            using var scope = _scopeFactory.CreateScope();
            var patchService = scope.ServiceProvider.GetRequiredService<PatchService>();
            var notifier = scope.ServiceProvider.GetService<INotifier>();
            var sw = Stopwatch.StartNew();

            var opResult = false;

            try
            {
                await patchService.SyncronizePatchInfo();
                opResult = true;
            }
            catch (Exception ex)
            {
                if (notifier is not null)
                {
                    await notifier.NotifyAsync(new NotifyMessage($"<b>[{DateTime.UtcNow} UTC]</b> - SyncPatchInfo failed. Exception:\n{ex.Message}."));
                }
            }
            finally
            {
                sw.Stop();
                if (notifier is not null)
                {
                    await notifier.NotifyAsync(new NotifyMessage(
                        $"<b>[{DateTime.UtcNow} UTC]</b> - SyncPatchInfo finished with success - <b>{opResult.ToString().ToUpperInvariant()}</b> \nTime elapsed - {sw.Elapsed.TotalSeconds} s."));
                }
            }
        }
    }
}

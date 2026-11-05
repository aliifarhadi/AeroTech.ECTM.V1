using AeroTech.ECTM.Application.ProcessingJobAggregate.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AeroTech.ECTM.Consumers.Jobs
{
    public sealed class ProcessingJobPoller : BackgroundService
    {
        private readonly IProcessingJobRunner _runner;
        private readonly ProcessingJobOptions _options;
        private readonly ILogger<ProcessingJobPoller> _logger;

        public ProcessingJobPoller(
            IProcessingJobRunner runner,
            IOptions<ProcessingJobOptions> options,
            ILogger<ProcessingJobPoller> logger)
        {
            _runner = runner;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var idle = TimeSpan.FromSeconds(_options.PollIntervalSeconds);

            while (!stoppingToken.IsCancellationRequested)
            {
                var worked = false;

                try
                {
                    worked = await _runner.RunNextAsync(stoppingToken);
                }
                catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogError(exception, "Processing job loop failed.");
                }

                if (!worked)
                    await Task.Delay(idle, stoppingToken);
            }
        }
    }
}

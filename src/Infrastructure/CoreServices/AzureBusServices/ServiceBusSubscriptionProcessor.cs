namespace UsuariosAPI.Infrastructure.CoreServices.AzureBusServices
{
    using System.Text.Json;
    using APICore.Interfaces.Responses;
    using APICore.Interfaces.Services.AzureServiceBus;
    using APICore.Services.AzureServiceBus;
    using Azure.Messaging.ServiceBus;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using UsuariosAPI.Application.Features.Usuario.CommandActualizarLegajos;

    public class ServiceBusSubscriptionProcessor : IServiceBusSubscriptionProcessor
    {
        private readonly ServiceBusClient client;
        private readonly List<ServiceBusProcessor> processors = new ();
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<IServiceBusSubscriptionProcessor> logger;

        public ServiceBusSubscriptionProcessor(
            IOptions<ServiceBusSetting> settings,
            IServiceProvider serviceProvider,
            ILogger<IServiceBusSubscriptionProcessor> logger)
        {
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            var connectionString = settings.Value.ConnectionStrings;

            this.client = new ServiceBusClient(connectionString, new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets,
            });

            var processorOptions = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false,
            };

            foreach (var topicConfig in settings.Value.TopicsToListen)
            {
                if (string.IsNullOrWhiteSpace(topicConfig.TopicName) || string.IsNullOrWhiteSpace(topicConfig.SubscriptionName))
                {
                    continue;
                }

                var processor = this.client.CreateProcessor(topicConfig.TopicName, topicConfig.SubscriptionName, processorOptions);
                processor.ProcessMessageAsync += this.MessageHandler;
                processor.ProcessErrorAsync += this.ErrorHandler;
                this.processors.Add(processor);
            }
        }

        public async Task StartProcessingAsync(CancellationToken cancellationToken = default)
        {
            foreach (var processor in this.processors)
            {
                if (!processor.IsProcessing)
                {
                    await processor.StartProcessingAsync(cancellationToken);
                }
            }
        }

        public async Task StopProcessingAsync(CancellationToken cancellationToken = default)
        {
            foreach (var processor in this.processors)
            {
                if (processor.IsProcessing)
                {
                    await processor.StopProcessingAsync(cancellationToken);
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var processor in this.processors)
            {
                await processor.DisposeAsync();
            }

            await this.client.DisposeAsync();
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var messageType = args.Message.Subject;
            var body = args.Message.Body.ToString();

            this.logger.LogInformation("[ASB RECEPCION] MessageType={MessageType}", messageType);

            using var scope = this.serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            try
            {
                var command = GetCommand(messageType, body);

                if (command is null)
                {
                    this.logger.LogWarning("[ASB] Tipo de mensaje no soportado: {MessageType}", messageType);
                    await args.CompleteMessageAsync(args.Message);
                    return;
                }

                var result = await mediator.Send(command);

                if (result is IResponse response && !response.IsSuccess)
                {
                    this.logger.LogError("[ASB] Command falló: {MessageType} - {Error}", messageType, response.Errors?.FirstOrDefault());
                }

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "[ASB] Error procesando mensaje: {MessageType}", messageType);
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            this.logger.LogCritical(args.Exception, "[ASB ERROR FATAL] Source={Source}", args.ErrorSource);
            return Task.CompletedTask;
        }

        private static object? GetCommand(string messageType, string body)
        {
            return messageType switch
            {
                "SYNC_LEGAJOS_USUARIOS" => new ActualizarLegajosCommand(),
                _ => null,
            };
        }
    }
}

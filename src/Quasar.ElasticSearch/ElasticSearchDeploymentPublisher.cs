using Quasar.Core;
using System;
using Nest;
using Microsoft.Extensions.Logging;

namespace Quasar.ElasticSearch
{
    public class ElasticSearchDeploymentPublisher : IDeploymentPublisher
    {
        Uri esUri = new Uri("http://localhost:9200");
        ElasticClient _elasticClient;
        ILogger<ElasticSearchDeploymentPublisher> _logger;

        public ElasticSearchDeploymentPublisher(ILogger<ElasticSearchDeploymentPublisher> logger)
        {
            var settings = new ConnectionSettings(esUri);
            settings.DefaultIndex("quasar-deployments");
            settings.DefaultMappingFor<DeploymentInstance>(a => a.IndexName("quasar-state"));
            _elasticClient = new ElasticClient(settings);
            _logger = logger;
        }

        public void BeginDeploymentRun(DeploymentRun deploymentRun)
        {
            WriteDeploymentRun(deploymentRun);
            WriteEvent(deploymentRun, "Starting Deployment", EventType.Start);
        }

        public void DeploymentRunError(DeploymentRun deploymentRun, string error)
        {
            WriteLogEntry(deploymentRun, error, EntryType.Error);
        }

        public void DeploymentRunInfo(DeploymentRun deploymentRun, string info)
        {
            WriteLogEntry(deploymentRun, info, EntryType.Info);
        }

        public void EndDeploymentRun(DeploymentRun deploymentRun)
        {
            WriteDeploymentRun(deploymentRun);
            WriteEvent(deploymentRun, "Deployment Complete", EventType.Complete);
            WriteDeployment(deploymentRun);
        }

        private void WriteDeploymentRun(DeploymentRun deploymentRun)
        {
            _logger.LogInformation("Indexing begin deployment {0} {1}", deploymentRun.id, deploymentRun.DeploymentStatus);
            var response = _elasticClient.IndexDocument(deploymentRun);
            _logger.LogInformation("Indexing begin deployment {0} Result={1}", deploymentRun.id, response.Result);
        }

        private void WriteLogEntry(DeploymentRun deploymentRun, string text, EntryType entryType)
        {
            _logger.LogInformation("Indexing {2} log for {0}: {1}", deploymentRun.id, text, entryType);
            var logEntry = new DeploymentLogEntry()
            {
                id = Guid.NewGuid(),
                DeploymentRunId = deploymentRun.id,
                Message = text,
                MessageType = entryType,
                Timestamp = DateTime.Now
            };
            var response = _elasticClient.IndexDocument(logEntry);
            _logger.LogInformation("Indexing {2} log for {0}: Result={1}", deploymentRun.id, response.Result, entryType);
        }

        private void WriteEvent(DeploymentRun deploymentRun, string text, EventType eventType)
        {
            _logger.LogInformation("Indexing {2} event for {0}: {1}", deploymentRun.id, text, eventType);
            var eventEntry = new DeploymentEvent()
            {
                id = Guid.NewGuid(),
                DeploymentRun = deploymentRun,
                DeploymentRunId = deploymentRun.id,
                Description = text,
                EventType = eventType,
                Timestamp = DateTime.Now
            };
            var response = _elasticClient.IndexDocument(eventEntry);
            _logger.LogInformation("Indexing {2} log for {0}: Result={1}", deploymentRun.id, response.Result, eventType);
        }

        private void WriteDeployment(DeploymentRun deploymentRun)
        {
            _logger.LogInformation("Indexing deployment for {0}: {1}", deploymentRun.id, deploymentRun.Product.Name);
            var deployment = new DeploymentInstance()
            {
                id = deploymentRun.Product.Name+"_"+deploymentRun.EnvironmentName,
                product = deploymentRun.Product,
                environment = deploymentRun.EnvironmentName,
                deploymentRunId = deploymentRun.id,
                latestDeployment = DateTime.Now 
            };
            var response = _elasticClient.IndexDocument(deployment);
            _logger.LogInformation("Indexing deployment for {0}: {1}", deploymentRun.id, response.Result);
        }
    }
}

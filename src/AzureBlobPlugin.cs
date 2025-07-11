using FlowSynx.PluginCore;
using FlowSynx.Plugins.Azure.Blobs.Models;
using FlowSynx.PluginCore.Extensions;
using FlowSynx.Plugins.Azure.Blobs.Services;
using FlowSynx.PluginCore.Helpers;

namespace FlowSynx.Plugins.Azure.Blobs;

public class AzureBlobPlugin : IPlugin
{
    private IAzureBlobManager _manager = null!;
    private AzureBlobSpecifications _azureBlobSpecifications = null!;
    private bool _isInitialized;

    public PluginMetadata Metadata
    {
        get
        {
            return new PluginMetadata
            {
                Id = Guid.Parse("7f21ba04-ea2a-4c78-a2f9-051fa05391c8"),
                Name = "Azure.Blobs",
                CompanyName = "FlowSynx",
                Description = Resources.PluginDescription,
                Version = new Version(1, 1, 0),
                Category = PluginCategory.Cloud,
                Authors = new List<string> { "FlowSynx" },
                Copyright = "© FlowSynx. All rights reserved.",
                Icon = "flowsynx.png",
                ReadMe = "README.md",
                RepositoryUrl = "https://github.com/flowsynx/plugin-azure-blobs",
                ProjectUrl = "https://flowsynx.io",
                Tags = new List<string>() { "FlowSynx", "Azure", "Blobs", "Cloud" },
                MinimumFlowSynxVersion = new Version(1, 1, 1),
            };
        }
    }

    public PluginSpecifications? Specifications { get; set; }
    public Type SpecificationsType => typeof(AzureBlobSpecifications);

    public Task Initialize(IPluginLogger logger)
    {
        if (ReflectionHelper.IsCalledViaReflection())
            throw new InvalidOperationException(Resources.ReflectionBasedAccessIsNotAllowed);

        ArgumentNullException.ThrowIfNull(logger);
        var connection = new AzureBlobConnection();
        _azureBlobSpecifications = Specifications.ToObject<AzureBlobSpecifications>();
        var client = connection.Connect(_azureBlobSpecifications);
        _manager = new AzureBlobManager(logger, client, _azureBlobSpecifications.ContainerName);
        _isInitialized = true;
        return Task.CompletedTask;
    }

    public async Task<object?> ExecuteAsync(PluginParameters parameters, CancellationToken cancellationToken)
    {
        if (ReflectionHelper.IsCalledViaReflection())
            throw new InvalidOperationException(Resources.ReflectionBasedAccessIsNotAllowed);

        if (!_isInitialized)
            throw new InvalidOperationException($"Plugin '{Metadata.Name}' v{Metadata.Version} is not initialized.");

        var operationParameter = parameters.ToObject<OperationParameter>();
        var operation = operationParameter.Operation;

        if (OperationMap.TryGetValue(operation, out var handler))
            return handler(parameters, cancellationToken);

        throw new NotSupportedException($"Microsoft Azure Blobs plugin: Operation '{operation}' is not supported.");
    }

    private Dictionary<string, Func<PluginParameters, CancellationToken, Task<object?>>> OperationMap => new(StringComparer.OrdinalIgnoreCase)
    {
        ["create"] = async (parameters, cancellationToken) => { await _manager.Create(parameters, cancellationToken); return null; },
        ["delete"] = async (parameters, cancellationToken) => { await _manager.Delete(parameters, cancellationToken); return null; },
        ["exist"] = async (parameters, cancellationToken) => await _manager.Exist(parameters, cancellationToken),
        ["list"] = async (parameters, cancellationToken) => await _manager.List(parameters, cancellationToken),
        ["purge"] = async (parameters, cancellationToken) => { await _manager.Purge(parameters, cancellationToken); return null; },
        ["read"] = async (parameters, cancellationToken) => await _manager.Read(parameters, cancellationToken),
        ["write"] = async (parameters, cancellationToken) => { await _manager.Write(parameters, cancellationToken); return null; },
    };

    public IReadOnlyCollection<string> SupportedOperations => OperationMap.Keys;
}
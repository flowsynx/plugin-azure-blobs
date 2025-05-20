using Azure.Storage;
using Azure.Storage.Blobs;
using FlowSynx.Plugins.Azure.Blobs.Models;

namespace FlowSynx.Plugins.Azure.Blobs.Services;

internal class AzureBlobConnection : IAzureBlobConnection
{
    public BlobServiceClient Connect(AzureBlobSpecifications specifications)
    {
        ValidateSpecifications(specifications);

        var uri = BuildServiceUri(specifications.AccountName);
        var credential = CreateCredential(specifications.AccountName, specifications.AccountKey);

        return new BlobServiceClient(serviceUri: uri, credential: credential);
    }

    private void ValidateSpecifications(AzureBlobSpecifications specifications)
    {
        if (string.IsNullOrWhiteSpace(specifications?.AccountName) ||
            string.IsNullOrWhiteSpace(specifications?.AccountKey))
        {
            throw new ArgumentException(Resources.PropertiesShouldHaveValue);
        }
    }

    private Uri BuildServiceUri(string accountName)
    {
        return new Uri($"https://{accountName}.blob.core.windows.net");
    }

    private StorageSharedKeyCredential CreateCredential(string accountName, string accountKey)
    {
        return new StorageSharedKeyCredential(accountName, accountKey);
    }
}

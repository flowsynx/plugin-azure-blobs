using Azure.Storage.Blobs;
using FlowSynx.Plugins.Azure.Blobs.Models;
using FlowSynx.Plugins.Azure.Blobs.Services;

namespace FlowSynx.Plugins.Azure.Blobs.UnitTests.Services;

public class AzureBlobConnectionTests
{
    private readonly AzureBlobConnection _connection;

    public AzureBlobConnectionTests()
    {
        _connection = new AzureBlobConnection();
    }

    [Fact]
    public void Connect_WithValidSpecifications_ReturnsBlobServiceClient()
    {
        // Arrange
        var specifications = new AzureBlobSpecifications
        {
            AccountName = "testaccount",
            AccountKey = "dGVzdGtleQ=="
        };

        // Act
        var client = _connection.Connect(specifications);

        // Assert
        Assert.NotNull(client);
        Assert.IsType<BlobServiceClient>(client);
        Assert.Equal($"https://{specifications.AccountName}.blob.core.windows.net/", client.Uri.ToString());
    }

    [Theory]
    [InlineData(null, "validKey")]
    [InlineData("validAccount", null)]
    [InlineData("", "validKey")]
    [InlineData("validAccount", "")]
    [InlineData(" ", "validKey")]
    [InlineData("validAccount", " ")]
    public void Connect_WithInvalidSpecifications_ThrowsArgumentException(string accountName, string accountKey)
    {
        // Arrange
        var specifications = new AzureBlobSpecifications
        {
            AccountName = accountName,
            AccountKey = accountKey
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _connection.Connect(specifications));
        Assert.Equal("Both AccountKey and AccountName properties in the azure blob specifications must have value.", exception.Message);
    }

    [Fact]
    public void Connect_WithNullSpecifications_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _connection.Connect(null));
        Assert.Equal("Both AccountKey and AccountName properties in the azure blob specifications must have value.", exception.Message);
    }
}
# FlowSynx Azure Blobs plugin

The Azure Blob Plugin is a pre-packaged, plug-and-play integration component for the FlowSynx engine. It enables interacting with Azure Blob Storage to manage containers and blobs, supporting a variety of operations such as uploading, downloading, listing, and purging blob data. Designed for FlowSynx’s no-code/low-code automation workflows, this plugin simplifies cloud storage integration and file management.

This plugin is automatically installed by the FlowSynx engine when selected within the platform. It is not intended for manual installation or standalone developer use outside the FlowSynx environment.

---

## Purpose

The Azure Blob Plugin allows FlowSynx users to:

- Upload and download files to and from Azure Blob Storage.
- Manage blobs and containers with create, delete, and purge operations.
- List contents of containers with filtering and metadata support.
- Perform existence checks for files or folders in workflows without writing code.

---

## Supported Operations

- **create**: Creates a new blob in the specified bucket and path.
- **delete**: Deletes an blob at the specified path in the bucket.
- **exist**: Checks if an blob exists at the specified path.
- **list**: Lists blobs under a specified path (prefix), with filtering and optional metadata.
- **purge**: Deletes all blobs under the specified path, optionally forcing deletion.
- **read**: Reads and returns the contents of an blob at the specified path.
- **write**: Writes data to a specified path in the bucket, with support for overwrite.

---

## Plugin Specifications

The plugin requires the following configuration:

- `AccountName` (string): **Required.** The Azure Storage account name.
- `AccountKey` (string): **Required.** The access key for the Azure Storage account.
- `ContainerName` (string): **Required.** The name of the blob container to use.

### Example Configuration

```json
{
  "AccountName": "myazureaccount",
  "AccountKey": "abc123xyz456==",
  "ContainerName": "flowfiles"
}
```

---

## Input Parameters

Each operation accepts specific parameters:

### Create
| Parameter     | Type    | Required | Description                              |
|---------------|---------|----------|------------------------------------------|
| `Path`        | string  | Yes      | The path where the new blob is created.|

### Delete
| Parameter     | Type    | Required | Description                              |
|---------------|---------|----------|------------------------------------------|
| `Path`        | string  | Yes      | The path of the blob to delete.        |

### Exist
| Parameter     | Type    | Required | Description                              |
|---------------|---------|----------|------------------------------------------|
| `Path`        | string  | Yes      | The path of the blob to check.         |

### List
| Parameter         | Type    | Required | Description                                         |
|--------------------|---------|----------|-----------------------------------------------------|
| `Path`             | string  | Yes      | The prefix path to list blob from.              |
| `Filter`           | string  | No       | A filter pattern for blob names.                 |
| `Recurse`          | bool    | No       | Whether to list recursively. Default: `false`.     |
| `CaseSensitive`    | bool    | No       | Whether the filter is case-sensitive. Default: `false`. |
| `IncludeMetadata`  | bool    | No       | Whether to include blob metadata. Default: `false`. |
| `MaxResults`       | int     | No       | Maximum number of blobs to list. Default: `2147483647`. |

### Purge
| Parameter     | Type    | Required | Description                                    |
|---------------|---------|----------|------------------------------------------------|
| `Path`        | string  | Yes      | The path prefix to purge.                     |
| `Force`       | bool    | No       | Whether to force deletion without confirmation.|

### Read
| Parameter     | Type    | Required | Description                              |
|---------------|---------|----------|------------------------------------------|
| `Path`        | string  | Yes      | The path of the blob to read.          |

### Write
| Parameter     | Type    | Required | Description                                                  |
|---------------|---------|----------|--------------------------------------------------------------|
| `Path`        | string  | Yes      | The path where data should be written.                      |
| `Data`        | object  | Yes      | The data to write to the Azure blob.                          |
| `Overwrite`   | bool    | No       | Whether to overwrite if the blob already exists. Default: `false`. |

### Example input (Write)

```json
{
  "Operation": "write",
  "Path": "documents/report.json",
  "Data": {
    "title": "Monthly Report",
    "content": "This is the report content."
  },
  "Overwrite": true
}
```

---

## Debugging Tips

- Verify the `AccountName`, `AccountKey`, and `ContainerName` values are correct and have sufficient permissions.
- Ensure the `Path` is valid and does not conflict with existing blobs (especially for create/write).
- For write operations, confirm that `Data` is properly encoded or formatted for upload.
- When using list, adjust filters carefully to match blob names (wildcards like `*.txt` are supported).
- Purge will fail on non-empty folders unless `Force` is set to `true`.

---

## Azure Blob Storage Considerations

- Case Sensitivity: Blob paths are case-sensitive; use the `CaseSensitive` flag in list if needed.
- Hierarchy Simulation: Azure Blob Storage uses a flat namespace. "Folders" are simulated using blob path prefixes (e.g., `folder1/file.txt`).
- Large File Uploads: For large files, uploads are automatically chunked by the plugin.
- Metadata: When using `IncludeMetadata` in list, additional API calls may increase latency.

---

## Security Notes

- The plugin uses Azure Storage SDK authentication with the provided `AccountKey`.
- No credentials or blob data are persisted outside the execution scope unless explicitly configured.
- Only authorized FlowSynx platform users can view or modify plugin configurations.

---

## License

© FlowSynx. All rights reserved.
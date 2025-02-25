using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Pinterest.Application.FileStorage.Interfaces;
using Pinterest.Application.FileStorage.Models;
using Pinterest.Domain.FileStorage.Settings;
using Pinterest.Shared.Security.Models;
using Pinterest.Shared.Security.Settings;

namespace Pinterest.Api.FileStorage.Controllers;

[ApiController, Route("fileAccess")]
[Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
public class FileAccessController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _contentTypeProvider = new(); 
    private readonly IFileStorageService _fileStorageService;
    
    public FileAccessController(ILogger<FileAccessController> logger, 
        IFileStorageService fileStorageService,
        IOptions<StorageSettings> storageSettings)
    {
        _fileStorageService = fileStorageService;
        Logger = logger;
        StorageSettings = storageSettings.Value;
    }
    public ILogger<FileAccessController> Logger { get; }
    private StorageSettings StorageSettings { get; }

    [HttpGet, Route("info")]
    [ProducesResponseType(typeof(FileBasicInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetFileInfo([FromQuery] Guid fileUuid)
    {
        return Ok(await _fileStorageService.GetFileMetadata(fileUuid));
    }
    [HttpGet, Route("access")]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetFileData([FromQuery] Guid fileUuid)
    {
        var fileMetadata = await _fileStorageService.GetFileMetadata(fileUuid);
        Response.Headers["Accept-Ranges"] = "bytes";
        if (!Request.Headers.ContainsKey("Range"))
        {
            var fileData = await _fileStorageService.GetFileData(fileUuid);
            return File(fileData, fileMetadata.ContentType, GetFileName(fileUuid, fileMetadata.ContentType));
        }
        var range = ParseRange(Request.Headers["Range"].ToString(), fileMetadata.FileSize);
        Response.StatusCode = (int)HttpStatusCode.PartialContent;
        Response.Headers["Content-Range"] = $"bytes {range.Start}-{range.End}/{fileMetadata.FileSize}";
        
        var fileChuck = await _fileStorageService.GetFileData(fileUuid, range);
        return File(fileChuck, fileMetadata.ContentType, GetFileName(fileUuid, fileMetadata.ContentType));
    }
    [HttpHead, Route("access")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetFileHead([FromQuery] Guid fileUuid)
    {
        var metadata = await _fileStorageService.GetFileMetadata(fileUuid);
        Response.Headers["Content-Length"] = metadata.FileSize.ToString();
        Response.Headers["Accept-Ranges"] = "bytes";
        Response.Headers["Content-Type"] = metadata.ContentType;
        return NoContent();
    }
    private string GetFileName(Guid fileUuid, string contentType)
    {
        var extension = _contentTypeProvider.Mappings.FirstOrDefault(m => m.Value == contentType).Key ?? ".bin";
        return $"{fileUuid}{extension}";
    }
    private FileRangeInfo ParseRange(string rangeHeader, long fileSize)
    {
        const string prefix = "bytes=";
        if (!rangeHeader.StartsWith(prefix))
        {
            throw new ArgumentException("Invalid Range header");
        }
        var rangeValues = rangeHeader.Substring(prefix.Length).Split('-');
        long start = long.Parse(rangeValues[0]);
        long end = rangeValues.Length > 1 && !string.IsNullOrEmpty(rangeValues[1]) ? long.Parse(rangeValues[1]) : fileSize - 1;
        end = Math.Min(start + StorageSettings.MaxChunkSize - 1, end);
        
        return new FileRangeInfo() { Start = start, End = end };
    }
}
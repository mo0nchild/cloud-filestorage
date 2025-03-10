using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using CloudStorage.Application.FileStorage.Infrastructures.Models;
using CloudStorage.Application.FileStorage.Interfaces;
using CloudStorage.Application.FileStorage.Models;
using CloudStorage.Domain.FileStorage.Settings;
using CloudStorage.Shared.Security.Models;
using CloudStorage.Shared.Security.Settings;

namespace CloudStorage.Api.FileStorage.Controllers;

[ApiController, Route("thumbnail")]
[Authorize(SecurityInfo.User, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
public class ThumbnailController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _contentTypeProvider = new(); 
    private readonly IThumbnailService _thumbnailService;

    public ThumbnailController(IThumbnailService thumbnailService, 
        ILogger<ThumbnailController> logger,
        IOptions<StorageSettings> storageSettings)
    {
        _thumbnailService = thumbnailService;
        Logger = logger;
        StorageSettings = storageSettings.Value;
    }
    private ILogger<ThumbnailController> Logger { get; }
    private StorageSettings StorageSettings { get; }

    [HttpGet, Route("info")]
    [ProducesResponseType(typeof(FileMetadata), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetThumbnailInfo([FromQuery] Guid fileUuid)
    {
        return Ok(await _thumbnailService.GetThumbnailMetadata(fileUuid));
    }
    [HttpGet, Route("access")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetThumbnailData([FromQuery] Guid fileUuid)
    {
        var fileMetadata = await _thumbnailService.GetThumbnailMetadata(fileUuid);
        Response.Headers["Accept-Ranges"] = "bytes";
        if (!Request.Headers.ContainsKey("Range"))
        {
            var fileData = await _thumbnailService.GetFileThumbnail(fileUuid);
            return File(fileData, fileMetadata.ContentType, GetFileName(fileUuid, fileMetadata.ContentType));
        }
        var range = ParseRange(Request.Headers["Range"].ToString(), fileMetadata.FileSize);
        Response.StatusCode = (int)HttpStatusCode.PartialContent;
        Response.Headers["Content-Range"] = $"bytes {range.Start}-{range.End}/{fileMetadata.FileSize}";
        
        var fileChuck = await _thumbnailService.GetFileThumbnail(fileUuid, range);
        return File(fileChuck, fileMetadata.ContentType, GetFileName(fileUuid, fileMetadata.ContentType));
    }
    [HttpHead, Route("access")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetFileHead([FromQuery] Guid fileUuid)
    {
        var metadata = await _thumbnailService.GetThumbnailMetadata(fileUuid);
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
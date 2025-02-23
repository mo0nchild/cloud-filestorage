using System.Net;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.IdGenerators;
using Pinterest.Application.FileStorage.Interfaces;
using Pinterest.Application.FileStorage.Models;

namespace Pinterest.Api.FileStorage.Controllers;

[ApiController, Route("fileStorage")]
public class FileStorageController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;
    public FileStorageController(IFileStorageService fileStorageService, ILogger<FileStorageController> logger)
    {
        _fileStorageService = fileStorageService;
        Logger = logger;
    }
    private ILogger<FileStorageController> Logger { get; }

    [HttpPost, Route("initialize")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> InitializeUpload([FromQuery] NewFileInfo fileInfo)
    {
        return Ok(new { FileUuid = await _fileStorageService.InitializeUpload(fileInfo) });
    }
    [HttpPost, Route("uploadFile")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UploadFile([FromQuery] NewFileInfo fileInfo)
    {
        using var fileStream = new MemoryStream();
        await Request.Body.CopyToAsync(fileStream);
        fileStream.Seek(0, SeekOrigin.Begin);

        return Ok(new { FileUuid = await _fileStorageService.UploadFile(fileInfo, fileStream) });
    }
    [HttpPost, Route("uploadPart")]
    [ProducesResponseType(typeof(PartInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UploadChunk([FromQuery] Guid fileUuid, [FromQuery] int partNumber)
    {
        using var chunkStream = new MemoryStream();
        await Request.Body.CopyToAsync(chunkStream);
        chunkStream.Seek(0, SeekOrigin.Begin);

        var uploadResult = await _fileStorageService.UploadFilePart(new UploadChuckInfo()
        {
            PartNumber = partNumber,
            FileUuid = fileUuid,
            ChuckData = chunkStream,
        });
        return Ok(uploadResult);
    }
    [HttpPost, Route("complete")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CompleteUpload([FromBody] CompleteUploadInfo completeInfo)
    {
        await _fileStorageService.CompleteUpload(completeInfo);
        return Ok(new { Message = "File uploaded successfully" });
    }
}
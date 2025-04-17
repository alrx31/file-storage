using API.DTO;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class FilesController(IFilesService filesService) : ControllerBase
{
    [HttpPut]
    public async Task<IActionResult> UploadWithRewrite([FromForm] UploadFileDTO file)
    {
        await filesService.UploadWithRewrite(file);
        return Ok();
    }

    [HttpHead("{*path}")]
    public async Task<IActionResult> HeadInformation(string path)
    {
        var fileInfo = await filesService.HeadInformation(path);
    
        if (!fileInfo.Exists)
        {
            return NotFound("File not found");
        }

        Response.Headers["Content-Name"] = fileInfo.Name;
        Response.Headers["Content-Length"] = fileInfo.Size.ToString();
        Response.Headers["Last-Modified"] = fileInfo.LastModified.ToString("R");

        return Ok();
    }

    [HttpDelete("{*path}")]
    public async Task<IActionResult> Delete(string path)
    {
        await filesService.Delete(path);
        return Ok();
    }

    [HttpPatch("{*path}")]
    public async Task<IActionResult> CopyFileTo([FromBody] CopyFileDTO model, string path)
    {
        await filesService.CopyFileTo(model, path);
        return Ok();
    }
}
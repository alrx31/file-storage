using API.DTO;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("")]
public class IndexController(IFilesService filesService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await filesService.GetAll());
    }

    [HttpGet("{*path}")]
    public async Task<IActionResult> GetFile(string path)
    {
        return await filesService.GetFile(path);
    }
    
    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] UploadFileDTO file)
    {
        await filesService.Upload(file);
        return Ok();
    }
}
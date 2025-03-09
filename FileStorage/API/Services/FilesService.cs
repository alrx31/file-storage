using API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace API.Services;

public class FilesService(
    string dataFolderPath
    ): IFilesService
{
    public async Task<List<string>> GetAll()
    {
        return GetFiles(dataFolderPath);
    }

    public async Task<FileStreamResult> GetFile(string path)
    {
        var fullPath = GetFullPath(path);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("File not found", fullPath);
        }

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
        var contentType = GetContentType(fullPath);
        var fileName = Path.GetFileName(fullPath);

        return new FileStreamResult(stream, contentType)
        {
            FileDownloadName = fileName
        };
    }
    
    public async Task Upload(UploadFileDTO fileDto)
    {
        var file = fileDto.file;
        var p = CreatePath(fileDto.path,file.FileName);
        var directory = Path.GetDirectoryName(p);
        
        var files = await GetAll();

        if (files.Contains(p))
        {
            throw new Exception($"File {fileDto.path} already exists");
        }
        
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        using (var fileStream = new FileStream(p, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
    }

    public async Task UploadWithRewrite(UploadFileDTO fileDto)
    {
        var file = fileDto.file;
        var p = CreatePath(fileDto.path,file.FileName);
        var directory = Path.GetDirectoryName(p);
        
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        using (var fileStream = new FileStream(p, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
    }

    public async Task<HeadFileDTO> HeadInformation(string path)
    {
        var fullPath = GetFullPath(path);
        var file =  new FileInfo(fullPath);
        
        if (!file.Exists)
        {
            throw new FileNotFoundException("File not found", fullPath);
        }
        
        return new HeadFileDTO
        {
            Name = file.Name,
            Size = file.Length,
            LastModified = file.LastWriteTime,
            Exists = file.Exists
        };
    }

    public async Task Delete(string path)
    {
        var fullPath = GetFullPath(path);

        if (File.Exists(fullPath))
        {
            var file = new FileInfo(fullPath);
            file.Delete();
        }
        else if (Directory.Exists(fullPath))
        {
            var directory = new DirectoryInfo(fullPath);
            directory.Delete(true); 
        }
        else
        {
            throw new FileNotFoundException("Файл или директория не найдены.", fullPath);
        }
    }

   public async Task CopyFileTo(CopyFileDTO model, string path)
    {
        var sourceFilePath = GetFullPath(path);
        var destinationPath = GetFullPath(model.pathToCopy);
        
        if (!File.Exists(sourceFilePath))
        {
            throw new FileNotFoundException("Source file not found.", sourceFilePath);
        }

        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }

        string destinationFilePath = CreatePath(model.pathToCopy, Path.GetFileName(sourceFilePath));

        using (FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
        {
            using (FileStream destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.Write))
            {
                await sourceStream.CopyToAsync(destinationStream);
            }
        }
        
        await Delete(path);
    }




    private List<string> GetFiles(string folderPath)
    {
        var result = new List<string>();
        
        var files = Directory.GetFiles(folderPath);
        var dirs = Directory.GetDirectories(folderPath);
        
        result.AddRange(files);

        foreach (var dir in dirs)
        {
            result.AddRange(GetFiles(dir));
        }
        
        return result;
    }

    private string CreatePath(string folderPath, string fileName)
    {
        return Path.Join(dataFolderPath,Path.Join(folderPath, fileName));
    }

    private string GetFullPath(string path)
    {
        return Path.Join(dataFolderPath, path);
    }

    private static string GetContentType(string path)
    {
        var provider = new FileExtensionContentTypeProvider();
        
        if (!provider.TryGetContentType(path, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        
        return contentType;
    }
}
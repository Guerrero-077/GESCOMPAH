using Business.Interfaces.Implements.Utilities;
using Entity.DTOs.Implements.Utilities.Images;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebGESCOMPAH.Controllers.Module.Utilities;

namespace Tests.Web.Utilities;

public class ImagesControllerTests
{
    private readonly Mock<IImagesService> _svc = new();

    private ImagesController Create() => new(_svc.Object);

    [Fact]
    public async Task Upload_ReturnsBadRequest_WhenNoFiles()
    {
        var ctrl = Create();
        var files = new FormFileCollection();
        var result = await ctrl.Upload(1, files);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Upload_ReturnsOk_WithResult()
    {
        _svc.Setup(s => s.AddImagesAsync(1, It.IsAny<IFormFileCollection>()))
            .ReturnsAsync(new List<ImageSelectDto> { new(1, "a.jpg", "/a", "pid", 1) });
        var files = new FormFileCollection { new FormFile(Stream.Null, 0, 0, "f", "a.jpg") };
        var res = await Create().Upload(1, files);
        Assert.IsType<OkObjectResult>(res.Result);
    }

    [Fact]
    public async Task Delete_NoContent()
    {
        var res = await Create().Delete("pid");
        Assert.IsType<NoContentResult>(res);
    }

    [Fact]
    public async Task GetByEstablishment_ReturnsOk()
    {
        _svc.Setup(s => s.GetImagesByEstablishmentIdAsync(2))
            .ReturnsAsync(new List<ImageSelectDto> { new(3, "f.jpg", "/f", "p3", 2) });
        var res = await Create().GetByEstablishment(2);
        Assert.IsType<OkObjectResult>(res.Result);
    }
}

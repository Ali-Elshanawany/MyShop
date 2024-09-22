namespace MyShop.Web.Helpers;

public static class SaveImg
{


    public static async Task<string> SaveCover(IFormFile cover, string _imgPath)
    {
        var coveName = $"{Guid.NewGuid()}{Path.GetExtension(cover.FileName)}";

        var path = Path.Combine(_imgPath, coveName);

        using var stream = File.Create(path);

        await cover.CopyToAsync(stream);

        return coveName;
    }
}

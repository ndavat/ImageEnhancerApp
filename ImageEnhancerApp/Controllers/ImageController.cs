using ImageEnhancerApp.Models;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;


namespace ImageEnhancerApp.Controllers
{
    public class ImageController : Controller
    {
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ImageUploadModel model)
        {
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var fileName = Path.GetFileName(model.ImageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                // Process and improve the image
                using (Image image = Image.Load(filePath))
                {
                    image.Mutate(x => x
                        .AutoOrient()
                        .Resize(new ResizeOptions
                        {
                            Size = new Size(800, 600),
                            Mode = ResizeMode.Max
                        })
                        .GaussianSharpen(3f)
                    );

                    image.Save(filePath);
                }

                ViewBag.ImagePath = $"/images/{fileName}";
                return View("Result");
            }

            return View();
        }
    }
}


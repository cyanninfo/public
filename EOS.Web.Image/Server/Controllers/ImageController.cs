using EOS.Web.Image.Server.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;

namespace EOS.Web.Image.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public ImageController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile imageFile, [FromForm] string? texte = "")
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("Fichier image manquant.");

            using var stream = imageFile.OpenReadStream();
            using var image = new Bitmap(stream);

            var illustration = new Illustration(image, _env);
            Bitmap resultat = string.IsNullOrWhiteSpace(texte)
                ? illustration.Formate()
                : illustration.Formate(texte);

            using var output = new MemoryStream();
            
            resultat.Save(output, ImageFormat.Png);
            output.Position = 0;

            return File(output.ToArray(), "image/png", "image_transformee.png");
        }
    }
}

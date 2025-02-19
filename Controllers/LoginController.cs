using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using FaceAuth.Services;
using FaceAuth.Data;

namespace FaceAuth.Controllers
{
    public class LoginController : Controller
    {
        private readonly FaceRecoServ _faceRecoService;
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        // A��o para exibir a tela de login (apenas reconhecimento facial)
        public IActionResult Login()
        {
            return View();
        }

        // A��o POST para login com reconhecimento facial
        [HttpPost]
        public async Task<IActionResult> FaceLogin([FromBody] FaceLoginModel model)
        {

            try
            {
                var faceRecoService = new FaceRecoServ(_context);
                if (string.IsNullOrEmpty(model?.Image))
                {
                    return Json(new { success = false, message = "Imagem n�o fornecida" });
                }

                // Converter a imagem base64 para byte[]
                byte[] imageBytes = Convert.FromBase64String(model.Image.Split(',')[1]);

                // Tentar autenticar usando o servi�o de reconhecimento facial
                var user = faceRecoService.RecognizeFace(imageBytes);

                if (user != null)
                {
                    // Sucesso no login
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Autentica��o falhou" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // Modelo para o corpo da requisi��o (imagem base64)
    public class FaceLoginModel
    {
        public string Image { get; set; }
    }
}

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

        // Ação para exibir a tela de login (apenas reconhecimento facial)
        public IActionResult Login()
        {
            return View();
        }

        // Ação POST para login com reconhecimento facial
        [HttpPost]
        public async Task<IActionResult> FaceLogin([FromBody] FaceLoginModel model)
        {

            try
            {
                var faceRecoService = new FaceRecoServ(_context);
                if (string.IsNullOrEmpty(model?.Image))
                {
                    return Json(new { success = false, message = "Imagem não fornecida" });
                }

                // Converter a imagem base64 para byte[]
                byte[] imageBytes = Convert.FromBase64String(model.Image.Split(',')[1]);

                // Tentar autenticar usando o serviço de reconhecimento facial
                var user = faceRecoService.RecognizeFace(imageBytes);

                if (user != null)
                {
                    // Sucesso no login
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "Autenticação falhou" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    // Modelo para o corpo da requisição (imagem base64)
    public class FaceLoginModel
    {
        public string Image { get; set; }
    }
}

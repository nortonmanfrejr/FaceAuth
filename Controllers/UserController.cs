using FaceAuth.Data;
using FaceAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FaceAuth.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var MM = new UserViewModel()
            {
                Users = _context.User.Include(u => u.Pictures).ToList(),

            };

            return View(MM);
        }
        // Exibir o formulário de cadastro de usuário
        public IActionResult Create()
        {
            var MM = new UserViewModel()
            {
                //Title = "Cadastro de Usuário com Foto",
                User = new User()
                {
                    Email = "teste@teste.com"
                },
                Pictures = new List<IFormFile>()
            };
            return View(MM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel MM)
        {
            var reg = MM.User;
            if (ModelState.IsValid)
            {
                // Salvar usuário no banco
                _context.User.Add(reg);
                await _context.SaveChangesAsync();

                // Salvar fotos no banco
                if (MM.Pictures != null && MM.Pictures.Count > 0)
                {
                    foreach (var picture in MM.Pictures)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await picture.CopyToAsync(memoryStream);
                            var userPic = new UserPic
                            {
                                Picture = memoryStream.ToArray(),
                                UserId = reg.Id
                            };
                            _context.UserPics.Add(userPic);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }

            return View(MM);
        }

        
    }
}

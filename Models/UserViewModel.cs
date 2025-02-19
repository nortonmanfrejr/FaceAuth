using FaceAuth.Data;

namespace FaceAuth.Models
{
    public class UserViewModel
    {
        //public string Title { get; set; }
        public User User { get; set; }
        public List<IFormFile> Pictures { get; set; }

        public List<User> Users { get; set; } = new List<User>();
    }
}

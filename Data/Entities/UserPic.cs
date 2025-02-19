using System.ComponentModel.DataAnnotations;

namespace FaceAuth.Data
{
    public class UserPic
    {
        [Key]
        public int Id { get; set; }
        public byte[] Picture { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}

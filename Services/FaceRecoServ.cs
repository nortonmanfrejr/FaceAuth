using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using FaceAuth.Data;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Drawing.Imaging;

namespace FaceAuth.Services
{
    public class FaceRecoServ
    {
        private readonly AppDbContext _context;
        private readonly CascadeClassifier _faceCascade;

        public FaceRecoServ(AppDbContext context)
        {
            _context = context;
            _faceCascade = new CascadeClassifier(Path.Combine(Directory.GetCurrentDirectory(), "cascades", "haarcascade_frontalface_default.xml"));
        }

        public User RecognizeFace(byte[] imageBytes)
        {
            try
            {
                // Converter bytes para imagem
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    // Criar o Bitmap a partir do MemoryStream
                    Bitmap bitmap = new Bitmap(ms);

                    // Converter Bitmap para Image<Bgr, byte> diretamente
                    Image<Bgr, byte> inputImage = BitmapToImage<Bgr, byte>(bitmap);

                    var grayImage = inputImage.Convert<Gray, byte>(); // Converte para escala de cinza

                    // Detectar rostos na imagem capturada
                    var facesDetected = _faceCascade.DetectMultiScale(grayImage, 1.1, 10, Size.Empty);

                    if (facesDetected.Length == 0)
                        return null; // Nenhum rosto encontrado

                    // Obter o primeiro rosto encontrado
                    var faceRegion = grayImage.Copy(facesDetected[0]).Resize(100, 100, Inter.Linear);

                    // Buscar no banco um usuário com rosto semelhante
                    foreach (var user in _context.UserPics.Include(u => u.User))
                    {
                        Image<Gray, byte> dbFace = new Image<Gray, byte>(100, 100);
                        dbFace.Bytes = user.Picture;

                        // Comparar a imagem com a base de dados
                        double diff = CompareImages(dbFace, faceRegion);

                        if (diff < 3000) // Se for um valor baixo, significa que há correspondência
                        {
                            return user.User;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro no reconhecimento facial: " + ex.Message);
            }

            return null; // Nenhum usuário reconhecido
        }

        private Image<TColor, TDepth> BitmapToImage<TColor, TDepth>(Bitmap bitmap)
            where TColor : struct, IColor
            where TDepth : new()
        {
            // Converter Bitmap para Image em Emgu.CV
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            int bytesPerPixel = Bitmap.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            int byteCount = bmpData.Stride * bitmap.Height;
            byte[] pixelData = new byte[byteCount];

            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, pixelData, 0, byteCount);
            bitmap.UnlockBits(bmpData);

            // Criar a imagem Emgu.CV a partir dos dados
            var image = new Image<TColor, TDepth>(bitmap.Width, bitmap.Height);
            image.Bytes = pixelData;

            return image;
        }

        private double CompareImages(Image<Gray, byte> img1, Image<Gray, byte> img2)
        {
            return CvInvoke.Norm(img1, img2, NormType.L2);
        }
    }
}

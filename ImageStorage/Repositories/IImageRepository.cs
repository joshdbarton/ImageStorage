using System.IO;

namespace ImageStorage.Repositories
{
    public interface IImageRepository
    {
        Stream GetImageStreamById(int id);
        int CreateImage(byte[] data);

        void DeleteImage(int? id);
    }
}
using ProtectedFiles.Domain.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProtectedFiles.Domain
{
    public class LocalStorageFileManager : IFileManager
    {
        private LocalStorageFileManagerOptions _options;

        public LocalStorageFileManager(LocalStorageFileManagerOptions options)
        {
            _options = options;
        }

        public async Task UploadFileAsync(string fileName, Stream stream)
        {
            DirectoryEnsureCreated();
            var filePath = Path.Combine(_options.Directory, fileName);
            var fileNameWOExtension = Path.GetFileNameWithoutExtension(fileName);
            var files = Directory.GetFiles(_options.Directory);
            var existingFile = files.SingleOrDefault(
                f => Path.GetFileNameWithoutExtension(f) == fileNameWOExtension);

            if (existingFile != null)
            {
                File.Delete(existingFile);
            }

            using (var destinationStream = File.Create(filePath))
            {
                await stream.CopyToAsync(destinationStream);
            }
        }

        private void DirectoryEnsureCreated()
        {
            var directoryInfo = new DirectoryInfo(_options.Directory);

            if (directoryInfo.Exists)
            {
                return;
            }

            directoryInfo = Directory.CreateDirectory(_options.Directory);
            directoryInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        }
    }
}

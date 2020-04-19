using ProtectedFiles.Domain.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProtectedFiles.Domain
{
    public class LocalStorageFileManager : IFileManager
    {
        private LocalStorageFileManagerOptions Options { get; set; }

        public LocalStorageFileManager(LocalStorageFileManagerOptions options)
        {
            Options = options;
        }

        public async Task UploadFileAsync(string fileName, Stream stream)
        {
            DirectoryEnsureCreated();
            var filePath = Path.Combine(Options.Directory, fileName);
            var fileNameWOExtension = Path.GetFileNameWithoutExtension(fileName);
            var files = Directory.GetFiles(Options.Directory);
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
            var directoryInfo = new DirectoryInfo(Options.Directory);

            if (directoryInfo.Exists)
            {
                return;
            }

            directoryInfo = Directory.CreateDirectory(Options.Directory);
            directoryInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace RDAExplorer.FileDBTool.Commands
{
    class GenFileDBCommand : ManyConsole.ConsoleCommand
    {
        private string outputFileName;

        public GenFileDBCommand()
        {
            IsCommand("gen", "Generate file.db from RDA files");
            HasLongDescription("Generate file.db to standard output from RDA files given as parameters.");

            AllowsAnyAdditionalArguments("(RDA files)|(directory containing RDA files)");

            HasRequiredOption("output|o=", "The output file name", (value) => { this.outputFileName = value; });
        }

        public override int Run(string[] remainingArguments)
        {
            var archiveFiles = new AnnoRDA.FileDB.Writer.ArchiveFileMap();
            AnnoRDA.FileSystem fileSystem;

            if (remainingArguments.Length == 1 && this.PathIsDirectory(remainingArguments[0])) {
                var directoryLoader = new AnnoRDA.Loader.ContainerDirectoryLoader();
                var loadResult = directoryLoader.Load(remainingArguments[0], System.Threading.CancellationToken.None);

                fileSystem = loadResult.FileSystem;
                foreach (string path in loadResult.ContainerPaths) {
                    archiveFiles.Add(path, path);
                }
            } else {
                fileSystem = new AnnoRDA.FileSystem();
                var fileLoader = new AnnoRDA.Loader.ContainerFileLoader();
                foreach (var rdaFileName in remainingArguments) {
                    var loadedFS = fileLoader.Load(rdaFileName);
                    fileSystem.OverwriteWith(loadedFS, null, System.Threading.CancellationToken.None);

                    archiveFiles.Add(rdaFileName, rdaFileName);
                }
            }

            using (var outputStream = new System.IO.FileStream(this.outputFileName, System.IO.FileMode.Create, System.IO.FileAccess.Write)) {
                using (var writer = new AnnoRDA.FileDB.Writer.FileSystemWriter(outputStream, false)) {
                    writer.WriteFileSystem(fileSystem, archiveFiles);
                }
            }

            return 0;
        }

        private bool PathIsDirectory(string path)
        {
            return (System.IO.File.GetAttributes(path) & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory;
        }
    }
}

using System.IO;

using AnnoRDA.FileDB.Reader;
using AnnoRDA.FileDB.Structs;

namespace RDAExplorer.FileDBTool.Commands
{
    class DumpFileDBCommand : ManyConsole.ConsoleCommand
    {
        public DumpFileDBCommand()
        {
            IsCommand("dump", "Dump file.db");
            HasLongDescription("Dump contents of a file.db to standard output.");

            HasAdditionalArguments(1, " file.db");
        }

        public override int Run(string[] remainingArguments)
        {
            using (var fileStream = new FileStream(remainingArguments[0], FileMode.Open, FileAccess.Read)) {
                using (var reader = new DBReader(fileStream)) {
                    Node content = reader.ReadFile();
                    this.Dump(content, 0);
                }
            }
            return 0;
        }

        public void Dump(Node node, int level)
        {
            switch (node.Tag.Type) {
                case Tag.TagType.Attribute: {
                    this.DumpPadding(level);
                    this.DumpTag(node.Tag, false);
                    System.Console.Write(": ");
                    this.DumpAttributeValue(node, true);
                    break;
                }
                case Tag.TagType.StructureStart: {
                    this.DumpPadding(level);
                    this.DumpTag(node.Tag, true);
                    foreach (var child in node.Children) {
                        this.Dump(child, level + 1);
                    }
                    break;
                }
                case Tag.TagType.StructureEnd: {
                    throw new System.FormatException(System.String.Format("Unexpected tag type (StructureEnd) of tag: {0} ({1})", node.Tag.ID, node.Tag.Name));
                }
            }
        }

        public void DumpPadding(int level)
        {
            string padding = new System.String(' ', level * 2);
            System.Console.Out.Write(padding);
        }

        public void DumpTag(Tag tag, bool writeNewline)
        {
            System.Console.Out.Write(tag.ID.ToString("X4") + " " + tag.Name);
            if (writeNewline) {
                System.Console.Out.WriteLine("");
            }
        }

        public void DumpAttributeValue(Node node, bool writeNewline)
        {
            switch (node.Tag.Name)
            {
                case Tags.BuiltinTags.Names.String:
                case FileSystemTags.Attributes.Names.FileName:
                {
                    System.Console.Out.Write(node.ValueToString());
                    break;
                }
                case FileSystemTags.Attributes.Names.ArchiveFileIndex:
                {
                    System.Console.Out.Write(node.ValueToUInt32());
                    break;
                }
                case FileSystemTags.Attributes.Names.Position:
                {
                    System.Console.Out.Write(node.ValueToUInt64());
                    break;
                }
                case FileSystemTags.Attributes.Names.CompressedSize:
                {
                    System.Console.Out.Write(node.ValueToUInt64());
                    break;
                }
                case FileSystemTags.Attributes.Names.UncompressedSize:
                {
                    System.Console.Out.Write(node.ValueToUInt64());
                    break;
                }
                case FileSystemTags.Attributes.Names.ModificationTime:
                {
                    System.Console.Out.Write(node.ValueToUInt64());
                    break;
                }
                case FileSystemTags.Attributes.Names.Flags:
                {
                    System.Console.Out.Write(node.ValueToUInt32());
                    break;
                }
                case FileSystemTags.Attributes.Names.ResidentBufferIndex:
                {
                    System.Console.Out.Write(node.ValueToUInt32());
                    break;
                }
                case FileSystemTags.Attributes.Names.LastArchiveFile:
                {
                    System.Console.Out.Write(node.ValueToString());
                    break;
                }
                case FileSystemTags.Attributes.Names.Size:
                {
                    System.Console.Out.Write(node.ValueToUInt32());
                    break;
                }
                case FileSystemTags.Attributes.Names.Buffer:
                {
                    for (int i = 0; i < node.Value.Length && i < 10; ++i)
                    {
                        System.Console.Out.Write(System.String.Format("{0:X2} ", node.Value[i]));
                    }
                    if (node.Value.Length > 10)
                    {
                        System.Console.Out.Write("...");
                    }
                    break;
                }
                default:
                {
                    throw new System.ArgumentException("node.tag.Name");
                }
            }
            if (writeNewline) {
                System.Console.Out.WriteLine("");
            }
        }
    }
}

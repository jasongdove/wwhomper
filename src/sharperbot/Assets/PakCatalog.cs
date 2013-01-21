using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace sharperbot.Assets
{
    internal sealed class PakCatalog : IAssetCatalog
    {
        private const byte PakKey = 0xf7;
        private const byte EndOfContents = 0x80;

        private readonly string _fileName;
        private readonly List<PakEntry> _entries;

        public PakCatalog()
        {
            _fileName = ConfigurationManager.AppSettings["sharperbot.PakCatalog"];
            _entries = new List<PakEntry>();

            if (!File.Exists(_fileName))
            {
                throw new FileNotFoundException(_fileName);
            }

            var buffer = File.ReadAllBytes(_fileName);
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] ^= PakKey;
            }

            using (var ms = new MemoryStream(buffer))
            {
                using (var br = new BinaryReader(ms))
                {
                    // Validate header
                    var expectedHeader = new byte[] { 0xc0, 0x4a, 0xc0, 0xba };
                    for (int i = 0; i < 4; i++)
                    {
                        var actual = br.ReadByte();
                        if (actual != expectedHeader[i])
                        {
                            throw new FormatException("pak file is an unexpected format");
                        }
                    }

                    // Read TOC
                    ms.Seek(9, SeekOrigin.Begin);
                    long offset;
                    while (true)
                    {
                        var entry = new PakEntry();

                        byte fileNameLength = br.ReadByte();
                        entry.Name = Encoding.ASCII.GetString(br.ReadBytes(fileNameLength));
                        entry.Length = br.ReadUInt32();
                        br.ReadBytes(8); // skip

                        _entries.Add(entry);

                        byte flag = br.ReadByte();
                        if (flag == EndOfContents)
                        {
                            // Entry contents start here
                            offset = ms.Position;
                            break;
                        }
                    }

                    // Calculate offsets
                    foreach (var item in _entries)
                    {
                        item.Offset = offset;
                        offset += item.Length;
                    }
                }
            }
        }

        public void Dump(string targetDirectory)
        {
            foreach (var entry in _entries)
            {
                var fileName = Path.Combine(targetDirectory, entry.Name);
                var directory = Path.GetDirectoryName(fileName);
                if (directory != null)
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    using (var file = File.Create(fileName))
                    {
                        var bytes = GetEntryBytes(entry);
                        file.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public string GetEntryText(string fileName)
        {
            var entry = GetEntry(fileName);
            return Encoding.ASCII.GetString(GetEntryBytes(entry));
        }

        public Image<Bgra, byte> GetEntryImage(string fileName)
        {
            var entry = GetEntry(fileName);
            var stream = new MemoryStream(GetEntryBytes(entry));
            return new Image<Bgra, byte>(new Bitmap(stream));
        }

        public Image<Bgra, byte> GetCompositeImage(string fileName)
        {
            string pngFileName = fileName.Replace(".jpg", "_.png");

            // If there isn't a matching alpha image, just return the one image directly
            if (_entries.All(x => x.Name != pngFileName))
            {
                return GetEntryImage(fileName);
            }

            var jpgImage = GetEntryImage(fileName);
            var pngImage = GetEntryImage(pngFileName);

            return jpgImage.CombineAlpha(pngImage);
        }

        private byte[] GetEntryBytes(PakEntry entry)
        {
            using (var file = File.OpenRead(_fileName))
            {
                file.Seek((int)entry.Offset, SeekOrigin.Begin);
                var buffer = new byte[entry.Length];
                file.Read(buffer, 0, (int)entry.Length);
                for (int i = 0; i < buffer.Length; i++)
                {
                    buffer[i] ^= PakKey;
                }

                return buffer;
            }
        }

        private PakEntry GetEntry(string fileName)
        {
            return _entries.First(x => x.Name == fileName);
        }
    }
}
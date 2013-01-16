using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;

namespace wwhomper.Pak
{
    public class PakCatalog
    {
        private const byte PakKey = 0xf7;
        private const byte EndOfTOC = 0x80;

        private readonly string _fileName;
        private readonly List<PakEntry> _entries;

        public PakCatalog(string fileName)
        {
            _fileName = fileName;
            _entries = new List<PakEntry>();
        }

        public ReadOnlyCollection<PakEntry> Entries
        {
            get { return _entries.AsReadOnly(); }
        }

        public void Load()
        {
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
                        if (flag == EndOfTOC)
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

        public string GetEntryText(PakEntry entry)
        {
            return Encoding.ASCII.GetString(GetEntryBytes(entry));
        }

        public Image<Bgra, byte> GetEntryImage(PakEntry entry)
        {
            using (var stream = new MemoryStream(GetEntryBytes(entry)))
            {
                return new Image<Bgra, byte>(new Bitmap(stream));
            }
        }

        public Image<Bgra, byte> GetCompositeImage(PakEntry jpg, PakEntry png)
        {
            var jpgImage = GetEntryImage(jpg);

            // Convert png to grayscale so we get an intensity value
            var pngImage = GetEntryImage(png).Convert<Gray, byte>();

            for (int y = 0; y < jpgImage.Data.GetLength(0); y++)
            {
                for (int x = 0; x < jpgImage.Data.GetLength(1); x++)
                {
                    var existing = jpgImage[y, x];
                    jpgImage[y, x] = new Bgra(
                        existing.Blue,
                        existing.Green,
                        existing.Red,
                        pngImage[y, x].Intensity);
                }
            }

            return jpgImage;
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
    }
}
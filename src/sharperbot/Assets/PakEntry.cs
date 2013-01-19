using System;

namespace sharperbot.Assets
{
    public class PakEntry
    {
        public string Name { get; set; }
        public UInt32 Length { get; set; }
        public long Offset { get; set; } 
    }
}
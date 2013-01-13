using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;

namespace wwhomper
{
    public class TemplateSearchResult
    {
        public bool Success { get; set; }
        public Point Point { get; set; }
        public Image<Gray, byte> Template { get; set; }
    }
}
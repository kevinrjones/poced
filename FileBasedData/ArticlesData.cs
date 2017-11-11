using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using DataInterfaces;

namespace FileBasedData
{
    public class ArticlesData : IArticlesData
    {
        public byte[] GetImage(int id)
        {
            Image img = Image.FromFile($"{AppDomain.CurrentDomain.RelativeSearchPath}/images/{id}.png");

            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }
    }
}

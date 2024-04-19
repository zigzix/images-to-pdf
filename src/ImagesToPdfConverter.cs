using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesToPdf
{
    internal class ImagesToPdfConverter
    {
        List<string> _imageFilenames = new List<string>();
        string _outputFolder;
        string _pdfFilename;

        string[] _supportImages = new string[] { ".png", ".jpg", ".bmp", ".gif", ".jpeg", ".tiff" };

        public ImagesToPdfConverter(List<string> imageFiles, string outputFolder, string pdfFilename)
        {
            foreach (var filename in imageFiles)
            {
                if (_supportImages.Contains(Path.GetExtension(filename).ToLower()) == false)
                    continue;

                _imageFilenames.Add(filename);
            }
            _outputFolder = outputFolder;
            _pdfFilename = pdfFilename;
        }

        public ImagesToPdfConverter(string imagesFolder, string outputFolder, string pdfFilename)
        {
            foreach (var filename in Directory.EnumerateFiles(imagesFolder))
            {
                if (_supportImages.Contains(Path.GetExtension(filename).ToLower()) == false)
                    continue;

                _imageFilenames.Add(filename);
            }
            _outputFolder = outputFolder;
            _pdfFilename = pdfFilename;
        }

        public void Convert()
        {
            _imageFilenames.Sort(delegate (string x, string y)
            {
                string xFilename = System.IO.Path.GetFileNameWithoutExtension(x);
                string yFilename = System.IO.Path.GetFileNameWithoutExtension(y);

                int xNo, yNo;

                if (int.TryParse(xFilename, out xNo) && int.TryParse(yFilename, out yNo))
                {
                    return xNo - yNo;
                }
                return Comparer<string>.Default.Compare(xFilename, yFilename);
            });

            Rectangle rect = PageSize.A4;
            iTextSharp.text.Image firstImage = iTextSharp.text.Image.GetInstance(_imageFilenames.First());
            if (firstImage.Width > firstImage.Height)
            {
                rect = rect.Rotate();
            }

            string outputFilename = Path.Combine(_outputFolder, _pdfFilename);

            Document doc = new Document(rect);
            PdfWriter.GetInstance(doc, new FileStream(outputFilename, FileMode.Create));
            doc.Open();
            foreach (var imageFilename in _imageFilenames)
            {
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageFilename);
                
                image.ScaleToFit(rect);
                image.SetAbsolutePosition(0, 0);
                doc.Add(image);
                doc.NewPage();
            }
            doc.Close();
        }
    }
}
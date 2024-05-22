using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
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
                if (_supportImages.Contains(System.IO.Path.GetExtension(filename).ToLower()) == false)
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
                if (_supportImages.Contains(System.IO.Path.GetExtension(filename).ToLower()) == false)
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

            Image firstImage = new Image(ImageDataFactory.Create(_imageFilenames.First()));
            PageSize pageSize = new PageSize(firstImage.GetImageWidth(), firstImage.GetImageHeight());
            string outputFilename = System.IO.Path.Combine(_outputFolder, _pdfFilename);

            PdfWriter pdfWriter = new PdfWriter(outputFilename);
            PdfDocument pdf = new PdfDocument(pdfWriter);
            
            Document doc = new Document(pdf, pageSize);
            doc.SetMargins(0, 0, 0, 0);
            
            //doc.Open();
            for (int i = 0; i < _imageFilenames.Count(); i++)
            {
                pdf.AddNewPage(pageSize);

                Image image = new Image(ImageDataFactory.Create(_imageFilenames[i]));
                image.SetFixedPosition(i+1, 0, 0);
                doc.Add(image);
            }
            doc.Close();
        }
    }
}
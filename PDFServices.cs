using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.Services
{
    public class PDFServices
    {

        public static byte[] GetPDF(string pHTML)
        {
            byte[] bPDF = null;



            MemoryStream ms = new MemoryStream();
            TextReader txtReader = new StringReader(pHTML);
            //var fontFamily = FontFactory.GetFont("Arial", 14, Font.BOLD);

            // 1: create object of a itextsharp document class
            Document doc = new Document(PageSize.LETTER, 60f, 60f, 50f, 50f);

            
            // 2: we create a itextsharp pdfwriter that listens to the document and directs a XML-stream to a file
            PdfWriter oPdfWriter = PdfWriter.GetInstance(doc, ms);


            // 3: we create a worker parse the document
            HTMLWorker htmlWorker = new HTMLWorker(doc);

            //var style = new StyleSheet();
            //style.LoadTagStyle("body", "size", "19px");
            //style.LoadTagStyle("body", "font-family", "Calibri");
            //htmlWorker.SetStyleSheet(style);
            
            // 4: we open document and start the worker on the document
            doc.Open();
            htmlWorker.StartDocument();

            // 5: parse the html into the document
            htmlWorker.Parse(txtReader);

            

            // 6: close the document and the worker
            htmlWorker.EndDocument();
            htmlWorker.Close();
            doc.Close();

            bPDF = ms.ToArray();

            return bPDF;
        }

    }
}

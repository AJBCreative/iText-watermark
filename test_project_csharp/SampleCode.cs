using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Drawing;
using Rectangle = iText.Kernel.Geom.Rectangle;

namespace test_project_csharp;

class SampleCode
{
    const string BaseUri = "../../../resources/";
    const string Src = BaseUri + "input.pdf";
    const string Dest = BaseUri + "watermarked_output.pdf";

    public static void Main()
    {
        new SampleCode().ManipulatePdf(Src,Dest);
    }

    protected void ManipulatePdf(string inFilePath, string outFilePath)
    {

        PdfDocument pdfDoc = new PdfDocument(new PdfReader(inFilePath), new PdfWriter(outFilePath));


        for (int i = 1; i<= pdfDoc.GetNumberOfPages(); i++)
        {
            // we create a new canvas immediately before the writing the data from "input.pdf" by using the .NewContentStreamBefore method
            // we loop through each page starting at page 1 
            PdfCanvas under = new PdfCanvas(pdfDoc.GetPage(i).NewContentStreamBefore(), new PdfResources(), pdfDoc);
            // build a "page" object to store parameters of data to use later

            PdfPage page = pdfDoc.GetPage(i);

            //we get the exact middle of the page from pdfDoc
            var pageSize = page.GetPageSize();
            //get width and divide by 2
            float xPage = (pageSize.GetWidth() / 2);
            //get height and divide by 2
            float yPage = (pageSize.GetHeight() / 2);
            float fontSize = 30;
            var rotation = 45;
            float opacity = 0.5f;
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            dynamic paragraph = new Paragraph("Owned By: John Doe")
                       .SetFont(font)
                       .SetFontSize(fontSize)
                       .SetFontColor(ColorConstants.RED);
            // Set transparency using PdfExtGState
            PdfExtGState transparencyState = new PdfExtGState().SetFillOpacity(opacity);

            under.SaveState();
            under.SetExtGState(transparencyState);


            //we write our watermarkCanvas before the original contents of the input.pdf
            Canvas watermarkCanvas = new Canvas(under, pageSize)
                .ShowTextAligned(paragraph,
                    // set our exact middle to render the paragraph
                    xPage, yPage, 1,
                    TextAlignment.CENTER,
                    VerticalAlignment.TOP,
                    //Rotation likely as expected now that we're getting the exact middle of the page
                    rotation
                );

            watermarkCanvas.Close();
            //restore the graphics state so we don't accidentally write new content using our transparency params in case of other Canvas items
            under.RestoreState();
        }

        
        pdfDoc.Close();
    }
}
﻿// Copyright (c) 2001-2020 Aspose Pty Ltd. All Rights Reserved.
//
// This file is part of Aspose.Words. The source code in this file
// is only intended as a supplement to the documentation, and is provided
// "as is", without warranty of any kind, either expressed or implied.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using Aspose.Pdf.Text;
using Aspose.Words;
using Aspose.Words.Fonts;
using Aspose.Words.Rendering;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using NUnit.Framework;
using FolderFontSource = Aspose.Words.Fonts.FolderFontSource;
#if NET462 || JAVA
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing.Text;
#elif NETCOREAPP2_1 || __MOBILE__
using SkiaSharp;
#endif

namespace ApiExamples
{
    [TestFixture]
    public class ExRendering : ApiExampleBase
    {
        [Test]
        public void SaveToPdfStreamOnePage()
        {
            //ExStart
            //ExFor:FixedPageSaveOptions.PageIndex
            //ExFor:FixedPageSaveOptions.PageCount
            //ExFor:Document.Save(Stream, SaveOptions)
            //ExSummary:Shows how to convert only some of the pages in a document to PDF.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Page 1.");
            builder.InsertBreak(BreakType.PageBreak);
            builder.Writeln("Page 2.");
            builder.InsertBreak(BreakType.PageBreak);
            builder.Writeln("Page 3.");

            using (Stream stream = File.Create(ArtifactsDir + "Rendering.SaveToPdfStreamOnePage.pdf"))
            {
                // Create a "PdfSaveOptions" object which we can pass to the document's "Save" method
                // to modify the way in which that method converts the document to .PDF.
                PdfSaveOptions options = new PdfSaveOptions();

                // Set the "PageIndex" to "1" to render a portion of the document starting from the second page.
                options.PageIndex = 1;

                // Set the "PageCount" to "1" to render only one page of the document,
                // starting from the page that the "PageIndex" property specified.
                options.PageCount = 1;
                
                // This document will contain one page starting from page two, which means it will only contain the second page.
                doc.Save(stream, options);
            }
            //ExEnd

#if NET462 || NETCOREAPP2_1 || JAVA
            Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(ArtifactsDir + "Rendering.SaveToPdfStreamOnePage.pdf");

            Assert.AreEqual(1, pdfDocument.Pages.Count);

            TextFragmentAbsorber textFragmentAbsorber = new TextFragmentAbsorber();
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            Assert.AreEqual("Page 2.", textFragmentAbsorber.Text);
#endif
        }

        [Test]
        public void OnePage()
        {
            //ExStart
            //ExFor:Document.Save(String, SaveOptions)
            //ExFor:FixedPageSaveOptions
            //ExFor:ImageSaveOptions.PageIndex
            //ExSummary:Shows how to render one page from a document to a JPEG image.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Page 1.");
            builder.InsertBreak(BreakType.PageBreak);
            builder.Writeln("Page 2.");
            builder.InsertImage(ImageDir + "Logo.jpg");
            builder.InsertBreak(BreakType.PageBreak);
            builder.Writeln("Page 3.");

            // Create an "ImageSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method renders the document into an image.
            ImageSaveOptions options = new ImageSaveOptions(SaveFormat.Jpeg);

            // Set the "PageIndex" to "1" to select the second page via
            // the zero-based index to start rendering the document from.
            options.PageIndex = 1;

            // When we save the document to the JPEG format, Aspose.Words only renders one page.
            // This image will contain one page starting from page two,
            // which will just be the second page of the original document.
            doc.Save(ArtifactsDir + "Rendering.OnePage.jpg", options);
            //ExEnd

            TestUtil.VerifyImage(816, 1056, ArtifactsDir + "Rendering.OnePage.jpg");
        }

        [Test, Category("SkipMono")]
        public void PageByPage()
        {
            //ExStart
            //ExFor:Document.Save(String, SaveOptions)
            //ExFor:FixedPageSaveOptions
            //ExFor:ImageSaveOptions.PageIndex
            //ExFor:ImageSaveOptions.PageCount
            //ExSummary:Shows how to render every page of a document to a separate TIFF image.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Page 1.");
            builder.InsertBreak(BreakType.PageBreak);
            builder.Writeln("Page 2.");
            builder.InsertImage(ImageDir + "Logo.jpg");
            builder.InsertBreak(BreakType.PageBreak);
            builder.Writeln("Page 3.");

            // Create an "ImageSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method renders the document into an image.
            ImageSaveOptions options = new ImageSaveOptions(SaveFormat.Tiff);

            // Set the "PageCount" property to "1" to render only one page of the document.
            // Many other image formats only render one page at a time, and do not use this property.
            options.PageCount = 1;

            for (int i = 0; i < doc.PageCount; i++)
            {
                // Set the "PageIndex" property to the number of the first page from
                // which to start rendering the document from.
                options.PageIndex = i;

                doc.Save(ArtifactsDir + $"Rendering.PageByPage.{i + 1}.tiff", options);
            }
            //ExEnd

            List<string> imageFileNames = Directory.GetFiles(ArtifactsDir, "*.tiff")
                .Where(item => item.Contains("Rendering.PageByPage.") && item.EndsWith(".tiff")).ToList();

            Assert.AreEqual(3, imageFileNames.Count);

            foreach (string imageFileName in imageFileNames)
                TestUtil.VerifyImage(816, 1056, imageFileName);
        }

        [TestCase(PdfTextCompression.None)]
        [TestCase(PdfTextCompression.Flate)]
        public void TextCompression(PdfTextCompression pdfTextCompression)
        {
            //ExStart
            //ExFor:PdfSaveOptions
            //ExFor:PdfSaveOptions.TextCompression
            //ExFor:PdfTextCompression
            //ExSummary:Shows how to apply text compression when saving a document to PDF.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            for (int i = 0; i < 100; i++)
                builder.Writeln("Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
                                "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");

            // Create a "PdfSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method converts the document to .PDF.
            PdfSaveOptions options = new PdfSaveOptions();

            // Set the "TextCompression" property to "PdfTextCompression.None" to not apply any
            // compression to text when we save the document to PDF.
            // Set the "TextCompression" property to "PdfTextCompression.Flate" to apply ZIP compression
            // to text when we save the document to PDF. The larger the document, the bigger the impact that this will have.
            options.TextCompression = pdfTextCompression;

            doc.Save(ArtifactsDir + "Rendering.TextCompression.pdf", options);

            switch (pdfTextCompression)
            {
                case PdfTextCompression.None:
                    Assert.That(60000, Is.LessThan(new FileInfo(ArtifactsDir + "Rendering.TextCompression.pdf").Length));
                    TestUtil.FileContainsString("5 0 obj\r\n<</Length 9 0 R>>stream", ArtifactsDir + "Rendering.TextCompression.pdf"); //ExSkip
                    break;
                case PdfTextCompression.Flate:
                    Assert.That(30000, Is.AtLeast(new FileInfo(ArtifactsDir + "Rendering.TextCompression.pdf").Length));
                    TestUtil.FileContainsString("5 0 obj\r\n<</Length 9 0 R/Filter /FlateDecode>>stream", ArtifactsDir + "Rendering.TextCompression.pdf"); //ExSkip
                    break;
            }
            //ExEnd
        }

        [TestCase(false)]
        [TestCase(true)]
        public void PreserveFormFields(bool preserveFormFields)
        {
            //ExStart
            //ExFor:PdfSaveOptions.PreserveFormFields
            //ExSummary:Shows how to save a document to the PDF format using the Save method and the PdfSaveOptions class.
            // Open the document
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Write("Please select a fruit: ");

            // Insert a combo box which will allow a user to choose an option from a collection of strings.
            builder.InsertComboBox("MyComboBox", new[] { "Apple", "Banana", "Cherry" }, 0);

            // Create a "PdfSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method converts the document to .PDF.
            PdfSaveOptions pdfOptions = new PdfSaveOptions();

            // Set the "PreserveFormFields" property to "true" to save form fields as interactive objects in the output PDF.
            // Set the "PreserveFormFields" property to "false" to freeze all form fields in the document at
            // their current values, and display them as plain text in the output PDF.
            pdfOptions.PreserveFormFields = preserveFormFields;

            doc.Save(ArtifactsDir + "Rendering.PreserveFormFields.pdf", pdfOptions);
            //ExEnd

#if NET462 || NETCOREAPP2_1 || JAVA
            Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(ArtifactsDir + "Rendering.PreserveFormFields.pdf");

            Assert.AreEqual(1, pdfDocument.Pages.Count);

            TextFragmentAbsorber textFragmentAbsorber = new TextFragmentAbsorber();
            pdfDocument.Pages.Accept(textFragmentAbsorber);

            if (preserveFormFields)
            {
                Assert.AreEqual("Please select a fruit: ", textFragmentAbsorber.Text);
                TestUtil.FileContainsString("10 0 obj\r\n" +
                                            "<</Type /Annot/Subtype /Widget/P 4 0 R/FT /Ch/F 4/Rect [168.39199829 707.35101318 217.87442017 722.64007568]/Ff 131072/T(þÿ\0M\0y\0C\0o\0m\0b\0o\0B\0o\0x)/Opt " +
                                            "[(þÿ\0A\0p\0p\0l\0e) (þÿ\0B\0a\0n\0a\0n\0a) (þÿ\0C\0h\0e\0r\0r\0y) ]/V(þÿ\0A\0p\0p\0l\0e)/DA(0 g /FAAABC 12 Tf )/AP<</N 11 0 R>>>>", 
                    ArtifactsDir + "Rendering.PreserveFormFields.pdf");
            }
            else
            {
                Assert.AreEqual("Please select a fruit: Apple", textFragmentAbsorber.Text);
                Assert.Throws<AssertionException>(() =>
                {
                    TestUtil.FileContainsString("/Widget", 
                        ArtifactsDir + "Rendering.PreserveFormFields.pdf");
                });
            }
#endif
        }

        [Test]
        public void SaveAsXps()
        {
            //ExStart
            //ExFor:XpsSaveOptions
            //ExFor:XpsSaveOptions.#ctor
            //ExFor:XpsSaveOptions.OutlineOptions
            //ExFor:XpsSaveOptions.SaveFormat
            //ExSummary:Shows how to limit the level of headings that will appear in the outline of a saved XPS document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Insert headings that can serve as TOC entries of levels 1, 2, and then 3.
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;

            Assert.True(builder.ParagraphFormat.IsHeading);

            builder.Writeln("Heading 1");

            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading2;

            builder.Writeln("Heading 1.1");
            builder.Writeln("Heading 1.2");

            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading3;

            builder.Writeln("Heading 1.2.1");
            builder.Writeln("Heading 1.2.2");

            // Create an "XpsSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method converts the document to .XPS.
            XpsSaveOptions saveOptions = new XpsSaveOptions();
            
            Assert.AreEqual(SaveFormat.Xps, saveOptions.SaveFormat);

            // The output XPS document will contain an outline, which is a table of contents that lists headings in the document body.
            // Clicking on an entry in this outline will take us to the location of its respective heading.
            // Set the "HeadingsOutlineLevels" property to "2" to exclude all headings whose levels are above 2 from the outline.
            // The last two headings we have inserted above will not appear.
            saveOptions.OutlineOptions.HeadingsOutlineLevels = 2;

            doc.Save(ArtifactsDir + "Rendering.SaveAsXps.xps", saveOptions);
            //ExEnd
        }

        [TestCase(false)]
        [TestCase(true)]
        public void SaveAsXpsBookFold(bool renderTextAsBookfold)
        {
            //ExStart
            //ExFor:XpsSaveOptions.#ctor(SaveFormat)
            //ExFor:XpsSaveOptions.UseBookFoldPrintingSettings
            //ExSummary:Shows how to save a document to the XPS format in the form of a book fold.
            Document doc = new Document(MyDir + "Paragraphs.docx");

            // Create an "XpsSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method converts the document to .XPS.
            XpsSaveOptions xpsOptions = new XpsSaveOptions(SaveFormat.Xps);

            // Set the "UseBookFoldPrintingSettings" property to "true" to arrange the contents
            // in the output XPS in a way that helps us use it to make a booklet.
            // Set the "UseBookFoldPrintingSettings" property to "false" to render the XPS normally.
            xpsOptions.UseBookFoldPrintingSettings = true;

            // If we are rendering the document as a booklet, we must set the "MultiplePages"
            // properties of all page setup objects of all sections to "MultiplePagesType.BookFoldPrinting".
            if (renderTextAsBookfold)
                foreach (Section s in doc.Sections)
                {
                    s.PageSetup.MultiplePages = MultiplePagesType.BookFoldPrinting;
                }

            // Once we print this document, we can turn it into a booklet by stacking the pages
            // in the order they come out of the printer and then folding down the middle
            doc.Save(ArtifactsDir + "Rendering.SaveAsXpsBookFold.xps", xpsOptions);
            //ExEnd
        }

        [Test]
        public void SaveAsImage()
        {
            //ExStart
            //ExFor:Document.Save(String, SaveOptions)
            //ExFor:FixedPageSaveOptions.JpegQuality
            //ExFor:ImageSaveOptions
            //ExFor:ImageSaveOptions.#ctor
            //ExFor:ImageSaveOptions.JpegQuality
            //ExSummary:Shows how to configure compression while saving a document as a JPEG.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertImage(ImageDir + "Logo.jpg");
            
            // Create an "ImageSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method renders the document into an image.
            ImageSaveOptions imageOptions = new ImageSaveOptions(SaveFormat.Jpeg);

            // Set the "JpegQuality" property to "10" to use stronger compression when rendering the document.
            // This will reduce the file size of the document, but the image will display more prominent compression artifacts.
            imageOptions.JpegQuality = 10;

            doc.Save(ArtifactsDir + "Rendering.SaveAsImage.HighCompression.jpg", imageOptions);

            Assert.That(20000, Is.AtLeast(new FileInfo(ArtifactsDir + "Rendering.SaveAsImage.HighCompression.jpg").Length));

            // Set the "JpegQuality" property to "100" to use weaker compression when rending the document.
            // This will improve the quality of the image, but will also increase the file size.
            imageOptions.JpegQuality = 100;

            doc.Save(ArtifactsDir + "Rendering.SaveAsImage.HighQuality.jpg", imageOptions);

            Assert.That(60000, Is.LessThan(new FileInfo(ArtifactsDir + "Rendering.SaveAsImage.HighQuality.jpg").Length));
            //ExEnd
        }

        [Test, Category("SkipMono")]
        public void SaveToTiffDefault()
        {
            Document doc = new Document(MyDir + "Rendering.docx");
            doc.Save(ArtifactsDir + "Rendering.SaveToTiffDefault.tiff");
        }

        [TestCase(TiffCompression.None), Category("SkipMono")]
        [TestCase(TiffCompression.Rle), Category("SkipMono")]
        [TestCase(TiffCompression.Lzw), Category("SkipMono")]
        [TestCase(TiffCompression.Ccitt3), Category("SkipMono")]
        [TestCase(TiffCompression.Ccitt4), Category("SkipMono")]
        public void SaveToTiffCompression(TiffCompression tiffCompression)
        {
            //ExStart
            //ExFor:TiffCompression
            //ExFor:ImageSaveOptions.TiffCompression
            //ExSummary:Shows how to select the compression scheme to apply to a document that we convert into a TIFF image.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.InsertImage(ImageDir + "Logo.jpg");

            // Create an "ImageSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method renders the document into an image.
            ImageSaveOptions options = new ImageSaveOptions(SaveFormat.Tiff);

            // Set the "TiffCompression" property to "TiffCompression.None" to apply no compression while saving,
            // which may result in a very large output file.
            // Set the "TiffCompression" property to "TiffCompression.Rle" to apply RLE compression
            // Set the "TiffCompression" property to "TiffCompression.Lzw" to apply LZW compression.
            // Set the "TiffCompression" property to "TiffCompression.Ccitt3" to apply CCITT3 compression.
            // Set the "TiffCompression" property to "TiffCompression.Ccitt4" to apply CCITT4 compression.
            options.TiffCompression = tiffCompression;

            doc.Save(ArtifactsDir + "Rendering.SaveToTiffCompression.tiff", options);

            switch (tiffCompression)
            {
                case TiffCompression.None:
                    Assert.That(3000000, Is.LessThan(new FileInfo(ArtifactsDir + "Rendering.SaveToTiffCompression.tiff").Length));
                    break;
                case TiffCompression.Rle:
                    Assert.That(600000, Is.LessThan(new FileInfo(ArtifactsDir + "Rendering.SaveToTiffCompression.tiff").Length));
                    break;
                case TiffCompression.Lzw:
                    Assert.That(200000, Is.LessThan(new FileInfo(ArtifactsDir + "Rendering.SaveToTiffCompression.tiff").Length));
                    break;
                case TiffCompression.Ccitt3:
                    Assert.That(90000, Is.AtLeast(new FileInfo(ArtifactsDir + "Rendering.SaveToTiffCompression.tiff").Length));
                    break;
                case TiffCompression.Ccitt4:
                    Assert.That(20000, Is.AtLeast(new FileInfo(ArtifactsDir + "Rendering.SaveToTiffCompression.tiff").Length));
                    break;
            }
            //ExEnd
        }

        [Test]
        public void SetImageResolution()
        {
            //ExStart
            //ExFor:ImageSaveOptions
            //ExFor:ImageSaveOptions.Resolution
            //ExSummary:Shows how to specify a resolution while rendering a document to PNG.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Name = "Times New Roman";
            builder.Font.Size = 24;
            builder.Writeln("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");

            builder.InsertImage(ImageDir + "Logo.jpg");

            // Create an "ImageSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method renders the document into an image.
            ImageSaveOptions options = new ImageSaveOptions(SaveFormat.Png);

            // Set the "Resolution" property to "72" to render the document in 72dpi.
            options.Resolution = 72;
            
            doc.Save(ArtifactsDir + "Rendering.SetImageResolution.72dpi.png", options);

            Assert.That(120000, Is.AtLeast(new FileInfo(ArtifactsDir + "Rendering.SetImageResolution.72dpi.png").Length));

#if NET462 || JAVA
            Image image = Image.FromFile(ArtifactsDir + "Rendering.SetImageResolution.72dpi.png");

            Assert.AreEqual(612, image.Width);
            Assert.AreEqual(792, image.Height);
#elif NETCOREAPP2_1 || __MOBILE__
            using (SKBitmap image = SKBitmap.Decode(ArtifactsDir + "Rendering.SetImageResolution.72dpi.png")) 
            {
                Assert.AreEqual(612, image.Width);
                Assert.AreEqual(792, image.Height);
            }
#endif
            // Set the "Resolution" property to "300" to render the document in 300dpi.
            options.Resolution = 300;

            doc.Save(ArtifactsDir + "Rendering.SetImageResolution.300dpi.png", options);

            Assert.That(1100000, Is.LessThan(new FileInfo(ArtifactsDir + "Rendering.SetImageResolution.300dpi.png").Length));

#if NET462 || JAVA
            image = Image.FromFile(ArtifactsDir + "Rendering.SetImageResolution.300dpi.png");

            Assert.AreEqual(2550, image.Width);
            Assert.AreEqual(3300, image.Height);
#elif NETCOREAPP2_1 || __MOBILE__
            using (SKBitmap image = SKBitmap.Decode(ArtifactsDir + "Rendering.SetImageResolution.300dpi.png")) 
            {
                Assert.AreEqual(2550, image.Width);
                Assert.AreEqual(3300, image.Height);
            }
#endif
            //ExEnd
        }

        [Test]
        public void SetImagePaperColor()
        {
            //ExStart
            //ExFor:ImageSaveOptions
            //ExFor:ImageSaveOptions.PaperColor
            //ExSummary:Renders a page of a Word document into an image with transparent or colored background.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Name = "Times New Roman";
            builder.Font.Size = 24;
            builder.Writeln("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");

            builder.InsertImage(ImageDir + "Logo.jpg");

            // Create an "ImageSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method renders the document into an image.
            ImageSaveOptions imgOptions = new ImageSaveOptions(SaveFormat.Png);

            // Set the "PaperColor" property to a transparent color to apply a transparent
            // background to the document while rendering it to an image.
            imgOptions.PaperColor = Color.Transparent;

            doc.Save(ArtifactsDir + "Rendering.SetImagePaperColor.Transparent.png", imgOptions);

            // Set the "PaperColor" property to an opaque color to apply that color
            // as the background of the document as we render it to an image.
            imgOptions.PaperColor = Color.LightCoral;

            doc.Save(ArtifactsDir + "Rendering.SetImagePaperColor.LightCoral.png", imgOptions);
            //ExEnd

            TestUtil.ImageContainsTransparency(ArtifactsDir + "Rendering.SetImagePaperColor.Transparent.png");
            Assert.Throws<AssertionException>(() =>
                TestUtil.ImageContainsTransparency(ArtifactsDir + "Rendering.SetImagePaperColor.LightCoral.png"));
        }

        [Test]
        public void SaveToImageStream()
        {
            //ExStart
            //ExFor:Document.Save(Stream, SaveFormat)
            //ExSummary:Shows how to save a document to an image via stream, and then read the image from that stream.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Name = "Times New Roman";
            builder.Font.Size = 24;
            builder.Writeln("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.");

            builder.InsertImage(ImageDir + "Logo.jpg");

            // Save the document to a stream.
            using (MemoryStream stream = new MemoryStream())
            {
                doc.Save(stream, SaveFormat.Bmp);

                stream.Position = 0;
                
                // Read the stream back into an image.
#if NET462 || JAVA
                using (Image image = Image.FromStream(stream))
                {
                    Assert.AreEqual(ImageFormat.Bmp, image.RawFormat);
                    Assert.AreEqual(816, image.Width);
                    Assert.AreEqual(1056, image.Height);
                }
#elif NETCOREAPP2_1 || __MOBILE__
                using (SKBitmap image = SKBitmap.Decode(stream))
                {
                    Assert.AreEqual(816, image.Width);
                    Assert.AreEqual(1056, image.Height);
                }

                stream.Position = 0;

                SKCodec codec = SKCodec.Create(stream);

                Assert.AreEqual(SKEncodedImageFormat.Bmp, codec.EncodedFormat);
#endif
            }
            //ExEnd
        }

#if NET462 || JAVA
        [Test]
        public void RenderToSize()
        {
            //ExStart
            //ExFor:Document.RenderToSize
            //ExSummary:Shows how to render a document to a bitmap at a specified location and size.
            Document doc = new Document(MyDir + "Rendering.docx");
            
            using (Bitmap bmp = new Bitmap(700, 700))
            {
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    // Set the "PageUnit" property to "GraphicsUnit.Inch" to use inches as the
                    // measurement unit for any transformations and dimensions that we will define.
                    gr.PageUnit = GraphicsUnit.Inch;

                    // Offset the output 0.5" from the edge.
                    gr.TranslateTransform(0.5f, 0.5f);

                    // Rotate the output by 10 degrees.
                    gr.RotateTransform(10);

                    // Draw a 3"x3" rectangle.
                    gr.DrawRectangle(new Pen(Color.Black, 3f / 72f), 0f, 0f, 3f, 3f);
                    
                    // Draw the first page of our document with the same dimensions and transformation as the rectangle.
                    // The rectangle will frame the first page.
                    float returnedScale = doc.RenderToSize(0, gr, 0f, 0f, 3f, 3f);

                    // This is the scaling factor that the RenderToSize method applied to the first page to fit the size we specified.
                    Assert.AreEqual(0.2566f, returnedScale, 0.0001f);

                    // Set the "PageUnit" property to "GraphicsUnit.Millimeter" to use millimeters as the
                    // measurement unit for any transformations and dimensions that we will define.
                    gr.PageUnit = GraphicsUnit.Millimeter;

                    // Reset the transformations that we used from the previous rendering.
                    gr.ResetTransform();

                    // Apply another set of transformations. 
                    gr.TranslateTransform(10, 10);
                    gr.ScaleTransform(0.5f, 0.5f);
                    gr.PageScale = 2f;

                    // Create another rectangle, and use it to frame another page from the document.
                    gr.DrawRectangle(new Pen(Color.Black, 1), 90, 10, 50, 100);
                    doc.RenderToSize(1, gr, 90, 10, 50, 100);

                    bmp.Save(ArtifactsDir + "Rendering.RenderToSize.png");
                }
            }
            //ExEnd
        }

        [Test]
        public void Thumbnails()
        {
            //ExStart
            //ExFor:Document.RenderToScale
            //ExSummary:Shows how to the individual pages of a document to graphics to create one image with thumbnails of all pages.
            // The user opens or builds a document
            Document doc = new Document(MyDir + "Rendering.docx");

            // Calculate the number of rows and columns that we will fill with thumbnails.
            const int thumbColumns = 2;
            int thumbRows = Math.DivRem(doc.PageCount, thumbColumns, out int remainder);

            if (remainder > 0)
                thumbRows++;

            // Scale the thumbnails relative to the size of the first page. 
            const float scale = 0.25f;
            Size thumbSize = doc.GetPageInfo(0).GetSizeInPixels(scale, 96);

            // Calculate the size of the image that will contain all the thumbnails.
            int imgWidth = thumbSize.Width * thumbColumns;
            int imgHeight = thumbSize.Height * thumbRows;
            
            using (Bitmap img = new Bitmap(imgWidth, imgHeight))
            {
                using (Graphics gr = Graphics.FromImage(img))
                {
                    gr.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                    // Fill the background, which is transparent by default, in white.
                    gr.FillRectangle(new SolidBrush(Color.White), 0, 0, imgWidth, imgHeight);

                    for (int pageIndex = 0; pageIndex < doc.PageCount; pageIndex++)
                    {
                        int rowIdx = Math.DivRem(pageIndex, thumbColumns, out int columnIdx);

                        // Specify where we want the thumbnail to appear.
                        float thumbLeft = columnIdx * thumbSize.Width;
                        float thumbTop = rowIdx * thumbSize.Height;

                        // Render a page as a thumbnail, and then frame it in a rectangle of the same size.
                        SizeF size = doc.RenderToScale(pageIndex, gr, thumbLeft, thumbTop, scale);
                        gr.DrawRectangle(Pens.Black, thumbLeft, thumbTop, size.Width, size.Height);
                    }

                    img.Save(ArtifactsDir + "Rendering.Thumbnails.png");
                }
            }
            //ExEnd
        }

        [Ignore("Run only when the printer driver is installed")]
        [Test]
        public void CustomPrint()
        {
            //ExStart
            //ExFor:PageInfo.GetDotNetPaperSize
            //ExFor:PageInfo.Landscape
            //ExSummary:Shows how to customize printing of Aspose.Words documents.
            Document doc = new Document(MyDir + "Rendering.docx");

            MyPrintDocument printDoc = new MyPrintDocument(doc);
            printDoc.PrinterSettings.PrintRange = System.Drawing.Printing.PrintRange.SomePages;
            printDoc.PrinterSettings.FromPage = 1;
            printDoc.PrinterSettings.ToPage = 1;

            printDoc.Print();
        }

        /// <summary>
        /// Selects an appropriate paper size, orientation, and paper tray when printing.
        /// </summary>
        public class MyPrintDocument : PrintDocument
        {
            public MyPrintDocument(Document document)
            {
                mDocument = document;
            }

            /// <summary>
            /// Initializes the range of pages to be printed according to the user selection.
            /// </summary>
            protected override void OnBeginPrint(PrintEventArgs e)
            {
                base.OnBeginPrint(e);

                switch (PrinterSettings.PrintRange)
                {
                    case System.Drawing.Printing.PrintRange.AllPages:
                        mCurrentPage = 1;
                        mPageTo = mDocument.PageCount;
                        break;
                    case System.Drawing.Printing.PrintRange.SomePages:
                        mCurrentPage = PrinterSettings.FromPage;
                        mPageTo = PrinterSettings.ToPage;
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported print range.");
                }
            }

            /// <summary>
            /// Called before each page is printed. 
            /// </summary>
            protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
            {
                base.OnQueryPageSettings(e);

                // A single Microsoft Word document can have multiple sections that specify pages with different sizes, 
                // orientations, and paper trays. The .NET printing framework calls this code before 
                // each page is printed, which gives us a chance to specify how to print the current page.
                PageInfo pageInfo = mDocument.GetPageInfo(mCurrentPage - 1);
                e.PageSettings.PaperSize = pageInfo.GetDotNetPaperSize(PrinterSettings.PaperSizes);

                // Microsoft Word stores the paper source (printer tray) for each section as a printer-specific value.
                // To obtain the correct tray value you will need to use the RawKindValue, which your printer should return.
                e.PageSettings.PaperSource.RawKind = pageInfo.PaperTray;
                e.PageSettings.Landscape = pageInfo.Landscape;
            }

            /// <summary>
            /// Called for each page to render it for printing. 
            /// </summary>
            protected override void OnPrintPage(PrintPageEventArgs e)
            {
                base.OnPrintPage(e);

                // Aspose.Words rendering engine creates a page that is drawn from the origin (x = 0, y = 0) of the paper.
                // There will be a hard margin in the printer, which will render each page. We need to offset by that hard margin.
                float hardOffsetX, hardOffsetY;

                // Below are two ways of setting a hard margin.
                if (e.PageSettings != null && e.PageSettings.HardMarginX != 0 && e.PageSettings.HardMarginY != 0)
                {
                    // 1 -  Via the "PageSettings" property.
                    hardOffsetX = e.PageSettings.HardMarginX;
                    hardOffsetY = e.PageSettings.HardMarginY;
                }
                else
                {
                    // 2 -  Using our own values, if the "PageSettings" property is unavailable.
                    hardOffsetX = 20;
                    hardOffsetY = 20;
                }

                mDocument.RenderToScale(mCurrentPage, e.Graphics, -hardOffsetX, -hardOffsetY, 1.0f);

                mCurrentPage++;
                e.HasMorePages = mCurrentPage <= mPageTo;
            }

            private readonly Document mDocument;
            private int mCurrentPage;
            private int mPageTo;
        }
        //ExEnd

        [Test]
        [Ignore("Run only when the printer driver is installed")]
        public void PrintPageInfo()
        {
            //ExStart
            //ExFor:PageInfo
            //ExFor:PageInfo.GetSizeInPixels(Single, Single, Single)
            //ExFor:PageInfo.GetSpecifiedPrinterPaperSource(PaperSourceCollection, PaperSource)
            //ExFor:PageInfo.HeightInPoints
            //ExFor:PageInfo.Landscape
            //ExFor:PageInfo.PaperSize
            //ExFor:PageInfo.PaperTray
            //ExFor:PageInfo.SizeInPoints
            //ExFor:PageInfo.WidthInPoints
            //ExSummary:Shows how to print page size and orientation information for every page in a Word document.
            Document doc = new Document(MyDir + "Rendering.docx");

            // The first section has 2 pages. We will assign a different printer paper tray to each one,
            // whose number will match a kind of paper source. These sources and their Kinds will vary
            // depending on the installed printer driver.
            PrinterSettings.PaperSourceCollection paperSources = new PrinterSettings().PaperSources;

            doc.FirstSection.PageSetup.FirstPageTray = paperSources[0].RawKind;
            doc.FirstSection.PageSetup.OtherPagesTray = paperSources[1].RawKind;

            Console.WriteLine("Document \"{0}\" contains {1} pages.", doc.OriginalFileName, doc.PageCount);

            float scale = 1.0f;
            float dpi = 96;

            for (int i = 0; i < doc.PageCount; i++)
            {
                // Each page has a PageInfo object, whose index is the respective page's number.
                PageInfo pageInfo = doc.GetPageInfo(i);

                // Print the page's orientation and dimensions.
                Console.WriteLine($"Page {i + 1}:");
                Console.WriteLine($"\tOrientation:\t{(pageInfo.Landscape ? "Landscape" : "Portrait")}");
                Console.WriteLine($"\tPaper size:\t\t{pageInfo.PaperSize} ({pageInfo.WidthInPoints:F0}x{pageInfo.HeightInPoints:F0}pt)");
                Console.WriteLine($"\tSize in points:\t{pageInfo.SizeInPoints}");
                Console.WriteLine($"\tSize in pixels:\t{pageInfo.GetSizeInPixels(1.0f, 96)} at {scale * 100}% scale, {dpi} dpi");

                // Print the source tray information.
                Console.WriteLine($"\tTray:\t{pageInfo.PaperTray}");
                PaperSource source = pageInfo.GetSpecifiedPrinterPaperSource(paperSources, paperSources[0]);
                Console.WriteLine($"\tSuitable print source:\t{source.SourceName}, kind: {source.Kind}");
            }
            //ExEnd
        }

        [Test]
        [Ignore("Run only when the printer driver is installed")]
        public void PrinterSettingsContainer()
        {
            //ExStart
            //ExFor:PrinterSettingsContainer
            //ExFor:PrinterSettingsContainer.#ctor(PrinterSettings)
            //ExFor:PrinterSettingsContainer.DefaultPageSettingsPaperSource
            //ExFor:PrinterSettingsContainer.PaperSizes
            //ExFor:PrinterSettingsContainer.PaperSources
            //ExSummary:Shows how to access and list your printer's paper sources and sizes.
            // The "PrinterSettingsContainer" contains a "PrinterSettings" object,
            // which contains unique data for different printer drivers.
            PrinterSettingsContainer container = new PrinterSettingsContainer(new PrinterSettings());

            Console.WriteLine($"This printer contains {container.PaperSources.Count} printer paper sources:");
            foreach (PaperSource paperSource in container.PaperSources)
            {
                bool isDefault = container.DefaultPageSettingsPaperSource.SourceName == paperSource.SourceName;
                Console.WriteLine($"\t{paperSource.SourceName}, " +
                                  $"RawKind: {paperSource.RawKind} {(isDefault ? "(Default)" : "")}");
            }

            // The "PaperSizes" property contains the list of paper sizes that we can instruct the printer to use.
            // Both the PrinterSource and PrinterSize contain a "RawKind" attribute,
            // which equates to a paper type listed on the PaperSourceKind enum.
            // If there is a paper source with the same "RawKind" value as that of the page we are printing,
            // the printer will print the page using the provided paper source and size.
            // Otherwise, the printer will default to the source designated by the "DefaultPageSettingsPaperSource" property.
            Console.WriteLine($"{container.PaperSizes.Count} paper sizes:");
            foreach (System.Drawing.Printing.PaperSize paperSize in container.PaperSizes)
            {
                Console.WriteLine($"\t{paperSize}, RawKind: {paperSize.RawKind}");
            }
            //ExEnd
        }

        [Ignore("Run only when the printer driver is installed")]
        [Test]
        public void Print()
        {
            //ExStart
            //ExFor:Document.Print
            //ExFor:Document.Print(String)
            //ExSummary:Shows how to print a document using the default printer.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Writeln("Hello world!");

            // Below are two ways of printing our document.
            // 1 -  Print using the default printer:
            doc.Print();

            // 2 -  Specify a printer that we wish to print the document with by name:
            string myPrinter = System.Drawing.Printing.PrinterSettings.InstalledPrinters[4];

            Assert.AreEqual("HPDAAB96 (HP ENVY 5000 series)", myPrinter);

            doc.Print(myPrinter);
            //ExEnd
        }
        
        [Ignore("Run only when the printer driver is installed")]
        [Test]
        public void PrintRange()
        {
            //ExStart
            //ExFor:Document.Print(PrinterSettings)
            //ExFor:Document.Print(PrinterSettings, String)
            //ExSummary:Shows how to print a range of pages.
            Document doc = new Document(MyDir + "Rendering.docx");
            
            // Create a "PrinterSettings" object to modify the way in which we print the document.
            PrinterSettings printerSettings = new PrinterSettings();

            // Set the "PrintRange" property to "PrintRange.SomePages" to
            // tell the printer that we intend to print only some pages of the document.
            printerSettings.PrintRange = System.Drawing.Printing.PrintRange.SomePages;

            // Set the "FromPage" property to "1", and the "ToPage" property to "3" to print pages 1 through to 3.
            // Page indexing is 1-based.
            printerSettings.FromPage = 1;
            printerSettings.ToPage = 3;

            // Below are two ways of printing our document.
            // 1 -  Print while applying our printing settings:
            doc.Print(printerSettings);

            // 2 -  Print while applying our printing settings, while also
            // giving the document a custom name that we may recognize in the printer queue:
            doc.Print(printerSettings, "My rendered document");
            //ExEnd
        }

        [Ignore("Run only when the printer driver is installed")]
        [Test]
        public void PreviewAndPrint()
        {
            //ExStart
            //ExFor:AsposeWordsPrintDocument.#ctor(Document)
            //ExFor:AsposeWordsPrintDocument.CachePrinterSettings
            //ExSummary:Shows how to select a page range and a printer to print the document with, and then bring up a print preview.
            Document doc = new Document(MyDir + "Rendering.docx");

            PrintPreviewDialog previewDlg = new PrintPreviewDialog();

            // Call the "Show" method to get the print preview form to show on top.
            previewDlg.Show();

            // Initialize the Print Dialog with the number of pages in the document.
            PrintDialog printDlg = new PrintDialog();
            printDlg.AllowSomePages = true;
            printDlg.PrinterSettings.MinimumPage = 1;
            printDlg.PrinterSettings.MaximumPage = doc.PageCount;
            printDlg.PrinterSettings.FromPage = 1;
            printDlg.PrinterSettings.ToPage = doc.PageCount;

            if (!printDlg.ShowDialog().Equals(DialogResult.OK))
                return;

            // Create the "Aspose.Words" implementation of the .NET print document,
            // and then pass the printer settings from the dialog to it.
            AsposeWordsPrintDocument awPrintDoc = new AsposeWordsPrintDocument(doc);
            awPrintDoc.PrinterSettings = printDlg.PrinterSettings;

            // Use the "CachePrinterSettings" method to reduce time of first call of Print() method.
            awPrintDoc.CachePrinterSettings();

            // Call the "Hide", and then the "InvalidatePreview" methods to get the print preview to show on top.
            previewDlg.Hide();
            previewDlg.PrintPreviewControl.InvalidatePreview();

            // Pass the "Aspose.Words" print document to the .NET Print Preview dialog.
            previewDlg.Document = awPrintDoc;

            previewDlg.ShowDialog();
            //ExEnd
        }

#elif NETCOREAPP2_1 || __MOBILE__
        [Test]
        public void RenderToSizeNetStandard2()
        {
            //ExStart
            //ExFor:Document.RenderToSize
            //ExSummary:Shows how to render the document as a bitmap at a specified location and size (.NetStandard 2.0).
            Document doc = new Document(MyDir + "Rendering.docx");
            
            using (SKBitmap bitmap = new SKBitmap(700, 700))
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    // Apply a scaling factor of 70% to the page that we will render using this canvas.
                    canvas.Scale(70);

                    // Offset the page 0.5" from the top and left edges of the page.
                    canvas.Translate(0.5f, 0.5f);

                    // Rotate the rendered page by 10 degrees.
                    canvas.RotateDegrees(10);

                    // Create and draw a rectangle.
                    SKRect rect = new SKRect(0f, 0f, 3f, 3f);
                    canvas.DrawRect(rect, new SKPaint
                    {
                        Color = SKColors.Black,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = 3f / 72f
                    });

                    // Render the first page of the document to the same size as the above rectangle. 
                    // The rectangle will frame this page.
                    float returnedScale = doc.RenderToSize(0, canvas, 0f, 0f, 3f, 3f);

                    Console.WriteLine("The image was rendered at {0:P0} zoom.", returnedScale);

                    // Reset the matrix, and then apply a new set of scaling and translations.
                    canvas.ResetMatrix();
                    canvas.Scale(5);
                    canvas.Translate(10, 10);

                    // Create another rectangle.
                    rect = new SKRect(0, 0, 50, 100);
                    rect.Offset(90, 10);
                    canvas.DrawRect(rect, new SKPaint
                    {
                        Color = SKColors.Black,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = 1
                    });

                    // Render the first page within the newly created rectangle once again.
                    doc.RenderToSize(0, canvas, 90, 10, 50, 100);

                    using (SKFileWStream fs = new SKFileWStream(ArtifactsDir + "Rendering.RenderToSizeNetStandard2.png"))
                    {
                        bitmap.PeekPixels().Encode(fs, SKEncodedImageFormat.Png, 100);
                    }
                }
            }            
            //ExEnd
        }

        [Test]
        public void CreateThumbnailsNetStandard2()
        {
            //ExStart
            //ExFor:Document.RenderToScale
            //ExSummary:Renders individual pages to graphics to create one image with thumbnails of all pages (.NetStandard 2.0).
            Document doc = new Document(MyDir + "Rendering.docx");

            // Calculate the number of rows and columns that we will fill with thumbnails.
            const int thumbnailColumnsNum = 2;
            int thumbRows = Math.DivRem(doc.PageCount, thumbnailColumnsNum, out int remainder);

            if (remainder > 0)
                thumbRows++;

            // Scale the thumbnails relative to the size of the first page. 
            const float scale = 0.25f;
            Size thumbSize = doc.GetPageInfo(0).GetSizeInPixels(scale, 96);

            // Calculate the size of the image that will contain all the thumbnails.
            int imgWidth = thumbSize.Width * thumbnailColumnsNum;
            int imgHeight = thumbSize.Height * thumbRows;

            using (SKBitmap bitmap = new SKBitmap(imgWidth, imgHeight))
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    // Fill the background, which is transparent by default, in white.
                    canvas.Clear(SKColors.White);

                    for (int pageIndex = 0; pageIndex < doc.PageCount; pageIndex++)
                    {
                        int rowIdx = Math.DivRem(pageIndex, thumbnailColumnsNum, out int columnIdx);

                        // Specify where we want the thumbnail to appear.
                        float thumbLeft = columnIdx * thumbSize.Width;
                        float thumbTop = rowIdx * thumbSize.Height;

                        SizeF size = doc.RenderToScale(pageIndex, canvas, thumbLeft, thumbTop, scale);

                        // Render a page as a thumbnail, and then frame it in a rectangle of the same size.
                        SKRect rect = new SKRect(0, 0, size.Width, size.Height);
                        rect.Offset(thumbLeft, thumbTop);
                        canvas.DrawRect(rect, new SKPaint
                        {
                            Color = SKColors.Black,
                            Style = SKPaintStyle.Stroke
                        });
                    }

                    using (SKFileWStream fs = new SKFileWStream(ArtifactsDir + "Rendering.CreateThumbnailsNetStandard2.png"))
                    {
                        bitmap.PeekPixels().Encode(fs, SKEncodedImageFormat.Png, 100);
                    }
                }
            }            
            //ExEnd
        }
#endif

        [Test]
        public void UpdatePageLayout()
        {
            //ExStart
            //ExFor:StyleCollection.Item(String)
            //ExFor:SectionCollection.Item(Int32)
            //ExFor:Document.UpdatePageLayout
            //ExSummary:Shows when to recalculate the page layout of the document.
            Document doc = new Document(MyDir + "Rendering.docx");

            // Saving a document to PDF, to an image, or printing for the first time will automatically
            // cache the layout of the document within its pages.
            doc.Save(ArtifactsDir + "Rendering.UpdatePageLayout.1.pdf");

            // Modify the document in some way.
            doc.Styles["Normal"].Font.Size = 6;
            doc.Sections[0].PageSetup.Orientation = Aspose.Words.Orientation.Landscape;

            // In the current version of Aspose.Words, modifying the document does not automatically rebuild 
            // the cached page layout. If we wish for the cached layout
            // to stay up-to-date, we will need to update it manually.
            doc.UpdatePageLayout();

            doc.Save(ArtifactsDir + "Rendering.UpdatePageLayout.2.pdf");
            //ExEnd
        }

        [TestCase(false)]
        [TestCase(true)]
        public void SetFontsFolder(bool recursive)
        {
            //ExStart
            //ExFor:FontSettings
            //ExFor:FontSettings.SetFontsFolder(String, Boolean)
            //ExSummary:Shows how to set a font source directory.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Name = "Arvo";
            builder.Writeln("Hello world!");
            builder.Font.Name = "Amethysta";
            builder.Writeln("The quick brown fox jumps over the lazy dog.");

            // Our font sources do not contain the font that we have used for text in this document.
            // If we use these font settings while rendering this document,
            // Aspose.Words will apply a fallback font to text which has a font that Aspose.Words cannot locate.
            FontSourceBase[] originalFontSources = FontSettings.DefaultInstance.GetFontsSources();

            Assert.AreEqual(1, originalFontSources.Length);
            Assert.AreEqual(480, originalFontSources[0].GetAvailableFonts().Count);
            Assert.True(originalFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Arial"));

            // The default font sources are missing the two fonts that we are using in this document.
            Assert.False(originalFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Arvo"));
            Assert.False(originalFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Amethysta"));

            // Use the "SetFontsFolder" method to set a directory which will act as a new font source.
            // Pass "false" as the "recursive" argument to include fonts from all the font files that are in the directory
            // that we are passing in the first argument, but not include any fonts in any of that directory's subfolders.
            // Pass "true" as the "recursive" argument to include all font files in the directory that we are passing
            // in the first argument, as well as all the fonts in its subdirectories.
            FontSettings.DefaultInstance.SetFontsFolder(FontsDir, recursive);

            FontSourceBase[] newFontSources = FontSettings.DefaultInstance.GetFontsSources();

            Assert.AreEqual(1, newFontSources.Length);
            Assert.False(newFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Arial"));
            Assert.True(newFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Arvo"));

            // The "Amethysta" font is in a subfolder of the font directory.
            if (recursive)
            {
                Assert.AreEqual(22, newFontSources[0].GetAvailableFonts().Count);
                Assert.True(newFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Amethysta"));
            }
            else
            {
                Assert.AreEqual(15, newFontSources[0].GetAvailableFonts().Count);
                Assert.False(newFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Amethysta"));
            }

            doc.Save(ArtifactsDir + "Rendering.SetFontsFolder.pdf");

            // Restore the original font sources.
            FontSettings.DefaultInstance.SetFontsSources(originalFontSources);
            //ExEnd
        }

        [TestCase(false)]
        [TestCase(true)]
        public void SetFontsFolders(bool recursive)
        {
            //ExStart
            //ExFor:FontSettings
            //ExFor:FontSettings.SetFontsFolders(String[], Boolean)
            //ExSummary:Shows how to set multiple font source directories.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Name = "Amethysta";
            builder.Writeln("The quick brown fox jumps over the lazy dog.");
            builder.Font.Name = "Junction Light";
            builder.Writeln("The quick brown fox jumps over the lazy dog.");

            // Our font sources do not contain the font that we have used for text in this document.
            // If we use these font settings while rendering this document,
            // Aspose.Words will apply a fallback font to text which has a font that Aspose.Words cannot locate.
            FontSourceBase[] originalFontSources = FontSettings.DefaultInstance.GetFontsSources();

            Assert.AreEqual(1, originalFontSources.Length);
            Assert.AreEqual(480, originalFontSources[0].GetAvailableFonts().Count);
            Assert.True(originalFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Arial"));

            // The default font sources are missing the two fonts that we are using in this document.
            Assert.False(originalFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Amethysta"));
            Assert.False(originalFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Junction Light"));

            // Use the "SetFontsFolders" method to create a font source from each font directory that we pass as the first argument.
            // Pass "false" as the "recursive" argument to include fonts from all the font files that are in the directories
            // that we are passing in the first argument, but not include any fonts from any of the directories' subfolders.
            // Pass "true" as the "recursive" argument to include all font files in the directories that we are passing
            // in the first argument, as well as all the fonts in their subdirectories.
            FontSettings.DefaultInstance.SetFontsFolders(new [] {FontsDir + "/Amethysta", FontsDir + "/Junction"}, recursive);

            FontSourceBase[] newFontSources = FontSettings.DefaultInstance.GetFontsSources();

            Assert.AreEqual(2, newFontSources.Length);
            Assert.False(newFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Arial"));
            Assert.AreEqual(1, newFontSources[0].GetAvailableFonts().Count);
            Assert.True(newFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Amethysta"));

            // The "Junction" folder itself contains no font files, but has subfolders that do.
            if (recursive)
            {
                Assert.AreEqual(6, newFontSources[1].GetAvailableFonts().Count);
                Assert.True(newFontSources[1].GetAvailableFonts().Any(f => f.FullFontName == "Junction Light"));
            }
            else
            {
                Assert.AreEqual(0, newFontSources[1].GetAvailableFonts().Count);
            }

            doc.Save(ArtifactsDir + "Rendering.SetFontsFolders.pdf");

            // Restore the original font sources.
            FontSettings.DefaultInstance.SetFontsSources(originalFontSources);
            //ExEnd
        }

        [Test]
        public void AddFontSource()
        {
            //ExStart
            //ExFor:FontSettings            
            //ExFor:FontSettings.GetFontsSources()
            //ExFor:FontSettings.SetFontsSources()
            //ExSummary:Shows how to add a font source to our existing font sources.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Name = "Arial";
            builder.Writeln("Hello world!");
            builder.Font.Name = "Amethysta";
            builder.Writeln("The quick brown fox jumps over the lazy dog.");
            builder.Font.Name = "Junction Light";
            builder.Writeln("The quick brown fox jumps over the lazy dog.");

            FontSourceBase[] originalFontSources = FontSettings.DefaultInstance.GetFontsSources();

            Assert.AreEqual(1, originalFontSources.Length);

            Assert.True(originalFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Arial"));

            // The default font source is missing two of the fonts that we are using in our document.
            // When we save this document, Aspose.Words will apply fallback fonts to all text formatted with inaccessible fonts.
            Assert.False(originalFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Amethysta"));
            Assert.False(originalFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Junction Light"));

            // Create a font source from a folder that contains fonts.
            FolderFontSource folderFontSource = new FolderFontSource(FontsDir, true);

            // Apply a new array of font sources that contains the original font sources, as well as our custom fonts.
            FontSourceBase[] updatedFontSources = { originalFontSources[0], folderFontSource };
            FontSettings.DefaultInstance.SetFontsSources(updatedFontSources);

            // Verify that Aspose.Words has access to all required fonts before we render the document to PDF.
            updatedFontSources = FontSettings.DefaultInstance.GetFontsSources();

            Assert.True(updatedFontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Arial"));
            Assert.True(updatedFontSources[1].GetAvailableFonts().Any(f => f.FullFontName == "Amethysta"));
            Assert.True(updatedFontSources[1].GetAvailableFonts().Any(f => f.FullFontName == "Junction Light"));

            doc.Save(ArtifactsDir + "Rendering.AddFontSource.pdf");

            // Restore the original font sources.
            FontSettings.DefaultInstance.SetFontsSources(originalFontSources);
            //ExEnd
        }

        [Test]
        public void SetSpecifyFontFolder()
        {
            FontSettings fontSettings = new FontSettings();
            fontSettings.SetFontsFolder(FontsDir, false);

            // Using load options
            LoadOptions loadOptions = new LoadOptions();
            loadOptions.FontSettings = fontSettings;

            Document doc = new Document(MyDir + "Rendering.docx", loadOptions);

            FolderFontSource folderSource = ((FolderFontSource) doc.FontSettings.GetFontsSources()[0]);

            Assert.AreEqual(FontsDir, folderSource.FolderPath);
            Assert.False(folderSource.ScanSubfolders);
        }

        [Test]
        public void TableSubstitution()
        {
            //ExStart
            //ExFor:Document.FontSettings
            //ExFor:TableSubstitutionRule.SetSubstitutes(String, String[])
            //ExSummary:Shows how set font substitution rules.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Name = "Arial";
            builder.Writeln("Hello world!");
            builder.Font.Name = "Amethysta";
            builder.Writeln("The quick brown fox jumps over the lazy dog.");

            FontSourceBase[] fontSources = FontSettings.DefaultInstance.GetFontsSources();

            // The default font sources contain the first font that the document uses.
            Assert.AreEqual(1, fontSources.Length);
            Assert.True(fontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Arial"));

            // The second font, "Amethysta", is unavailable.
            Assert.False(fontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Amethysta"));

            // We can configure a font substitution table which determines
            // which fonts Aspose.Words will use as substitutes for unavailable fonts.
            // Set two substitution fonts for "Amethysta": "Arvo", and "Courier New".
            // If the first substitute is unavailable, Aspose.Words attempts to use the second substitute, and so on.
            doc.FontSettings = new FontSettings();
            doc.FontSettings.SubstitutionSettings.TableSubstitution.SetSubstitutes(
                "Amethysta", new[] { "Arvo", "Courier New" });

            // "Amethysta" is unavailable, and the substitution rule states that the first font to use as a substitute is "Arvo". 
            Assert.False(fontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Arvo"));

            // "Arvo" is also unavailable, but "Courier New" is. 
            Assert.True(fontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Courier New"));

            // The output document will display the text that uses the "Amethysta" font formatted with "Courier New".
            doc.Save(ArtifactsDir + "Rendering.TableSubstitution.pdf");
            //ExEnd
        }

        [Test]
        public void SetSpecifyFontFolders()
        {
            FontSettings fontSettings = new FontSettings();
            fontSettings.SetFontsFolders(new string[] { FontsDir, @"C:\Windows\Fonts\" }, true);

            // Using load options
            LoadOptions loadOptions = new LoadOptions();
            loadOptions.FontSettings = fontSettings;
            Document doc = new Document(MyDir + "Rendering.docx", loadOptions);

            FolderFontSource folderSource = ((FolderFontSource) doc.FontSettings.GetFontsSources()[0]);
            Assert.AreEqual(FontsDir, folderSource.FolderPath);
            Assert.True(folderSource.ScanSubfolders);

            folderSource = ((FolderFontSource) doc.FontSettings.GetFontsSources()[1]);
            Assert.AreEqual(@"C:\Windows\Fonts\", folderSource.FolderPath);
            Assert.True(folderSource.ScanSubfolders);
        }

        [Test]
        public void AddFontSubstitutes()
        {
            FontSettings fontSettings = new FontSettings();
            fontSettings.SubstitutionSettings.TableSubstitution.SetSubstitutes("Slab", new string[] { "Times New Roman", "Arial" });
            fontSettings.SubstitutionSettings.TableSubstitution.AddSubstitutes("Arvo", new string[] { "Open Sans", "Arial" });

            Document doc = new Document(MyDir + "Rendering.docx");
            doc.FontSettings = fontSettings;

            string[] alternativeFonts = doc.FontSettings.SubstitutionSettings.TableSubstitution.GetSubstitutes("Slab").ToArray();
            Assert.AreEqual(new string[] { "Times New Roman", "Arial" }, alternativeFonts);

            alternativeFonts = doc.FontSettings.SubstitutionSettings.TableSubstitution.GetSubstitutes("Arvo").ToArray();
            Assert.AreEqual(new string[] { "Open Sans", "Arial" }, alternativeFonts);
        }

        [Test]
        public void DefaultFontName()
        {
            //ExStart
            //ExFor:DefaultFontSubstitutionRule.DefaultFontName
            //ExSummary:Shows how to specify a default font.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Name = "Arial";
            builder.Writeln("Hello world!");
            builder.Font.Name = "Arvo";
            builder.Writeln("The quick brown fox jumps over the lazy dog.");

            FontSourceBase[] fontSources = FontSettings.DefaultInstance.GetFontsSources();

            // The font sources that the document uses contain the font "Arial", but not "Arvo".
            Assert.AreEqual(1, fontSources.Length);
            Assert.True(fontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Arial"));
            Assert.False(fontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Arvo"));

            // Set the "DefaultFontName" property to "Courier New" to,
            // while rendering the document, apply that font in all cases when another font is not available. 
            FontSettings.DefaultInstance.SubstitutionSettings.DefaultFontSubstitution.DefaultFontName = "Courier New";

            Assert.True(fontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Courier New"));

            // Now the set default font is used in place of any missing fonts during any rendering calls.
            doc.Save(ArtifactsDir + "Rendering.DefaultFontName.pdf");
            doc.Save(ArtifactsDir + "Rendering.DefaultFontName.docx");
            //ExEnd
        }

        [Test]
        public void UpdatePageLayoutWarnings()
        {
            // Store the font sources currently used so we can restore them later
            FontSourceBase[] originalFontSources = FontSettings.DefaultInstance.GetFontsSources();

            // Load the document to render
            Document doc = new Document(MyDir + "Document.docx");

            // Create a new class implementing IWarningCallback and assign it to the PdfSaveOptions class
            HandleDocumentWarnings callback = new HandleDocumentWarnings();
            doc.WarningCallback = callback;

            // We can choose the default font to use in the case of any missing fonts
            FontSettings.DefaultInstance.SubstitutionSettings.DefaultFontSubstitution.DefaultFontName = "Arial";

            // For testing we will set Aspose.Words to look for fonts only in a folder which does not exist. Since Aspose.Words won't
            // find any fonts in the specified directory, then during rendering the fonts in the document will be substituted with the default 
            // font specified under FontSettings.DefaultFontName. We can pick up on this substitution using our callback
            FontSettings.DefaultInstance.SetFontsFolder(string.Empty, false);

            // When you call UpdatePageLayout the document is rendered in memory. Any warnings that occurred during rendering
            // are stored until the document save and then sent to the appropriate WarningCallback
            doc.UpdatePageLayout();

            // Even though the document was rendered previously, any save warnings are notified to the user during document save
            doc.Save(ArtifactsDir + "Rendering.UpdatePageLayoutWarnings.pdf");
            
            Assert.That(callback.FontWarnings.Count, Is.GreaterThan(0));
            Assert.True(callback.FontWarnings[0].WarningType == WarningType.FontSubstitution);
            Assert.True(callback.FontWarnings[0].Description.Contains("has not been found"));

            // Restore default fonts
            FontSettings.DefaultInstance.SetFontsSources(originalFontSources);
        }

        public class HandleDocumentWarnings : IWarningCallback
        {
            /// <summary>
            /// Our callback only needs to implement the "Warning" method. This method is called whenever there is a
            /// potential issue during document processing. The callback can be set to listen for warnings generated during document
            /// load and/or document save.
            /// </summary>
            public void Warning(WarningInfo info)
            {
                // We are only interested in fonts being substituted
                if (info.WarningType == WarningType.FontSubstitution)
                {
                    Console.WriteLine("Font substitution: " + info.Description);
                    FontWarnings.Warning(info); //ExSkip
                }
            }

            public WarningInfoCollection FontWarnings = new WarningInfoCollection(); //ExSkip
        }

        [TestCase(false)]
        [TestCase(true)]
        public void EmbedFullFonts(bool embedFullFonts)
        {
            //ExStart
            //ExFor:PdfSaveOptions.#ctor
            //ExFor:PdfSaveOptions.EmbedFullFonts
            //ExSummary:Shows how to enable or disable subsetting when embedding fonts while rendering a document to PDF.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Name = "Arial";
            builder.Writeln("Hello world!");
            builder.Font.Name = "Arvo";
            builder.Writeln("The quick brown fox jumps over the lazy dog.");

            // Configure our font sources to ensure that we have access to both the fonts in this document.
            FontSourceBase[] originalFontsSources = FontSettings.DefaultInstance.GetFontsSources();
            FolderFontSource folderFontSource = new FolderFontSource(FontsDir, true);
            FontSettings.DefaultInstance.SetFontsSources(new [] { originalFontsSources[0], folderFontSource });

            FontSourceBase[] fontSources = FontSettings.DefaultInstance.GetFontsSources();
            Assert.True(fontSources[0].GetAvailableFonts().Any(f => f.FullFontName == "Arial"));
            Assert.True(fontSources[1].GetAvailableFonts().Any(f => f.FullFontName == "Arvo"));

            // Create a "PdfSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method converts the document to .PDF.
            PdfSaveOptions options = new PdfSaveOptions();

            // Since our document contains a custom font, embedding in the output document may be desirable.
            // Set the "EmbedFullFonts" property to "true" to embed every glyph of every embedded font in the output PDF.
            // The size of the document may become very large, but we will have full use of all fonts if we edit the PDF.
            // Set the "EmbedFullFonts" property to "false" to apply subsetting to fonts, saving only the glyphs
            // that the document is using. The file will be considerably smaller,
            // but we may need access to any custom fonts if we edit the document.
            options.EmbedFullFonts = embedFullFonts;

            doc.Save(ArtifactsDir + "Rendering.EmbedFullFonts.pdf", options);

            if (embedFullFonts) 
                Assert.That(500000, Is.LessThan(new FileInfo(ArtifactsDir + "Rendering.EmbedFullFonts.pdf").Length));
            else
                Assert.That(25000, Is.AtLeast(new FileInfo(ArtifactsDir + "Rendering.EmbedFullFonts.pdf").Length));

            // Restore the original font sources.
            FontSettings.DefaultInstance.SetFontsSources(originalFontsSources);
            //ExEnd
        }

        [TestCase(PdfFontEmbeddingMode.EmbedAll)]
        [TestCase(PdfFontEmbeddingMode.EmbedNone)]
        [TestCase(PdfFontEmbeddingMode.EmbedNonstandard)]
        public void EmbedWindowsFonts(PdfFontEmbeddingMode pdfFontEmbeddingMode)
        {
            //ExStart
            //ExFor:PdfSaveOptions.FontEmbeddingMode
            //ExFor:PdfFontEmbeddingMode
            //ExSummary:Shows how to set Aspose.Words to skip embedding Arial and Times New Roman fonts into a PDF document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // "Arial" is a standard font, and "Courier New" is a nonstandard font.
            builder.Font.Name = "Arial";
            builder.Writeln("Hello world!");
            builder.Font.Name = "Courier New";
            builder.Writeln("The quick brown fox jumps over the lazy dog.");

            // Create a "PdfSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method converts the document to .PDF.
            PdfSaveOptions options = new PdfSaveOptions();

            // Set the "EmbedFullFonts" property to "true" to embed every glyph of every embedded font in the output PDF.
            options.EmbedFullFonts = true;

            // Set the "FontEmbeddingMode" property to "EmbedAll" to embed all fonts in the output PDF.
            // Set the "FontEmbeddingMode" property to "EmbedNonstandard" to only allow the embedding of
            // nonstandard fonts in the output PDF.
            // Set the "FontEmbeddingMode" property to "EmbedNone" to not embed any fonts in the output PDF.
            options.FontEmbeddingMode = pdfFontEmbeddingMode;

            // The output PDF will be saved without embedding standard windows fonts
            doc.Save(ArtifactsDir + "Rendering.EmbedWindowsFonts.pdf", options);

            switch (pdfFontEmbeddingMode)
            {
                case PdfFontEmbeddingMode.EmbedAll:
                    Assert.That(1000000, Is.LessThan(new FileInfo(ArtifactsDir + "Rendering.EmbedWindowsFonts.pdf").Length));
                    break;
                case PdfFontEmbeddingMode.EmbedNonstandard:
                    Assert.That(480000, Is.LessThan(new FileInfo(ArtifactsDir + "Rendering.EmbedWindowsFonts.pdf").Length));
                    break;
                case PdfFontEmbeddingMode.EmbedNone:
                    Assert.That(4000, Is.AtLeast(new FileInfo(ArtifactsDir + "Rendering.EmbedWindowsFonts.pdf").Length));
                    break;
            }
            //ExEnd
        }

        [TestCase(false)]
        [TestCase(true)]
        public void EmbedCoreFonts(bool useCoreFonts)
        {
            //ExStart
            //ExFor:PdfSaveOptions.UseCoreFonts
            //ExSummary:Shows how enable/disable PDF Type 1 font substitution.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Name = "Arial";
            builder.Writeln("Hello world!");
            builder.Font.Name = "Courier New";
            builder.Writeln("The quick brown fox jumps over the lazy dog.");

            // Create a "PdfSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method converts the document to .PDF.
            PdfSaveOptions options = new PdfSaveOptions();

            // Set the "UseCoreFonts" property to "true" to replace some fonts,
            // which include the two fonts in our document, with their PDF Type 1 equivalents.
            // Set the "UseCoreFonts" property to "false" to not apply PDF Type 1 fonts.
            options.UseCoreFonts = useCoreFonts;

            doc.Save(ArtifactsDir + "Rendering.EmbedCoreFonts.pdf", options);

            if (useCoreFonts)
                Assert.That(3000, Is.AtLeast(new FileInfo(ArtifactsDir + "Rendering.EmbedCoreFonts.pdf").Length));
            else
                Assert.That(30000, Is.LessThan(new FileInfo(ArtifactsDir + "Rendering.EmbedCoreFonts.pdf").Length));
            //ExEnd
        }

        [Test]
        public void EncryptionPermissions()
        {
            //ExStart
            //ExFor:PdfEncryptionDetails.#ctor
            //ExFor:PdfSaveOptions.EncryptionDetails
            //ExFor:PdfEncryptionDetails.Permissions
            //ExFor:PdfEncryptionDetails.EncryptionAlgorithm
            //ExFor:PdfEncryptionDetails.OwnerPassword
            //ExFor:PdfEncryptionDetails.UserPassword
            //ExFor:PdfEncryptionAlgorithm
            //ExFor:PdfPermissions
            //ExFor:PdfEncryptionDetails
            //ExSummary:Shows how to set permissions on a saved PDF document.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Hello world!");

            PdfEncryptionDetails encryptionDetails =
                new PdfEncryptionDetails("password", string.Empty, PdfEncryptionAlgorithm.RC4_128);

            // Start by disallowing all permissions.
            encryptionDetails.Permissions = PdfPermissions.DisallowAll;

            // Extend permissions to allow the editing of annotations.
            encryptionDetails.Permissions = PdfPermissions.ModifyAnnotations | PdfPermissions.DocumentAssembly;

            // Create a "PdfSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method converts the document to .PDF.
            PdfSaveOptions saveOptions = new PdfSaveOptions();

            // Enable encryption via the "EncryptionDetails" property.
            saveOptions.EncryptionDetails = encryptionDetails;

            // When we open this document, we will need to provide the password before we can access its contents.
            doc.Save(ArtifactsDir + "Rendering.EncryptionPermissions.pdf", saveOptions);
            //ExEnd
        }

        [TestCase(NumeralFormat.ArabicIndic)]
        [TestCase(NumeralFormat.Context)]
        [TestCase(NumeralFormat.EasternArabicIndic)]
        [TestCase(NumeralFormat.European)]
        [TestCase(NumeralFormat.System)]
        public void SetNumeralFormat(NumeralFormat numeralFormat)
        {
            //ExStart
            //ExFor:FixedPageSaveOptions.NumeralFormat
            //ExFor:NumeralFormat
            //ExSummary:Demonstrates how to set the numeral format used when saving to PDF.
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.LocaleId = new CultureInfo("ar-AR").LCID;
            builder.Writeln("1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 50, 100");

            // Create a "PdfSaveOptions" object which we can pass to the document's "Save" method
            // to modify the way in which that method converts the document to .PDF.
            PdfSaveOptions options = new PdfSaveOptions();

            // Set the "NumeralFormat" property to "NumeralFormat.ArabicIndic" to
            // use glyphs from the U+0660 to U+0669 range as numbers.
            // Set the "NumeralFormat" property to "NumeralFormat.Context" to
            // look up the locale to determine what number glyphs to use.
            // Set the "NumeralFormat" property to "NumeralFormat.EasternArabicIndic" to
            // use glyphs from the U+06F0 to U+06F9 range as numbers.
            // Set the "NumeralFormat" property to "NumeralFormat.European" to use european numerals.
            // Set the "NumeralFormat" property to "NumeralFormat.System" to determine the symbol set from regional settings.
            options.NumeralFormat = numeralFormat;

            doc.Save(ArtifactsDir + "Rendering.SetNumeralFormat.pdf", options);
            //ExEnd
        }
    }
}
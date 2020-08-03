using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using ProtokolyPomiarow.Data;
using ProtokolyPomiarow.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HorizontalAlignment = iText.Layout.Properties.HorizontalAlignment;
using TextAlignment = iText.Layout.Properties.TextAlignment;
using VerticalAlignment = iText.Layout.Properties.VerticalAlignment;

namespace ProtokolyPomiarow.PDF
{
    public static class PdfDesigner
    {
        public static void MakePDF(Project project, string pdfLocation)
        {
            if(project == null)
            {
                MessageBox.Show("Obiekt projektu jest wartością NULL.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(string.IsNullOrEmpty(pdfLocation))
            {
                MessageBox.Show("Ciąg pliku wyjściowego jest błędny", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
           
            PdfProgress progressWindow = new PdfProgress();
            progressWindow.Show();

            using (var writer = new PdfWriter(File.Open(pdfLocation, FileMode.OpenOrCreate)))
            {
                using (var pdf = new PdfDocument(writer))
                {
                    pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new HeaderHendler());
                    pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterHendler());
                    using (var document = new Document(pdf))
                    {
                        document.SetTopMargin(65);
                        document.SetBottomMargin(30);
                        var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA, PdfEncodings.CP1250, true);
                        var h1Font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD, PdfEncodings.CP1250, true);
                        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD, PdfEncodings.CP1250, true);

                        document.SetFontSize(8);

                        document.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                        document.Add(new Paragraph($"Data dokumentu {project.DocumentDate.ToShortDateString()}")
                            .SetFont(font));

                        document.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        document.Add(new Paragraph($"Protokół nr {project.ProtocolNumber}")
                            .SetFont(h1Font)
                            .SetFontSize(12));

                        document.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        document.Add(new Paragraph("z pomiaru bilansu mocy lini światłowodowej")
                            .SetFont(boldFont));

                        document.SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT);
                        document.Add(new Paragraph("1. Zleceniodawca")
                            .SetFont(boldFont)
                            .Add(new Text('\n' + project.CustomerInfo ?? " ")
                                .SetFont(font)));

                        document.Add(new Paragraph("2. Obiekt")
                            .SetFont(boldFont)
                            .Add(new Text('\n' + project.ObjectInfo ?? " ")
                                .SetFont(font)));

                        document.Add(new Paragraph($"3. Data badania: {project.MesurementDate?.ToShortDateString() ?? " "}")
                            .SetFont(boldFont));

                        document.Add(new Paragraph("4. Przyrządy pomiarowe")
                            .SetFont(boldFont)
                            .Add(new Text($"\n1. {project.GaugeInfo ?? " "}\n2. {project.LightSourceInfo ?? " "}")
                                .SetFont(font)));

                        document.Add(new Paragraph("5. Wyniki pomiarów tłumienności")
                            .SetFont(boldFont));

                        progressWindow.SetProgress(30);

                        Table mesurements = new Table(new float[] { 1, 4, 4, 2, 4, 3, 3, 3, 3, 4 });
                        mesurements.SetFontSize(7);
                        mesurements.SetWidth(UnitValue.CreatePercentValue(100));
                        mesurements.AddHeaderCell(new Cell().Add(new Paragraph("Lp.").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)).SetVerticalAlignment(VerticalAlignment.MIDDLE));
                        mesurements.AddHeaderCell(new Cell().Add(new Paragraph("Źródło").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)).SetVerticalAlignment(VerticalAlignment.MIDDLE));
                        mesurements.AddHeaderCell(new Cell().Add(new Paragraph("Koniec").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)).SetVerticalAlignment(VerticalAlignment.MIDDLE));
                        mesurements.AddHeaderCell(new Cell().Add(new Paragraph("Numer włókna").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)).SetVerticalAlignment(VerticalAlignment.MIDDLE));
                        mesurements.AddHeaderCell(new Cell().Add(new Paragraph("Typ włókna").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)).SetVerticalAlignment(VerticalAlignment.MIDDLE));
                        mesurements.AddHeaderCell(new Cell().Add(new Paragraph("Długość [km]").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)).SetVerticalAlignment(VerticalAlignment.MIDDLE));
                        mesurements.AddHeaderCell(new Cell().Add(new Paragraph("Ilość spawów [szt]").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)).SetVerticalAlignment(VerticalAlignment.MIDDLE));
                        mesurements.AddHeaderCell(new Cell().Add(new Paragraph("Rmax [dB]").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)).SetVerticalAlignment(VerticalAlignment.MIDDLE));
                        mesurements.AddHeaderCell(new Cell().Add(new Paragraph("R [dB]").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)).SetVerticalAlignment(VerticalAlignment.MIDDLE));
                        mesurements.AddHeaderCell(new Cell().Add(new Paragraph("Ocena pomiaru").SetFont(boldFont).SetTextAlignment(TextAlignment.CENTER)).SetVerticalAlignment(VerticalAlignment.MIDDLE));

                        progressWindow.SetProgress(40);

                        foreach (var mesure in project.Mesurements)
                        {
                            mesurements.AddCell(new Cell().Add(new Paragraph(mesure.Number?.ToString() ?? " ").SetFont(font).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE)));
                            if (mesure.Number == null)
                            {
                                mesurements.AddCell(new Cell(1, 9).Add(new Paragraph(mesure.Source ?? " ").SetFont(boldFont).SetTextAlignment(TextAlignment.LEFT).SetVerticalAlignment(VerticalAlignment.MIDDLE)));
                                continue;
                            }
                            mesurements.AddCell(new Cell().Add(new Paragraph(mesure.Source ?? " ").SetFont(font).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE)));
                            mesurements.AddCell(new Cell().Add(new Paragraph(mesure.Destination ?? " ").SetFont(font).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE)));
                            mesurements.AddCell(new Cell().Add(new Paragraph(mesure.NumberOfWire?.ToString() ?? " ").SetFont(font).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE)));
                            mesurements.AddCell(new Cell().Add(new Paragraph(mesure.Type?.Name?.ToString() ?? " ").SetFont(font).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE)));
                            mesurements.AddCell(new Cell().Add(new Paragraph(mesure.Distance?.ToString() ?? " ").SetFont(font).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE)));
                            mesurements.AddCell(new Cell().Add(new Paragraph(mesure.CountOfWeld?.ToString() ?? " ").SetFont(font).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE)));
                            mesurements.AddCell(new Cell().Add(new Paragraph(mesure.MaxAttenuation?.ToString() ?? " ").SetFont(font).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE)));
                            mesurements.AddCell(new Cell().Add(new Paragraph(mesure.RealAttenuation?.ToString() ?? " ").SetFont(font).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE)));
                            mesurements.AddCell(new Cell().Add(new Paragraph(mesure.PropperValue == true ? "Pozytywna" : "Negatywna" ?? " ").SetFont(font).SetTextAlignment(TextAlignment.CENTER).SetVerticalAlignment(VerticalAlignment.MIDDLE)));
                        }
                        document.Add(mesurements);
                        document.Add(new Paragraph("Oznaczenia: ").SetFont(boldFont).SetFontSize(5).Add(new Text("Lp - liczba porządkowa, R - tłumienność zmierzona, Rmax - obliczona tłumienność maksymalna").SetFont(font)));

                        progressWindow.SetProgress(80);

                        document.Add(new Paragraph("6. Uwagi i wnioski\n")
                            .SetFont(boldFont)
                            .Add(new Text($"{project.Conclusions ?? " "}")
                                .SetFont(font)));

                        document.Add(new Paragraph("7. Orzeczenie\n")
                            .SetFont(boldFont)
                            .Add(new Text($"{project.Opinion ?? " "}")
                                .SetFont(font)));

                        document.Add(new Paragraph("8. Wykowanca pomiarów")
                            .SetFont(boldFont));

                        progressWindow.SetProgress(90);

                        Table tab = new Table(2);
                        tab.SetBorder(Border.NO_BORDER);
                        tab.SetWidth(UnitValue.CreatePercentValue(100));
                        tab.AddHeaderCell(new Cell().Add(new Paragraph("Wykonał:").SetFont(font)).SetBorder(Border.NO_BORDER));
                        tab.AddHeaderCell(new Cell().Add(new Paragraph("Sprawdził:").SetFont(font)).SetBorder(Border.NO_BORDER));
                        tab.AddCell(new Cell().Add(new Paragraph(project.DoingPerson ?? " ").SetFont(font)).SetBorder(Border.NO_BORDER));
                        tab.AddCell(new Cell().Add(new Paragraph(project.VeryfingPerson ?? " ").SetFont(font)).SetBorder(Border.NO_BORDER));
                        document.Add(tab);

                        document.Close();
                        progressWindow.SetProgress(100);
                    }
                }
            }
            progressWindow.Close();
            MessageBox.Show( "Plik został utworzony!", "Zakończone", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private class HeaderHendler : IEventHandler
        {
            public virtual void HandleEvent(Event currentEvent)
            {
                if (MainWindow.activeWorkspace.LogoImg == null)
                    return;

                PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;
                PdfDocument pdf = docEvent.GetDocument();

                PdfPage page = docEvent.GetPage();
                Rectangle pageSize = page.GetPageSize().ApplyMargins(10, 30, 0, 30, false);

                Canvas canvas = new Canvas(new PdfCanvas(page), pdf, pageSize);
                canvas.SetHorizontalAlignment(HorizontalAlignment.LEFT);
                var logo = new Image(ImageDataFactory.Create(MainWindow.activeWorkspace.LogoImg));
                canvas.Add(logo.SetMaxHeight(50));
                canvas.Close();

            }
        }
        private class FooterHendler : IEventHandler
        {
            public virtual void HandleEvent(Event currentEvent)
            {
                PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;
                PdfDocument pdf = docEvent.GetDocument();

                PdfPage page = docEvent.GetPage();
                Rectangle pageSize = page.GetPageSize().ApplyMargins(10, 30, 10, 30, false);
                Rectangle box = new Rectangle(545, 20, 100, 10);

                int pagenum = docEvent.GetDocument().GetPageNumber(page);

                Canvas canvas = new Canvas(new PdfCanvas(page), pdf, box);
                canvas.SetFontSize(8);
                canvas.SetHorizontalAlignment(HorizontalAlignment.RIGHT);
                canvas.Add(new Paragraph($"{pagenum}"));
                canvas.Close();
            }
        }
    }
}

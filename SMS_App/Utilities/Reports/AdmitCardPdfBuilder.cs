using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SMS.Entities;
using SMS.Entities.RptModels;

namespace SMS_App.Utilities.Reports;

public class AdmitCardPdfBuilder : IDocument
{
    private readonly Institute _institute;
    private readonly string _logoBase64;
    private readonly List<RptAdmitCardVM> _data;

    public AdmitCardPdfBuilder(Institute institute, string logoBase64, List<RptAdmitCardVM> data)
    {
        _institute = institute;
        _logoBase64 = logoBase64;
        _data = data;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        var students = _data
            .GroupBy(d => d.StudentId)
            .Select(g => (Student: g.First(), Subjects: g.ToList()))
            .ToList();

        foreach (var (student, subjects) in students)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(9));
                page.Content().Element(c => ComposeCard(c, student, subjects));
            });
        }
    }

    private void ComposeCard(IContainer container, RptAdmitCardVM student, List<RptAdmitCardVM> subjects)
    {
        container.Border(1).Padding(10).Column(col =>
        {
            // Header
            col.Item().Row(row =>
            {
                if (!string.IsNullOrEmpty(_logoBase64))
                {
                    try { row.ConstantItem(50).Image(Convert.FromBase64String(_logoBase64)).FitArea(); } catch { }
                }
                row.RelativeItem().Column(c =>
                {
                    c.Item().AlignCenter().Text(_institute.Name ?? "").Bold().FontSize(12);
                    if (!string.IsNullOrEmpty(_institute.Address))
                        c.Item().AlignCenter().Text(_institute.Address).FontSize(8);
                    if (!string.IsNullOrEmpty(_institute.EIIN))
                        c.Item().AlignCenter().Text($"EIIN: {_institute.EIIN}").FontSize(8);
                });
            });

            col.Item().PaddingVertical(4).AlignCenter().Text("ADMIT CARD").Bold().FontSize(12);
            col.Item().Text($"Exam: {student.ExamTypeName}  |  Session: {student.SessionName}").FontSize(9);
            col.Item().PaddingVertical(4).LineHorizontal(0.5f);

            // Student info
            col.Item().Table(t =>
            {
                t.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); });
                t.Cell().Text("Student Name:").SemiBold(); t.Cell().Text(student.StudentName ?? "");
                t.Cell().Text("Class:").SemiBold(); t.Cell().Text($"{student.ClassName} - {student.SectionName}");
                t.Cell().Text("Roll:").SemiBold(); t.Cell().Text(student.ClassRoll.ToString());
                t.Cell().Text("Father's Name:").SemiBold(); t.Cell().Text(student.FatherName ?? "");
                t.Cell().Text("Gender:").SemiBold(); t.Cell().Text(student.Gender ?? "");
                t.Cell().Text("Religion:").SemiBold(); t.Cell().Text(student.Religion ?? "");
            });

            col.Item().PaddingVertical(6).LineHorizontal(0.5f);

            // Subjects table
            col.Item().Text("Subjects:").SemiBold();
            col.Item().PaddingTop(4).Table(t =>
            {
                t.ColumnsDefinition(c => { c.ConstantColumn(30); c.RelativeColumn(); });
                static IContainer H(IContainer c) => c.Background(Colors.Grey.Lighten1).Padding(3);
                t.Header(h => { h.Cell().Element(H).Text("SL").Bold(); h.Cell().Element(H).Text("Subject").Bold(); });

                int sl = 1;
                foreach (var sub in subjects)
                {
                    var bg = sl % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White;
                    IContainer D(IContainer c) => c.Background(bg).Padding(3);
                    t.Cell().Element(D).AlignCenter().Text(sl.ToString());
                    t.Cell().Element(D).Text(sub.SubjectName ?? "");
                    sl++;
                }
            });

            col.Item().PaddingTop(20).Row(row =>
            {
                row.RelativeItem().AlignCenter().Column(c =>
                {
                    c.Item().LineHorizontal(0.5f);
                    c.Item().AlignCenter().Text("Student Signature").FontSize(8);
                });
                row.ConstantItem(20);
                row.RelativeItem().AlignCenter().Column(c =>
                {
                    c.Item().LineHorizontal(0.5f);
                    c.Item().AlignCenter().Text("Principal Signature").FontSize(8);
                });
            });
        });
    }
}

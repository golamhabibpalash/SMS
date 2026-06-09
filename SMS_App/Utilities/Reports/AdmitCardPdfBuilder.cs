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
                page.Size(PageSizes.A5.Width, 152.4f);
                page.Margin(8);
                page.DefaultTextStyle(x => x.FontSize(7));
                page.Content().Element(c => ComposeCard(c, student, subjects));
            });
        }
    }

    private void ComposeCard(IContainer container, RptAdmitCardVM student, List<RptAdmitCardVM> subjects)
    {
        container.Border(1).Padding(4).Column(col =>
        {
            // Header
            col.Item().Row(row =>
            {
                if (!string.IsNullOrEmpty(_logoBase64))
                {
                    try { row.ConstantItem(28).Image(Convert.FromBase64String(_logoBase64)).FitArea(); } catch { }
                }
                row.RelativeItem().Column(c =>
                {
                    c.Item().AlignCenter().Text(_institute.Name ?? "").Bold().FontSize(10);
                    if (!string.IsNullOrEmpty(_institute.Address))
                        c.Item().AlignCenter().Text(_institute.Address).FontSize(6);
                    if (!string.IsNullOrEmpty(_institute.EIIN))
                        c.Item().AlignCenter().Text($"EIIN: {_institute.EIIN}").FontSize(6);
                });
            });

            col.Item().PaddingVertical(2).AlignCenter().Text("ADMIT CARD").Bold().FontSize(10);
            col.Item().Text($"Exam: {student.ExamTypeName}  |  Session: {student.SessionName}").FontSize(7);
            col.Item().PaddingVertical(1).LineHorizontal(0.5f);

            // Student info - two columns (left / right)
            col.Item().Row(studentRow =>
            {
                studentRow.RelativeItem().Column(left =>
                {
                    left.Item().Text(t =>
                    {
                        t.Span("Student Name: ").SemiBold().FontSize(7);
                        t.Span(student.StudentName ?? "").FontSize(7);
                    });
                    left.Item().Text(t =>
                    {
                        t.Span("Father's Name: ").SemiBold().FontSize(7);
                        t.Span(student.FatherName ?? "").FontSize(7);
                    });
                    left.Item().Text(t =>
                    {
                        t.Span("Class: ").SemiBold().FontSize(7);
                        t.Span($"{student.ClassName} - {student.SectionName}").FontSize(7);
                    });
                    left.Item().Text(t =>
                    {
                        t.Span("Roll: ").SemiBold().FontSize(7);
                        t.Span(student.ClassRoll.ToString()).FontSize(7);
                    });
                });

                studentRow.RelativeItem().Column(right =>
                {
                    right.Item().Text(t =>
                    {
                        t.Span("Mother's Name: ").SemiBold().FontSize(7);
                        t.Span(student.MotherName ?? "").FontSize(7);
                    });
                    right.Item().Text(t =>
                    {
                        t.Span("Gender: ").SemiBold().FontSize(7);
                        t.Span(student.Gender ?? "").FontSize(7);
                    });
                    right.Item().Text(t =>
                    {
                        t.Span("Religion: ").SemiBold().FontSize(7);
                        t.Span(student.Religion ?? "").FontSize(7);
                    });
                });
            });

            col.Item().PaddingVertical(1).LineHorizontal(0.5f);

            // Subjects - 3 columns (3 subjects per row, code+name)
            col.Item().Text("Subjects:").SemiBold().FontSize(7);
            col.Item().PaddingTop(1).Table(t =>
            {
                t.ColumnsDefinition(c =>
                {
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                });

                for (int i = 0; i < subjects.Count; i += 3)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        var idx = i + j;
                        if (idx < subjects.Count)
                        {
                            var bg = (i / 3) % 2 == 0 ? Colors.White : Colors.Grey.Lighten3;
                            IContainer D(IContainer c) => c.Background(bg).Padding(1).BorderBottom(0.2f).BorderColor(Colors.Grey.Lighten2);
                            t.Cell().Element(D).AlignLeft().Text(txt =>
                            {
                                txt.Span(subjects[idx].SubjectCode?.ToString() ?? "").SemiBold().FontSize(6);
                                txt.Span(" ").FontSize(6);
                                txt.Span(subjects[idx].SubjectName ?? "").FontSize(6);
                            });
                        }
                        else
                        {
                            t.Cell().Text("");
                        }
                    }
                }
            });

            // Bottom section: directions (left) + controller signature (right)
            col.Item().PaddingTop(6).Row(bottomRow =>
            {
                bottomRow.RelativeColumn(3).Column(directions =>
                {
                    directions.Item().Text("Direction:").SemiBold().FontSize(6);
                    directions.Item().Text("1. The Examinee must bring the Admit Card in the Examination hall.").FontSize(6);
                    directions.Item().Text("2. The examinee must sign in the attendance sheet for each subject in the examination hall otherwise will be treated as absent in the respective subject(s).").FontSize(6);
                });

                bottomRow.RelativeColumn(2).AlignRight().Column(sig =>
                {
                    sig.Item().LineHorizontal(0.5f);
                    sig.Item().AlignCenter().Text("Controller of Examinations").SemiBold().FontSize(6);
                    sig.Item().AlignCenter().Text(_institute.Name ?? "").FontSize(6);
                });
            });
        });
    }
}

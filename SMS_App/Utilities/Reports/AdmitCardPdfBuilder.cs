using System;
using System.Collections.Generic;
using System.Globalization;
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
    private readonly string _signatureBase64;
    private readonly List<RptAdmitCardVM> _data;

    private static string ToTitle(string s) =>
        string.IsNullOrWhiteSpace(s) ? ""
            : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLowerInvariant());

    public AdmitCardPdfBuilder(Institute institute, string logoBase64, string signatureBase64, List<RptAdmitCardVM> data)
    {
        _institute = institute;
        _logoBase64 = logoBase64;
        _signatureBase64 = signatureBase64;
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
                page.Size(210.82f, 119.38f);
                page.Margin(5);
                page.DefaultTextStyle(x => x.FontSize(7));

                page.Content().Border(0.5f).Padding(1.5f).Column(col =>
                {
                    ComposeBody(col, student, subjects);
                });

                page.Footer().PaddingHorizontal(1.5f).PaddingTop(1.5f).Row(bottomRow =>
                {
                    bottomRow.RelativeItem(3).Column(directions =>
                    {
                        directions.Item().Text("Direction:").SemiBold().FontSize(3.5f);
                        directions.Item().Text("1. The Examinee must bring the Admit Card in the Examination hall.").FontSize(3.5f);
                        directions.Item().Text("2. The examinee must sign in the attendance sheet for each subject in the examination hall otherwise will be treated as absent in the respective subject(s).").FontSize(3.5f);
                    });

                    bottomRow.RelativeItem(2).AlignRight().Column(sig =>
                    {
                        if (!string.IsNullOrEmpty(_signatureBase64))
                        {
                            try
                            {
                                sig.Item().Height(14).AlignRight().Image(
                                    Convert.FromBase64String(_signatureBase64)).FitArea();
                            }
                            catch { }
                        }
                        sig.Item().AlignCenter().Text("Controller of Examinations").SemiBold().FontSize(3.5f);
                        sig.Item().AlignCenter().Text(_institute.Name ?? "").FontSize(3.5f);
                    });
                });
            });
        }
    }

    private void ComposeBody(ColumnDescriptor col, RptAdmitCardVM student, List<RptAdmitCardVM> subjects)
    {
        void InfoCell(IContainer c, string label, string value)
        {
            c.Padding(0.5f).Text(t =>
            {
                t.Span(label).SemiBold().FontSize(4.5f);
                t.Span(value).FontSize(3.5f);
            });
        }

        // Header: logo + institute
        col.Item().Row(headerRow =>
        {
            if (!string.IsNullOrEmpty(_logoBase64))
            {
                try { headerRow.ConstantItem(18).Image(Convert.FromBase64String(_logoBase64)).FitArea(); } catch { }
            }
            headerRow.RelativeItem().Column(c =>
            {
                c.Item().AlignCenter().Text(_institute.Name ?? "").Bold().FontSize(6);
                if (!string.IsNullOrEmpty(_institute.Address))
                    c.Item().AlignCenter().Text(_institute.Address).FontSize(4);
                if (!string.IsNullOrEmpty(_institute.EIIN))
                    c.Item().AlignCenter().Text($"EIIN: {_institute.EIIN}").FontSize(4);
            });
        });

        // Title + exam info on same row
        col.Item().Row(titleRow =>
        {
            titleRow.RelativeItem(2).AlignCenter().Text("ADMIT CARD").Bold().FontSize(6);
            titleRow.RelativeItem(3).AlignCenter().Text($"{student.ExamGroupName ?? student.ExamTypeName}  |  {student.SessionName}").FontSize(4.5f);
        });

        col.Item().LineHorizontal(0.5f);

        // Name fields in 2-column table (wider for long names)
        col.Item().PaddingVertical(0.5f).Table(nameTable =>
        {
            nameTable.ColumnsDefinition(c =>
            {
                c.RelativeColumn();
                c.RelativeColumn();
            });

            nameTable.Cell().Element(c => InfoCell(c, "Student Name: ", ToTitle(student.StudentName)));
            nameTable.Cell().Element(c => InfoCell(c, "Father's Name: ", ToTitle(student.FatherName)));
            nameTable.Cell().Element(c => InfoCell(c, "Mother's Name: ", ToTitle(student.MotherName)));
            nameTable.Cell().Element(c => InfoCell(c, "Gender: ", student.Gender ?? ""));
        });

        // Other fields in 3-column table
        col.Item().Table(otherTable =>
        {
            otherTable.ColumnsDefinition(c =>
            {
                c.RelativeColumn();
                c.RelativeColumn();
                c.RelativeColumn();
            });

            otherTable.Cell().Element(c => InfoCell(c, "Class: ", $"{student.ClassName} - {student.SectionName}"));
            otherTable.Cell().Element(c => InfoCell(c, "Roll: ", student.ClassRoll.ToString()));
            otherTable.Cell().Element(c => InfoCell(c, "Religion: ", student.Religion ?? ""));
            otherTable.Cell().Element(c => InfoCell(c, "", ""));
            otherTable.Cell().Element(c => InfoCell(c, "", ""));
            otherTable.Cell().Element(c => InfoCell(c, "", ""));
        });

        col.Item().LineHorizontal(0.5f);

        // Subjects in 4-column table
        col.Item().Text("Subjects:").SemiBold().FontSize(4.5f);
        col.Item().PaddingTop(0.5f).Table(t =>
        {
            t.ColumnsDefinition(c =>
            {
                c.RelativeColumn();
                c.RelativeColumn();
                c.RelativeColumn();
                c.RelativeColumn();
            });

            for (int i = 0; i < subjects.Count; i += 4)
            {
                for (int j = 0; j < 4; j++)
                {
                    var idx = i + j;
                    if (idx < subjects.Count)
                    {
                        var bg = (i / 4) % 2 == 0 ? Colors.White : Colors.Grey.Lighten3;
                        IContainer D(IContainer c) => c.Background(bg).Padding(0.5f).BorderBottom(0.2f).BorderColor(Colors.Grey.Lighten2);
                        t.Cell().Element(D).AlignLeft().Text(txt =>
                        {
                            txt.Span(subjects[idx].SubjectCode?.ToString() ?? "").SemiBold().FontSize(3.5f);
                            txt.Span(" ").FontSize(3.5f);
                            txt.Span(subjects[idx].SubjectName ?? "").FontSize(3.5f);
                        });
                    }
                    else
                    {
                        t.Cell().Text("");
                    }
                }
            }
        });
    }
}

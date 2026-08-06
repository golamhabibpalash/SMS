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
    // The admit card is printed on fixed 8.5in x 4.5in stock. Every size below is
    // tuned to that page, so change these together if the card size ever changes.
    private const float CardWidthInches = 8.5f;
    private const float CardHeightInches = 4.5f;

    private const float InstituteNameFont = 16f;
    private const float InstituteMetaFont = 10f;
    private const float TitleFont = 17f;
    private const float ExamInfoFont = 12f;
    private const float LabelFont = 11f;
    private const float ValueFont = 10f;
    private const float SubjectFont = 9.5f;
    private const float DirectionFont = 8f;

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
                page.Size(CardWidthInches, CardHeightInches, Unit.Inch);
                page.Margin(14);
                page.DefaultTextStyle(x => x.FontSize(ValueFont));

                page.Content().Border(1).Padding(4).Column(col =>
                {
                    ComposeBody(col, student, subjects);
                });

                page.Footer().PaddingHorizontal(4).PaddingTop(4).Row(bottomRow =>
                {
                    bottomRow.RelativeItem(3).Column(directions =>
                    {
                        directions.Item().Text("Direction:").SemiBold().FontSize(DirectionFont);
                        directions.Item().Text("1. The Examinee must bring the Admit Card in the Examination hall.").FontSize(DirectionFont);
                        directions.Item().Text("2. The examinee must sign in the attendance sheet for each subject in the examination hall otherwise will be treated as absent in the respective subject(s).").FontSize(DirectionFont);
                    });

                    bottomRow.RelativeItem(2).AlignRight().Column(sig =>
                    {
                        if (!string.IsNullOrEmpty(_signatureBase64))
                        {
                            try
                            {
                                sig.Item().Height(38).AlignRight().Image(
                                    Convert.FromBase64String(_signatureBase64)).FitArea();
                            }
                            catch { }
                        }
                        sig.Item().AlignCenter().Text("Controller of Examinations").SemiBold().FontSize(DirectionFont);
                        sig.Item().AlignCenter().Text(_institute.Name ?? "").FontSize(DirectionFont);
                    });
                });
            });
        }
    }

    private void ComposeBody(ColumnDescriptor col, RptAdmitCardVM student, List<RptAdmitCardVM> subjects)
    {
        void InfoCell(IContainer c, string label, string value)
        {
            c.Padding(1.5f).Text(t =>
            {
                t.Span(label).SemiBold().FontSize(LabelFont);
                t.Span(value).FontSize(ValueFont);
            });
        }

        // Header: logo + institute
        col.Item().Row(headerRow =>
        {
            if (!string.IsNullOrEmpty(_logoBase64))
            {
                try { headerRow.ConstantItem(50).Image(Convert.FromBase64String(_logoBase64)).FitArea(); } catch { }
            }
            headerRow.RelativeItem().Column(c =>
            {
                c.Item().AlignCenter().Text(_institute.Name ?? "").Bold().FontSize(InstituteNameFont);
                if (!string.IsNullOrEmpty(_institute.Address))
                    c.Item().AlignCenter().Text(_institute.Address).FontSize(InstituteMetaFont);
                if (!string.IsNullOrEmpty(_institute.EIIN))
                    c.Item().AlignCenter().Text($"EIIN: {_institute.EIIN}").FontSize(InstituteMetaFont);
            });
        });

        // Title + exam info on same row
        col.Item().Row(titleRow =>
        {
            titleRow.RelativeItem(2).AlignCenter().Text("ADMIT CARD").Bold().FontSize(TitleFont);
            titleRow.RelativeItem(3).AlignCenter().Text($"{student.ExamGroupName ?? student.ExamTypeName}  |  {student.SessionName}").FontSize(ExamInfoFont);
        });

        col.Item().LineHorizontal(1);

        // Name fields in 2-column table (wider for long names)
        col.Item().PaddingVertical(1.5f).Table(nameTable =>
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
        });

        col.Item().LineHorizontal(1);

        // Subjects in 4-column table. The card must never spill onto a second page,
        // so tighten the rows as the subject count grows.
        var rowCount = (int)Math.Ceiling(subjects.Count / 4.0);
        var (subjectFont, subjectPadding) = rowCount switch
        {
            <= 3 => (SubjectFont, 1.5f),
            4 => (8f, 1f),
            5 => (7f, 0.75f),
            _ => (6f, 0.5f),
        };

        col.Item().Text("Subjects:").SemiBold().FontSize(LabelFont);
        col.Item().PaddingTop(1.5f).Table(t =>
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
                        IContainer D(IContainer c) => c.Background(bg).Padding(subjectPadding).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2);
                        t.Cell().Element(D).AlignLeft().Text(txt =>
                        {
                            txt.Span(subjects[idx].SubjectCode?.ToString() ?? "").SemiBold().FontSize(subjectFont);
                            txt.Span(" ").FontSize(subjectFont);
                            txt.Span(subjects[idx].SubjectName ?? "").FontSize(subjectFont);
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

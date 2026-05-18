using System;
using System.Collections.Generic;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SMS.Entities;
using SMS.Entities.RptModels.Results;
using SMS_App.ViewModels.ReportVM.MarkSheet;

namespace SMS_App.Utilities.Reports;

public class MarkSheetPdfBuilder
{
    private readonly Institute _institute;
    private readonly string _logoBase64;
    private readonly List<StudentWiseMarkSheetVM> _results;
    private readonly List<GradingTable> _gradingTables;
    private readonly Dictionary<int, List<SubRerportAnnualReport>> _annualReports;
    private readonly string _examName;
    private readonly string _className;
    private readonly string _publicationDate;
    private readonly string _highestMarks;
    private readonly Dictionary<string, double> _maxMarksPerSubject;

    public MarkSheetPdfBuilder(
        Institute institute,
        string logoBase64,
        List<StudentWiseMarkSheetVM> results,
        List<GradingTable> gradingTables,
        Dictionary<int, List<SubRerportAnnualReport>> annualReports,
        string examName,
        string className,
        string publicationDate,
        string highestMarks)
    {
        _institute = institute;
        _logoBase64 = logoBase64;
        _results = results;
        _gradingTables = gradingTables;
        _annualReports = annualReports;
        _examName = examName;
        _className = className;
        _publicationDate = publicationDate;
        _highestMarks = highestMarks;
        _maxMarksPerSubject = results
            .GroupBy(r => r.SubjectName)
            .ToDictionary(g => g.Key, g => g.Max(r => r.ObtainMark));
    }

    public byte[] Generate()
    {
        var students = _results
            .GroupBy(r => r.StudentId)
            .ToList();

        return Document.Create(container =>
        {
            foreach (var studentGroup in students)
            {
                var first = studentGroup.First();
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Element(h => BuildHeader(h, first));
                    page.Content().Element(c => BuildContent(c, studentGroup.ToList(), first));
                    page.Footer().Element(f => BuildFooter(f, first));
                });
            }
        }).GeneratePdf();
    }

    void BuildHeader(IContainer container, StudentWiseMarkSheetVM student)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                if (!string.IsNullOrEmpty(_logoBase64))
                {
                    row.ConstantItem(60).AlignLeft().AlignMiddle().Image(
                        Convert.FromBase64String(_logoBase64)).FitArea();
                }

                row.RelativeItem().PaddingLeft(10).Column(c =>
                {
                    c.Item().AlignCenter().Text(_institute.Name ?? "").Bold().FontSize(16);
                    c.Item().AlignCenter().Text(_institute.Address ?? "").FontSize(9);
                    if (!string.IsNullOrEmpty(_institute.EIIN))
                        c.Item().AlignCenter().Text($"EIIN: {_institute.EIIN}").FontSize(9);
                });

                row.ConstantItem(130).AlignRight().AlignTop().Column(c =>
                {
                    c.Item().Border(1).Padding(0).Column(g =>
                    {
                        g.Item().Background(Colors.Grey.Lighten3).Padding(2).Text("Marks and Grade\nDistribution System").Bold().FontSize(6).AlignCenter();
                        g.Item().Row(r =>
                        {
                            r.ConstantItem(40).Border(1).Padding(1).Background(Colors.Grey.Lighten4).Text("Marks").Bold().FontSize(5).AlignCenter();
                            r.ConstantItem(40).Border(1).Padding(1).Background(Colors.Grey.Lighten4).Text("Grade").Bold().FontSize(5).AlignCenter();
                            r.RelativeItem().Border(1).Padding(1).Background(Colors.Grey.Lighten4).Text("Point").Bold().FontSize(5).AlignCenter();
                        });
                        foreach (var gt in _gradingTables.OrderBy(x => x.NumberRangeMin))
                        {
                            g.Item().Row(r =>
                            {
                                r.ConstantItem(40).Border(1).Padding(1).Text($"{gt.NumberRangeMin}-{gt.NumberRangeMax}").FontSize(5).AlignCenter();
                                r.ConstantItem(40).Border(1).Padding(1).Text(gt.LetterGrade).FontSize(5).AlignCenter();
                                r.RelativeItem().Border(1).Padding(1).Text(gt.GradePoint.ToString("F2")).FontSize(5).AlignCenter();
                            });
                        }
                    });
                });
            });

            col.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Black);
            col.Item().AlignCenter().Text("ACADEMIC TRANSCRIPT").Bold().FontSize(14);
            col.Item().AlignCenter().Text($"{_examName} - {_className}").FontSize(10);
            col.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Black);
        });
    }

    void BuildContent(IContainer container, List<StudentWiseMarkSheetVM> subjects, StudentWiseMarkSheetVM student)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"Student Name: {student.StudentName}").Bold();
                    c.Item().Text($"Father's Name: {student.FatherName}");
                    c.Item().Text($"Mother's Name: {student.MotherName}");
                    c.Item().Text($"Class Roll: {student.ClassRoll}");
                    c.Item().Text($"Section: {student.SectionName}");
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"Date of Birth: {student.DOB:dd MMM yyyy}");
                    c.Item().Text($"Gender: {student.GenderName}");
                    c.Item().Text($"Religion: {student.ReligionName}");
                    c.Item().Text($"Published: {_publicationDate}");
                    c.Item().Text($"Highest Marks: {_highestMarks}");
                });
            });

            col.Item().PaddingTop(8).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(3);
                    c.RelativeColumn(1.2f);
                    c.RelativeColumn(1.2f);
                    c.RelativeColumn(1);
                    c.RelativeColumn(1);
                    c.RelativeColumn(1);
                    c.RelativeColumn(1.5f);
                    c.RelativeColumn(1.5f);
                });

                table.Header(h =>
                {
                    h.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3).Text("Subject").Bold().FontSize(8);
                    h.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3).Text("Total Mark").Bold().FontSize(8).AlignCenter();
                    h.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3).Text("Obtain Mark").Bold().FontSize(8).AlignCenter();
                    h.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3).Text("GPA").Bold().FontSize(8).AlignCenter();
                    h.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3).Text("Grade").Bold().FontSize(8).AlignCenter();
                    h.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3).Text("Max").Bold().FontSize(8).AlignCenter();
                    h.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3).Text("Final Point").Bold().FontSize(8).AlignCenter();
                    h.Cell().Border(1).Padding(3).Background(Colors.Grey.Lighten3).Text("Final Grade").Bold().FontSize(8).AlignCenter();
                });

                for (int i = 0; i < subjects.Count; i++)
                {
                    var subj = subjects[i];
                    _maxMarksPerSubject.TryGetValue(subj.SubjectName, out var maxMark);

                    table.Cell().Border(1).Padding(3).Text(subj.SubjectName).FontSize(8);
                    table.Cell().Border(1).Padding(3).Text(subj.TotalMark.ToString("F2")).FontSize(8).AlignCenter();
                    table.Cell().Border(1).Padding(3).Text(subj.ObtainMark.ToString("F2")).FontSize(8).AlignCenter();
                    table.Cell().Border(1).Padding(3).Text(subj.GPA.ToString("F2")).FontSize(8).AlignCenter();
                    table.Cell().Border(1).Padding(3).Text(subj.Grade).FontSize(8).AlignCenter();
                    table.Cell().Border(1).Padding(3).Text(maxMark.ToString("F2")).FontSize(8).AlignCenter();

                    if (i == 0)
                    {
                        table.Cell().RowSpan((uint)subjects.Count).Border(1).AlignMiddle().AlignCenter().Text(student.FinalGPA).Bold().FontSize(9);
                        table.Cell().RowSpan((uint)subjects.Count).Border(1).AlignMiddle().AlignCenter().Text(student.FinalGrade).Bold().FontSize(9);
                    }
                }
            });

            col.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Border(1).Padding(3).Column(c =>
                {
                    c.Item().Text($"Final Grade: {student.FinalGrade}").Bold();
                });
                row.RelativeItem().Border(1).Padding(3).Column(c =>
                {
                    c.Item().Text($"Merit Position: {student.MeritPosition}");
                    c.Item().Text($"Total Fails: {student.TotalFails}");
                    c.Item().Text($"Attendance: {student.AttendancePercentage:F1}%");
                });
            });

            if (!string.IsNullOrEmpty(student.GradeComments))
            {
                col.Item().PaddingTop(3).Text($"Comments: {student.GradeComments}").Italic().FontSize(8);
            }

            if (_annualReports.TryGetValue(student.StudentId, out var annualData) && annualData.Any())
            {
                col.Item().PaddingTop(8).Border(1).Padding(3).Column(ac =>
                {
                    ac.Item().Text("Annual Progress Report").Bold().FontSize(10);
                    ac.Item().Table(t =>
                    {
                        t.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(2);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1);
                            c.RelativeColumn(1);
                        });

                        t.Header(h =>
                        {
                            h.Cell().Padding(2).Background(Colors.Grey.Lighten3).Text("Month").Bold().FontSize(7);
                            h.Cell().Padding(2).Background(Colors.Grey.Lighten3).Text("Attendance").Bold().FontSize(7).AlignCenter();
                            h.Cell().Padding(2).Background(Colors.Grey.Lighten3).Text("Position").Bold().FontSize(7).AlignCenter();
                            h.Cell().Padding(2).Background(Colors.Grey.Lighten3).Text("Total Std").Bold().FontSize(7).AlignCenter();
                        });

                        foreach (var month in annualData.Where(m => !string.IsNullOrEmpty(m.Month)))
                        {
                            t.Cell().Padding(2).Text(month.Month).FontSize(7);
                            t.Cell().Padding(2).Text(month.AttendancePercent ?? "-").FontSize(7).AlignCenter();
                            t.Cell().Padding(2).Text(month.MeritPosition?.ToString() ?? "-").FontSize(7).AlignCenter();
                            t.Cell().Padding(2).Text(month.TotalStudent?.ToString() ?? "-").FontSize(7).AlignCenter();
                        }
                    });
                });
            }

        });
    }

    void BuildFooter(IContainer container, StudentWiseMarkSheetVM student)
    {
        container.AlignCenter().Text(t =>
        {
            t.Span("Generated on: ").FontSize(7);
            t.Span($"{DateTime.Now:dd MMM yyyy HH:mm}").FontSize(7).Light();
        });
    }
}

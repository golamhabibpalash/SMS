create view vw_schedule_wise_stu_paid
as
select s.Id[StudentId],s.ClassRoll,s.name[StudentName], c.Id[ClassId], c.Name[ClassName],f.Name[PaymentType], d.PaidAmount,p.TotalPayment
from StudentPaymentDetails d
inner join StudentPayment p on d.StudentPaymentId = p.Id 
left join student s on p.StudentId = s.Id
left join AcademicClass c on s.AcademicClassId = c.Id
left join StudentFeeHead f on d.StudentFeeHeadId=f.Id;
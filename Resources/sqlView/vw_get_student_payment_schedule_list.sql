CREATE VIEW vw_get_student_payment_schedule_list 
AS
select s.Id, 
s.ClassRoll, 
s.Name[StudentName],
c.Name[ClassName],
sFee.Name[PaymentType],
cFee.Amount,
sFee.Repeatedly,
sFee.YearlyFrequency
from student s
left join AcademicClass c on s.AcademicClassId=c.Id
left join ClassFeeList cFee on c.Id = cFee.AcademicClassId
left join StudentFeeHead sFee on cFee.StudentFeeHeadId = sFee.Id;
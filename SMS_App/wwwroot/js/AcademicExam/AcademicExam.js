
$(document).ready(function () {
    $('.lockBtn').click(function () {
        if (confirm("Are you sure you want to unlock this?")) {
            let btnId = $(this).prop('id');
            unlockExam($(this).data("id"), btnId);
        }
        else {
            console.log("No");
            return false;
        }
    });

    $('.unLockBtn').click(function () {
        if (confirm("Are you sure you want to unlock this?")) {
            let btnId = $(this).prop('id');
            lockExam($(this).data("id"), btnId);
        }
        else {
            console.log("No");
            return false;
        }
    });
    //$('.examEditBtn').click(function () {
    //    let id = $(this).data('examid');
    //    let marks = $('#editBtn_' + id).data('marks');
    //    let groupId = $('#editBtn_' + id).data('groupid');
    //    let classId = $('#editBtn_' + id).data('classid');
    //    let subjectid = $('#editBtn_' + id).data('subjectid');
    //    let sectionId = $('#editBtn_' + id).data('sectionid');
    //    let teacherId = $('#editBtn_' + id).data('employeeid');
    //    let isActive = $('#editBtn_' + id).data('status');

    //    $('#Id').val(id);
    //    $('#AcademicExamGroupId').val(groupId).trigger('change');

    //    $('#AcademicClassId').val(classId).trigger('change');

    //    $('#TotalMarks').val(marks);
    //    $('#EmployeeId').val(teacherId).trigger('change');
    //    $('#AcademicSubjectId').val(subjectid).trigger('change');
    //    $('#AcademicSectionId').val(sectionId).trigger('change');

    //    if (isActive == "True") {
    //        $('#Status').prop('checked', true);
    //    }
    //    else {
    //        $('#Status').prop('checked', false);
    //    }
    //    //document.getElementById('AcademicExamGroupId').disabled = "disabled";
    //    //document.getElementById('AcademicClassId').disabled = "disabled";
    //});
});
function unlockExam(id, btnId) {
    $.ajax({
        type: "post",
        url: '/AcademicExams/UnlockExam',
        data: "exId=" + id,
        success: function (response) {
            location.reload(true);
        },
        error: {

        }
    });
}

function lockExam(id, btnId) {
    $.ajax({
        type: "post",
        url: '/AcademicExams/lockExam',
        data: "exId=" + id,
        success: function (response) {
            location.reload(true);
        },
        error: {

        }
    });
}
$('#AcademicClassId').change(function () {
    let id = $('#AcademicClassId option:selected').val();

    let subjectsLoaded = $.ajax({
        url: '/AcademicSubjects/GetSubjectsByClassId?classId=' + id,
        method: 'POST',
        dataType: 'JSON',
        success: function (data) {
            $('#AcademicSubjectId').empty();
            $.each(data, function (i, obj) {
                var op = "<option value='" + obj.id + "'>" + obj.subjectName + "</option>";
                $('#AcademicSubjectId').append(op);
            });
        },
        error: function () { }
    });

    let sectionsLoaded = $.ajax({
        url: "/api/academicsections/getbyclasswithsessionId?classId=" + id + "&sessionId=" + null,
        dataType: "JSON",
        type: "POST",
        cache: false,
        success: function (data) {
            $('#AcademicSectionId').empty();

            if (data && data.length > 0) {
                var o2 = '<option value="">All Section</option>';
                $('#AcademicSectionId').append(o2);
                $.each(data, function (i, obj) {
                    var op = '<option value="' + obj.id + '">' + obj.name + '</option>';
                    $('#AcademicSectionId').append(op);
                });
            } else {
                var o = '<option disabled selected>Section Not Found</option>';
                $('#AcademicSectionId').append(o);
            }
        },
        error: function (err) {
            console.log(err);
        }
    });

    // 🔔 When BOTH ajax calls complete, trigger event
    $.when(subjectsLoaded, sectionsLoaded).done(function () {
        $(document).trigger('classDataLoaded');
    });
});

function validateForm() {
    let marks = document.getElementById('TotalMarks').value;
    let groupId = document.getElementById('AcademicExamGroupId').value;
    let classId = document.getElementById('AcademicClassId').value;
    let subjectid = document.getElementById('AcademicSubjectId').value;
    let sectionId = document.getElementById('AcademicSectionId').value;
    let teacherId = document.getElementById('EmployeeId').value;
    if (groupId <= 0) {
        alert("Please Select Academic Exam Group");
        document.getElementById('AcademicExamGroupId').focus();
        return false;
    }
    else if (classId <=0) {
        alert("Please select class");
        document.getElementById('AcademicClassId').focus();
        return false;
    }
    else if (subjectid <= 0) {
        alert("Please select Subject from list");
        document.getElementById('AcademicSubjectId').focus();
        return false;
    }
    else if (teacherId <= 0) {
        alert("Please select Teacher from list");
        document.getElementById('EmployeeId').focus();
        return false;
    }
    else if (marks < 10 || marks > 100) {
        alert("Total Marks Field is not valid");
        document.getElementById('TotalMarks').focus();
        return false;
    }
    return true;
}

function ClearBtnClicked() {
    $("#detailsTable > tbody").empty();
}
function removeRow(button) {
    var row = button.parentNode.parentNode;
    row.parentNode.removeChild(row);
}


function ExamAddBtnClick() {
    let isValidate = validateForm();
    if (isValidate) {
        let marks = document.getElementById('TotalMarks').value;

        let groupId = document.getElementById('AcademicExamGroupId').value;

        var classIdElement = document.getElementById("AcademicClassId");
        var classIdOption = classIdElement.options[classIdElement.selectedIndex];
        let classId = classIdOption.value;
        let classIdText = classIdOption.text;

        let examCategoryDD = document.getElementById('ExamCategory');
        let examCategory = examCategoryDD.options[examCategoryDD.selectedIndex];

        let subjectidElement = document.getElementById('AcademicSubjectId');
        let subjectidOption = subjectidElement.options[subjectidElement.selectedIndex];
        let subjectid = subjectidOption.value;
        let subjectIdText = subjectidOption.text;

        let sectionIdElement = document.getElementById('AcademicSectionId');
        let sectionIdOption = sectionIdElement.options[sectionIdElement.selectedIndex];
        let sectionId = sectionIdOption.value;
        let sectionIdText = sectionIdOption.text;

        let teacherIdElement = document.getElementById('EmployeeId');
        let teacherIdOption = teacherIdElement.options[teacherIdElement.selectedIndex];
        let teacherId = teacherIdOption.value;
        let teacherIdText = teacherIdOption.text;

        //let status = document.getElementById('Status').value;

        let tableBody = document.getElementById('tableBodyId');
        var indexCount = $("#detailsTable > tbody").children().length;

        let rowCount = indexCount + 1; 

        let serial_td = '<td><input type="hidden" name="AcademicExam[' + indexCount + '].AcademicExamGroupId" value="' + groupId + '" />' + rowCount + '</td>'
        let subject_td = '<td>  <input type="hidden" name="AcademicExam[' + indexCount + '].AcademicSubjectId" value="' + subjectid + '" />' + subjectIdText + '</td>';
        let examCategory_td = '<td><input type="hidden" name="AcademicExam[' + indexCount + '].ExamCategory" value="' + examCategory.value + '" /> ' + examCategory.value + '</td>';
        let class_td = '<td> <input type="hidden" name="AcademicExam[' + indexCount + '].AcademicClassId" value="' + classId + '" />' + classIdText + '</td>';
        let section_td = '<td> <input type="hidden" name="AcademicExam[' + indexCount + '].AcademicSectionId" value="' + sectionId + '" />' + sectionIdText + '</td>';
        let teacher_td = '<td> <input type="hidden" name="AcademicExam[' + indexCount + '].EmployeeId" value="' + teacherId + '" />' + teacherIdText + '</td>';
        let marks_td = '<td class="text-end"> <input type="hidden" name="AcademicExam[' + indexCount + '].TotalMarks" value="' + marks + '" />' + marks + '</td>';
        //let status_td = '<td> <input type="hidden" name="AcademicExam[' + indexCount + '].Status" value="' + status + '" />' + status + '</td>';
        let action_td = '<td><button onclick="removeRow(this)" class="removeBtn btn btn-sm btn-warning" value="Remove">Remove</button></td>';
        let tr = '<tr>' + serial_td + subject_td + examCategory_td + class_td + section_td + teacher_td + marks_td /*+ status_td*/ + action_td+ '</tr>';
        tableBody.innerHTML +=tr;
    }
}

$('#submitBtn').click(function () {

    showLoader()
    $('#submitForm').submit(function () {
            $('#createUpdateModal').modal('hide');
        document.getElementById("loading").style.display = "block";
        setTimeout(function () {

            document.getElementById("myForm").reset();
        }, 2000);

    });
});
function DeleteExam(id) {
    var result = confirm("Are you sure you want to proceed to delete?");
    if (result) {
        $.ajax({
            url: '/AcademicExams/Delete?Id=' + id,
            method: 'Post',
            type: 'JSON',
            success: function (data) {
                location.reload();
            },
            error: function () { }
        });

    } else {
        // User clicked "Cancel"
        // Handle cancel action or do nothing
    }
}

async function EditExamClick(id) {

    //examGroup dropdown 
    let groupId = $('#editBtn_' + id).data('groupid');
    $('#AcademicExamGroupId').val(groupId).trigger('change');

    //Class Dropdown
    let classId = $('#editBtn_' + id).data('classid');
    $('#AcademicClassId').val(classId).trigger('change');

    // 🕒 Wait for both subjects and sections to load
    await new Promise(resolve => $(document).one('classDataLoaded', resolve));

    //Now safely set dependent dropdowns
    let sectionId = $('#editBtn_' + id).data('sectionid');
    $('#AcademicSectionId').val(sectionId).trigger('change');

    let subjectid = $('#editBtn_' + id).data('subjectid');
    $('#AcademicSubjectId').val(subjectid).trigger('change');

    //Mark input fields
    let marks = $('#editBtn_' + id).data('marks');
    $('#TotalMarks').val(marks);

    //Exam Category
    let cat = $('#editBtn_' + id).data('category');
    $('#ExamCategory').val(cat);

    //Teacher
    let teacherId = $('#editBtn_' + id).data('employeeid');
    $('#EmployeeId').val(teacherId).trigger('change');

    let isActive = $('#editBtn_' + id).data('status');
    $('#Id').val(id);
    $('#Status').prop('checked', isActive === "True");

    // UI adjustments
    document.getElementById('editupdatemodalfooter').style.display = 'none';
    document.getElementById('createUpdateModalLabel').innerHTML = "Edit Exam Info";
    document.getElementById('examAddBtn').style.display = 'none';
    document.getElementById('updateFormSubmitBtn').style.display = 'block';
}



$('#updateFormSubmitBtn').click(function () {
    /*modalFooter*/
    let isValidate = validateForm();
    if (isValidate) {
        alertify.confirm("This is a confirm dialog.",
            function () {
                alertify.success('Ok');
            },
            function () {
                alertify.error('Cancel');
            });
    }

});

function examinationAddButtonClicked(groupId) {

    let modalFooter = document.getElementById('editupdatemodalfooter');
    modalFooter.style.display = 'block';
    var dropdown = document.getElementById("AcademicExamGroupId");

    if (dropdown) {
        dropdown.value = groupId; // set selected value
    }
    dropdown.dispatchEvent(new Event('change'));
    let examAddBtn = document.getElementById('examAddBtn');
    examAddBtn.style.display = 'block';

    let modalUpdateBtn = document.getElementById('updateFormSubmitBtn');
    modalUpdateBtn.style.display = 'none';
}
function showLoader() {
    document.getElementById('loading').classList.remove('d-none');
}
function hideLoader() {
    document.getElementById('loading').classList.add('d-none');
}
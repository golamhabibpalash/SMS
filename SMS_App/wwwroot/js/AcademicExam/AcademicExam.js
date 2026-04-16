
$(document).ready(function () {
// Select2 initialization will be handled by the global initialization in _HeadPartial.cshtml
// This ensures consistent configuration across all select2 elements

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
            var $sel = $('#AcademicSectionId');
            $sel.empty();

            if (data && data.length > 0) {
                // Don't add an empty "All Section" option for multiselect
                $.each(data, function (i, obj) {
                    var op = '<option value="' + obj.id + '">' + obj.name + '</option>';
                    $sel.append(op);
                });
            } else {
                var o = '<option disabled selected>Section Not Found</option>';
                $sel.append(o);
            }

            // Ensure multiple attribute is present
            $sel.prop('multiple', true);

            // Fully destroy any previous Select2 instance and its container
            try {
                if ($sel.data('select2')) {
                    $sel.select2('destroy');
                    $sel.next('.select2-container').remove();
                }
            } catch (ex) {
                console.warn('Error destroying select2:', ex);
            }

            // Use modal .modal-content as dropdown parent for better positioning
            var dropdownParent = $('#createUpdateModal .modal-content');

            // Reinitialize Select2 after populating options
            $sel.select2({
                theme: 'bootstrap-5',
                placeholder: 'Select Section(s)',
                allowClear: true,
                width: '100%',
                dropdownParent: dropdownParent.length ? dropdownParent : null,
                multiple: true
            });

            // Trigger update
            $sel.trigger('change.select2');
            console.log('AcademicSectionId reinitialized with', $sel.find('option').length, 'options');
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


async function ExamAddBtnClick() {
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

        // Get all selected sections from Select2
        let selectedSectionIds = $('#AcademicSectionId').val() || [];
        if (!Array.isArray(selectedSectionIds)) {
            selectedSectionIds = [selectedSectionIds];
        }

        if (selectedSectionIds.length === 0) {
            alertify.error('Please select at least one section.');
            return;
        }

        let teacherIdElement = document.getElementById('EmployeeId');
        let teacherIdOption = teacherIdElement.options[teacherIdElement.selectedIndex];
        let teacherId = teacherIdOption.value;
        let teacherIdText = teacherIdOption.text;

        let tableBody = document.getElementById('tableBodyId');
        var indexCount = $("#detailsTable > tbody").children().length;

        // Validate duplicate against existing rows in table
        let existingRows = $("#detailsTable > tbody tr");
        let duplicatesInTable = [];
        
        selectedSectionIds.forEach((sectionId, idx) => {
            let isDuplicate = false;
            existingRows.each(function() {
                let row = $(this);
                let rowGroupId = row.find('input[name$=".AcademicExamGroupId"]').val();
                let rowClassId = row.find('input[name$=".AcademicClassId"]').val();
                let rowSubjectId = row.find('input[name$=".AcademicSubjectId"]').val();
                let rowSectionId = row.find('input[name$=".AcademicSectionId"]').val();
                let rowCategory = row.find('input[name$=".ExamCategory"]').val();
                
                if (rowGroupId == groupId && rowClassId == classId && rowSubjectId == subjectid && rowSectionId == sectionId && rowCategory == examCategory.value) {
                    isDuplicate = true;
                    return false;
                }
            });
            
            if (isDuplicate) {
                duplicatesInTable.push(sectionId);
            }
        });

        if (duplicatesInTable.length > 0) {
            alertify.error('Duplicate entry exists in pending table for selected section(s).');
            return;
        }

        // Server-side validation for duplicates in database
        try {
            const response = await $.ajax({
                url: '/AcademicExams/CheckDuplicates',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({
                    ExamGroupId: parseInt(groupId),
                    ClassId: parseInt(classId),
                    SubjectId: parseInt(subjectid),
                    ExamCategory: examCategory.value,
                    SectionIds: selectedSectionIds.map(id => parseInt(id))
                })
            });

            if (response.results) {
                const duplicates = response.results.filter(r => r.isDuplicate);
                if (duplicates.length > 0) {
                    const dupSectionNames = duplicates.map(d => {
                        const sectionOpt = Array.from(document.getElementById('AcademicSectionId').options).find(opt => opt.value === d.sectionId.toString());
                        return sectionOpt ? sectionOpt.text : 'Section ' + d.sectionId;
                    });
                    alertify.error('Duplicate exam(s) already exist for: ' + dupSectionNames.join(', '));
                    return;
                }
            } else if (response.isDuplicate) {
                alertify.error('Duplicate exam already exists in database for this combination.');
                return;
            }
        } catch (xhr) {
            console.error('Error checking duplicates:', xhr);
            // Continue without blocking if API fails
        }

        // Add a row for each selected section
        selectedSectionIds.forEach((sectionId, idx) => {
            let sectionIdElement = document.getElementById('AcademicSectionId');
            let sectionIdOption = Array.from(sectionIdElement.options).find(opt => opt.value === sectionId);
            let sectionIdText = sectionIdOption ? sectionIdOption.text : 'All Section';

            let rowCount = indexCount + idx + 1;

            let serial_td = '<td><input type="hidden" name="AcademicExams[' + (indexCount + idx) + '].AcademicExamGroupId" value="' + groupId + '" />' + rowCount + '</td>'
            let subject_td = '<td>  <input type="hidden" name="AcademicExams[' + (indexCount + idx) + '].AcademicSubjectId" value="' + subjectid + '" />' + subjectIdText + '</td>';
            let examCategory_td = '<td><input type="hidden" name="AcademicExams[' + (indexCount + idx) + '].ExamCategory" value="' + examCategory.value + '" /> ' + examCategory.value + '</td>';
            let class_td = '<td> <input type="hidden" name="AcademicExams[' + (indexCount + idx) + '].AcademicClassId" value="' + classId + '" />' + classIdText + '</td>';
            let section_td = '<td> <input type="hidden" name="AcademicExams[' + (indexCount + idx) + '].AcademicSectionId" value="' + sectionId + '" />' + sectionIdText + '</td>';
            let teacher_td = '<td> <input type="hidden" name="AcademicExams[' + (indexCount + idx) + '].EmployeeId" value="' + teacherId + '" />' + teacherIdText + '</td>';
            let marks_td = '<td class="text-end"> <input type="hidden" name="AcademicExams[' + (indexCount + idx) + '].TotalMarks" value="' + marks + '" />' + marks + '</td>';
            let action_td = '<td><button onclick="removeRow(this)" class="removeBtn btn btn-sm btn-warning" value="Remove">Remove</button></td>';
            let tr = '<tr>' + serial_td + subject_td + examCategory_td + class_td + section_td + teacher_td + marks_td + action_td + '</tr>';
            tableBody.innerHTML += tr;
        });
        
        alertify.success('Exam(s) added successfully.');
    }
}

$('#submitBtn').click(function () {
    showLoader();
    $('#submitForm').off('submit').on('submit', function (e) {
        e.preventDefault();
        
        // Submit the form
        $.ajax({
            type: 'POST',
            url: $(this).attr('action') || '/AcademicExams/Create',
            data: $(this).serialize(),
            success: function (response) {
                $('#createUpdateModal').modal('hide');
                document.getElementById("loading").style.display = "block";
                
                // Clear the table
                $("#detailsTable > tbody").empty();
                
                // Reset the form
                $('#submitForm')[0].reset();
                $('#AcademicSectionId').val([]).trigger('change.select2');
                
                setTimeout(function () {
                    hideLoader();
                    document.getElementById("loading").style.display = "none";
                    location.reload();
                }, 1500);
            },
            error: function (xhr, status, error) {
                hideLoader();
                document.getElementById("loading").style.display = "none";
                alertify.error('Error submitting form. Please try again.');
            }
        });
    });
    
    $(this).closest('form').submit();
});
function DeleteExam(id) {
    var result = confirm("Are you sure you want to proceed to delete?");
    if (result) {
        $.ajax({
            url: '/AcademicExams/Delete?Id=' + id,
            method: 'Post',
            success: function (response) {
                if (response && response.success) {
                    var row = $('button[data-id="' + id + '"]').closest('tr');
                    if (row.length) {
                        row.fadeOut(300, function() {
                            $(this).remove();
                        });
                    }
                    alertify.success(response.message);
                } else {
                    alertify.error(response ? response.message : "Failed to delete");
                }
            },
            error: function () {
                alertify.error("Error occurred while deleting.");
            }
        });
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
    
    // For Select2, we need to set value differently for multiple selects
    if (sectionId) {
        $('#AcademicSectionId').val([sectionId]).trigger('change.select2');
    }

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
    
    // Reset form fields
    document.getElementById('AcademicSubjectId').value = '';
    document.getElementById('TotalMarks').value = '';
    document.getElementById('EmployeeId').value = '';
    document.getElementById('ExamCategory').value = '';
    document.getElementById('Id').value = '';
    
    // Reset Select2 value (for multiple select)
    $('#AcademicSectionId').val([]).trigger('change.select2');
    
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
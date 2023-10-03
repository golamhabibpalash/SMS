﻿/////////////////////// 2nd time(Current) created code


$('#modalAcademicClassId').change(function () {
    let id = $('#modalAcademicClassId option:selected').val();

    $.ajax({
        url: "/api/Students/getbyclasswithsessionId?academicClassId=" + id + "&academicSessionId=" + null,
        dataType: "JSON",
        type: "POST",
        cache: false,
        success: function (data) {
            $('#studentListId').empty();

            if (data != null || data != '') {
                var o = '<option disabled selected>Select Student</option>';

                $('#studentListId').append(o);

                $.each(data, function (i, obj) {
                    console.log(obj.name);
                    var op = '<option value="' + obj.classRoll + '">' + obj.classRoll + " - " + obj.name + '</option>';
                    $('#studentListId').append(op);
                });
            }
            else {
                var o = '<option disabled selected>Section Not Found</option>';
                $('#studentListId').append(o);
            }
        },
        error: function (err) {
            console.log(err);
        }
    });
});
$('#studentListId').change(function () {
    let cRoll = $('#studentListId option:selected').val();
    $('#rollInput').val(cRoll);
    $('#frmSearchByRollId').submit();
});
//Payment Details Option Changed
$('#StudentPayment_StudentPaymentDetails_0__PaidAmount').on('input', function () {
    var paidAmount = $(this).val();
    $('#StudentPayment_TotalPayment').val(paidAmount);

    $.ajax({
        url: '/StudentPayments/GetTextByAmount',
        data: { amount: paidAmount },
        cache: false,
        type: 'POST',
        dataType: 'json',
        success: function (d) {
            $('#amountText').html(d);
            $('#totalAmountText').html(d);
        },
        error: function (err) {
            console.log(err);
        }
    });

}); $('#StudentPayment_TotalPayment').change('input', function () {
    var paidAmount = $(this).val();
    $('#StudentPayment_TotalPayment').val(paidAmount);

    $.ajax({
        url: '/StudentPayments/GetTextByAmount',
        data: { amount: paidAmount },
        cache: false,
        type: 'POST',
        dataType: 'json',
        success: function (d) {
            $('#amountText').html(d);
            $('#totalAmountText').html(d);
        },
        error: function (err) {
            console.log(err);
        }
    });

});
//Payment for select list option chooose
$('#StudentPayment_StudentPaymentDetails_0__StudentFeeHeadId').change(function () {
    let id = $('#StudentPayment_StudentPaymentDetails_0__StudentFeeHeadId option:selected').val();
    let classId = $('#academicClassId').val();
    $.ajax({
        url: '/StudentFeeHeads/GetById',
        data: { id: id, classId: classId },
        cache: false,
        type: 'POST',
        dataType: 'json',
        success: function (d) {

            $('#StudentPayment_StudentPaymentDetails_0__PaidAmount').val('');
            $('#StudentPayment_TotalPayment').val('');
            let pAmount = d.amount;
            //d.studentFeeHead.repeatedly == true ? $('#howManyTimes').show() : $('#howManyTimes').hide();
            $('#StudentPayment_StudentPaymentDetails_0__PaidAmount').val(pAmount);
            $('#StudentPayment_TotalPayment').val(pAmount);

            $.ajax({
                url: '/StudentPayments/GetTextByAmount',
                data: { amount: pAmount },
                cache: false,
                type: 'POST',
                dataType: 'json',
                success: function (d) {
                    $('#amountText').html(d);
                    $('#totalAmountText').html(d);
                },
                error: function (err) {
                    console.log(err);
                }
            });
        },
        error: function (err) {
            console.log(err);
        }
    });

});

//Payment for select list option chooose
jQuery('#howManyTimes').change(function () {
    let id = $('#StudentPayment_StudentPaymentDetails_0__StudentFeeHeadId option:selected').val();
    let howManyTimes = $('#howManyTimes option:selected').val();
    let classId = $('#academicClassId').val();
    $.ajax({
        url: '/StudentFeeHeads/GetById',
        data: { id: id, classId: classId },
        cache: false,
        type: 'POST',
        dataType: 'json',
        success: function (d) {
            let pAmount = d.amount;
            let times = $('#howManyTimes option:selected').val();
            let total = GetTotalPayment(pAmount, times);
            $('#StudentPayment_TotalPayment').val(total);
        },
        error: function (err) {
            console.log(err);
        }

    });
});

//Minus Button Click
jQuery('#minusButton').click(function () {
    let id = $('#feeSelectId').val();
    let classId = $('#academicClassId').val();
    var existAmount = $('#FeeAmountId').val();
    $.ajax({
        url: '/StudentFeeHeads/GetById',
        data: { id: id, classId: classId },
        cache: false,
        type: 'POST',
        dataType: 'json',
        success: function (data) {
            existAmount = parseInt(existAmount) - parseInt(data.amount);
            $('#FeeAmountId').val(existAmount);
        },
        error: function (err) {
            console.log(err);
        }

    });
});
//Plus Button Click
jQuery('#plusButton').click(function () {
    let id = $('#feeSelectId').val();
    let classId = $('#academicClassId').val();
    var existAmount = $('#FeeAmountId').val();
    $.ajax({
        url: '/StudentFeeHeads/GetById',
        data: { id: id, classId: classId },
        cache: false,
        type: 'POST',
        dataType: 'json',
        success: function (data) {
            existAmount = parseInt(existAmount) + parseInt(data.amount);
            $('#FeeAmountId').val(existAmount);            
        },
        error: function (err) {
            console.log(err);
        }

    });

    
});
/////////////////////// 1st time(Previous) created code
//Code for waiverFor div show and hide
jQuery('#waiverCheckId').click(function () {
    let isChecked = $('#waiverCheckId').is(':checked');
    if (isChecked == true) {
        $('#waiverDivId').show();
        $('#StudentPayment_WaiverAmount').attr('disabled', false);
    }
    else {
        $('#waiverDivId').hide();
        $('#StudentPayment_WaiverAmount').attr('disabled', true);
    }
});

//Code for Attachement Enable or Disable
jQuery('#attachmentCheckId').click(function () {
    let isChecked = $('#attachmentCheckId').is(':checked');
    if (isChecked == true) {
        $('#docAttachId').attr('disabled', false);
    }
    else {
        $('#docAttachId').attr('disabled', true);
    }

});

//Code for Fee Head Select Chekbox
jQuery('.feeHeadIdCheckbox').change(function () {
    let isChecked = $(this).is(':checked');
    if (isChecked) {
        $(this).next().attr('disabled', false);
    }
    else {
        $(this).next().attr('disabled', true);
    }
});

//Code for Date formation
let mkdate = function (dateObject) {
    var d = new Date(dateObject);
    var day = d.getDate();
    var monthName = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var month = d.getMonth() + 1;
    var year = d.getFullYear();
    if (day < 10) {
        day = "0" + day;
    }
    //if (month < 10) {
    //    month = "0" + month;
    //}
    var date = day + " " + monthName[month] + " " + year;
    return date;
};

//Modal Code for Payment Slip
jQuery('.modalViewId').click(function () {
    
    let receiptNo = $(this).data('receipt');
    let getData = $(this).data('paiddate');
    let myDate = mkdate(getData);
    let academicClass = $(this).data('academicclass');
    let classRoll = $(this).data('classroll');
    let studentName = $(this).data('studentname');
    let academicSession = $(this).data('academicsession');
    let totalAmount = $(this).data('amount');
    let paymentType = $(this).data('paymenttype');
    let amountInWord = inWords($(this).data('amount'));

    $('#rctReciptNo').html(receiptNo);
    $('#rctPaymentDate').html(myDate);
    $('#rctAcademicClass').html(academicClass);
    $('#rctClassRoll').html(classRoll);
    $('#rctStudentName').html(studentName);
    $('#rctAcademicSession').html(academicSession);
    $('#rctAmount').html(totalAmount+'Tk');
    $('#rctPaymentType').html(paymentType);
    $('#rctAmountInWords').html('Taka ' + amountInWord + 'Only');
});

//function GetTotalPayment(amount, howTimes) {
//    var total = amount * howTimes;
//    return total;
//}

var a = ['', 'one ', 'two ', 'three ', 'four ', 'five ', 'six ', 'seven ', 'eight ', 'nine ', 'ten ', 'eleven ', 'twelve ', 'thirteen ', 'fourteen ', 'fifteen ', 'sixteen ', 'seventeen ', 'eighteen ', 'nineteen '];
var b = ['', '', 'twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];

function inWords(num) {
    if ((num = num.toString()).length > 9) return 'overflow';
    n = ('000000000' + num).substr(-9).match(/^(\d{2})(\d{2})(\d{2})(\d{1})(\d{2})$/);
    if (!n) return; var str = '';
    str += (n[1] != 0) ? (a[Number(n[1])] || b[n[1][0]] + ' ' + a[n[1][1]]) + 'crore ' : '';
    str += (n[2] != 0) ? (a[Number(n[2])] || b[n[2][0]] + ' ' + a[n[2][1]]) + 'lakh ' : '';
    str += (n[3] != 0) ? (a[Number(n[3])] || b[n[3][0]] + ' ' + a[n[3][1]]) + 'thousand ' : '';
    str += (n[4] != 0) ? (a[Number(n[4])] || b[n[4][0]] + ' ' + a[n[4][1]]) + 'hundred ' : '';
    str += (n[5] != 0) ? ((str != '') ? 'and ' : '') + (a[Number(n[5])] || b[n[5][0]] + ' ' + a[n[5][1]]) + 'only ' : '';
    return str;
}

jQuery(function () {

    $('#btnget').click(function () {
        alert($('#chkveg').val());
    });
});

jQuery('.editBtn').click(function () {
    alert($(this).data('id'));
    let paymentDetailId = $(this).data('id');
    $.ajax({
        url: '/StudentPaymentDetails/GetPaymentDetailById',
        type: 'GET',
        dataType: 'json',
        data: { paymentDetailId: paymentDetailId },
        success: function (data) {
            console.log(data);
            $('#StudentPayment_ReceiptNo').val(data.studentPayment.receiptNo);
            $('#StudentPayment_PaidDate').val(data.studentPayment.paidDate.substring(0, 10));
            $('#StudentPayment_StudentPaymentDetails_0__StudentFeeHeadId').val(data.studentFeeHeadId);
            $('#StudentPayment_TotalPayment').val(data.studentPayment.totalPayment);
            $('#StudentPayment_Remarks').val(data.studentPayment.remarks);
            $('#submitBtn').val('Update');

        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log(textStatus + ': ' + errorThrown);
        }
    });
});

$('.modalViewId').click(function () {
    let paymentId = $(this).data('paymentid');
    let reportType = "pdf";
    let myfileName = null;
    $('#myIframe').attr('src', '/Reports/ReceiptPaymentExport?reportType=' + reportType + '&paymentId=' + paymentId + '&fileName=' + myfileName);
    var iframe = document.getElementById('myIframe');
    iframe.onload = function () {
        $('#loading').hide();
        $('#exportOptions').show();
        $('#paymentIdExport').val(paymentId);
    };

});

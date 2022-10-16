/////////////////////// 2nd time(Current) created code
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
            let pAmount = d.amount;
            d.studentFeeHead.repeatedly == true ? $('#howManyTimes').show() : $('#howManyTimes').hide();
            $('#amountId').val(d.amount);
            let times = $('#howManyTimes option:selected').val();
            let total = GetTotalPayment(pAmount, times);
            $('#StudentPayment_TotalPayment').val(total);
        },
        error: function (err) {
            console.log(err);
        }
        
    });
});

//Payment for select list option chooose
$('#howManyTimes').change(function () {
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
$('#minusButton').click(function () {
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
$('#plusButton').click(function () {
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
$('#waiverCheckId').click(function () {
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
$('#attachmentCheckId').click(function () {
    let isChecked = $('#attachmentCheckId').is(':checked');
    if (isChecked == true) {
        $('#docAttachId').attr('disabled', false);
    }
    else {
        $('#docAttachId').attr('disabled', true);
    }

});

//Code for Fee Head Select Chekbox
$('.feeHeadIdCheckbox').change(function () {
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
    if (month < 10) {
        month = "0" + month;
    }
    var date = day + " " + monthName[month] + " " + year;
    return date;
};

//Modal Code for Payment Slip
$('#modalViewId').click(function () {
    let receiptNo = $(this).data('receipt');
    let getData = $(this).data('paidDate');
    let myDate = mkdate(getData);
    $('#receiptNo').html(receiptNo);
    $('#paidDateId').html(myDate);
});

function GetTotalPayment(amount, howTimes) {
    var total = amount * howTimes;
    return total;
}
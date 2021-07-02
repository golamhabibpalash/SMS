
$(document).ready(function () {

    if (created != "") {
        alertify.success(created);
        }

    if (failed != "") {
        alertify.warning(failed);
        }

    if (deleted != "") {
        alertify.error(deleted);
        }

    if (updated != "") {
        alertify.message(updated);
        }
    });

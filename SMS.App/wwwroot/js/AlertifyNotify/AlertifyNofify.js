﻿
$(document).ready(function () {
    try {
        if ( created != "") {
            alertify.success(created);
            }

        if (failed != "") {
            alertify.updated(failed);
            }

        if (deleted != "") {
            alertify.error(deleted);
            }

        if (updated != "") {
            alertify.message(updated);
            }
    } catch (e) {

    }
    });


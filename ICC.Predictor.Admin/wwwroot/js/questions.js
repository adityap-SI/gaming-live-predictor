var origin_url = window.location.origin
var origin_url = window.location.origin
let baseUrl = ""

$(document).ready(function () {
    var url = window.location.href;

    let find = origin_url.search("localhost");
    if (find >= 0) {
        baseUrl = origin_url;
    } else {
        baseUrl = origin_url + "/admin";
    }

    $('#sendNotification').click(function (e) {
        if (e.originalEvent.defaultPrevented) {
            return false;
        }
        e.preventDefault();
        window.location.href = $(this).attr('href').replace("text", $('#NotificationText').val());
    });

    $('#abandonMatch').click(function (e) {
        if (e.originalEvent.defaultPrevented) {
            return false;
        }
        e.preventDefault();
        window.location.href = $(this).attr('href').replace("AbandonMatchId", $('#AbandonedMatchId').val());
    });

    //var questionstatus = url.split('?')[1].split('&')[1].split('=')[1];
    var questionstatus = url.split('?')[1].split('&')[2].split('=')[1];
    var matchId = url.split('?')[1].split('&')[0].split('=')[1];
    $("#questions, #unresolved, #resolved,#notification,#pointscalculation,#allquestion").removeClass("SelectedTab");
    if (questionstatus == 0) {
        $("#questions").addClass("SelectedTab");
        $(document).prop('title', 'ICC | UnPublished');
    }
    else if (questionstatus == 1 || questionstatus == 2) {
        $("#unresolved").addClass("SelectedTab");
        $(document).prop('title', 'ICC | Unresolved');
    }
    else if (questionstatus == 3) {
        $("#resolved").addClass("SelectedTab");
        $(document).prop('title', 'ICC | Resolved');
    }
    else if (questionstatus == -3) {
        $("#notification").addClass("SelectedTab");
        $(document).prop('title', 'ICC | Notification');
    }
    else if (questionstatus == -4) {
        $("#pointscalculation").addClass("SelectedTab");
        $(document).prop('title', 'ICC | Points Calculation');
    }
    else if (questionstatus == -2) {
        $("#allquestion").addClass("SelectedTab");
        $(document).prop('title', 'ICC | All Questions');
    }

})
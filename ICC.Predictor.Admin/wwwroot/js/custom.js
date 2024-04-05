
/*=============================================================
    Authour URI: www.binarytheme.com
    License: Commons Attribution 3.0

    http://creativecommons.org/licenses/by/3.0/

    100% Free To use For Personal And Commercial Use.
    IN EXCHANGE JUST GIVE US CREDITS AND TELL YOUR FRIENDS ABOUT US
   
    ========================================================  */

(function ($) {
    "use strict";
    var mainApp = {
        slide_fun: function () {

            $('#carousel-example').carousel({
                interval: 3000 // THIS TIME IS IN MILLI SECONDS
            })

        },
        dataTable_fun: function () {

            //$('#dataTables-example').dataTable();

        },

        custom_fun: function () {
            /*====================================
             WRITE YOUR   SCRIPTS  BELOW
            ======================================*/




        },

        HighlightNavigation: function () {

            $("#menu-top").removeClass("menu-top-active");
            var href = window.location.href;
            if (href.toLowerCase().indexOf("feedingestion") > -1)
                $("#aFeedIngestion").addClass("menu-top-active");
            else if (href.toLowerCase().indexOf("datapopulation") > -1)
                $("#aDataPopulation").addClass("menu-top-active");
            else if (href.toLowerCase().indexOf("analytics") > -1)
                $("#aAnalytics").addClass("menu-top-active");
            else if (href.toLowerCase().indexOf("adminleaderboard") > -1)
                $("#aAdminLeaderBoard").addClass("menu-top-active");
            else if (href.toLowerCase().indexOf("matchschedule") > -1)
                $("#aMatchSchedule").addClass("menu-top-active");
            else if (href.toLowerCase().indexOf("questions") > -1)
                $("#aQuestions").addClass("menu-top-active");
        }
    }


    $(document).ready(function () {
        mainApp.slide_fun();
        mainApp.dataTable_fun();
        mainApp.custom_fun();
        mainApp.HighlightNavigation();
    });
}(jQuery));



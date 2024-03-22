var optionsConditionsFilled = false;
var optionsConditionsAlreadyFilled = false;
var questionsConditionsFilled = false;
var questionsConditionsAlreadyFilled = false;
var triviaConditionsFilled = false;
var triviaConditionsAlreadyFilled = false;
//var singleCheckboxCondition = false;
//var singleCheckboxConditionAlready = false;

function ButtonSubmit() {
    if ((optionsConditionsFilled || optionsConditionsAlreadyFilled) && (questionsConditionsFilled || questionsConditionsAlreadyFilled) && (triviaConditionsFilled || triviaConditionsAlreadyFilled)) {
        $('#savebtn').prop('disabled', false);
    }
    else {
        $('#savebtn').prop('disabled', true);
    }
}

$(document).ready(function () {
    //Check if Options are already checked for trivia
    if ($('#QuestionType').val() == "QS_TRIVIA" && ($('input[type="checkbox"]:checked').length <= 0)) {
        //$('#savebtn').prop('disabled', true);
        triviaConditionsAlreadyFilled = false;
        ButtonSubmit()
    }
    else {
        triviaConditionsAlreadyFilled = true;
        ButtonSubmit()
    }

    //if ($('#QuestionType').val() == "QS_PRED" && ($('input[type="checkbox"]:checked').length > 1)) {
    //    //$('#savebtn').prop('disabled', true);
    //    singleCheckboxConditionAlready = false;
    //    ButtonSubmit()
    //}
    //else {
    //    singleCheckboxConditionAlready = true;
    //    ButtonSubmit()
    //}

    //Check if Options are already checked for trivia

    //Check if Options are checked for trivia while filling
    $('input[type="checkbox"]').change(function () {
        if ($('#QuestionType').val() == "QS_TRIVIA") {
            if ($('input[type="checkbox"]:checked').length > 0 /* = 1*/) {
                //$('#savebtn').prop('disabled', false);
                triviaConditionsFilled = true;
                ButtonSubmit()
            }
            else {
                //$('#savebtn').prop('disabled', true);
                triviaConditionsFilled = false;
                triviaConditionsAlreadyFilled = false;
                ButtonSubmit()
            }
        }

        //if ($('#QuestionType').val() == "QS_PRED") {
        //    if ($('input[type="checkbox"]:checked').length > 1) {
        //        singleCheckboxCondition = false;
        //    }
        //    else {
        //        singleCheckboxCondition = true;
        //    }
        //}

    });
    
    $('input[type="checkbox"]').click(function () {
        $('input[type="checkbox"]').not(this).prop('checked', false);
    });
    //Check if Options are checked for trivia while filling

    //Check if Options are already filled
    var inputfield = 0;
    $('input[type="text"]').each(function () {
        if ($(this).val() != "") {
            inputfield++;
        }
    });
    if (inputfield > 1) {
        //$('#savebtn').prop('disabled', false);
        optionsConditionsAlreadyFilled = true;
        ButtonSubmit()
    }
    else {
        //$('#savebtn').prop('disabled', true);
        optionsConditionsAlreadyFilled = false;
        ButtonSubmit()
    }
    //Check if Options are already filled

    //Check if Options are filled while filling
    $('input[type="text"]').change(function () {
        var textfield = 0;
        $('input[type="text"]').each(function () {
            if ($(this).val() != "") {
                textfield++;
            }
        });
        if (textfield > 1) {
            //$('#savebtn').prop('disabled', false);
            optionsConditionsFilled = true;
            ButtonSubmit()
        }
        else {
            //$('#savebtn').prop('disabled', true);
            optionsConditionsFilled = false;
            optionsConditionsAlreadyFilled = false;
            ButtonSubmit()
        }

        //Enabling and disabling checkbox based on filling textbox
        var textNo = $(this).attr('Id').split('_')[1];
        if ($(this).val() != "") {
            $('#Options_' + textNo + '__IsCorrectBool').prop('disabled', false);
        }
        else {
            $('#Options_' + textNo + '__IsCorrectBool').prop('disabled', true);
            $('#Options_' + textNo + '__IsCorrectBool').prop('checked', false);
        }
        //Enabling and disabling checkbox based on filling textbox
    });
    //Check if Options are filled while filling

    //Check if Question is already filled
    if ($('#QuestionDesc').val() != "") {
        //$('#savebtn').prop('disabled', false);
        questionsConditionsAlreadyFilled = true;
        ButtonSubmit()
    }
    else {
        //$('#savebtn').prop('disabled', true);
        questionsConditionsAlreadyFilled = true;
        ButtonSubmit()
    }
    //Check if Question is already filled

    //Check if Question is already while filling
    $('#QuestionDesc').change(function () {
        if ($('#QuestionDesc').val() != "") {
            //$('#savebtn').prop('disabled', false);
            questionsConditionsFilled = true;
            ButtonSubmit()
        }
        else {
            //$('#savebtn').prop('disabled', true);
            questionsConditionsFilled = false;
            ButtonSubmit()
        }
    });
    //Check if Question is already while filling

    //loop to check checkbox to be disabled.
    $('input[type="text"]').each(function () {
        var textNo = $(this).attr('Id').split('_')[1];
        if ($(this).val() != "") {
            $('#Options_' + textNo + '__IsCorrectBool').prop('disabled', false);
        }
        else {
            $('#Options_' + textNo + '__IsCorrectBool').prop('disabled', true);
        }
    });
    //loop to check checkbox to be disabled.

    ButtonSubmit();
})
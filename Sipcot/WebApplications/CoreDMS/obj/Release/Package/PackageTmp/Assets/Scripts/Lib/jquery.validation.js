$().ready(function() {
    // validate the comment form when it is submitted
    $("#commentForm").validate();
    var addRules = '';
    //validate mini registration form
    $("#aspnetForm").validate({
        onsubmit: false,
        rules: {
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucRegistration$txtCode: {
                required: true,
                remoteTxtPromoCaptchaT5: {
                    type: "post",
                    url: "/MicroRegistration.aspx/ValidateCode"
                }
            },
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucRegistration$ddlCountry: {
                Countryrequired: true
            },
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucRegistration$ddlCompanyType: {
                Companyrequired: true
            },
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucRegistration$Txtemail: {
                required: true,
                email: true,
                remote: {
                    type: "post",
                    url: "/MicroRegistration.aspx/CheckForDuplicateEmail1"
                }
            },
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucRegistration$txtPassword: {
                required: true,
                minlength: 6,
                maxlength: 50
            },
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucRegistration$txtConfirmPassword: {
                required: true,
                minlength: 6,
                maxlength: 50,
                equalTo: "#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_txtPassword"
            },
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucRegistration$chkTermsAndCondition: "required",
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucLogin$txtbxUsername: "required",
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucLogin$txtbxPassword: "required",
            ctl00$ctl00$PageBody$ProfileMainContent$ctl01$txtFeedbackComments:"required"            
        },
        messages: {
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucRegistration$Txtemail: {
                required: "We need your email address to contact you.",
                email: "Please enter a valid email address.",
                remote: "This E-mail address is already registered with Toboc. Please use another."

            },
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucRegistration$txtPassword: {
                required: "Please provide a password.",
                minlength: "Your password must be at least 6 characters long."
            },
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucRegistration$txtConfirmPassword: {
                required: "Please provide a password.",
                minlength: "Your password must be at least 6 characters long.",
                equalTo: "Enter password should be same as above."
            },
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucRegistration$txtCode: {
                required: "This field is required.",
                remoteTxtPromoCaptchaT5: "Please enter a valid code."
            },
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucRegistration$chkTermsAndCondition: "Please accept our policy.",
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucLogin$txtbxUsername: "Please Enter User Name.",
            ctl00$ctl00$PageBody$ProfileMainContent$Authentication$ucLogin$txtbxPassword: "Please Enter Password.",
            Companyrequired: "Please select your Company type.",
            Countryrequired: "Please select Country.",
            ctl00$ctl00$PageBody$ProfileMainContent$ctl01$txtFeedbackComments: "Please Enter the Description."
        }
        //        ,invalidHandler: addRules
        //        ,
        //        errorPlacement: function(error, element) {
        //            error.appendTo($("#dialog"));
        //        },
        //        showErrors: function(errorMap, errorList) {
        //            this.defaultShowErrors();
        //            $("#dialog").dialog('open');
        //        }
    });

    //    $("#dialog").dialog({
    //        autoOpen: false,
    //        modal: true,
    //        close: function(event, ui) {
    //            $("#dialog").html("");
    //        }
    //    });
    //addRules();
    $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_btnRegisterNewUser").click(function() {
        $(".T5MiniLogin").each(function() {
            $(this).rules("remove");
        });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_ddlCountry").rules("add", { Countryrequired: true });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_ddlCompanyType").rules("add", { Companyrequired: true });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_Txtemail").rules("add", { required: true });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_Txtemail").rules("add", { email: true });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_Txtemail").rules("add", { remote: { type: "post", url: "/MicroRegistration.aspx/CheckForDuplicateEmail1"} });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_txtPassword").rules("add", { required: true });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_txtPassword").rules("add", { minlength: 6 });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_txtPassword").rules("add", { maxlength: 50 });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_txtConfirmPassword").rules("add", { required: true });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_txtConfirmPassword").rules("add", { minlength: 6 });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_txtConfirmPassword").rules("add", { maxlength: 50 });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_txtConfirmPassword").rules("add", { equalTo: "#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_txtPassword" });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_txtCode").rules("add", { required: true });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_txtCode").rules("add", { remoteTxtPromoCaptchaT5: { type: "post", url: "/MicroRegistration.aspx/ValidateCode"} });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucRegistration_chkTermsAndCondition").rules("add", { required: true });
        return $("#aspnetForm").valid();
    });

    $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucLogin_btnSubmit").click(function() {
        $(".T5MiniRegistration").each(function() {
            $(this).rules("remove");
        });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucLogin_txtbxUsername").rules("add", { required: true });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_Authentication_ucLogin_txtbxPassword").rules("add", { required: true });
        return $("#aspnetForm").valid();
    });
    
    $("#ctl00_ctl00_PageBody_ProfileMainContent_ctl01_btnSubmit").click(function() {
    $(".T5MiniLogin").each(function() {
            $(this).rules("remove");
        });
        $(".T5MiniRegistration").each(function() {
            $(this).rules("remove");
        });
        $("#ctl00_ctl00_PageBody_ProfileMainContent_ctl01_txtFeedbackComments").rules("add", { required: true });
        return $("#aspnetForm").valid();
    });
    // validate signup form on keyup and submit
    $("#signupForm").validate({
        rules: {
            firstname: "required",
            lastname: "required",
            username: {
                required: true,
                minlength: 2
            },
            MicroRegistration1$Txtpwd: {
                required: true,
                minlength: 6,
                maxlength: 50
            },
            MicroRegistration1$TxtCpwd: {
                required: true,
                minlength: 6,
                maxlength: 50,
                equalTo: "#MicroRegistration1_Txtpwd"
            },
            MicroRegistration1$Txtemail: {
                required: true,
                email: true,
                remote: {
                    type: "post",
                    url: "MicroRegistration.aspx/CheckForDuplicateEmail1"
                }
            },
            MicroRegistration1$txtCode: {
                required: true,
                remoteTxtPromoCaptcha: {
                    type: "post",
                    url: "MicroRegistration.aspx/ValidateCode"
                }
            },
            MicroRegistration1$ddlCountry1: {
                Countryrequired: true
            },
            MicroRegistration1$ddlCompany1: {
                Companyrequired: true
            },

            topic: {
                required: "#newsletter:checked",
                minlength: 2
            },
            agree: "required",
            txtbxSubject: {
                required: true,
                maxlength: 100
            },
            txbBody: {
                required: true
            }

        },
        messages: {
            firstname: "Please enter your firstname.",
            lastname: "Please enter your lastname.",
            username: {
                required: "Please enter a username.",
                minlength: "Your username must consist of at least 2 characters."
            },
            MicroRegistration1$Txtpwd: {
                required: "Please provide a password.",
                minlength: "Your password must be at least 6 characters long."
            },
            MicroRegistration1$TxtCpwd: {
                required: "Please provide a password.",
                minlength: "Your password must be at least 6 characters long.",
                equalTo: "Enter password should be same as above."
            },
            MicroRegistration1$Txtemail: {
                required: "We need your email address to contact you.",
                email: "Please enter a valid email address.",
                remote: "This E-mail address is already registered with Toboc. Please use another."

            },
            agree: "Please accept our policy.",
            MicroRegistration1$txtCode: {
                required: "This field is required.",
                remoteTxtPromoCaptcha: "Please enter a valid code."
            },
            Companyrequired: "Please select your Company type.",
            Countryrequired: "Please select Country.",

            txtbxSubject: "Please enter message subject.",
            txbBody: "Please enter message body."
        }
    });

    // propose username by combining first- and lastname
    $("#username").focus(function() {
        var firstname = $("#firstname").val();
        var lastname = $("#lastname").val();
        if (firstname && lastname && !this.value) {
            this.value = firstname + "." + lastname;
        }
    });

    //code to hide topic selection, disable for demo
    var newsletter = $("#newsletter");
    // newsletter topics are optional, hide at first
    var inital = newsletter.is(":checked");
    var topics = $("#newsletter_topics")[inital ? "removeClass" : "addClass"]("gray");
    var topicInputs = topics.find("input").attr("disabled", !inital);
    // show when newsletter is checked
    newsletter.click(function() {
        topics[this.checked ? "removeClass" : "addClass"]("gray");
        topicInputs.attr("disabled", !this.checked);
    });
});

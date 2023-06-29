
var varType = null;
var varUrl = null;
var varData = null;
var varContentType = null;
var varDataType = null;
var varProcessData = null;
var browser = "";
//cmmon control Ids for session handline
var loginOrgIdControlID = null;
var loginTokenControlID = null;
var pageRightsContorlId = null;
// For Javascript invocation if the Data/Operation is valid
var btnSubmitControlID = null;
//GridData
var recordCountControlID = null;
var pageDisplayControlID = null;
var pagingTextControlID = null;


//FilterDate
var FromDateControlId = null;
var ToDateControlId = null

$(document).ajaxError(function (event, request, settings) {
    alert("<Error requesting page " + settings.url + ".");
});


//********************************************************
//PostScalar Function with Successs and Failur Rutine
//********************************************************

function CallPostScalar(msgControl, action, params) {
    //reset variables
    var browser = "";
    // Get organisation Id and GUID [session]
    var loginOrgId = document.getElementById(loginOrgIdControlID).value;
    var loginToken = document.getElementById(loginTokenControlID).value;
    if (loginOrgId == "") { loginOrgId = "0"; }
    //check browser
    if (jQuery.browser.mozilla == true) browser = "firefox"
    else if (jQuery.browser.msie == true) browser = "ie"

    varType = "POST";
    varUrl = coreServiceURL + "/PostScalar";
    //We are passing multiple paramers to the service in json format
    //varData = '{"action": "' + action + '","methodParams": "' + params + '"}';
    varData = '{"loginOrgId": "' + loginOrgId + '","loginToken": "' + loginToken + '","action": "' + action + '","methodParams": "' + params + '","browser": "' + browser + '"}';
    varContentType = "application/json; charset=utf-8";
    varDataType = "json";
    varProcessData = true;
    //CallService
    $.ajax({
        type: varType, //GET or POST or PUT or DELETE verb
        url: varUrl, // Location of the service
        data: varData, //Data sent to server
        contentType: varContentType, // content type sent to server
        dataType: varDataType, //Expected data format from server
        processdata: varProcessData, //True or False
        success: function (msg) {//On Successfull service call
            PostScalarSucceeded(msgControl, action, msg);
        },
        error: PostScalarServiceFailed// When Service call fails
    });
}
function PostScalarSucceeded(msgControl, action, result) {
    var resultObject = null;
    status = true;
    if (varDataType == "json") {
        // Constructed object name will be object.[ServiceName]Result
        resultObject = result.PostScalarResult;
    }
    if (resultObject == null) {
        $(msgControl).html("An error occurred, please try again. If problem exists please contact Administrator");
    }
    else {
        if (resultObject.ActionStatus == "SUCCESS" || resultObject.ActionStatus == "EXPAIRED") {
            // To get the page permissions
            var editRight = false;
            var deleteRight = false;
            if (pageRightsContorlId != null || pageRightsContorlId != undefined) {
                var pageRights = document.getElementById(pageRightsContorlId).value;
                var rightArray = pageRights.split(",");
                $.each(rightArray, function (index, item) {
                    if (item == "Edit") {
                        editRight = true;
                    }
                    if (item == "Delete") {
                        deleteRight = true;
                    }
                });
            }
            //set the message control font color
            $(msgControl).css("color", "green");
            //reset the text of message control to empty string
            $(msgControl).html("");
            if (action == "ValidateUser") {
                document.getElementById(loginOrgIdControlID).value = resultObject.UserData.LoginOrgId;
                document.getElementById(loginTokenControlID).value = resultObject.UserData.LoginToken;
                document.getElementById(btnSubmitControlID).click();
            }
            else if (action == "ForgotPassword") {
                document.getElementById(loginOrgIdControlID).value = resultObject.UserData.LoginOrgId;
                document.getElementById(btnSubmitControlID).click();
                //$(msgControl).html("resultObject.Message");
                //GoToHomePage();
            }
            else if (action == "SearchOrgs" || action == "DeleteOrgAndSearch") {
                var table = '<table border= \"1\" class=\"box-table-a\"  summary=\"Customer Search Results\" ><tr>';
                if (editRight) {
                    table += '<th>Edit</th>';
                }
                if (deleteRight) {
                    table += '<th>Delete</th>';
                }
                table += '<th>Name</th>';
                table += '<th>Address</th>';
                table += '<th>Email</th>';
                table += '<th>Phone</th>';
                table += '<th>Fax</th>';
                table += '<th>CreatedDate</th></tr>';

                if (resultObject.Orgs == null) {
                    table += '<tr><td colspan=\"8\">&nbsp;' +
                    resultObject.RecordCountText + '</td></tr>';
                    $("#divRecordCountText").html("");
                    $("#divPagingText").html("");
                }
                else {
                    /* loop over each object in the array to create rows*/
                    $.each(resultObject.Orgs, function (index, item) {
                        /* add to html string started above*/
                        if (editRight) {
                            table += '<tr><td>&nbsp;' + '<a href=\"ManageOrg.aspx?action=edit&id=' + item.OrgId + '\" ><img border=\"0\" src = \"' + authority + '\\Assets\\Skin\\Images\\edit.gif\" alt=\"edit\" /> </a>';
                        }
                        if (deleteRight) {
                            if (item.OrgParentId == 0) {
                                table += '</td><td>&nbsp;';
                            }
                            else {
                                table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"DeleteOrg(\'' + item.OrgId + '\',\'' + item.OrgName + '\')\"><img border=\"0\" src = \"' + authority + '\\Assets\\Skin\\Images\\del.gif\" alt=\"delete\" /> </a>';
                            }
                        }
                        table += '</td><td>&nbsp;' + item.OrgName;
                        table += '</td><td>&nbsp;' + item.OrgAddress;
                        table += '</td><td>&nbsp;' + item.OrgEmailId;
                        table += '</td><td>&nbsp;' + item.PhoneNo;
                        table += '</td><td>&nbsp;' + item.FaxNo;
                        table += '</td><td>&nbsp;' + item.CreatedDate;
                        table += '</td></tr>';
                    });
                    $("#divRecordCountText").html(resultObject.RecordCountText);
                    $("#divPagingText").html(resultObject.PagingText);
                }
                table += '</table>';
                /* insert the html string*/
                $("#divSearchResults").html(table);
                $(msgControl).html(resultObject.Message);
            }
            else if (action == "SearchUsers" || action == "DeleteUserAndSearch") {
                var table = '<table border= \"1\" class=\"box-table-a\"  summary=\"User Search Results\" ><tr>';
                if (editRight) {
                    table += '<th>Edit</th>';
                }
                if (deleteRight) {
                    table += '<th>Delete</th>';
                }
                table += '<th>User</th>';
                table += '<th>First Name</th>';
                table += '<th>Last Name</th>';
                table += '<th>Email</th>';
                table += '<th>Mobile</th>';
                table += '<th>CreatedDate</th>'
                table += '<th>Active</th></tr>';
                if (resultObject.Users == null) {
                    table += '<tr><td colspan=\"9\">&nbsp;' +
                    resultObject.RecordCountText + '</td></tr>';
                    $("#divRecordCountText").html("");
                    $("#divPagingText").html("");
                }
                else {

                    /* loop over each object in the array to create rows*/
                    $.each(resultObject.Users, function (index, item) {
                        /* add to html string started above*/
                        if (editRight) {
                            table += '<tr><td>&nbsp;' + '<a href=\"ManageUser.aspx?action=edit&id=' + item.UserId + '\" ><img border=\"0\" src = \"' + authority + '\\Assets\\Skin\\Images\\edit.gif\" alt=\"edit\" /> </a>'
                        }
                        if (deleteRight) {
                            table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"DeleteUser(\'' + item.UserId + '\',\'' + item.UserName + '\')\"><img border=\"0\" src = \"' + authority + '\\Assets\\Skin\\Images\\del.gif\" alt=\"delete\" /> </a>'
                        }
                        table += '</td><td>&nbsp;' + item.UserName
                        table += '</td><td>&nbsp;' + item.FirstName
                        table += '</td><td>&nbsp;' + item.LastName
                        table += '</td><td>&nbsp;' + item.EmailId
                        table += '</td><td>&nbsp;' + item.MobileNo
                        table += '</td><td>&nbsp;' + item.CreatedDate
                        table += '</td><td>&nbsp;' + GetBooleanText(item.Active)
                        table += '</td></tr>';
                    });
                    $("#divRecordCountText").html(resultObject.RecordCountText);
                    $("#divPagingText").html(resultObject.PagingText);
                }
                table += '</table>';
                /* insert the html string*/
                $("#divSearchResults").html(table);
                $(msgControl).html(resultObject.Message);
            }
            else if (action == "SearchGroups" || action == "DeleteGroupAndSearch") {
                var table = '<table border= \"1\" class=\"box-table-a\"  summary=\"Group Search Results\" ><tr>';
                if (editRight) {
                    table += '<th>Edit</th>';
                }
                if (deleteRight) {
                    table += '<th>Delete</th>';
                }
                table += '<th>Group</th>';
                table += '<th>Description</th>';
                table += '<th>No of Users</th>';
                table += '<th>CreatedDate</th>';
                table += '<th>Active</th></tr>';
                if (resultObject.Groups == null) {
                    table += '<tr><td colspan=\"7\">&nbsp;' +
                    resultObject.RecordCountText + '</td></tr>';
                    $("#divRecordCountText").html("");
                    $("#divPagingText").html("");
                }
                else {

                    /* loop over each object in the array to create rows*/
                    $.each(resultObject.Groups, function (index, item) {
                        /* add to html string started above*/
                        if (item.Fixed) {
                            if (editRight) {
                                table += '<tr><td>&nbsp;' + '<a href=\"javascript:void(0)\"  onclick=\"alert(\'This group is not modifiable\');\" ><img border=\"0\" src = \"' + authority + '\\Assets\\Skin\\Images\\edit.gif\" alt=\"This group is not modifiable\" /> </a>';
                            }
                            if (deleteRight) {
                                table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"alert(\'This group is not modifiable\');\" ><img border=\"0\" src = \"' + authority + '\\Assets\\Skin\\Images\\del.gif\" alt=\"This group is not modifiable\" /> </a>';
                            }
                        }
                        else {
                            if (editRight) {
                                table += '<tr><td>&nbsp;' + '<a href=\"ManageGroup.aspx?action=edit&id=' + item.GroupId + '\" ><img border=\"0\" src = \"' + authority + '\\Assets\\Skin\\Images\\edit.gif\" alt=\"edit\" /> </a>';
                            }
                            if (deleteRight) {
                                table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"DeleteGroup(\'' + item.GroupId + '\',\'' + item.GroupName + '\')\"><img border=\"0\" src = \"' + authority + '\\Assets\\Skin\\Images\\del.gif\" alt=\"delete\" /> </a>';
                            }
                        }
                        table += '</td><td>&nbsp;' + item.GroupName;
                        table += '</td><td>&nbsp;' + item.Description;
                        table += '</td><td>&nbsp;' + item.UserCount;
                        table += '</td><td>&nbsp;' + item.CreatedDate
                        table += '</td><td>&nbsp;' + GetBooleanText(item.Active);
                        table += '</td></tr>';

                    });
                    $("#divRecordCountText").html(resultObject.RecordCountText);
                    $("#divPagingText").html(resultObject.PagingText);
                }
                table += '</table>';
                /* insert the html string*/
                $("#divSearchResults").html(table);
                $(msgControl).html(resultObject.Message);
            }
            else if (action == "SearchDocumentTypes" || action == "DeleteDocumentTypeAndSearch") {
                var table = '<table border= \"1\" class=\"box-table-a\"  summary=\"Group Search Results\" ><tr>';
                if (editRight) {
                    table += '<th>Edit</th>';
                }
                if (deleteRight) {
                    table += '<th>Delete</th>';
                }
                table += '<th>Name</th>';
                table += '<th>Description</th>';
                table += '<th>No of Groups</th>';
                table += '<th>CreatedDate</th>'
                table += '<th>Active</th></tr>';
                if (resultObject.DocumentTypes == null) {
                    table += '<tr><td colspan=\"7\">&nbsp;' +
                    resultObject.RecordCountText + '</td></tr>';
                    $("#divRecordCountText").html("");
                    $("#divPagingText").html("");
                }
                else {
                    /* loop over each object in the array to create rows*/
                    $.each(resultObject.DocumentTypes, function (index, item) {
                        /* add to html string started above*/
                        if (editRight) {
                            table += '<tr><td>&nbsp;' + '<a href=\"ManageDocumentType.aspx?action=edit&id=' + item.DocumentTypeId + '\" ><img border=\"0\" src = \"' + authority + '\\Assets\\Skin\\Images\\edit.gif\" alt=\"edit\" /> </a>'
                        }
                        if (deleteRight) {
                            table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"DeleteDocumentType(\'' + item.DocumentTypeId + '\',\'' + item.DocumentTypeName + '\')\"><img border=\"0\" src = \"' + authority + '\\Assets\\Skin\\Images\\del.gif\" alt=\"delete\" /> </a>'
                        }
                        table += '</td><td>&nbsp;' + item.DocumentTypeName
                        table += '</td><td>&nbsp;' + item.Description
                        table += '</td><td>&nbsp;' + item.GroupCount
                        table += '</td><td>&nbsp;' + item.CreatedDate
                        table += '</td><td>&nbsp;' + GetBooleanText(item.Active)
                        table += '</td></tr>';
                    });
                    $("#divRecordCountText").html(resultObject.RecordCountText);
                    $("#divPagingText").html(resultObject.PagingText);
                }
                table += '</table>';
                /* insert the html string*/
                $("#divSearchResults").html(table);
                $(msgControl).html(resultObject.Message);
            }
            else if (action == "SearchDocuments") {
                var table = '<table border= \"1\" class=\"box-table-a\"  summary=\"Document Search Results\" ><tr>';
                table += '<th>Edit</th>';
                table += '<th>FileName</th>';
                table += '<th>Version</th>'
                table += '<th>Type</th></tr>';
                if (resultObject.DocumentDownloads == null) {
                    table += '<tr><td colspan=\"7\">&nbsp;' +
                    resultObject.RecordCountText + '</td></tr>';
                    $("#divRecordCountText").html("");
                    $("#divPagingText").html("");
                }
                else {
                    /* loop over each object in the array to create rows*/
                    $.each(resultObject.DocumentDownloads, function (index, item) {
                        /* add to html string started above*/
                        table += '<tr><td>&nbsp;' + '<a href=\"DocumentDownloadDetails.aspx?id=' + item.ProcessID + '&docid=' + item.DocId + '&depid=' + item.DepID + '\" ><img border=\"0\" src = \"' + authority + '\\Assets\\Skin\\Images\\edit.gif\" alt=\"edit\" /> </a>'
                        table += '</td><td>&nbsp;' + item.FileName
                        table += '</td><td>&nbsp;' + item.Version
                        table += '</td><td>&nbsp;' + item.Type
                        table += '</td></tr>';
                    });
                    $("#divRecordCountText").html(resultObject.RecordCountText);
                    $("#divPagingText").html(resultObject.PagingText);
                }
                table += '</table>';
                /* insert the html string*/
                $("#divSearchResults").html(table);
                $(msgControl).html(resultObject.Message);
            }
            else if (action == "SearchTemplates" || action == "DeleteTemplateAndSearch") {
                var table = '<table border= \"1\" class=\"box-table-a\"  summary=\"Group Search Results\" ><tr>';
                if (editRight) {
                    table += '<th>Edit</th>';
                }
                if (deleteRight) {
                    table += '<th>Delete</th>';
                }
                table += '<th>Name</th>';
                table += '<th>No. of Fields</th>';
                table += '<th>CreatedDate</th>'
                table += '<th>Active</th></tr>';
                if (resultObject.Templates == null) {
                    table += '<tr><td colspan=\"7\">&nbsp;' +
                    resultObject.RecordCountText + '</td></tr>';
                    $("#divRecordCountText").html("");
                    $("#divPagingText").html("");
                }
                else {
                    /* loop over each object in the array to create rows*/
                    $.each(resultObject.Templates, function (index, item) {
                        /* add to html string started above*/
                        if (editRight) {
                            table += '<tr><td>&nbsp;' + '<a href=\"ManageTemplate.aspx?action=edit&id=' + item.TemplateId + '\" ><img border=\"0\" src = \"' + authority + '\\Assets\\Skin\\Images\\edit.gif\" alt=\"edit\" /> </a>'
                        }
                        if (deleteRight) {
                            table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"DeleteTemplate(\'' + item.TemplateId + '\',\'' + item.TemplateName + '\')\"><img border=\"0\" src = \"' + authority + '\\Assets\\Skin\\Images\\del.gif\" alt=\"delete\" /> </a>'
                        }
                        table += '</td><td>&nbsp;' + item.TemplateName
                        table += '</td><td>&nbsp;' + item.FieldCount
                        table += '</td><td>&nbsp;' + item.CreatedDate
                        table += '</td><td>&nbsp;' + GetBooleanText(item.Active)
                        table += '</td></tr>';
                    });
                    $("#divRecordCountText").html(resultObject.RecordCountText);
                    $("#divPagingText").html(resultObject.PagingText);
                }
                table += '</table>';
                /* insert the html string*/
                $("#divSearchResults").html(table);
                $(msgControl).html(resultObject.Message);
            }
            else { //"ChangePassword" || "AddOrg" || "EditOrg" || "DeleteOrg"
                $(msgControl).html(resultObject.Message);
                if (action == "AddOrg" && resultObject.UserPassData != "") {

                    document.getElementById(hdnCredentialsControlID).value = resultObject.UserPassData;
                    document.getElementById(hdnOrglinkControlID).value = authority + "/Accounts/Login.aspx?org=" + resultObject.NewOrgCode;
                    document.getElementById(btnSubmitControlID).click();
                    alert("Credentials for Admin user is :" + resultObject.UserPassData);
                    var url = "The Company Access URL is: " + authority + "/Accounts/Login.aspx?org=" + resultObject.NewOrgCode;
                    alert(url);
                }
            }
        }
        else {
            $(msgControl).css("color", "red");
            $(msgControl).html(resultObject.Message);
        }
    }
    varType = null; varUrl = null; varData = null; varContentType = null; varDataType = null; varProcessData = null;
    var browser = "";
}

function PostScalarServiceFailed(result) {
    alert('Service call failed: ' + result.status + '' + result.statusText);
    varType = null; varUrl = null; varData = null; varContentType = null; varDataType = null; varProcessData = null;
    var browser = "";
    location.href = "error.aspx";
}

function CallPostTable(msgControl, action, params, data) {
    //reset variables
    var browser = "";
    // Get organisation Id and GUID [session]
    var loginOrgId = document.getElementById(loginOrgIdControlID).value;
    var loginToken = document.getElementById(loginTokenControlID).value;
    if (loginOrgId == "") { loginOrgId = "0"; }
    //check browser
    if (jQuery.browser.mozilla == true) browser = "firefox"
    else if (jQuery.browser.msie == true) browser = "ie"

    varType = "POST";
    varUrl = coreServiceURL + "/PostTable";
    //We are passing multiple paramers to the service in json format
    varData = '{"loginOrgId": "' + loginOrgId + '","loginToken": "' + loginToken + '","action": "' + action + '","methodParams": "' + params + '","data": "' + data + '","browser": "' + browser + '"}';
    varContentType = "application/json; charset=utf-8";
    varDataType = "json";
    varProcessData = true;
    //CallService
    $.ajax({
        type: varType, //GET or POST or PUT or DELETE verb
        url: varUrl, // Location of the service
        data: varData, //Data sent to server
        contentType: varContentType, // content type sent to server
        dataType: varDataType, //Expected data format from server
        processdata: varProcessData, //True or False
        success: function (msg) {//On Successfull service call
            PostTableSucceeded(msgControl, action, msg);
        },
        error: PostTableServiceFailed// When Service call fails
    });
}
function PostTableSucceeded(msgControl, action, result) {
    var resultObject = null;
    status = true;
    if (varDataType == "json") {
        // Constructed object name will be object.[ServiceName]Result
        resultObject = result.PostTableResult;
    }
    if (resultObject == null) {
        $(msgControl).html("An error occurred, please try again. If problem exists please contact Administrator");
    }
    else {
        if (resultObject.ActionStatus == "SUCCESS") {
            $(msgControl).css("color", "green");
            $(msgControl).html("");
            if (action == "ValidateUser") {
                document.getElementById(loginOrgIdControlID).value = resultObject.UserData.LoginOrgId;
                document.getElementById(loginTokenControlID).value = resultObject.UserData.LoginToken;
                document.getElementById(btnSubmitControlID).click();
            }
            else {
                $(msgControl).html(resultObject.Message);
            }
        }
        else {
            $(msgControl).css("color", "red");
            $(msgControl).html(resultObject.Message);
        }
    }
    varType = null; varUrl = null; varData = null; varContentType = null; varDataType = null; varProcessData = null;
    var browser = "";
}

function PostTableServiceFailed(result) {
    alert('Service call failed: ' + result.status + '' + result.statusText);
    varType = null; varUrl = null; varData = null; varContentType = null; varDataType = null; varProcessData = null;
    var browser = "";
    location.href = "error.aspx";
}

//*********************Common Functions *******************************************//
function GetBooleanText(value) {
    if (value == true) {
        return "Yes";
    }
    else {
        return "No";
    }
}

//function DateLocalToDbFormat(localDate) {
//    var dbDate = "";
//    var dateParts = localDate.split("/");
//    if (dateParts.length == 3) {
//        dbDate = dateParts[1] + "/" + dateParts[0] + "/" + dateParts[2];

//    }
//    return dbDate;
//}
//function DbFormatToDateLocal(serverDate) {
//    var dbDate = "";
//    var dateParts = serverDate.split("/");
//    if (dateParts.length == 3) {
//        dbDate = dateParts[1] + "/" + dateParts[0] + "/" + dateParts[2];

//    }
//    return dbDate;
//}

Date.prototype.addDays = function (days) {
    this.setDate(this.getDate() + days);
    return this;
};
Date.prototype.substractDays = function (days) {
    this.setDate(this.getDate() - days);
    return this;
};
Date.prototype.ddmmyyyy = function () {
    var yyyy = this.getFullYear().toString();
    var mm = (this.getMonth() + 1).toString(); // getMonth() is zero-based
    var dd = this.getDate().toString();
    var rsDate = (dd.length == 2 ? dd : "0" + dd) + '/' + (mm.length == 2 ? mm : "0" + mm) + '/' + yyyy; // padding
    return rsDate
};

function PopulateDatePeriod(Mode) {
    var fromDate = new Date();
    var toDate = new Date();
    var fromDateString = "";
    var toDateString = "";

    switch (Mode.toLowerCase()) {
        case "last 1 week":
            fromDate.substractDays(7);
            break;
        case "last 1 month":
            fromDate.substractDays(30);
            break;
        case "last 6 months":
            fromDate.substractDays(180);
            break;
        default:
            fromDate = new Date();
            break;
    }
    if (fromDate != null) {
        var fromDateString = fromDate.ddmmyyyy();
    }
    if (toDate != null) {
        var toDateString = toDate.ddmmyyyy();
    }
    $(FromDateControlId).val(fromDateString);
    $(ToDateControlId).val(toDateString);
}

//texbox validation - Only Characters and '-' Symbol
function CheckvarcharKeyInfo(event) {
    var char1 = (event.keyCode ? event.keyCode : event.which);
    if ((char1 >= 65 && char1 <= 90) || (char1 >= 97 && char1 <= 122) || char1 == 45 || char1 == 32) {
        RetVal = true;
    }
    else {
        RetVal = false;
    }
    return RetVal;
}

//texbox validation - Only numbers and '-' Symbol
function CheckNumericKeyInfo(event) {
    var char1 = (event.keyCode ? event.keyCode : event.which);
    if ((char1 >= 48 && char1 <= 57) || char1 == 45 || char1 == 43) {
        RetVal = true;
    }
    else {
        RetVal = false;
    }
    return RetVal;
}


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
var btnFilterRow = null;
var btnFilterRow_BS = null;
//GridData
var recordCountControlID = null;
var pageDisplayControlID = null;
var pagingTextControlID = null;
var hdnDocTypeCheckStatus = null;
var hdnTotalRowCount = null;
var hdnTotalRowCount_BS = null;



//FilterDate
var FromDateControlId = null;
var ToDateControlId = null;


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
                url = authority + "/Accounts/Login.aspx?org="; //orgcode added in the postback call
                document.getElementById(hdnOrglinkControlID).value = url;
                document.getElementById(btnSubmitControlID).click();
                //$(msgControl).html("resultObject.Message");
                //GoToHomePage();
            }
            //New 12 jun Gokul
            else if (action == "DocTypeCheckBeforeRemove") {
                document.getElementById(hdnDocTypeCheckStatus).value = resultObject.DocTypeCheckDeleteStatus;

                if (resultObject.DocTypeCheckDeleteStatus == "DELETE") {

                    document.getElementById(btnSubmitControlID).click();
                }
                else {
                    $(msgControl).css("color", "Red");
                    $(msgControl).html("Can not be removed as it is already used!");
                    document.getElementById(hdnDocTypeCheckStatus).value = "";

                }


            }

            else if (action == "SearchOrgs" || action == "DeleteOrgAndSearch") {
                var table = '<table border= \"1\" class=\"mGrid\" AlternatingRowStyle-CssClass=\"alt\"  summary=\"Customer Search Results\" ><tr>';
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
                            table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"Redirect(\'' + item.OrgId + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/edit.png" alt=\"edit\" /> </a>';
                        }
                        if (deleteRight) {
                            if (item.OrgParentId == 0) {
                                table += '</td><td>&nbsp;';
                            }
                            else {
                                table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"DeleteOrg(\'' + item.OrgId + '\',\'' + item.OrgName + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/del.gif" alt=\"delete\" /> </a>';
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
            else if (action == "SearchDepartmentsForDeptPage" || action == "DeleteDepartmentAndSearch") {
                var table = '<table border= \"1\" class=\"mGrid\" AlternatingRowStyle-CssClass=\"alt\" summary=\"User Search Results\" ><tr>';
                if (editRight) {
                    table += '<th>Edit</th>';
                }
                if (deleteRight) {
                    table += '<th>Delete</th>';
                }
                table += '<th>Name</th>';
                table += '<th>Description</th>';
                table += '<th>Head</th>';
                table += '<th>CreatedDate</th>'
                table += '<th>Active</th></tr>';
                if (resultObject.Departments == null) {
                    table += '<tr><td colspan=\"9\">&nbsp;' +
                    resultObject.RecordCountText + '</td></tr>';
                    $("#divRecordCountText").html("");
                    $("#divPagingText").html("");
                }
                else {

                    /* loop over each object in the array to create rows*/
                    $.each(resultObject.Departments, function (index, item) {
                        /* add to html string started above*/
                        if (editRight) {
                            table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"Redirect(\'' + item.DepartmentId + '\',\'' + item.DepartmentName + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/edit.png" alt=\"edit\" /> </a>'
                        }
                        if (deleteRight) {
                            table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"DeleteDept(\'' + item.DepartmentId + '\',\'' + item.DepartmentName + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/del.gif" alt=\"delete\" /> </a>'
                        }
                        table += '</td><td>&nbsp;' + item.DepartmentName
                        table += '</td><td>&nbsp;' + item.Description
                        table += '</td><td>&nbsp;' + item.Head
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
            else if (action == "SearchUsers" || action == "DeleteUserAndSearch") {
                var table = '<table border= \"1\" class=\"mGrid\" AlternatingRowStyle-CssClass=\"alt\" summary=\"User Search Results\" ><tr>';
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
                table += '<th>Active</th>';
                table += '<th>Active Directory User</th>';
                table += '<th>Active Directory</th></tr>';
                if (resultObject.Users == null) {
                    table += '<tr><td colspan=\"11\">&nbsp;' +
                    resultObject.RecordCountText + '</td></tr>';
                    $("#divRecordCountText").html("");
                    $("#divPagingText").html("");
                }
                else {

                    /* loop over each object in the array to create rows*/
                    $.each(resultObject.Users, function (index, item) {
                        /* add to html string started above*/
                        if (editRight) {
                            table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"Redirect(\'' + item.UserId + '\',\'' + item.UserName + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/edit.png" alt=\"edit\" /> </a>'
                        }
                        if (deleteRight) {
                            table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"DeleteUser(\'' + item.UserId + '\',\'' + item.UserName + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/del.gif" alt=\"delete\" /> </a>'
                        }
                        table += '</td><td>&nbsp;' + item.UserName
                        table += '</td><td>&nbsp;' + item.FirstName
                        table += '</td><td>&nbsp;' + item.LastName
                        table += '</td><td>&nbsp;' + item.EmailId
                        table += '</td><td>&nbsp;' + item.MobileNo
                        table += '</td><td>&nbsp;' + item.CreatedDate
                        table += '</td><td>&nbsp;' + GetBooleanText(item.Active)
                        table += '</td><td>&nbsp;' + GetBooleanText(item.DomainUser)
                        table += '</td><td>&nbsp;' + item.DomainName
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
                var table = '<table border= \"1\" class=\"mGrid\" AlternatingRowStyle-CssClass=\"alt\"  summary=\"Role Search Results\" ><tr>';
                if (editRight) {
                    table += '<th>Edit</th>';
                }
                if (deleteRight) {
                    table += '<th>Delete</th>';
                }
                table += '<th>Role</th>';
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
                                table += '<tr><td>&nbsp;' + '<a href=\"javascript:void(0)\"  onclick=\"alert(\'This role is not modifiable\');\" ><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/edit.png" alt=\"This role is not modifiable\" /> </a>';
                            }
                            if (deleteRight) {
                                table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"alert(\'This role is not modifiable\');\" ><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/del.gif" alt=\"This role is not modifiable\" /> </a>';
                            }
                        }
                        else {
                            if (editRight) {
                                table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"Redirect(\'' + item.GroupId + '\',\'' + item.GroupName + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/edit.png" alt=\"edit\" /> </a>';
                            }
                            if (deleteRight) {
                                table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"DeleteGroup(\'' + item.GroupId + '\',\'' + item.GroupName + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/del.gif" alt=\"delete\" /> </a>';
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
            else if (action == "ExportDocumentType") {
                var table = '<table id=\"template1\" border= \"1\" class=\"mGrid\" AlternatingRowStyle-CssClass=\"alt\"  summary=\"Role Search Results\" ><tr>';
                if (resultObject.IndexFields == null) {
                    table += '<td colspan=\"7\">&nbsp;' +
                    resultObject.RecordCountText + '</td></tr>';
                }
                else {
                    /* loop over each object in the array to create rows*/
                    $.each(resultObject.IndexFields, function (index, item) {
                        /* add to html string started above*/
                        table += '<th>' + item.IndexName
                        table += '</th>';
                    });
                }
                table += '</tr></table>';
                /* insert the html string*/
                $("#divSearchResults").html(table);
                var exportTableId = document.getElementById("template1");
                write_to_excel(document.getElementById("template1"));

            }
            else if (action == "SearchDocumentTypes" || action == "DeleteDocumentTypeAndSearch") {
                var table = '<table border= \"1\" class=\"mGrid\" AlternatingRowStyle-CssClass=\"alt\"  summary=\"Role Search Results\" ><tr>';
                if (editRight) {
                    table += '<th>Edit</th>';
                }
                if (deleteRight) {
                    table += '<th>Delete</th>';
                }
                if (deleteRight) {
                    table += '<th>Excel</th>';
                }
                if (deleteRight) {
                    table += '<th>CSV</th>';
                }
                if (deleteRight) {
                    table += '<th>Text</th>';
                }
                table += '<th>Name</th>';
                table += '<th>Description</th>';
                table += '<th>No of Roles</th>';
                table += '<th>Department</th>'
                table += '<th>Template</th>'
                table += '<th>CreatedDate</th>'
                table += '<th>Active</th></tr>';
                if (resultObject.DocumentTypes == null) {
                    table += '<tr><td colspan=\"12\">&nbsp;' +
                    resultObject.RecordCountText + '</td></tr>';
                    $("#divRecordCountText").html("");
                    $("#divPagingText").html("");
                }
                else {
                    /* loop over each object in the array to create rows*/
                    $.each(resultObject.DocumentTypes, function (index, item) {
                        /* add to html string started above*/
                        if (editRight) {
                            table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"Redirect(\'' + item.DocumentTypeId + '\',\'' + item.DocumentTypeName + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/edit.png" alt=\"edit\" /> </a>'
                        }
                        if (deleteRight) {
                            if (item.CanDelete == 1) {

                                table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"DeleteDocumentType(\'' + item.DocumentTypeId + '\',\'' + item.DocumentTypeName + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/del.gif" alt=\"delete\" /> </a>'
                            }
                            else {
                                table += '</td><td>&nbsp;'
                            }
                        }
                        if (deleteRight) {
                            table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"ExportDocumentType(\'' + item.DocumentTypeId + '\',\'' + item.DocumentTypeName + '\',\'Excel\',\'' + item.TemplateId + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/excel.png" alt=\"Excel\" /> </a>'
                        }
                        if (deleteRight) {
                            table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"ExportDocumentType(\'' + item.DocumentTypeId + '\',\'' + item.DocumentTypeName + '\',\'CSV\',\'' + item.TemplateId + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/csv.png" alt=\"CSV\" /> </a>'
                        }
                        if (deleteRight) {
                            table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"ExportDocumentType(\'' + item.DocumentTypeId + '\',\'' + item.DocumentTypeName + '\',\'Text\',\'' + item.TemplateId + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/text.png" alt=\"Text\" /> </a>'
                        }
                        table += '</td><td>&nbsp;' + item.DocumentTypeName
                        table += '</td><td>&nbsp;' + item.Description
                        table += '</td><td>&nbsp;' + item.GroupCount
                        table += '</td><td>&nbsp;' + item.DepartmentName
                        table += '</td><td>&nbsp;' + item.TemplateName
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

                var activeTabIndex = $find(tabSearch)._activeTabIndex;
                if (activeTabIndex == 0) {

                    // Html table binding
                    //Total rows returned
                    document.getElementById(hdnTotalRowCount_BS).value = resultObject.DocumentDownloads[0].TotalRowcount;
                    $("#divRecordCountText_BS").html(resultObject.RecordCountText);
                    $("#divPagingText_BS").html(resultObject.PagingText);

                    // Bind html table to div
                    $("#divSearchResults_BS").html(resultObject.DocumentDownloads[0].HtmlTable);
                    document.getElementById(btnFilterRow_BS).click();
                    $(msgControl).html(resultObject.Message);

                    if (document.getElementById(hdnTotalRowCount_BS).value > 0) {
                        document.getElementById("paging_BS").style.visibility = "visible";
                    }
                    else {
                        document.getElementById("paging_BS").style.visibility = "hidden";
                    }
                }
                //DMS5-4365 BS commented
//                else {

//                    // Html table binding
//                    //Total rows returned
//                    document.getElementById(hdnTotalRowCount).value = resultObject.DocumentDownloads[0].TotalRowcount;

//                    $("#divRecordCountText").html(resultObject.RecordCountText);
//                    $("#divPagingText").html(resultObject.PagingText);

//                    // Bind html table to div
//                    $("#divSearchResults").html(resultObject.DocumentDownloads[0].HtmlTable);
//                    document.getElementById(btnFilterRow).click();
//                    $(msgControl).html(resultObject.Message);

//                    if (document.getElementById(hdnTotalRowCount).value > 0) {
//                        document.getElementById("paging").style.visibility = "visible";
//                    }
//                    else {
//                        document.getElementById("paging").style.visibility = "hidden";
//                    }
                //                }
                //DMS5-4365 BE
            }

            else if (action == "SearchTemplates" || action == "DeleteTemplateAndSearch") {
                var table = '<table border= \"1\" class=\"mGrid\" AlternatingRowStyle-CssClass=\"alt\"  summary=\"Role Search Results\" ><tr>';
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

                            table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"Redirect(\'' + item.TemplateId + '\',\'' + item.TemplateName + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/edit.png"edit/" /> </a>'
                        }
                        if (deleteRight) {
                            if (item.CanDelete == 1) {

                                table += '</td><td>&nbsp;' + '<a href=\"javascript:void(0)\" onclick=\"DeleteTemplate(\'' + item.TemplateId + '\',\'' + item.TemplateName + '\')\"><img border=\"0\" src = \"' + authority + '/Assets/Skin/Images/del.gif" alt=\"delete\" /> </a>'
                            }
                            else {
                                table += '</td><td>&nbsp;'
                            }
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

            //____________________________________________________________________________________________________
            // GetBatchUploadData : by Yogish
            else if (action == "GetBatchUploadData") {

            //changed by gokul
            if (resultObject.BatchUpload.length > 0) {
                //Total rows returned
                document.getElementById(hdnTotalRowCount).value = resultObject.BatchUpload[0].TotalRowcount;
                $(msgControl).html(resultObject.Message);
           
                // Bind html table to div
                $("#divSearchResults").html(resultObject.BatchUpload[0].batchData);
            }
                $(msgControl).html(resultObject.Message);

                // Html table binding
                $("#divRecordCountText").html(resultObject.RecordCountText);
                $("#divPagingText").html(resultObject.PagingText);
                document.getElementById(btnFilterRow).click();


                if (document.getElementById(hdnTotalRowCount).value > 0) {
                    document.getElementById("paging").style.visibility = "visible";
                }
                else {
                    document.getElementById("paging").style.visibility = "hidden";
                }

            }
            //________________________________________________________________________________________________
            else if (action == "ManageTagDetails") {
                $(msgControl).html(resultObject.Message);
                if (resultObject.TotalTagPages != null) {
                    document.getElementById("taggedpagescount").innerHTML = resultObject.TotalTagPages;
                }
            }
            else if (action == "GetTagDetails") {
                $(msgControl).html(resultObject.Message);
                var isFound = false;
                var dd = document.getElementById('ctl00_ContentPlaceHolder2_ContentPlaceHolder1_cmbMainTag');
                if (dd != undefined) {
                    if (resultObject.MainTag != null) {
                        dd.options.length = 0;
                        var opt = document.createElement("option");
                        opt.text = '<Select>';
                        opt.value = 0;
                        dd.options.add(opt);
                        $.each(resultObject.MainTag, function (index, item) {
                            var opt = document.createElement("option");
                            opt.text = item.MainTagValue;
                            opt.value = item.MainTagID;
                            dd.options.add(opt);
                        });

                    }

                    for (var i = 0; i < dd.options.length; i++) {
                        if (parseInt(dd.options[i].value) == resultObject.PageMainTagID) {
                            dd.selectedIndex = i;
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound == false) {
                        dd.selectedIndex = 0;
                        isFound = false;
                    }
                    var dd = document.getElementById('ctl00_ContentPlaceHolder2_ContentPlaceHolder1_cmbSubTag');
                    if (resultObject.SubTag != null) {
                        dd.options.length = 0;
                        var opt = document.createElement("option");
                        opt.text = '<Select>';
                        opt.value = 0;
                        dd.options.add(opt);
                        $.each(resultObject.SubTag, function (index, item) {
                            var opt = document.createElement("option");
                            opt.text = item.SubTagValue;
                            opt.value = item.SubTagID;
                            dd.options.add(opt);
                        });
                    }
                    for (var i = 0; i < dd.options.length; i++) {
                        if (parseInt(dd.options[i].value) == resultObject.PageSubTagID) {
                            dd.selectedIndex = i;
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound == false) {
                        dd.selectedIndex = 0;
                        isFound = false;
                    }

                    if (resultObject.TotalTagPages != null) {
                        document.getElementById("taggedpagescount").innerHTML = resultObject.TotalTagPages;
                    }

                }
            }
            else { //"ChangePassword" || "AddOrg" || "AddUser" || "EditOrg" || "DeleteOrg"
                $(msgControl).html(resultObject.Message);
                if ((action == "AddOrg" || action == "AddUser") && resultObject.UserPassData != "") {
                    var userCredentials = resultObject.UserPassData;
                    var url = "";
                    if (action == "AddOrg") {
                        url = authority + "/Accounts/Login.aspx?org=" + resultObject.NewOrgCode;
                        document.getElementById(hdnCredentialsControlID).value = userCredentials;
                        document.getElementById(hdnOrglinkControlID).value = url;
                        document.getElementById(btnSubmitControlID).click();
                        //alert("Credentials for Admin user is :" + userCredentials);
                        //alert("The Company Access URL is: " + url);
                    }
                    else if (action == "AddUser") {
                        $(msgControl).html("");
                        $(msgControl).html(resultObject.Message + " Please Wait..");
                        url = authority + "/Accounts/Login.aspx?org="; //orgcode added in the postback call
                        document.getElementById(hdnCredentialsControlID).value = userCredentials;
                        document.getElementById(hdnOrglinkControlID).value = url;
                        document.getElementById(btnSubmitControlID).click();
                        //alert("User has been created successfully. User Password : " + userCredentials);
                    }
                }
                else if (action == "EditOrg") {
                    document.getElementById(btnSubmitControlID).click();
                }
                else if (action == "EditUser" || action == "AddUser") {
                    document.getElementById(btnSaveControlID).disabled = false;
                    //Wrtier DMS ENHSMT 2-1692
                    if (action == "EditUser" && document.getElementById(btnresetpasswordControlID) != null && document.getElementById(chkDomainUserControlID).checked == true) {
                        document.getElementById(btnresetpasswordControlID).className = "HiddenButton";
                    }
                    else if (action == "EditUser" && document.getElementById(btnresetpasswordControlID) != null && document.getElementById(chkDomainUserControlID).checked == false) {
                        document.getElementById(btnresetpasswordControlID).className = "btnresetpassword";
                    }
                }
                else if (action == "EditTempUser") {
                    location.href = "BulkUploadUsers.aspx?act=editsuccess";
                }
            }
        }
        else {
            $(msgControl).css("color", "red");
            $(msgControl).html(resultObject.Message);
            if (resultObject.Message == "" && action == "EditTempUser") {
                document.getElementById(btnSaveControlID).disabled = false;
                if (resultObject.ActionStatus == "DUPLICATENAME") {
                    $(msgControl).html("Duplicate UserName!");
                }
                else if (resultObject.ActionStatus == "DUPLICATEEMAIL") {
                    $(msgControl).html("Duplicate Email!");
                }
            }
            if (action == "EditUser" || action == "AddUser") {
                document.getElementById(btnSaveControlID).disabled = false;
            }
            else if (action == "ForgotPassword") {
                document.getElementById(btnRequestPasswordControlID).disabled = false;

            }
        }
    }
    varType = null; varUrl = null; varData = null; varContentType = null; varDataType = null; varProcessData = null;
    var browser = "";
}



//********************************************************
//Export html table to excel
//********************************************************

function write_to_excel(tableId) {
    str = "";
    var mytable = tableId;
    var rowCount = mytable.rows.length;
    var colCount = mytable.getElementsByTagName("tr")[0].getElementsByTagName("th").length;
    var ExcelApp = new ActiveXObject("Excel.Application");
    var ExcelSheet = new ActiveXObject("Excel.Sheet");
    //ExcelSheet.Application.Visible = true;
    for (var i = 0; i < rowCount; i++) {
        for (var j = 0; j < colCount; j++) {
            if (i == 0) {
                str = mytable.getElementsByTagName("tr")[i].getElementsByTagName("th")[j].innerText;
            }
            else {
                str = mytable.getElementsByTagName("tr")[i].getElementsByTagName("td")[j].innerText;
            }
            ExcelSheet.ActiveSheet.Cells(i + 1, j + 1).Value = str;
        }
    }
    ExcelSheet.autofit;
    ExcelSheet.Application.Visible = true;
    DisplayAlerts = true;
    CollectGarbage();
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

            //Recently added by Enh4
            if (action == "AddTemplate") {

                document.getElementById(btnSubmitControlID).click();

            }
            else {
                $(msgControl).html(resultObject.Message);

            }
        }
        else {
            $(msgControl).css("color", "red");
            $(msgControl).html(resultObject.Message);
            if (action == "AddTemplate") {
                AfterEditShowTable();
            }

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
    if ((char1 >= 65 && char1 <= 90) || (char1 >= 97 && char1 <= 122) || char1 == 45 || char1 == 32 || char1 == 8 || char1 == 9) {
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
    if ((char1 >= 48 && char1 <= 57) || char1 == 45 || char1 == 43 || char1 == 8 || char1 == 9) {
        RetVal = true;
    }
    else {
        RetVal = false;
    }
    return RetVal;
}

//texbox validation - Only numbers
function CheckNumericKey(event) {
    var char1 = (event.keyCode ? event.keyCode : event.which);
    if (char1 >= 48 && char1 <= 57 || char1 == 8 || char1 == 9) {
        RetVal = true;
    }
    else {
        RetVal = false;
    }
    return RetVal;
}
//texbox validation - Only o and 1
function CheckBoolean(event) {
    var char1 = (event.keyCode ? event.keyCode : event.which);
    if (char1 == 48 || char1 == 49 || char1 == 8 || char1 == 9) {
        RetVal = true;
    }
    else {
        RetVal = false;
    }
    return RetVal;
}


function CheckSpecialcharact(event) {

    var char1 = (event.keyCode ? event.keyCode : event.which);
    if ((char1 >= 48 && char1 <= 57) || char1 == 45 || char1 == 43 || char1 >= 32) {
        RetVal = true;
    }
    else {
        RetVal = false;
    }
    return RetVal;
}



//textbox validation - Date Field
function CheckDateFormat(char1, txtid) {
    var txtcontrolname = 'ctl00_ContentPlaceHolder2_ContentPlaceHolder1_' + txtid;
    var RetVal = true;
    var txt = document.getElementById(txtcontrolname);
    if (char1 >= 47 && char1 <= 57) {
        if (txt.value.length == 2 || txt.value.length == 5) {
            if (char1 != 47) {
                txt.value = txt.value + '/';
            }
        }
        else if (txt.value.length != 2 || txt.value.length != 5) {
            if (char1 == 47) {
                return false;
            }
            else {
                return true;
            }
        }
    }
    else {
        return false;
    }
    return RetVal;
}

//Textbox validation final Date field
function CheckFinalDate(txtid) {
    var txtcontrolname = 'ctl00_ContentPlaceHolder2_ContentPlaceHolder1_' + txtid;
    var msgControl = document.getElementById("<%= divMsg.ClientID %>");
    $(msgControl).html("");
    var txt = document.getElementById(txtcontrolname);
    if (txt.value.length == 10) {
        var DateSplit = $(txtcontrolname).val().split('/');
        if (DateSplit[0].length != 2 || parseInt(DateSplit[0]) > 31 || parseInt(DateSplit[0]) > 1) {
            $(msgControl).html("Date Field -DAY- not in correct format.");
            return false;
        }
        else if (DateSplit[1].length != 2 || parseInt(DateSplit[1]) > 12 || parseInt(DateSplit[1]) > 1) {
            $(msgControl).html("Date Field -MONTH- not in correct format.");
            return false;
        }
        else if (DateSplit[2].length != 4 || parseInt(DateSplit[2]) > 2050 || parseInt(DateSplit[2]) < 2000) {
            $(msgControl).html("Date Field -YEAR- not in correct format.");
            return false;
        }
        else {
            return true;
        }
    }
    else {
        $(msgControl).html("Date Field not in correct format.");
        return false;
    }
}



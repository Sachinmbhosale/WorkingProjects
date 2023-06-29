var ajaxobj;
//Username Validation;
function checkUN()
{
	uname = document.getElementById("txtUserName").value;
	if (uname == "")
	{
	    document.getElementById("resultUN").innerHTML = "";
	    return;
	}
	if ( window.XMLHttpRequest )
		ajaxobj = new XMLHttpRequest();
	else if ( window.ActiveXObject )
		ajaxobj = new ActiveXObject("Microsoft.XMLHTTP");
	if ( ajaxobj == null )
	{
		alert("You Browser doesn't support AJAX");
		return
	}

	
	
	param = "un="+uname;

	ajaxobj.onreadystatechange = returnResult;	
	document.getElementById("imgload").style.visibility = "visible";
	document.getElementById("resultUN").innerHTML = "Loading...";
	url = "clientsidevalidation.aspx?"+param;

	ajaxobj.open("GET", url, true);
	ajaxobj.send(null);
}

function returnResult()
{
	if ( ajaxobj.readyState  == 4 )
	{
		if ( ajaxobj.status == 200 )
		{
			document.getElementById("imgload").style.visibility = "hidden";
			document.getElementById("resultUN").innerHTML = ajaxobj.responseText;
		}
		else
		{
			alert ("Error in responsing");
		}
	}
}


//Email Validation
function checkEmail()
{

	uname = document.getElementById("txtEmail").value;
	if (uname == "")
	{
	    document.getElementById("resultemail").innerHTML = "";
	    return;
	}
	if ( window.XMLHttpRequest )
		ajaxobj = new XMLHttpRequest();
	else if ( window.ActiveXObject )
		ajaxobj = new ActiveXObject("Microsoft.XMLHTTP");
	if ( ajaxobj == null )
	{
		alert("You Browser doesn't support AJAX");
		return
	}

	uname = document.getElementById("txtEmail").value;
	
	param = "email="+uname;

	ajaxobj.onreadystatechange = returnEmailResult;	
	document.getElementById("imgloademail").style.visibility = "visible";
	document.getElementById("resultemail").innerHTML = "Loading...";
	url = "clientsidevalidation.aspx?"+param;

	ajaxobj.open("GET", url, true);
	ajaxobj.send(null);
}

function returnEmailResult()
{
	if ( ajaxobj.readyState  == 4 )
	{
		if ( ajaxobj.status == 200 )
		{
			document.getElementById("imgloademail").style.visibility = "hidden";
			document.getElementById("resultemail").innerHTML = ajaxobj.responseText;
		}
		else
		{
			alert ("Error in responsing");
		}
	}
}

//Subdomain Validation
function checkSubDomain()
{


//	uname = document.getElementById("txtSubDomain").value;
//	if (uname == "")
//	{
//	    document.getElementById("resultsubdomain").innerHTML = "";
//	    return;
//	}
//	if ( window.XMLHttpRequest )
//		ajaxobj = new XMLHttpRequest();
//	else if ( window.ActiveXObject )
//		ajaxobj = new ActiveXObject("Microsoft.XMLHTTP");
//	if ( ajaxobj == null )
//	{
//		alert("You Browser doesn't support AJAX");
//		return
//	}

//	uname = document.getElementById("txtSubDomain").value;
//	
//	param = "sub="+uname;

//	ajaxobj.onreadystatechange = returnSubDomainResult;	
//	document.getElementById("imgloadsubdomain").style.visibility = "visible";
//	document.getElementById("resultsubdomain").innerHTML = "Loading...";
//	url = "clientsidevalidation.aspx?"+param;

//	ajaxobj.open("GET", url, true);
//	ajaxobj.send(null);
}

function returnSubDomainResult()
{
	if ( ajaxobj.readyState  == 4 )
	{
		if ( ajaxobj.status == 200 )
		{
			document.getElementById("imgloadsubdomain").style.visibility = "hidden";
			document.getElementById("resultsubdomain").innerHTML = ajaxobj.responseText;
		}
		else
		{
			alert ("Error in responsing");
		}
	}
}


<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<div id="userpanel">
    <% if (AppHelper.UserIsLoggedIn())
	   { %>
   		<%= AppHelper.GetUserName() %> | <%= Html.ActionLink("Logga ut", "Logout", "Account") %>
    <% }
	   else
	   { %>
	   <%= Html.ActionLink("Logga in", "Login", "Account") %> | <%= Html.ActionLink("Registrera användare", "Create", "Account") %>
    <% } %>
</div>

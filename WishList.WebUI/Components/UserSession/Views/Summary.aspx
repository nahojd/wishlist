<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Summary.aspx.cs" Inherits="WishList.WebUI.Components.UserSession.Views.Summary" %>

<div id="userpanel">
    <% if (WishList.WebUI.Helpers.AppHelper.UserIsLoggedIn())
       { %>
    <%= ViewData.Model.Name %> | <a href="#">logga ut</a>
    <% }
       else
       { %>
    <a href="#">logga in</a> | <a href="#">skapa användare</a>
    <% } %>
</div>

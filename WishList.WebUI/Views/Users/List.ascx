<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<UsersListData>" %>
<ul>
    <% foreach (WishList.Data.User user in ViewData.Model.Users) { %>
    <li>
        <% if ( !string.IsNullOrEmpty(ViewData.Model.SelectedUsername) && ViewData.Model.SelectedUsername == user.Name)
           { %>
           <%= Html.ActionLink( user.Name, "Show", "List", new { id = user.Name }, new { @class = "active" } ) %>
        <% }
           else
           {%>
             <%= Html.ActionLink( user.Name, "Show", "List", new { id = user.Name }, null ) %>
        <% } %>
    </li>       
    <% } %>
</ul>

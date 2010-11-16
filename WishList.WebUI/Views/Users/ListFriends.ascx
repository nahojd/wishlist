<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<ListFriendsModel>" %>
<ul>
    <% foreach (var user in ViewData.Model.Friends) { %>
    <li <%: ViewData.Model.SelectedUsername == user.Name ? "class=active" : ""  %>>
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

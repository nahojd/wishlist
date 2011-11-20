<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<WishList.WebUI.ViewModels.WishListViewModel>" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
<table class="wishTable">
        <tr>
            <th class="name">Namn</th>
            <th class="description">Beskrivning</th>
        </tr>
        <% foreach (var wish in ViewData.Model.Wishes)
           { %>
            <tr>
                <td>
                    <%= Html.ActionLink( wish.Name, "Edit", "Wish", new { wishId = wish.Id }, new { title = "Redigera önskning" } )%>
                </td>
                <td>
					<%= wish.Description %>
					 <% if (!string.IsNullOrEmpty( wish.LinkUrl )) { %>
                        <p><a href="<%= wish.LinkUrl %>" title="<%= wish.LinkUrl %>" target="_blank"><%= AppHelper.ShortUrl(wish.LinkUrl) %></a></p>
                    <% } %>
                </td>
            </tr>
        <% } %>
    </table>
	<%= Html.ActionLink( "Ny önskning", "New", "Wish" ) %>
</asp:Content>

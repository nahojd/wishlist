<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<WishList.Data.Wish>>" %>

<table class="wishTable">
    <tr>
        <th>Önskad av</th>
        <th class="name">Namn</th>
        <th class="description">Beskrivning</th>
        <th class="link">Länk</th>
    </tr>
<% foreach (WishList.Data.Wish wish in ViewData.Model) { %>
    <tr>
        <td><%= Html.ActionLink( wish.Owner.Name, "Show", "List", new { id = wish.Owner.Name }, null ) %></td>
        <td><%= wish.Name %></td>
        <td><%= wish.Description %></td>
        <td>
            <% if (!string.IsNullOrEmpty( wish.LinkUrl )) { %>
                <a href="<%= wish.LinkUrl %>" target="_blank">Klicka här</a>
            <% } %>
        </td>
    </tr>
<% } %>
</table>
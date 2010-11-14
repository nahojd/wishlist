<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WishList.WebUI.Controllers.LatestActivityViewModel>" %>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
	<h2>Senaste �nskningar</h2>

	<table class="wishTable">
		<tr>
			<th>�nskad av</th>
			<th class="name">Namn</th>
			<th class="description">Beskrivning</th>
			<th class="link">�ndrad</th>
		</tr>
	<% foreach (WishList.Data.Wish wish in ViewData.Model.Wishes) { %>
		<tr>
			<td><%= Html.ActionLink( wish.Owner.Name, "Show", "List", new { id = wish.Owner.Name }, null ) %></td>
			<td><%= wish.Name %></td>
			<td><%= wish.Description %></td>
			<td><%= wish.Changed.Value.ToShortDateString() %></td>
		</tr>
	<% } %>
	</table>
</asp:Content>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WishList.WebUI.Controllers.AccountController+UserData>" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%=Notify.Error(ViewData.Model.Error) %>
    <%=Notify.Info(ViewData.Model.Info) %>
    <fieldset>
		<legend>Ändra uppgifter</legend>
		<% using (Html.BeginForm("Edit", "Account")) { %>
			<p><label for="Name">Namn</label>
			<%=Html.TextBox( "Name", ViewData.Model.User.Name, new { disabled = true } )%></p>
			<p><label for="Email">E-post</label>
			<%=Html.TextBox("Email", ViewData.Model.User.Email)%></p>
			<p class="checkboxBlock">
				<%=Html.CheckBox("NotifyOnChange", ViewData.Model.User.NotifyOnChange) %>
				<label for="NotifyOnChange">Meddela mig via e-post när en önskning jag tjingat ändras</label>
			</p>

			<input type="submit" value="Spara" />
		<% } %>
    </fieldset>
    
    <fieldset>
		<legend>Ändra lösenord</legend>
		
		<% using (Html.BeginForm("ChangePassword", "Account")) { %>
			<p><label for="password">Lösenord</label>
			<%=Html.Password("password")%>
			<a href="javascript:togglePassword('password', 'togglePasswordLink');" id="togglePasswordLink">Visa tecken</a>
			</p>
			
			<input type="submit" value="Ändra lösenord" />
		<% } %>
    </fieldset>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="LeftContent">
	
	<% if (false) { %><script type="text/javascript" src="~/Scripts/jquery-1.3.2-vsdoc.js"></script><% } %>
	<script type="text/javascript">
		function saveFriend(name, addFriend) {
			var url;
			if (addFriend)
				url = '<%: Url.Action("AddFriend") %>?username=' + name;
			else
				url = '<%: Url.Action("RemoveFriend") %>?username=' + name;

			$.getJSON(url, function (data) {
				alert(data.status);
			});
		}
	</script>

	
	<h3>Vänner</h3>
	<%: Html.HiddenFor(m => m.User.Id)%>
	<ul>
	<% foreach (var item in Model.Friends) { %>
		<li>
			<input type="checkbox" name="friend_<%: item.Key  %>" <%: item.Value ? "checked=checked" : "" %> onclick="saveFriend('<%: item.Key %>', this.checked);" />
			<%: item.Key %>
		</li>
	<% } %>
	</ul>
		
</asp:Content>

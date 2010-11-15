<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WishList.WebUI.Controllers.AccountController+UserData>" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <%=Notify.Error(ViewData.Model.Error) %>
    <%=Notify.Info(ViewData.Model.Info) %>
    <fieldset>
		<legend>�ndra uppgifter</legend>
		<% using (Html.BeginForm("Edit", "Account")) { %>
			<p><label for="Name">Namn</label>
			<%=Html.TextBox( "Name", ViewData.Model.User.Name, new { disabled = true } )%></p>
			<p><label for="Email">E-post</label>
			<%=Html.TextBox("Email", ViewData.Model.User.Email)%></p>
			<p class="checkboxBlock">
				<%=Html.CheckBox("NotifyOnChange", ViewData.Model.User.NotifyOnChange) %>
				<label for="NotifyOnChange">Meddela mig via e-post n�r en �nskning jag tjingat �ndras</label>
			</p>

			<input type="submit" value="Spara" />
		<% } %>
    </fieldset>
    
    <fieldset>
		<legend>�ndra l�senord</legend>
		
		<% using (Html.BeginForm("ChangePassword", "Account")) { %>
			<p><label for="password">L�senord</label>
			<%=Html.Password("password")%>
			<a href="javascript:togglePassword('password', 'togglePasswordLink');" id="togglePasswordLink">Visa tecken</a>
			</p>
			
			<input type="submit" value="�ndra l�senord" />
		<% } %>
    </fieldset>

</asp:Content>

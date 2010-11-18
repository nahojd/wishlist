<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WishList.WebUI.Controllers.AccountController+SaveAccountResult>" %>

<asp:Content  ContentPlaceHolderID="MainContent" runat="server">

	<h2>Nytt konto</h2>

    <%=Notify.Error( ViewData.Model.StatusMessage )%>
    <% using (Html.BeginForm()) { %>
     <p><label for="Name">Namn</label>
     <%=Html.TextBox("name")%></p>
     <p><label for="password">Lösenord</label>
     <%=Html.Password("password")%></p>
     <p><label for="Email">E-post</label>
     <%=Html.TextBox("Email")%></p>
     <p><label for="message">Meddelande (frivilligt)</label>
     <%=Html.TextArea("message")%></p>
     
	 <input type="submit" value="Skapa konto" />
     
    <% } %>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="LeftContent">
	<p>Innan du kan använda ditt konto måste det aktiveras. Det sker så fort Johan hinner.</p>
	<p>Om du inte vet vem Johan är ska du nog inte registrera ett konto. Om du tror att Johan inte vet vem du är är det lämpligt att berätta det i meddelanderutan.</p>
</asp:Content>

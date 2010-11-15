<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WishList.WebUI.Controllers.AccountController+SaveAccountResult>" %>

<asp:Content  ContentPlaceHolderID="MainContent" runat="server">
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

<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WishList.WebUI.Controllers.AccountController+UserData>" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <h1>Välkommen!</h1>

    <p>Du måste logga in för att använda Önskelistemaskinen. Om du inte har ett konto kan du skapa ett genom att klicka på "Registrera användare" uppe till höger.</p>

    <%=Notify.Error(ViewData.Model.Error) %>
    <% using (Html.BeginForm("Authenticate", "Account")) { %>
     <table>
        <tr>
            <td>Login</td>
            <td><%=Html.TextBox("username")%>
            </td>
        </tr>
         <tr>
            <td>Password</td>
            <td><%=Html.Password("password")%></td>
        </tr>
        <tr>
            <td colspan="2">
                <input type="submit" value="Login" />
            </td>
        </tr>
     </table>
    <% } %>
    <br />
    <p>
        <%=Html.ActionLink("Glömt lösenordet?", "ForgotPassword") %>
    </p>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="LeftContent"></asp:Content>

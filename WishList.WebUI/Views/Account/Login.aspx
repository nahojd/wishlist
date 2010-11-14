<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WishList.WebUI.Controllers.AccountController+UserData>" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <h1>Välkommen!</h1>
    
    <p>Du måste logga in för att använda Önskelistemaskinen. Om du inte har ett konto kan du skapa ett genom att klicka på "Registrera användare" uppe till höger.</p>

    <%=Notify.Error(ViewData.Model.Error) %>
    <% using (Html.BeginForm<AccountController>( x => x.Authenticate() )) { %>
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
                <%=Html.SubmitButton("btnLogin", "Login")%>
            </td>
        </tr>     
     </table>
    <% } %>

</asp:Content>

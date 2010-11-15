<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage<WishList.Data.Wish>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.ValidationSummary() %>
    <% using (Html.BeginForm())
       { %>
        <p><label for="Name">Namn</label><%= Html.EditorFor( w => w.Name ) %></p>
        <p><label for="Description">Beskrivning</label><%= Html.TextAreaFor( w => w.Description, new { maxlength = 500 } )%></p>
        <p><label for="LinkUrl">Länk</label><%= Html.EditorFor( w => w.LinkUrl )%></p>
        
		<input type="submit" value="Spara" />
    <% } %>
    
    <%= Html.ActionLink( "Ta bort önskning", "Delete", new { wishId = Model.Id } )%>   <a href="javascript:history.go(-1)">Avbryt</a>

</asp:Content>

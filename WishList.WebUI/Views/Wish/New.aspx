<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<WishList.Data.Wish>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.ValidationSummary() %>
    <% using (Html.BeginForm())
       { %>
        <p><label for="Name">Namn</label><%= Html.TextBox( "Name", null, new { maxlength = 100 } )%></p>
        <p><label for="Description">Beskrivning</label><%= Html.TextArea( "Description", null, new { maxlength = 500 } )%></p>
        <p><label for="LinkUrl">Länk</label><%= Html.TextBox( "LinkUrl", null, new { maxlength = 255 } )%></p>
       
        <%= Html.SubmitButton("Save", "Spara") %>
        
    <% } %>
</asp:Content>

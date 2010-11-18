<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<string>" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <em><%= ViewData.Model %></em>
</asp:Content>

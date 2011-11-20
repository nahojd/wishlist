<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<WishList.WebUI.ViewModels.WishListViewModel>" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <table class="wishTable">
        <tr>
            <th class="name">Namn</th>
            <th class="description">Beskrivning</th>
            <th class="calledBy">Tjingad av</th>
        </tr>
        <% foreach (var wish in ViewData.Model.Wishes)
           { %>
            <tr>
                <td>
                    <%= wish.Name %>
                </td>
                <td>
					<%= wish.Description %>
					 <% if (!string.IsNullOrEmpty( wish.LinkUrl )) { %>
                        <a class="wishLink" href="<%= wish.LinkUrl %>" title="<%= wish.LinkUrl %>" target="_blank"><%= AppHelper.ShortUrl(wish.LinkUrl) %></a>
                    <% } %>
                </td>
                <td class="toolCell">
                    <% if (wish.IsCalled && !AppHelper.IsCurrentUserId( wish.CalledByUserId.Value ))
                       { %>
                           <%= wish.CalledByUserName%>
                    <% }
                       else
                       { %>
                           <a href="#" id="<%: "link" + wish.Id %>" class="<%= wish.IsCalled ? "actionLinkChecked" : "actionLink" %>"></a>
                           <script type="text/javascript">
                               $(document).ready(
                                function () {
                                    $('#<%: "link" + wish.Id %>').click( function() {
                                        <%  if ( !wish.IsCalled ) {
                                                Response.Write( string.Format("call({0}, '{1}', '{2}')", wish.Id, Url.Action("Call", "Wish", new { wishId = wish.Id }), Url.Action("UnCall", "Wish", new { wishId = wish.Id }) ) );
                                            } else {
                                                Response.Write( string.Format("uncall({0}, '{1}', '{2}')", wish.Id, Url.Action("UnCall", "Wish", new { wishId = wish.Id }), Url.Action("Call", "Wish", new { wishId = wish.Id }) ) );
                                            }
                                         %>
                                    }).text('<%=  wish.IsCalled ? "Ta bort" : "Tjinga" %>');
                                });
                           </script>
                    <% } %>
                    
                </td>
            </tr>
        <% } %>
    </table>
    
    <script type="text/javascript">
        function call(id, linkUrl, newLinkUrl) {
            $.get(linkUrl, { wishId: id }, function () {
                $("#link" + id).unbind('click');
                $("#link" + id).text("Ta bort").attr("class", "actionLinkChecked").click(function () {
                    uncall(id, newLinkUrl, linkUrl);
                });
            }, "text");
        }
        function uncall(id, linkUrl, newLinkUrl) {
            $.get(linkUrl, { wishId: id }, function () {
                $("#link" + id).unbind('click');
                $("#link" + id).text("Tjinga").attr("class", "actionLink").click(function () {
                    call(id, newLinkUrl, linkUrl);
                });
            }, "text");
        }
    </script>
</asp:Content>

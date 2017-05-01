<%@ Page Title="Title" Language="C#" Inherits="System.Web.Mvc.ViewPage<MovieTracker.Data.Movie>"
    MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent">
</asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>"
        type="text/javascript"></script>
    <% using (Html.BeginForm())
        { %>
    <div class="movieDetail">

        <a href="<%:Url.Action("Delete", new { id = Model.Id }) %>" class="delete" title="Delete <%:Model.Name %>"></a>
        <a href="<%:Url.Action("Edit", new { id = Model.Id }) %>" class="edit" title="Edit <%:Model.Name %>"></a>
        <h1>
            <%:Model.Name%></h1>
        <%:Model.Stars%>
        <fieldset>
            <legend>Movie Details</legend>
            <%: Html.ValidationSummary(false) %>
            <table>
                <tbody>
                    <tr>
                        <td>
                            <%:Html.LabelFor(m => m.GenreId)%>
                        </td>
                        <td>
                            <%:Model.Genre.Name%>
                        </td>
                        <td>
                            <%:Html.LabelFor(m => m.Rating)%>
                        </td>
                        <td>
                            <%:Model.Rating%>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <%:Html.LabelFor(m => m.Directors)%>
                        </td>
                        <td>
                            <%:Model.Directors%>
                        </td>
                        <td>
                            <%:Html.LabelFor(m => m.Writers)%>
                        </td>
                        <td>
                            <%:Model.Writers%>
                        </td>
                    </tr>
                </tbody>
                <tfoot>
                </tfoot>
            </table>
        </fieldset>
        <fieldset>
            <table data-movieid="<%:Model.Id %>" class="MovieLender">
                <tr>
                    <td>
                        <%: Html.LabelFor(m => m.LentToName) %></td>
                </tr>
                <tr>
                    <td><%: Html.TextBoxFor(m => m.LentToName) %></td>
                </tr>
                <tr>
                    <td><%: Html.LabelFor(m => m.LentToDate) %></td>
                </tr>
                <tr>
                    <td>
                        <%: Html.TextBoxFor(m => m.LentToDate) %></td>
                </tr>
            </table>
            <div style="text-align: right">
                <input type="button" id="btnSave" value="Save" />
            </div>
        </fieldset>
        <div class="navInput">
            <%:Html.ActionLink("Back", "Index") %>
        </div>
    </div>

    <script type="text/javascript">
        $('#btnSave').click(function () {

            var movie = {
                movieId: $(".MovieLender").data('movieid'),
                lendToName: $("#LentToName").val(),
                lendToDate: $("#LentToDate").val()
            };

            $.ajax('<%:Url.Action("SaveMovieLendTo") %>', {
                type: 'post',
                data: movie,
                success: function (data) {

                }
            });
        });
    </script>
    <%} %>
</asp:Content>

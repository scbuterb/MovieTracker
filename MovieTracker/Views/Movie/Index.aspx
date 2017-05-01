<%@ Page Title="Title" Language="C#" Inherits="System.Web.Mvc.ViewPage<System.Collections.Generic.IEnumerable<MovieTracker.Data.Movie>>"
    MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent">
    My Movies
</asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

    <script type="text/javascript">
        $(function () {
            $(".clearRating").click(function () {
                var $td = $(this).closest("td");
                var movieToClearRating = {
                    movieId: $(this).closest(".movieRating").data('movieid')
                };
                $.ajax('<%:Url.Action("ClearRating") %>', {
                    type: 'post',
                    data: movieToClearRating,
                    success: function (data) {
                        $td.empty().append(data);
                    }
                });
            });
            $(".star").click(function () {
                var $td = $(this).closest("td");
                var rating = $(this).index() + 1;

                var movieToRate = {
                    movieId: $(this).closest(".movieRating").data('movieid'),
                    rating: rating
                };

                $.ajax('<%:Url.Action("AddRating") %>', {
                    type: 'post',
                    data: movieToRate,
                    success: function (data) {
                        $td.empty().append(data);
                    }
                });
            });
            $(".star").hover(function () {
                $(this).prevAll(".star").addBack().addClass("hovered");

            }, function () {
                $(this).siblings(".star").addBack().removeClass("hovered");
            });
        })


        $(document).ready(function () {
            $('#tblMovieList').DataTable();
        });

    </script>

    <table id="tblMovieList" class="display movieList">
        <thead>
            <tr>
                <th>Edit
                </th>
                <th>Name
                </th>
                <th>Rating
                </th>
                <th>Movie Lent To
                </th>
                <th>Movie Lent To Date
                </th>
                <th>Delete
                </th>
            </tr>
        </thead>
        <tbody>
            <% foreach (var movie in Model)
                {
            %>
            <tr>
                <td>
                    <a href="<%:Url.Action("Edit", new { id = movie.Id }) %>" class="edit" title="Edit <%:movie.Name %>"></a>
                </td>
                <td>
                    <%:Html.ActionLink(movie.Name, "Detail", new {id=movie.Id}) %>
                </td>
                <td>
                    <%: Html.Partial("MovieRatingControl", movie) %>
                </td>
                <td>
                    <%: movie.LentToName %>
                    <p>
                        <%
                            if (!string.IsNullOrEmpty(movie.LentToName))
                            { %>
                        <%:Html.ActionLink("Return Movie", "ReturnMovieFromFriend", new { movieId = movie.Id }) %>
                        <% }
                        %>
                    </p>
                </td>
                <td><%: movie.LentToDate %>

                </td>
                <td>
                    <a href="<%:Url.Action("Delete", new { id = movie.Id }) %>" class="delete" title="Delete <%:movie.Name %>"></a>
                </td>
            </tr>
            <%
                } %>
        </tbody>

    </table>
    <div><%:Html.ActionLink("Add new movie", "Add") %></div>
</asp:Content>

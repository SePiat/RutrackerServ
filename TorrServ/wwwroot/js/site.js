
$(document).ready(function () {
    $('#Btn_Title').click(
        SearchMoviesFromTitle
    );
    $('#Btn_CommentIndex').click(
        SearchMoviesFromIndex
    );
    $('#Btn_Genre').click(
        SearchMoviesFromGenre
    );
    $('#Btn_Country').click(
        SearchMoviesFromCountry
    );
    $('#Btn_EarOfIssue').click(
        SearchMoviesEarOfIssue
    );
    $('#Btn_VideoQuality').click(
        SearchMoviesVideoQuality
    );
    $('#Btn_Size').click(
        SearchMoviesSize
    );
    $('#Btn_Downloads').click(
        SearchMoviesDownloads
    );
    $('#Btn_PathDowLoad').click(
        SearchMoviesPathDowLoad
    );
});

function SearchMoviesFromTitle() {
    $.ajax({
        url: '/Home/SearchFromTitle?SerchText=' + $('#Inp_Title').val(),
        type: "Get",
        success: function (data) {
            $('#moviescontainer').fadeOut(400,
                function () {
                    $('#moviescontainer').html(data);
                    $(this).fadeIn(300);
                });
        },
        error: errorFunc

    });
}
function SearchMoviesFromIndex() {    
    $.ajax({
        url: '/Home/SearhFromIndex?indexText=' + $('#Inp_CommentIndex').val(),
        type: "Get",
        success: function (data) {            
            $('#moviescontainer').fadeOut(400,
                function () {
                    $('#moviescontainer').html(data);
                    $(this).fadeIn(300);
                });
        },
        error: errorFunc
    });
}

function SearchMoviesFromGenre() {
    $.ajax({
        url: '/Home/SearhFromGenre?GenreText=' + $('#Inp_Genre').val(),
        type: "Get",
        success: function (data) {
            $('#moviescontainer').fadeOut(400,
                function () {
                    $('#moviescontainer').html(data);
                    $(this).fadeIn(300);
                });
        },
        error: errorFunc
    });
}

function SearchMoviesFromCountry() {
    $.ajax({
        url: '/Home/SearhFromCountry?CountryText=' + $('#Inp_Country').val(),
        type: "Get",
        success: function (data) {
            $('#moviescontainer').fadeOut(400,
                function () {
                    $('#moviescontainer').html(data);
                    $(this).fadeIn(300);
                });
        },
        error: errorFunc
    });
}


function SearchMoviesEarOfIssue() {
    $.ajax({
        url: '/Home/SearhFromEarOfIssue?EarOfIssueText=' + $('#Inp_EarOfIssue').val(),
        type: "Get",
        success: function (data) {
            $('#moviescontainer').fadeOut(400,
                function () {
                    $('#moviescontainer').html(data);
                    $(this).fadeIn(300);
                });
        },
        error: errorFunc
    });
}

function SearchMoviesVideoQuality() {
    $.ajax({
        url: '/Home/SearhFromVideoQuality?VideoQualityText=' + $('#Inp_VideoQuality').val(),
        type: "Get",
        success: function (data) {
            $('#moviescontainer').fadeOut(400,
                function () {
                    $('#moviescontainer').html(data);
                    $(this).fadeIn(300);
                });
        },
        error: errorFunc
    });
}

function SearchMoviesSize() {
    $.ajax({
        url: '/Home/SearhFromSize?SizeText=' + $('#Inp_Size').val(),
        type: "Get",
        success: function (data) {
            $('#moviescontainer').fadeOut(400,
                function () {
                    $('#moviescontainer').html(data);
                    $(this).fadeIn(300);
                });
        },
        error: errorFunc
    });
}
function SearchMoviesDownloads() {
    $.ajax({
        url: '/Home/SearhFromDownloads?DownloadsText=' + $('#Inp_Downloads').val(),
        type: "Get",
        success: function (data) {
            $('#moviescontainer').fadeOut(400,
                function () {
                    $('#moviescontainer').html(data);
                    $(this).fadeIn(300);
                });
        },
        error: errorFunc
    });
}

function SearchMoviesPathDowLoad() {
    $.ajax({
        url: '/Home/SearhFromPathDowLoad?PathDowLoadText=' + $('#Inp_PathDowLoad').val(),
        type: "Get",
        success: function (data) {
            $('#moviescontainer').fadeOut(400,
                function () {
                    $('#moviescontainer').html(data);
                    $(this).fadeIn(300);
                });
        },
        error: errorFunc
    });
}



















function errorFunc(errorData) {    
    alert('Error' + errorData.responseText);
}


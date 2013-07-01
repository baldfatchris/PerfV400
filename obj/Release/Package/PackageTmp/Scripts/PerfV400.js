/*!
* PerfV400.js
* Created: 2012-05-15
*/




/*! datepicker 
$(function () {
$('.datepicker').datepicker({ dateFormat: "dd MM yy" }); getJqueryUserLanguage
.datepicker.regional[getJqueryUserLanguage()]
});
*/


$(function () {

    /*! highlight the input fields */
    $('input').hover(function () { $(this).toggleClass('highlight'); });

    /*! animate the thumbnails */
    $('.thumb').mouseover(function () {
        $(this).animate({ height: '+=4', width: '+=4' }).animate({ height: '-=4', width: '-=4' });
    });

    /*! toggle the artist toolbar */
    $('.Photo150').hover(function () {
        $(this).removeClass('toggle-toolbar');
    }, function () {
        $(this).addClass('toggle-toolbar');
    });

    /*! activate the date picker */
    $('input.date').datepicker({ dateFormat: "dd MM yy" });

});








/*! search error message */
function searchFailed() { $("#searchresults").html("Sorry, there was a problem with the search."); }


/*! photo upload */
function EditPhoto(entityType, entityId) { $("#EditPhotoForm" + entityType + entityId).submit(); }

function EditPhotoComplete(entityType, entityId) {
    //Check to see if this is the first load of the iFrame
    if (isFirstLoad == true) {
        isFirstLoad = false;
        return;
    }

    //Reset the image form so the file won't get uploaded again
    if (document.getElementById("EditPhotoForm" + entityType + entityId) == null) {
        alert('can not find image form EditPhotoForm' + entityType + entityId);
        return;
    }
    document.getElementById("EditPhotoForm" + entityType + entityId).reset();


    //Create a new image and insert it into the Images div.  
    var imgDiv = document.getElementById("Photo" + entityType + entityId);
    var img = new Image();
    //img.src = newImg.ImagePath;

    img.src = "/" + entityType + "/Get" + entityType + "Image/" + entityId+"?" + "1";

    $(img).addClass("pic");
    imgDiv.innerHTML = "";
    imgDiv.appendChild(img);
}



/*! edit window */
$(function () {

    addPopUps();

});


function XdeleteSuccess() {
}


function addPopUps() {


    $('#EditDialogue').dialog({
        autoOpen: false,
        width: 800,
        resizable: true,
        modal: true,
        buttons: {
            "Update": function () {
                $("#update-message").html(''); //make sure there is nothing on the message before we continue                         
                $("#EditForm").submit();
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    });

    $('#CopyDialogue').dialog({
        autoOpen: false,
        width: 800,
        resizable: true,
        modal: true,
        buttons: {
            "Copy": function () {
                $("#update-message").html(''); //make sure there is nothing on the message before we continue                         
                $("#CopyForm").submit();
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    });

    $('#CreateDialogue').dialog({
        autoOpen: false,
        width: 800,
        resizable: true,
        modal: true,
        buttons: {
            "Add": function () {
                $("#update-message").html(''); //make sure there is nothing on the message before we continue                         
                $("#CreateForm").submit();
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    });

    $('#DeleteDialogue').dialog({
        autoOpen: false,
        width: 800,
        resizable: true,
        modal: true,
        buttons: {
            "Delete": function () {
                $("#update-message").html(''); //make sure there is nothing on the message before we continue                         
                $("#DeleteForm").submit();
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        }
    });




    $(".editLink").click(function () {
        //change the title of the dialog
        var linkObj = $(this);
        var dialogDiv = $('#EditDialogue');
        var viewUrl = linkObj.attr('href');
        var viewTitle = linkObj.data('dialog-title');

        $.get(viewUrl, function (data) {
            dialogDiv.html(data);
            //validation
            var $form = $("#EditForm");
            // Unbind existing validation
            $form.unbind();
            $form.data("validator", null);
            // Check document for changes
            $.validator.unobtrusive.parse(document);
            // Re add validation with changes
            $form.validate($form.data("unobtrusiveValidation").options);
            //open dialog
            dialogDiv.dialog('option', 'title', viewTitle);
            dialogDiv.dialog('open');
        });
        return false;
    });

    $(".copyLink").click(function () {
        //change the title of the dialog
        var linkObj = $(this);
        var dialogDiv = $('#CopyDialogue');
        var viewUrl = linkObj.attr('href');
        var viewTitle = linkObj.data('dialog-title');

        $.get(viewUrl, function (data) {
            dialogDiv.html(data);
            //validation
            var $form = $("#CopyForm");
            // Unbind existing validation
            $form.unbind();
            $form.data("validator", null);
            // Check document for changes
            $.validator.unobtrusive.parse(document);
            // Re add validation with changes
            $form.validate($form.data("unobtrusiveValidation").options);
            //open dialog
            dialogDiv.dialog('option', 'title', viewTitle);
            dialogDiv.dialog('open');
        });
        return false;
    });

    $(".createLink").click(function () {
        //change the title of the dialog
        var linkObj = $(this);
        var dialogDiv = $('#CreateDialogue');
        var viewUrl = linkObj.attr('href');
        var viewTitle = linkObj.data('dialog-title');

        $.get(viewUrl, function (data) {
            dialogDiv.html(data);
            //validation
            var $form = $("#CreateForm");
            // Unbind existing validation
            $form.unbind();
            $form.data("validator", null);
            // Check document for changes
            $.validator.unobtrusive.parse(document);
            // Re add validation with changes
            $form.validate($form.data("unobtrusiveValidation").options);
            //open dialog
            dialogDiv.dialog('option', 'title', viewTitle);
            dialogDiv.dialog('open');
        });
        return false;
    });

    $(".deleteLink").click(function () {
        //change the title of the dialog
        var linkObj = $(this);
        var dialogDiv = $('#DeleteDialogue');
        var viewUrl = linkObj.attr('href');
        var viewTitle = linkObj.data('dialog-title');

        $.get(viewUrl, function (data) {
            dialogDiv.html(data);
            //open dialog
            dialogDiv.dialog('option', 'title', viewTitle);
            dialogDiv.dialog('open');
        });
        return false;
    });



}








function updateSuccess() {
    $('#EditDialogue').dialog('close');
    //twitter type notification
    $('#commonMessage').html("Update Complete");
    $('#commonMessage').delay(400).slideDown(400).delay(3000).slideUp(400);
}

function copySuccess() {
    $('#CopyDialogue').dialog('close');
    addPopUps();

    //twitter type notification
    $('#commonMessage').html("Copy Complete");
    $('#commonMessage').delay(400).slideDown(400).delay(3000).slideUp(400);
}



function createSuccess() {
    $('#CreateDialogue').dialog('close');
    //twitter type notification
    $('#commonMessage').html("Update Complete");
    $('#commonMessage').delay(400).slideDown(400).delay(3000).slideUp(400);
    addPopUps();

}
function deleteSuccess() {
    $('#DeleteDialogue').dialog('close');

    //twitter type notification
    $('#commonMessage').html("Delete Complete");
    $('#commonMessage').delay(400).slideDown(400).delay(3000).slideUp(400);

}



$(function () {


    /*! debug */
    $('#helloworld').css('background-color', 'green');

});




/* Star Rating */
$(document).ready(function () {
    starRating.create('.stars');
});

// The widget
var starRating = {
    create: function (selector) {

        // Set the value
        var $rating = $(selector).attr('data-value');
        $(selector).find('input#rating-' + $rating).attr('checked', true);

        // loop over every element matching the selector
        $(selector).each(function () {
            var $list = $('<div></div>');
            // loop over every radio button in each container
            $(this)
              .find('input:radio')
              .each(function (i) {
                  var rating = $(this).parent().text();
                  var $item = $('<a href="#"></a>')
                    .attr('title', rating)
                    .addClass(i % 2 == 1 ? 'rating-right' : '')
                    .text($(this).attr('id'));

                  starRating.addHandlers($item);
                  $list.append($item);

                  if ($(this).is(':checked')) {
                      $item.prevAll().andSelf().addClass('rating');
                  }
              });

            // Hide the original radio buttons
            $(this).append($list).find('label').hide();
        });
    },
    addHandlers: function (item) {
        $(item).click(function (e) {
            // Handle Star click
            var $star = $(this);
            var $allLinks = $(this).parent();

            // Set the radio button value
            $allLinks.parent().find('input#' + $star.text()).attr('checked', true);

            // Set the ratings
            $allLinks.children().removeClass('rating');
            $star.prevAll().andSelf().addClass('rating');

            // prevent default link click
            e.preventDefault();

            // click the submit button
            $('#fs2').click();

        }).hover(function () {
            // Handle star mouse over
            $(this).prevAll().andSelf().addClass('rating-over');
        }, function () {
            // Handle star mouse out
            $(this).siblings().andSelf().removeClass('rating-over')
        });
    }

}




function getJqueryUserLanguage() {

    var language = (navigator.userLanguage) ? navigator.userLanguage : navigator.language;

    var l = language.toLowerCase().split('-');
    if (l.length == 1) {
        if (jQuery.datepicker.regional[l[0]] != undefined) return l[0];
        else return '';
    }
    else if (l.length > 1) {
        if (jQuery.datepicker.regional[l[0] + '-' + l[1].toUpperCase()] != undefined) return l[0] + '-' + l[1].toUpperCase();
        else if (jQuery.datepicker.regional[l[0]] != undefined) return l[0];
        else return '';
    }
    else {
        return '';
    }
}
function copyText(text, name) {

    navigator.clipboard.writeText(text).then(function () {
        bootbox.alert(name + " was copied to the clipboard.");
    }, function (err) {
        bootbox.alert("Could not copy " + name + "...");
    });

    /*  var copyText = document.getElementById("myAddress");
      copyText.select();
      document.execCommand("copy");
    */
}

function RegisterName() {
    bootbox.prompt({
        title: "Insert a name for your address:",
        message: "It might take a while to update your name.",
        callback:
            function (result) {
                var name = result;
                if (name === "") {
                    bootbox.alert("Name can not be empty");
                    return;
                }
                if (name.length < 4 || name.length > 15) {
                    bootbox.alert("Name must be bigger than 4 letters and less than 15");
                    return;
                }
                if (name == 'anonymous' || name == 'genesis') {
                    bootbox.alert("Name can not be 'anonymous' or 'genesis'");
                    return;
                }

                var index = 0;
                while (index < name.length) {
                    var charCode = name.charCodeAt(index);
                    index++;
                    if (charCode >= 97 && charCode <= 122) {
                        continue;
                    } // lowercase allowed
                    if (charCode === 95) {
                        continue;
                    } // underscore allowed
                    if (charCode >= 48 && charCode <= 57) {
                        continue;
                    } // numbers allowed

                    bootbox.alert("Only lowercase, underscore and numbers allowed");
                    return;
                }
                $('#nameSpinner').show();
                $.post('/register',
                    {
                        name: name
                    },
                    function (returnedData) {
                        $('#nameSpinner').hide();
                        if (returnedData !== "") {
                            window.location.replace("/waiting/" + returnedData);
                            console.log("error");
                        } else {
                            window.location.replace("/error");
                        }
                        console.log("Register name tx: " + returnedData);
                    }).fail(function () {
                        console.log("error registering name");
                        $('#nameSpinner').hide();
                    });
            }
    });
}

$(document).ready(function() {
    $("#editPencil").click(RegisterName);
    $("#editPencil2").click(RegisterName);
});

setInterval(function() {
  getChains();
}, 1000);

function getChains() {
  $.ajax({
         url : "http://45.77.48.103:7077/rpc",
         type : "POST",
         data : '{"jsonrpc":"2.0","method":"getBlockHeight","params":["main"],"id":1}',
         dataType: "json",
         success : function (datarpcnode) {
           datamain = datarpcnode;
           heightmain = '#' + numberWithCommas(datamain.result);
           if (document.getElementById("blockheight").innerHTML != heightmain) {
             document.getElementById("blockheight").innerHTML = heightmain;
             $(document.getElementById("blockheight")).addClass('flash').delay(150).queue(function(next){
                  $(this).removeClass('flash');
                  next();
             });
           }
           if (document.getElementById("blockheightparent").innerHTML != 'Main Chain Height') {
             document.getElementById("blockheightparent").innerHTML = 'Main Chain Height';
           }
         }
   })
}

function numberWithCommas(x) {
    var parts = x.toString().split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return parts.join(".")
}
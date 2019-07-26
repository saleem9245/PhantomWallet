function copyText(text, name) {

    navigator.clipboard.writeText(text).then(function () {
        bootbox.alert(name + " was copied to the clipboard.");
    }, function (err) {
        bootbox.alert("Could not copy " + name + "...");
    });

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

    // Custom check for night mode
    onloadtoggle = true;
    if (localStorage.getItem('themesetting') == 'dark') {
        if (document.getElementById("cb4")) {
          document.getElementById("cb4").checked = true;
        }
        toggleNightMode();
        onloadtoggle = false;
    }
    if (document.getElementById("blockheight")) {
      document.getElementById("blockheight").innerHTML = localStorage.getItem('lastheight');
    }
    if (document.getElementById("blockheight")) {
      document.getElementById("soulprice").innerHTML = localStorage.getItem('lastsoulprice');
    }
    var pathname = window.location.pathname;
    if (pathname != '/login' && pathname != '/create') {
      setInterval(function() {
        getChains();
      }, 2000);
      setInterval(function() {
        getPricing();
      }, 60000);
    }
});

function getChains() {
   $.get('/chains',
       function (returnedData) {
         heightmain = '#' + numberWithCommas(JSON.parse(returnedData)[0].height);
         if (document.getElementById("blockheight").innerHTML != heightmain) {
           document.getElementById("blockheight").innerHTML = heightmain;
           $(document.getElementById("blockheight")).addClass('flash').delay(150).queue(function(next){
                $(this).removeClass('flash');
                next();
           });
         }
         cacheheight = document.getElementById("blockheight").innerHTML;
         localStorage.setItem('lastheight', cacheheight);
       }).fail(function(e) {
          console.log(e);
       });
}

function getPricing() {
   $.get('https://api.coingecko.com/api/v3/simple/price?ids=phantasma&vs_currencies=usd',
       function (returnedData) {
         soulprice = '$' + numberWithCommas(returnedData.phantasma.usd);
         if (document.getElementById("soulprice").innerHTML != soulprice) {
           document.getElementById("soulprice").innerHTML = soulprice;
           $(document.getElementById("soulprice")).addClass('flash').delay(150).queue(function(next){
                $(this).removeClass('flash');
                next();
           });
         }
         cachesoulprice = document.getElementById("soulprice").innerHTML;
         localStorage.setItem('lastsoulprice', cachesoulprice);
       }).fail(function(e) {
          console.log(e);
       });
}

function numberWithCommas(x) {
    var parts = x.toString().split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return parts.join(".")
}

function toggleNightMode() {
  $('.well').toggleClass('dark-mode');
  $('.sidenav').toggleClass('dark-mode');
  // Handles toggle nightmode on refresh
  if (localStorage.getItem('themesetting') == 'dark') {
      if (onloadtoggle !== true) {
          localStorage.setItem('themesetting', 'light');
      }
  } else {
      localStorage.setItem('themesetting', 'dark');
      onloadtoggle = false;
  }
}

function toFixed(x) {
  if (Math.abs(x) < 1.0) {
    var e = parseInt(x.toString().split('e-')[1]);
    if (e) {
        x *= Math.pow(10,e-1);
        x = '0.' + (new Array(e)).join('0') + x.toString().substring(2);
    }
  } else {
    var e = parseInt(x.toString().split('+')[1]);
    if (e > 20) {
        e -= 20;
        x /= Math.pow(10,e);
        x += (new Array(e+1)).join('0');
    }
  }
  return x;
}
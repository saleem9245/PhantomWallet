function copyText(text, name) {

    navigator.clipboard.writeText(text).then(function () {
        bootbox.alert(name + " was copied to the clipboard.");
    }, function (err) {
        bootbox.alert("Could not copy " + name + "...");
    });

}

function RegisterName() {
    bootbox.prompt({
        title: "Insert a name for your address.<br>This can only be done once.",
        message: "It might take a while to update your name.",
        callback:
            function (result) {

              if (result) {
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

                  } else {
                    $('#nameSpinner').hide();
                  }
            }
    });
}

$(document).ready(function() {

    $("#editPencil").click(RegisterName);
    $("#editPencil2").click(RegisterName);

    // Custom check for night mode on initial load
    onloadtoggle = true;
    if (localStorage.getItem('themesetting') == 'dark') {
        if (document.getElementById("cb4")) {
          document.getElementById("cb4").checked = true;
        }
        toggleNightMode();
        onloadtoggle = false;
    }
    // check if height already cached
    if (document.getElementById("blockheight")) {
      document.getElementById("blockheight").innerHTML = localStorage.getItem('lastheight');
    }
    // check if price already cached
    if (document.getElementById("soulprice")) {
      document.getElementById("soulprice").innerHTML = localStorage.getItem('lastsoulprice');
    }
    getPricing();
    var pathname = window.location.pathname;
    // on all pages except login and create, getChains every 2 sec and getPricing every minute
    if (pathname != '/login' && pathname != '/create') {
      setInterval(function() {
        getChains();
      }, 2000);
      setInterval(function() {
        getPricing();
      }, 60000);
    }
});

// function getChains every 2 sec to get current blockheight
function getChains() {
   $.get('/chains',
       function (returnedData) {
         heightmain = '#' + numberWithCommas(JSON.parse(returnedData)[0].height);
         // if height exist and changed, flash height
         if (document.getElementById("blockheight").innerHTML != heightmain) {
           document.getElementById("blockheight").innerHTML = heightmain;
           $(document.getElementById("blockheight")).addClass('flash').delay(150).queue(function(next){
                $(this).removeClass('flash');
                next();
           });
         }
         // store height for later
         cacheheight = document.getElementById("blockheight").innerHTML;
         localStorage.setItem('lastheight', cacheheight);
       }).fail(function(e) {
          console.log(e);
       });
}

// function getPricing soul from coingecko
function getPricing() {

  // if session storage currency values not set, set them to USD / $
  if (localStorage.getItem('currency') !== null) {
    currency = localStorage.getItem('currency');
    if (localStorage.getItem('currency') == 'EUR') {
      currencysymbol = '€';
    } else if (localStorage.getItem('currency') == 'CAD') {
      currencysymbol = 'C$';
    } else if (localStorage.getItem('currency') == 'GBP') {
      currencysymbol = '£';
    } else if (localStorage.getItem('currency') == 'JPY') {
      currencysymbol = '¥';
    } else if (localStorage.getItem('currency') == 'AUD') {
      currencysymbol = 'A$';
    } else {
      currencysymbol = '$';
    }
  } else {
    currency = 'USD';
    currencysymbol = '$';
    localStorage.setItem('currency', currency);
  }
   $.get('https://api.coingecko.com/api/v3/simple/price?ids=phantasma&vs_currencies=' + currency.toLowerCase(),
       function (returnedData) {
         soulprice = currencysymbol + numberWithCommas(returnedData.phantasma[currency.toLowerCase()]);
         if (document.getElementById("soulprice")) {
           // if price exist and changed, flash price
           if (document.getElementById("soulprice").innerHTML != soulprice) {
             document.getElementById("soulprice").innerHTML = soulprice;
             $(document.getElementById("soulprice")).addClass('flash').delay(150).queue(function(next){
                  $(this).removeClass('flash');
                  next();
             });
           }
           // store price for later
           cachesoulprice = document.getElementById("soulprice").innerHTML;
           localStorage.setItem('lastsoulprice', cachesoulprice);
         }
       }).fail(function(e) {
          console.log(e);
       });
}

// function add comma to number
function numberWithCommas(x) {
    var parts = x.toString().split(".");
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    return parts.join(".")
}

// function toggle on/off dark theme
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

// function toggle on/off cosmic/chain swap
function toggleSwap() {
  $("#wrapper-swap-confirm").fadeTo(1, 0);
  $('#cosmicswap').toggleClass('highlighted');
  $('#chainswap').toggleClass('highlighted');

  if($('#cosmicswapdiv').css('display') === 'none')
  {
    $('#cosmicswapdiv').show();
    $('#chainswapdiv').hide();
  }
  else
  {
    $('#cosmicswapdiv').hide();
    $('#chainswapdiv').show();
  }
  $('#neoaddresslink').val('');
  $('#neohash').val('');
  $('#neoamount').val('');
  $('#neodstaddress').val('');
  $('#neopassphrase').val('');
  $('#neoprivatekey').val('');
}

// function toggle neo/phantasma chains
function toggleChains() {
  $("#wrapper-swap-confirm").fadeTo(1, 0);
  if ($('#neotophantasma').css('display') === 'none')
    {
      $('#neotophantasma').show();
      $('#phantasmatoneo').hide();
      $(".fa-sync").addClass("fa-spin");
      document.getElementById("chainswapdesc").innerHTML = 'From NEO Blockchain to Phantasma Blockchain';
      setTimeout(function() {
        $(".fa-sync").removeClass("fa-spin");
      }, 1000);
    }
    else
    {
      $('#neotophantasma').hide();
      $('#phantasmatoneo').show();
      $(".fa-sync-chains").addClass("fa-spin");
      document.getElementById("chainswapdesc").innerHTML = 'From Phantasma Blockchain to NEO Blockchain';
      setTimeout(function() {
        $(".fa-sync-chains").removeClass("fa-spin");
      }, 1000);
    }
    $('#neoaddresslink').val('');
    $('#neohash').val('');
    $('#neoamount').val('');
    $('#neodstaddress').val('');
    $('#neopassphrase').val('');
    $('#neoprivatekey').val('');
}

// function toggle private key / encrypted key
function toggleKeyType() {
  if ($('#neopassphrase').css('display') === 'none')
    {
      $('#neopassphrase').show();
      $(".fa-sync-neo").addClass("fa-spin");
      document.getElementById("keytypetoggle").innerHTML = 'Encrypted Key + Password';
      setTimeout(function() {
        $(".fa-sync-neo").removeClass("fa-spin");
      }, 1000);
    }
    else
    {
      $('#neopassphrase').hide();
      $(".fa-sync-neo").addClass("fa-spin");
      document.getElementById("keytypetoggle").innerHTML = 'Private Key';
      setTimeout(function() {
        $(".fa-sync-neo").removeClass("fa-spin");
      }, 1000);
    }
    $('#neopassphrase').val('');
    $('#neoprivatekey').val('');
    $('#neohash').val('');
}

// function fixed decimals value
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
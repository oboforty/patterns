// ==UserScript==
// @name         ATW regkieg
// @namespace    http://tampermonkey.net/
// @version      0.1
// @description  try to take over the world!
// @author       You
// @match        https://secure.atw.hu/*
// @grant        none
// ==/UserScript==

var nmID = '19bd12f9-16c7-41ce-8b02-991a7ea68735';
var email = 'rapandwarez@nincsmail.com';
var passw = '';

var BASEROUTE = 'http://secure.atw.hu/';
var ROUTE = window.location.href.substr(BASEROUTE.length+1);
if(ROUTE.indexOf('?') !== -1)
    ROUTE = ROUTE.substring(0, ROUTE.indexOf('?'));



// mnml jQuery
var $ = function(sel) {
    mark = sel.substring(0,1);
    sel = sel.substring(1);

    if(mark == '#') {
        var elem = document.getElementById(sel);
    } else if(mark == '=') {
        var elem = document.getElementsByName(sel)[0];
    } else if(mark == '.') {
        var elem = document.getElementsByClassName(sel)[0];
    }

    if(!elem)
        return null;

    elem.val = function(str0) {
        if(typeof str0 === 'undefined')
            return this.value;
        this.value = str0;
    };

    return elem;
};

String.prototype.replaceAll = function(search, replace) {
    if (replace === undefined) {
        return this.toString();
    }
    return this.split(search).join(replace);
};

function trans_captcha(captcha) {
    var repl = {
        'tíz' :  '10 + ',
        'tizen' :  '10 + ',
        'kettő' :  2,
        'két' :  2,
        'húsz' :  '20 + ',
        'huszon' :  '20 + ',
        'három' :  3,
        'harminc' :  '30 + ',
        'négy' :  4,
        'negyven' :  '40 + ',
        'öt' :  5,
        'hat' :  6,
        'hét' :  7,
        'het' :  7,
        'nyolc' :  8,
        'kilenc' : 9,
        'egy' :  1,
        'negy' : 4
    };

    var captcha = captcha.replace('száz', '*100 + ');
    captcha = captcha.replace('ven', '*10 + ');
    captcha = captcha.replace('van', '*10 + ');

    for(var rp in repl) {
        captcha = captcha.replaceAll(rp, repl[rp]);
    }
    if(captcha.substr(0,1) == '*')
        captcha = '1' + captcha;
    if(captcha.substr(-2) == '+ ')
        captcha = captcha + '0';
    captcha = captcha.replace('n1', '4');

    return captcha;
}

function redirect(loc) {
    window.location = loc;
}

function get_cookie(name) {
  var value = "; " + document.cookie;
  var parts = value.split("; " + name + "=");
  if (parts.length == 2) return parts.pop().split(";").shift();
}

function delete_cookie( name ) {
  document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}




/* * * * * * * * * * * * *
 *      REGISZTRACIO     *
 * * * * * * * * * * * * */
if(ROUTE == 'regisztracio') {
    $('=email').val(email);
    $('#szabalyzat').checked = true;

    //$('name=username').val('')
    $('=pass1').val(passw);
    $('=pass2').val(passw);

    $('=username').onkeyup=function(){
        $('=description').val(this.value);
        $('=sitename').val(this.value);
    };

    $('=kategoria').selectedIndex = Math.random() * 41 + 1;

    // captcha
    var asd = $('=reginput').parentElement.parentElement.parentElement.getElementsByTagName('b')[0];
    var kif = trans_captcha(asd.innerHTML);

    console.log('Captcha: ', kif);
    var captcha_megfejt = eval(kif + ';');
    $('=reginput').val(captcha_megfejt);

    $('=regform').onsubmit = function() {
        var name = $('name=username').value;
        document.cookie = "atwname=" + name;

        return true;
    };
}

if(ROUTE == 'regisztracio#regform') {
    redirect('http://nincsmail.hu/' + nmID);
}

if(ROUTE == 'aktival') {
    redirect(BASEROUTE + 'login');
}

if(ROUTE == 'login') {
    var name = get_cookie('atwname');
    delete_cookie('atwname');

    if(name) {
        $('=n001').val(name);
        $('=n002').val(passw);

        $('=login').submit();
    } else {
        console.log('No name found in cookie');
    }
}

if(ROUTE == 'adatmodositas') {
    redirect(BASEROUTE + 'mysql');
}

if(ROUTE == 'mysql') {
    $('=password1').val(passw);
    $('=password2').val(passw);

    $('=submit').click();

    setTimeout(function(){
        redirect(BASEROUTE + 'logout');
    }, 800);
}
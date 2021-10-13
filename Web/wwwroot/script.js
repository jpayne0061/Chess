var URL_ROOT = "https://chesswithhotsauce.azurewebsites.net";
//var URL_ROOT = "https://localhost:44320";

var GAME_ID = "";

var connection = null;

var container = document.getElementById('game-container');

var CHECK_MATE = false;

var PLAYER_COLOR = 0;

var YOUR_TURN = false;

var INVALID_PLAY_TYPES = {
    0: "Wrong player",
    1 : "Out of turn"
};

var pieceLookup = {
    "whiterook": "&#9814;",
    "whiteknight": "&#9816;",
    "whitebishop": "&#9815;",
    "whitequeen": "&#9813;",
    "whiteking": "&#9812;",
    "whitepawn": "&#9817;",
    "blackrook": "&#9820;",
    "blackknight": "&#9822;",
    "blackbishop": "&#9821;",
    "blackqueen": "&#9819;",
    "blackking": "&#9818;",
    "blackpawn": "&#9823;"
};

var globalStart = null;
var globalEnd = null;

function setUp() {
    connectToHub();

    buildBoard();

    hideGameInputs();

    PLAYER_COLOR = 0;
}

function joinGame() {

    GAME_ID = document.getElementById('join-game').value;

    checkIfGameIdValid(setUp);
}



function startGame() {
    //var gameId = document.getElementById('start-game').value;

    //GAME_ID = gameId;
    startNewGame();

    hideGameInputs();

    buildBoard();

    rotateBoard();

    PLAYER_COLOR = 1;

    YOUR_TURN = true;

    document.getElementById('messages').innerHTML = 'it is your turn';
}

function rotateBoard() {
    container.classList.add("rotated");

    var pieces = container.children;

    for (var i = 0; i < pieces.length; i++) {
        pieces[i].classList.add("rotated");
    }

}

function connectToHub() {
    connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    connection.on(GAME_ID, function (message) {
        var command = message.command;

        if (message.turn === PLAYER_COLOR) {
            document.getElementById('messages').innerHTML = 'it is your turn';
        }

        movePiece(message, command);
    });

    connection.start();
}

function hideGameInputs() {
    document.getElementById('join-start').style.display = "none";
}

function restartGame() {
    CHECK_MATE = false;

    container.innerHTML = '';
    document.getElementById('white-captured-pieces').innerHTML = '';
    document.getElementById('black-captured-pieces').innerHTML = '';

    buildBoard();

    startNewGame();
}


function startNewGame() {
    var xhttp = new XMLHttpRequest();
    xhttp.open("GET", URL_ROOT + "/api/values/SetGame", true);
    xhttp.send();

    xhttp.onload = function () {
        console.log("post game key response text: ", this.responseText);

        GAME_ID = this.responseText;

        connectToHub();

        alert('Your game key is ' + GAME_ID);

        document.getElementById('game-key').innerHTML = GAME_ID;
    };
}

function checkIfGameIdValid(setUp) {
    var xhttp = new XMLHttpRequest();
    xhttp.open("GET", URL_ROOT + "/api/GameState/" + GAME_ID, true);
    xhttp.send();

    xhttp.onload = function () {
        console.log("response text: ", this.responseText);

        if ('true' === this.responseText) {
            setUp();
        }
        else {
            alert('you have not entered a valid game id');
            return;
        }
    };
}


function buildBoard() {
    var x = 0;
    var y = 0;

    for (var i = 0; i < 8; i++) {
        for (var j = 0; j < 8; j++) {
            var node = document.createElement("div");

            node.setAttribute('dataX', j % 8);
            node.setAttribute('dataY', y);

            var locationCoords = y.toString() + (j % 8).toString();

            node.setAttribute('id', locationCoords);

            switch (locationCoords) {
                case "00":
                    node.innerHTML = "&#9814;"; //whiterook
                    break;
                case "01":
                    node.innerHTML = "&#9816;"; //whiteknight
                    break;
                case "02":
                    node.innerHTML = "&#9815;"; //whitebishop
                    break;
                case "03":
                    node.innerHTML = "&#9812;"; //whiteking
                    break;
                case "04":
                    node.innerHTML = "&#9813;"; //whitequeen
                    break;
                case "05":
                    node.innerHTML = "&#9815;"; //whitebishop
                    break;
                case "06":
                    node.innerHTML = "&#9816;"; //whiteknight
                    break;
                case "07":
                    node.innerHTML = "&#9814;"; //whiterook
                    break;
                case "10":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "11":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "12":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "13":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "14":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "15":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "16":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "17":
                    node.innerHTML = "&#9817;"; //whitepawn
                    break;
                case "70":
                    node.innerHTML = "&#9820;";
                    break;
                case "71":
                    node.innerHTML = "&#9822;";
                    break;
                case "72":
                    node.innerHTML = "&#9821;";
                    break;
                case "73":
                    node.innerHTML = "&#9818;";
                    break;
                case "74":
                    node.innerHTML = "&#9819;";
                    break;
                case "75":
                    node.innerHTML = "&#9821;";
                    break;
                case "76":
                    node.innerHTML = "&#9822;";
                    break;
                case "77":
                    node.innerHTML = "&#9820;";
                    break;
                case "60":
                    node.innerHTML = "&#9823;";
                    break;
                case "61":
                    node.innerHTML = "&#9823;";
                    break;
                case "62":
                    node.innerHTML = "&#9823;";
                    break;
                case "63":
                    node.innerHTML = "&#9823;";
                    break;
                case "64":
                    node.innerHTML = "&#9823;";
                    break;
                case "65":
                    node.innerHTML = "&#9823;";
                    break;
                case "66":
                    node.innerHTML = "&#9823;";
                    break;
                case "67":
                    node.innerHTML = "&#9823;";
                    break;
                default:
                    node.innerHTML = "&#9823;";
                    node.style.color = x % 2 === 0 ? '#adadad' : '#666666';
            }

            if (locationCoords[0] === "0" || locationCoords[0] === "1") {
                node.style.color = "#ebebeb";
            }

            node.style.backgroundColor = x % 2 === 0 ? '#adadad' : '#666666';
            node.style.height = '48px';
            node.style.width = '48px';
            node.style.fontSize = '34px';
            node.style.paddingLeft = '6px';
            node.style.display = 'inline-block';
            node.onclick = getCoordinates;

            container.appendChild(node);
            x++;
        }

        x++;
        y++;

        var breakNode = document.createElement("div");
        container.appendChild(breakNode);
    }
}



function getCoordinates(e) {

    console.log("e: ", e);

    if (CHECK_MATE) {
        alert("Game has ended in check mate.");
        return;
    }

    var x = e.target.getAttribute('datax');
    var y = e.target.getAttribute('datay');

    var character = document.getElementById(y.toString() + x.toString()).innerHTML;

    if ((character.charCodeAt(0) <= 9817 && PLAYER_COLOR !== 1 ||
        character.charCodeAt(0) > 9817 && PLAYER_COLOR !== 0) &&
        globalStart === null) {
        return;
    }

    giveBorder(e);

    if (globalStart === null) {


        globalStart = { x: x, y: y };
        return;
    }
    else {
        if (globalStart.x === x && globalStart.y === y) {
            globalStart = null;
            globalEnd = null;
            removeBorder(e);
            return;
        }

        globalEnd = { x: x, y: y };

        var move = globalStart.y + globalStart.x + ' ' + globalEnd.y + globalEnd.x + ' ' + GAME_ID;

        var xhttp = new XMLHttpRequest();
        xhttp.open("GET", URL_ROOT + "/api/values/" + encodeURIComponent(move), true);
        xhttp.send();

        xhttp.onload = function () {
            var playResult = JSON.parse(this.responseText);
            console.log(playResult);

            document.getElementById("messages").innerHTML = playResult.Message;

            if (playResult.IsCheck) {
                if (playResult.IsCheckMate) {
                    alert("Check mate");
                    CHECK_MATE = true;
                    document.getElementById("restart-game").style.display = "block";
                }
                else {
                    document.getElementById("messages").innerHTML = "Check";
                }

            }


            if (playResult.PlayValid) {
                movePiece(playResult, move);
            }
            else {
                removeBorders();
                globalStart = null;
                globalEnd = null;
            }

        };
    }
}

function movePiece(playResult, command) {

    cmdArgs = command.split(' ');
    start = cmdArgs[0];
    end = cmdArgs[1];

    var el = document.getElementById(start);

    var character = el.innerHTML;

    el.style.color = el.style.backgroundColor;


    var elTo = document.getElementById(end);
    elTo.innerHTML = character;

    if (character.charCodeAt(0) <= 9817) {
        elTo.style.color = "#ebebeb";
    }
    else {
        elTo.style.color = "#000";
    }

    if (globalStart !== null) {
        removeBorders();
    }


    globalStart = null;
    globalEnd = null;

    if (playResult.CapturedPiece !== null) {
        var color = playResult.CapturedPiece.Color === 0 ? "black" : "white";

        console.log("color: ", color);

        var piece = pieceLookup[color + playResult.CapturedPiece.Name.toLowerCase()];

        document.getElementById(color.toLowerCase() + "-captured-pieces").innerHTML += piece;
    }
}

function giveBorder(e) {
    var element = e.target;

    element.style.boxShadow = "0px 0px 0px 2px black inset";
}

function removeBorder(e) {
    var element = e.target;

    element.style.boxShadow = "0px 0px 0px 0px black inset";
}

function removeBorders() {
    document.getElementById(globalEnd.y + globalEnd.x).style.boxShadow = "0px 0px 0px 0px black inset";
    document.getElementById(globalStart.y + globalStart.x).style.boxShadow = "0px 0px 0px 0px black inset";
}

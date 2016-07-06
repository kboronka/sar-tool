var app = angular.module('testApp', ['luegg.directives']);

app.controller("WebSocketController", function($scope) {
  //var ws = new ReconnectingWebSocket("ws://localhost:81/Test");
  $scope.log = {
    messages: [ "init" ]
  };
  log = function(message)
  {
    $scope.log.messages.push(message);
  };

  $scope.jogxp = function() {
    log("jog x+");
    ws.send("jog x+");
  }

  $scope.jogxn = function() {
      log("jog x-");
      ws.send("jog x-");
  }

  //log("test");

  var ws = new ReconnectingWebSocket("ws://localhost:81/Test", null, {debug: true, reconnectInterval: 3000});
  ws.onopen = function() {
    //$scope.console.log("Websocket is open");
    log("Websocket is open");
    $scope.$apply();
  };

  ws.onmessage = function (evt)  {
    log("data recived: " + evt.data);
    $scope.$apply();
  };

  ws.onclose = function() {
    log("websocket is closed")
    $scope.$apply();
  };
});

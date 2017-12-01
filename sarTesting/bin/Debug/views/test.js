var app = angular.module('testApp', []);

app.directive("console", function() {
  return {
    link: function(scope, el, attrs) {
      scope.console = {
        log : function()  {
          var date = new Date();
          el.append(
            "<div>" +
              date.getHours() + ":" +
              date.getMinutes() + ":" +
              date.getSeconds() + ":" +
              date.getMilliseconds() +
              " :" +
              Array.prototype.slice.call(arguments).join(", ") +
            "<div>"
          );
        }
      };
    }
  };
});

app.controller("WebSocketController", function($scope) {
  //var ws = new ReconnectingWebSocket("ws://localhost:81/Test");
  var ws = new ReconnectingWebSocket("ws://localhost:81/Test", null, {debug: true, reconnectInterval: 3000});
  ws.onopen = function() {
    $scope.console.log("Websocket is open");
  };

  ws.onmessage = function (evt)  {
    $scope.console.log("data recived: " + evt.data);

    var data = evt.data[0] === "{" ? JSON.parse(evt.data) : evt.data;
    if (typeof data == "object") {
      $scope.console.log("received", data);
      $scope.$broadcast('received', data);
    }
  };

  ws.onclose = function() {
    $scope.console.log("websocket is closed")
  };
});

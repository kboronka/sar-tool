var app = angular.module('testApp', ['ngFileUpload']);

app.controller("WebSocketController", function($scope) {
  //var ws = new ReconnectingWebSocket("ws://localhost:81/Test");
  $scope.log = {
    messages: [ "init" ]
  };
  log = function(message)
  {
    $scope.log.messages.push(message);
  };

  $scope.gcode = "G90\n"
  $scope.gcode += "G1 Z-20 F100\n"
  $scope.gcode += "G1 X-50 F1500\n"
  $scope.gcode += "G1 X0 Y50\n"
  $scope.gcode += "G1 X50 Y0\n"
  $scope.gcode += "G1 X0 Y-50\n"
  $scope.gcode += "G1 X-50 Y0\n"
  $scope.gcode += "G1 Y25\n"
  $scope.gcode += "G1 X-75\n"
  $scope.gcode += "G1 Z0\n"
  $scope.gcode += "G1 X0 Y0";
  $scope.distance = 10;
  $scope.machine = {
    status : {},
    x : 0,
    y : 0,
    z : 0
  }

  $scope.work = {
    x : 0,
    y : 0,
    z : 0
  }

  $scope.commands = [];

  $scope.jogxp = function() {
    var distance = 10;
    ws.send("G91 X" + distance.toString());
  }

  $scope.jogxn = function() {
    var distance = 10;
    ws.send("G91 X-" + distance.toString());
  }

  $scope.grbl = function(command)
  {
    ws.send(command);
  }

  $scope.reset = function()
  {
    ws.send(String.fromCharCode(24));
  }

  //log("test");

  var ws = new ReconnectingWebSocket("ws://localhost:80/Test", null, {debug: true, reconnectInterval: 100});
  ws.onopen = function() {
    //$scope.console.log("Websocket is open");
    log("Websocket is open");
    $scope.$apply();
  };

  ws.onmessage = function (evt)  {
    // sarCNC JSON status
    if (evt.data.startsWith("{") && evt.data.endsWith("}"))
    {
      var data = JSON.parse(evt.data);
      if (data.status)
      {
        $scope.machine = data;
        /*
        $scope.machine.running = data.status.running;
        $scope.machine.x = data.status.mX;
        $scope.machine.y = data.status.mY;
        $scope.machine.z = data.status.mZ;
        $scope.work.x = data.status.wX;
        $scope.work.y = data.status.wY;
        $scope.work.z = data.status.wZ;
        */
      }
    }

    $scope.$apply();
  };

  ws.onclose = function() {
    log("websocket is closed")
    $scope.$apply();
  };
});

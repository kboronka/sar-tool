(function() {
  app.filter('readableTime', function() {
    return function(ms) {
      var seconds = ms / 1000;
      var minutes = seconds / 60;
      var hours = minutes / 60;

      if (seconds < 1) {
        return ms.toFixed(0) + " ms";
      }
      else if (minutes < 1.5) {
        return minutes.toFixed(1) + " s";
      }
      else if (minutes < 1.5) {
        return minutes.toFixed(1) + " s";
      }
      else if (minutes < 90) {
          return minutes.toFixed(0) + " m";
      }
      else {
        return hours.toFixed(0) + " hr";
      }
    };
  });
})();

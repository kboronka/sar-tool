app.controller('ProductEditorCtrl', ['$log', '$scope', 'Upload', function($log, $scope, Upload){

  var self = this;
  $scope.busy = true;
  $scope.ready = false;

  $scope.files = [];

  $scope.$watch('files', function () {
    $scope.upload($scope.files);
  });

  $scope.upload = function (files) {
    $log.debug("upload... ", files);

        if (files && files.length) {
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                Upload.upload({
                    url: '/Test/Ul',
                    fields: {
                      'filecontext': 'product',
                    },
                    file: file
                }).progress(function (evt) {
                    var progressPercentage = parseInt(100.0 * evt.loaded / evt.total);
                    $log.debug('progress: ' + progressPercentage + '% ' + evt.config.file.name);
                }).success(function (data, status, headers, config) {
                    $log.debug('file ' + config.file.name + 'uploaded. Response: ' + data);
                });
            }
        }
    };
}]);

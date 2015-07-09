/*globals angular */

(function () {

    // see: http://stackoverflow.com/a/6491621/605586
    Object.byString = function (o, s) {
        s = s.replace(/\[(\w+)\]/g, '.$1'); // convert indexes to properties
        s = s.replace(/^\./, '');           // strip a leading dot
        var a = s.split('.');
        for (var i = 0, n = a.length; i < n; ++i) {
            var k = a[i];
            if (k in o) {
                o = o[k];
            } else {
                return;
            }
        }
        return o;
    }

    'use strict';

    angular.module('tqFormExtensions', [])
      .directive('tqExternalFormConfig', ['$compile', '$parse', function ($compile, $parse) {
          return {
              mergedAttrs: [],
              restrict: 'A',
              scope: false,
              replace: true,
              priority: 500,
              terminal: true,
              compile: function () {
                  var _this = this;
                  return function (scope, elem, attr) {
                      var validationConfig = $parse(attr.tqExternalFormConfig)(scope);
                      elem.removeAttr("tq-external-form-config");
                      if (validationConfig) {
                          _this.findFormElements(elem.children(), validationConfig);
                      }
                      $compile(elem)(scope);
                  };
              },
              findFormElements: function (elements, validationConfig) {
                  var _this = this;
                  angular.forEach(elements, function (element) {
                      var jqElm = angular.element(element);
                      var modelAttr = jqElm.attr('ng-model')
                          || jqElm.attr('data-ng-model')
                          || jqElm.attr('ngModel')
                          || jqElm.attr('aa-field')
                          || jqElm.attr('data-aa-field')
                          || jqElm.attr('aa-field-group')
                          || jqElm.attr('data-aa-field-group');
                      if (modelAttr) {
                          _this.processElement(jqElm, modelAttr, validationConfig);
                      }
                      _this.findFormElements(jqElm.children(), validationConfig);
                  });
              },
              processElement: function (jqElm, nameAttr, validationConfig) {
                  if (!jqElm.attr('name')) {
                      jqElm.attr('name', nameAttr.substring(nameAttr.indexOf('.') + 1));
                  }
                  this.addAttributes(jqElm, Object.byString(validationConfig.validations
                      , nameAttr.substring(nameAttr.indexOf('.') + 1)));
              },
              addAttributes: function (jqElm, attrs) {
                  for (var name in attrs) {
                      if (name !== 'required') {
                          jqElm.attr(name, attrs[name]);
                      } else {
                          jqElm.prop(name, attrs[name]);
                      }
                  }
              }
          };
      }]);
})();
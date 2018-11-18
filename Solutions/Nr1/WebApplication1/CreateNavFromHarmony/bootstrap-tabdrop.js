/*
    jaknor 2014-01-28 - 
    I have modified this script to add support for drop down tab menu items. 
    A drop down menu item will be converted to a drop down sub menu item when 
    it can no longer fit on the page and it is put into the dropdown menu. 
    This is due to the drop down menu not working while inside another drop 
    down menu.
    
    There is also a script in dls.elements.js that makes sure the active tab 
    is set correctly.

    I’ve only added support for one level of the drop down menu so far and I 
    do not think it will work if you have drop down menus inside of drop down 
    menus.

    markauer 2014-03-04
    Registered resize to be called when all ajax calls have been completed.  
    This is so that additional menu items will be added to the dropdown menu
    if required if the page that is being loaded causes the scrollbar to apear.

    markauer 2015-10-08
    Changed dropdown menu containing hidden elemenets to be added at the end of the menu as the menu is now left aligned.
*/

/* =========================================================
 * bootstrap-tabdrop.js 
 * http://www.eyecon.ro/bootstrap-tabdrop
 * =========================================================
 * Copyright 2012 Stefan Petre
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * ========================================================= */

!function ($) {

    var WinReszier = (function () {
        var registered = [];
        var inited = false;
        var timer;
        var resize = function (ev) {
            clearTimeout(timer);
            timer = setTimeout(notify, 100);
        };
        var notify = function () {
            for (var i = 0, cnt = registered.length; i < cnt; i++) {
                registered[i].apply();
            }
        };
        return {
            register: function (fn) {
                registered.push(fn);
                if (inited === false) {
                    $(window).bind('resize', resize);
                    $(document).ajaxStop(resize);
                    inited = true;
                }
            },
            unregister: function (fn) {
                for (var i = 0, cnt = registered.length; i < cnt; i++) {
                    if (registered[i] == fn) {
                        delete registered[i];
                        break;
                    }
                }
            }
        }
    } ());

    var TabDrop = function (element, options) {
        this.element = $(element);
        this.dropdown = $('<li class="dropdown hide pull-right tabdrop"><a class="dropdown-toggle" data-toggle="dropdown" href="#">' + options.text + ' <b class="caret"></b></a><ul class="dropdown-menu"></ul></li>')
							.prependTo(this.element);
        if (this.element.parent().is('.tabs-below')) {
            this.dropdown.addClass('dropup');
        }
        WinReszier.register($.proxy(this.layout, this));
        this.layout();
    };

    TabDrop.prototype = {
        constructor: TabDrop,

        layout: function () {
            var collection = [];
            this.dropdown.removeClass('hide');

            var element = this.element;
            this.dropdown.find('li').not($('.dropdown-submenu li')).each(function () {
                if ($(this).hasClass('dropdown-submenu')) {
                    $(this).removeClass('dropdown-submenu');
                    $(this).removeClass('pull-left');
                    $(this).addClass('dropdown');
                }

                element.append($(this));
            });

            element
				.find('>li')
				.not('.tabdrop')
				.each(function () {
				    if (this.offsetTop > 0) {
				        collection.push(this);
				    }
				});
            if (collection.length > 0) {
                collection = $(collection);
                collection.each(function () {
                    if ($(this).hasClass('dropdown')) {
                        $(this).removeClass('dropdown');
                        $(this).addClass('dropdown-submenu');
                        $(this).addClass('pull-left');
                    }
                });
                this.dropdown
					.find('ul')
					.empty()
					.append(collection);
                if (this.dropdown.find('.active').length == 1) {
                    this.dropdown.addClass('active');
                } else {
                    this.dropdown.removeClass('active');
                }
            } else {
                this.dropdown.addClass('hide');
            }
        }
    }

    $.fn.tabdrop = function (option) {
        return this.each(function() {
            var $this = $(this),
                data = $this.data('tabdrop'),
                options = typeof option === 'object' && option;
            if (!data) {
                $this.data('tabdrop', (data = new TabDrop(this, $.extend({}, $.fn.tabdrop.defaults, options))));
            }
            if (typeof option == 'string') {
                data[option]();
            }
        });
    };

    $.fn.tabdrop.defaults = {
        text: '<i class="icon-align-justify"></i>'
    };

    $.fn.tabdrop.Constructor = TabDrop;

} (window.jQuery);
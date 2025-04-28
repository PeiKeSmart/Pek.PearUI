layui.define(['layer', 'toastr', 'ztree', 'ztreecheck', 'pjax'], function (exports) {
    "use strict";

    var $ = layui.jquery,
        layer = layui.layer,
        ztree = layui.ztree,
        zcheck = layui.ztreecheck,
        toastr = layui.toastr,
        pjax = layui.pjax;
    toastr.options = {
        "positionClass": "toast-top-right",
        "timeOut": "1500"
    };
    var tmls, tool = {
        error: function (msg) {
            toastr.error(msg);
        },
        warning: function (msg) {
            toastr.warning(msg);
        },
        success: function (msg) {
            toastr.success(msg);
        },
        info: function (msg) {
            toastr.info(msg);
        },
        ajax: function (url, options, callFun, method = 'post', isasync = true) {
            var httpUrl = "/";
            var _headers = {};
            if (token !== null) {
                _headers = {
                    'Authorization': 'Bearer ' + token
                };
            }
            options = method === 'get' ? options : JSON.stringify(options);
            //console.log(_headers);
            //console.log(options);
            $.ajax(httpUrl + url, {
                data: options,
                contentType: 'application/json',
                dataType: 'json', //服务器返回json格式数据
                type: method, //HTTP请求类型
                timeout: 10 * 1000, //超时时间设置为50秒；
                headers: _headers,
                async: isasync,
                success: function (data) {
                    callFun(data);
                },
                error: function (xhr, type, errorThrown) {
                    if (type === 'timeout') {
                        tool.error('连接超时，请稍后重试！');
                    } else if (type === 'error') {
                        tool.error('连接异常，请稍后重试！');
                    }
                }
            });
        },
        Open: function (title, url, width, height, fun) {
            top.layer.open({
                type: 2,
                title: title,
                shadeClose: false,
                shade: 0.2,
                //move:false,
                skin: 'layer-cur-open',
                maxmin: false, //开启最大化最小化按钮
                area: [width, height],
                content: url,
                resize: false,
                zIndex: "10000",
                end: function () {
                    if (fun) fun();
                }
            });
        },
        OpenNoTop: function (title, url, width, height, fun) {
            layer.open({
                type: 2,
                title: title,
                shadeClose: false,
                shade: 0.2,
                //move:false,
                skin: 'layer-cur-open',
                maxmin: false, //开启最大化最小化按钮
                area: [width, height],
                content: url,
                resize: false,
                zIndex: "10000",
                end: function () {
                    if (fun) fun();
                }
            });
        },
        OpenRight: function (title, url, width, height, fun, cancelFun) {
            var index = layer.open({
                title: title
                , type: 2
                , area: [width, height]
                , shade: [0.1, '#333']
                , resize: false
                , move: false
                , anim: -1
                , offset: 'rb'
                , zIndex: "1000"
                , shadeClose: false
                , skin: 'layer-anim-07'
                , content: url
                , end: function () {
                    if (fun) fun();
                }
                , cancel: function (index) {
                    if (cancelFun) cancelFun(index);
                }
            });
            return index;
        },
        closeOpen: function () {
            layer.closeAll();
        },
        tableLoading: function () {
            tmls = layer.msg('<i class="layui-icon layui-icon-loading layui-icon layui-anim layui-anim-rotate layui-anim-loop"></i> 正在加载数据哦', { time: 20000 });
        },
        tableLoadingClose: function () {
            setTimeout(function () {
                layer.close(tmls);
            }, 500);
        },
        load: function () {
            $('body').append('<div class="loader-cur-wall"><div class="loader-cur"></div></div>');
        },
        loadClose: function () {
            setTimeout(function () {
                $('.loader-cur-wall').remove();
            }, 100);
        },
        getUrlParam: function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        },
        formatdate: function (str) {
            if (str) {
                var d = eval('new ' + str.substr(1, str.length - 2));
                var ar_date = [
                    d.getFullYear(), d.getMonth() + 1, d.getDate()
                ];
                for (var i = 0; i < ar_date.length; i++) ar_date[i] = dFormat(ar_date[i]);
                return ar_date.slice(0, 3).join('-') + ' ' + ar_date.slice(3).join(':');

                function dFormat(i) { return i < 10 ? "0" + i.toString() : i; }
            } else {
                return "无信息";
            }
        },
        SetSession: function (key, options) {
            localStorage.setItem(key, JSON.stringify(options));
        },
        GetSession: function (key) {
            var obj = localStorage.getItem(key);
            if (obj != null) {
                return JSON.parse(obj);
            }
            return null;
        },
        SetPageNum: function (key) {
            sessionStorage.setItem(key, $(".layui-laypage-em").next().html() == undefined ? 1 : $(".layui-laypage-em").next().html());
        },
        GetPageNum: function (key) {
            return sessionStorage.getItem(key) == undefined ? 1 : parseInt(sessionStorage.getItem(key))
        },
        /**
         * 删除键值对json
         * @param {key} key : 键
         */
        SessionRemove: function (key) {
            localStorage.removeItem(key);
        },
        /**
         * 打印日志到控制台
         * @param {data} data : Json
         */
        log: function (data) {
            console.log(JSON.stringify(data));
        },
        cloudFile: function () {
            $(".fyt-cloud").click(function () {
                var input_text = $(this).data("text");
                var showImg = $(this).data('img');
                var type = $(this).data('type'); //edit=编辑器  sign=默认表单  iframe=弹出层  form=带图片显示
                var frameId = window.frameElement && window.frameElement.id || '', frameUrl = '';
                if (frameId) {
                    frameUrl = '&frameid=' + frameId;
                }
                tool.Open('媒体资源库', '/' + AdminPath + '/file/cloud/?type=' + type + '&img=' + showImg + '&control=' + input_text + frameUrl, '950px', '600px');
            });
        },
        isExtImage: function (name) {
            var imgExt = new Array(".png", ".jpg", ".jpeg", ".bmp", ".gif");
            name = name.toLowerCase();
            var i = name.lastIndexOf(".");
            var ext;
            if (i > -1) {
                ext = name.substring(i);
            }
            for (var i = 0; i < imgExt.length; i++) {
                if (imgExt[i] === ext)
                    return true;
            }
            return false;
        },
        /**
         * 自动调整操作列宽度的通用方法
         * 根据按钮的实际宽度和间距计算操作列所需的精确宽度
         */
        adjustOperationColumnWidth: function () {
            // 获取实际渲染在页面上的按钮
            var $actualButtons = $('.layui-table-fixed-r .layui-table-body tr:first-child .pear-btn');
            var operationWidth = 50; // 默认最小宽度

            if ($actualButtons.length > 0) {
                // 获取第一个按钮的左边距
                var firstButtonLeftMargin = parseInt($actualButtons.first().css('margin-left')) || 0;
                var firstButtonParentPadding = parseInt($actualButtons.first().parent().css('padding-left')) || 0;
                var firstButtonTotalLeftMargin = firstButtonLeftMargin + firstButtonParentPadding;

                // 获取所有按钮的宽度
                var totalButtonsWidth = 0;
                $actualButtons.each(function (index) {
                    totalButtonsWidth += $(this).outerWidth();
                });

                // 获取按钮之间的间距
                var totalSpacingsWidth = 0;
                for (var i = 0; i < $actualButtons.length - 1; i++) {
                    var button1 = $actualButtons[i];
                    var button2 = $actualButtons[i + 1];
                    var button1Right = $(button1).offset().left + $(button1).outerWidth();
                    var button2Left = $(button2).offset().left;
                    var spacing = button2Left - button1Right;
                    totalSpacingsWidth += spacing;
                }

                // 获取最后一个按钮的右边距（假设与左边距相同）
                var lastButtonRightMargin = firstButtonTotalLeftMargin;

                // 计算实际占用的总宽度
                operationWidth = firstButtonTotalLeftMargin + totalButtonsWidth + totalSpacingsWidth + lastButtonRightMargin;
            }

            // 使用计算出的实际宽度来调整操作列
            // 设置固定列宽度
            $('.layui-table-fixed-r').width(operationWidth);
            $('.layui-table-fixed-r .layui-table-header').width(operationWidth);
            $('.layui-table-fixed-r .layui-table-body').width(operationWidth);

            // 设置固定列中的单元格宽度
            $('.layui-table-fixed-r .layui-table-cell').width(operationWidth - 10); // 减去一些内边距

            // 确保按钮容器有足够的宽度
            $('.layui-table-fixed-r .layui-btn-container').width(operationWidth - 10);

            // 同时设置主表格中对应列的宽度
            var lastColIndex = $('.layui-table-header th:last').index();
            $('.layui-table-header th:eq(' + lastColIndex + ')').width(operationWidth);
            $('.layui-table-main td[data-field="' + lastColIndex + '"]').width(operationWidth);

            // 隐藏主表格中的操作列内容，避免重复显示
            $('.layui-table-main td:last-child .layui-table-cell').css({
                'visibility': 'hidden',
                'width': operationWidth + 'px'
            });

            // 刷新表格布局
            $(window).trigger('resize');
        }
    };
    exports('dgcommon', tool);
});
layui.define('view', function (exports) {
    var $ = layui.jquery
        , laytpl = layui.laytpl
        , element = layui.element
        , setter = layui.setter
        , view = layui.view
        , device = layui.device()

        , $win = $(window), $body = $('body')

        , SHOW = 'layui-show', HIDE = 'layui-hide', THIS = 'layui-this', DISABLED = 'layui-disabled', TEMP = 'template'
        , APP_BODY = '#LAY_app_body', APP_FLEXIBLE = 'LAY_app_flexible'
        , FILTER_TAB_TBAS = 'layadmin-layout-tabs'
        , APP_SPREAD_SM = 'layadmin-side-spread-sm', TABS_BODY = 'layadmin-tabsbody-item'
        , ICON_SHRINK = 'layui-icon-shrink-right', ICON_SPREAD = 'layui-icon-spread-left'
        , SIDE_SHRINK = 'layadmin-side-shrink', SIDE_MENU = 'LAY-system-side-menu'

        //通用方法
        , dg = {
            //弹出面板
            popup: view.popup

            //右侧面板
            , popupRight: function (options) {
                //layer.close(admin.popup.index);
                return dg.popup.index = layer.open($.extend({
                    type: 1
                    , id: 'LAY_adminPopupR'
                    , anim: -1
                    , title: false
                    , closeBtn: false
                    , offset: 'r'
                    , shade: 0.1
                    , shadeClose: true
                    , skin: 'layui-anim layui-anim-rl layui-layer-adminRight'
                    , area: '300px'
                }, options));
            }

            // 刷新
            , reload: function (tableId) {
                var bodyIframe = parent.$("#content").find(".layui-show iframe")[0].contentWindow;
                bodyIframe.layui.table.reload(tableId);
            }

        }

    //接口输出
    exports('dg', dg);
});
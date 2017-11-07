/** 
* 在iframe中调用，在父窗口中出提示框(herf方式不用调父窗口)
*/
$.extend({
    show_warning: function (strTitle, strMsg) {
        $.messager.show({
            title: strTitle,
            width: 300,
            height: 100,
            msg: strMsg,
            closable: true,
            timeout: 3000,
            showType: 'slide',
            style: {
                right: '',
                top: document.body.scrollTop + document.documentElement.scrollTop,
                bottom: ''
            }
        });
    }
});

/**
* @author 徐纪元/20170909
* 
* @requires jQuery,EasyUI
* 
* 扩展validatebox，弹框时提供异常订单号给用户，方便复制
*/
$.extend({
    show_error: function (strTitle, strMsg, strOrder) {
        if (strTitle == '')
            strTitle = "错误！";
        $.messager.show({
            title: strTitle,
            width: 470,
            height: 300,
            msg: '<p>异常信息：</p><textarea type="text" style="background:LightYellow;resize: none;width:430px;height:100px;font-size:12px;font-family: Microsoft YaHei;" >' + strMsg + '</textarea><br/>' +
                 '<p>订单信息：</p><textarea type="text" style="background:LightYellow;resize: none;width:430px;height:70px;font-size:12px;font-family: Microsoft YaHei;" >' + strOrder + '</textarea>',
            timeout: 0,
            closable: true,
            showType: 'slide',
            style: {
                right: '',
                top: document.body.scrollTop + document.documentElement.scrollTop,
                bottom: ''
            }
        });
    }
});

/** 
* 弹框
*/
$.extend({
    show_alert: function (strTitle, strMsg) {
        $.messager.alert(strTitle, strMsg);
    }
});

/**
* @author 孙宇
* 
* @requires jQuery,EasyUI
* 
* 扩展validatebox，添加验证两次密码功能
*/
$.extend($.fn.validatebox.defaults.rules, {
    eqPwd: {
        validator: function (value, param) {
            return value == $(param[0]).val();
        },
        message: '密码不一致！'
    }
});

/**
* @author 风骑士
* 
* @requires jQuery,EasyUI
* 
* 初始化datagrid toolbar
*/
getToolBar = function (data) {
    if (data.toolbar != undefined && data.toolbar != '') {
        var toolbar = [];
        $.each(data.toolbar, function (index, row) {
            var handler = row.handler;
            row.handler = function () { eval(handler); };
            toolbar.push(row);
        });
        return toolbar;
    } else {
        return [];
    }
}

/**
* @author 孙宇
*
* 接收一个以逗号分割的字符串，返回List，list里每一项都是一个字符串（做编辑功能的时候 传入id 然后自动勾选combo系列组件）
*
* @returns list
*/
stringToList = function (value) {
    if (value != undefined && value != '') {
        var values = [];
        var t = value.split(',');
        for (var i = 0; i < t.length; i++) {
            values.push('' + t[i]); /* 避免将ID当成数字 */
        }
        return values;
    } else {
        return [];
    }
};
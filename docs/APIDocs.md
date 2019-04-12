假设部署的地址为
http://111.230.238.192/learn/lost, 下文中要求请求的地址为 /user/login
则意味着完整URL应该为
http://111.230.238.192/learn/lost/user/login 
请自行正确构造请求URL，如无特殊需求下文不再赘述。

1. 用户登录态 & 账户资料
   
   (1) 发起QQ登陆。登陆成功后，向服务器发送以下JSON
        地址: /user/login
        方式: POST - body - application/json
        {
            "OpenID":"xxxx",
            "AccessToken":"把腾讯返回的AccessToken做HMACSHA256加密之后的结果",
            "NickName":"通过get_simple_info得到的当前登录账户的QQ昵称",
            "CurrentDeviceAndroidID":"获取这个设备的Built-in Android ID"
        }
        
        //服务器检测当前OpenID是否存在于User数据库。如果没有，则将这些信息添加到User表。同时创建一个Snowflake ID作为对此User的索引。客户端应该持久化这个Snowflake ID。请注意，以后所有需要从服务器查询用户信息的地方，一律采用Snowflake ID，不接受OpenID查询。

        服务器返回:
        application/json
        {
            "StatusCode":"此次请求的状态码",
            "UserID":"如果请求成功，则返回此账号对于的Snowflake ID"，客户端应该持久化.,
            "LastLoginDeviceAndroidID":"上次登录该账号的设备ID，可能返回null或者真实值，客户端判断是否别处登录，酌情踢掉当前账号。"
        }

    (2) 更新账户预置的联系方式
        地址 /user/info
        请求Header添加
        actk="上述传给服务器的AccessToken", userid="服务器下发的Snowflake ID"
        方式 PUT -- body -- application/json
        客户端向服务器发送
        {
            "QQ":"xxx",
            "WxID":"微信号",
            "PhoneNumber":"",
            "Email":""
        }

        //以上是支持预置的全部联系方式。想要更新哪个field，就往这个json object里塞不同的field即可。
        //比如说只想更新QQ号和微信号，就只添加QQ和WxID即可

        服务器就会更新相应的field名和状态码
        {
            "StatusCode":"xx",
            "Updated":["这个JSON数组返回已经更新了的资料field。一般情况下原路返回申请的field."]
        }

    (3) 获取账户预置的联系方式
        地址 /user/info
        方式 GET
        请求Header添加 actk=xxxxxx, userid=snowflakeid

        服务器接收到后，将返回
        {
            "StatusCode":状态码
            "Contacts":{  //如果请求成功，才会有这个field.
                "QQ":"xxx",
                “WxID":"xxx"
                预置了什么就会发回什么给客户端,和上述更新预置个人联系方式的内容一致
            }
        }
2. 失物类别信息
   
    (1) 获取失物类别

        
        GET /category
        服务器返回
        {
            "StatusCode":"xxx",
            "CategoryList":
            [{
                "Id":1,
                "Name":"日常用品"
            },
            {
                "Id":2,
                "Name":"卡类"
            }]
        }

    (2) 获取某类别下失物的细分类别

        GET /category/detail?id=1 （在QueryString上指定需要查询细分类别的Category ID）

        服务器返回
        {
            "StatusCode":
            "CategoryId":原路返回,
            "CategoryName":原路返回,
            "CategoryDetails":[{"Id":xxx,"Name":"yyy"}]  与上述类似
        }
        
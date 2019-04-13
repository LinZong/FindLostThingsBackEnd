假设部署的地址为
http://111.230.238.192/learn/lost, 下文中要求请求的地址为 /user/login
则意味着完整URL应该为
http://111.230.238.192/learn/lost/user/login 
请自行正确构造请求URL，如无特殊需求下文不再赘述。
响应的成功与否由每个Response的StatusCode描述。参考返回码一览表。
如果需要actk和userid鉴权的请求没有正确传递这些参数，会收到403 Forbidden错误。
如果收到502或别的奇怪错误，请联系后端。这种情况一般是服务器死了。


1. 用户登录态 & 账户资料
   
   (1) 发起QQ登陆。登陆成功后，向服务器发送以下JSON
        地址: /user/login
        方式: POST - body - application/json
        {
            "OpenID":"xxxx" -- string
            "AccessToken":"把腾讯返回的AccessToken做HMACSHA256加密之后的结果", -- string
            "NickName":"通过get_simple_info得到的当前登录账户的QQ昵称", -- string 
            "CurrentDeviceAndroidID":"获取这个设备的Built-in Android ID" -- string
        }
        
        //服务器检测当前OpenID是否存在于User数据库。如果没有，则将这些信息添加到User表。同时创建一个Snowflake ID作为对此User的索引。客户端应该持久化这个Snowflake ID。请注意，以后所有需要从服务器查询用户信息的地方，一律采用Snowflake ID，不接受OpenID查询。

        服务器返回:
        application/json
        {
            "StatusCode":"此次请求的状态码", -- int
            "UserID":"如果请求成功，则返回此账号对于的Snowflake ID"，客户端应该持久化., -- long
            "LastLoginDeviceAndroidID":"上次登录该账号的设备ID，可能返回null或者真实值，客户端判断是否别处登录，酌情踢掉当前账号。" -- string
        }

    (2) 更新账户预置的联系方式
        地址 /user/info
        请求Header添加
        actk="上述传给服务器的AccessToken", userid="服务器下发的Snowflake ID"
        方式 PUT -- body -- application/json
        客户端向服务器发送
        {
            "QQ":"xxx", -- string
            "WxID":"微信号",  -- string
            "PhoneNumber":"",  -- string
            "Email":""  -- string
        }

        //以上是支持预置的全部联系方式。想要更新哪个field，就往这个json object里塞不同的field即可。
        //比如说只想更新QQ号和微信号，就只添加QQ和WxID即可

        服务器就会更新相应的field名和状态码
        {
            "StatusCode":x, -- int
            "Updated":["这个JSON数组返回已经更新了的资料field。一般情况下原路返回申请的field."] -- string数组
        }

    (3) 获取账户预置的联系方式
        地址 /user/info
        方式 GET
        请求Header添加 actk=xxxxxx, userid=snowflakeid

        服务器接收到后，将返回
        {
            "StatusCode":状态码 -- int
            "Contacts":{  //如果请求成功，才会有这个field.
                "QQ":"xxx", -- string
                “WxID":"xxx"  -- string
                预置了什么就会发回什么给客户端,和上述更新预置个人联系方式的内容一致
            }
        }
2. 失物类别信息
   
    (1) 获取失物类别

        
        GET /category
        服务器返回
        {
            "StatusCode":xxx, -- int
            "CategoryList":
            [{
                "Id":1, -- int
                "Name":"日常用品" -- string
            },
            {
                "Id":2, -- int 
                "Name":"卡类" -- string
            }]
        }

    (2) 获取某类别下失物的细分类别

        GET /category/detail?id=1 （在QueryString上指定需要查询细分类别的Category ID）

        服务器返回
        {
            "StatusCode": -- int 
            "CategoryId":原路返回, -- int 
            "CategoryDetails":[{"Id":xxx,"Name":"yyy"}]  与上述类似 内部数据类型也类似
        }
        

 3. 与对象存储空间交互（上传图片）
    
    (1) 获取操作存储桶的临时Key:

    本项目采用腾讯云的对象存储服务。由于采取的策略为公有读私有写，客户端在需要向存储桶上传数据时，为了安全需要获得腾讯云授权本次操作（可以持续一段时间）的TmpSecretId、TmpSecretKey和Token。

    GET /tencent/coskey
    请求Header添加 actk=xxxxxx, userid=snowflakeid

    正常情况下服务器将返回类似消息。该返回较为复杂，写代码的时候要注意一些:
    ```
    {
        "StatusCode":0, -- int 
        "FullBucketName":"完整存储桶的名字，接下来上传文件时需要用到", -- string
        "Region":"本存储桶所在地区。接下来上传文件需要用到", -- string
        "Response":"响应主体, 由于整体较长，故单独抽出描述"
    }
    ```

    响应主体形如下:
    ```json
    {
        "ExpiredTime": 1555091373, -- long
        "Expiration": "2019-04-12T17:49:33Z", -- string 
        "Credentials": {
            "Token": "PRTGBfDqrKkCnHA7TyzZ2Qnd59cQZ7kj6475c6835f4a30c492ad8a58928ed4b4vNKGyPIDDoSGT63wxdFVHmNKoUh-s_V9mIc5GPateIVCSj9kedtNYWo8ZqrFl7z-pPFj001KqYcAVjHDe5IoOFPOd8ZfGk5_5BXh0U_65h21bRDpH6QwsaNJVGOXfV9TzLfLkVgc7AVMpQxaF\*\*后面还有，截掉了\*\*", -- string
            "TmpSecretId": "截掉-aPheldxEQwX05vY_F4IvGKqkhHP0", -- string
            "TmpSecretKey": "Lh1qQ截掉一些vjwek5u+sEotKC2k8b2OA=" -- string
        },
        "RequestId": "523b514f-截掉-442d-9fc8-一些" -- string
    }
    ```

    解析上述JSON中的内容填入Tencent COS SDK中上传文件要求的参数中。

    特别注意的是，一般而言这些临时Key都会有大约半小时的有效期。具体可以自行解析ExpiredTime得到结果。客户端应该在Key仍处于有效期的时候持久化好这些Key，在Key仍处于有效期的时候重复利用，避免每次上传文件都请求服务器给Key，而造成大量无谓发Key操作。有关客户端如何操作，请参考[腾讯云对于存储桶的官方文档](https://cloud.tencent.com/document/product/436/14048)。

    (2) 存储桶文件命名规范

    为了实现比较有序的管理存储桶中保存的静态文件，现在介绍向存储桶中上传文件的目录命名规范
        
    需要上传到存储桶的数据有两类，1是失物的图片数据，2是用户的身份认证信息，例如绿卡拍照，蓝卡拍照，身份证照等等。

    为了使得存储桶看上去干净整洁，这两种数据需要放在不同的文件夹下。

    对于1数据，需要将 [对象键](https://cloud.tencent.com/document/product/436/13324) 设置为
    
    ```lost/upload/things/<当前登录账号的user-id>/<201901,201902这样格式的年月字符串>/<发布失物的失物ID，预计使用GUID>/文件名.jpg```
    
    举例，今天是2019年4月13日，一个UserID为123456的用户发了一个失物请求，这个请求包含了4张图片。本地命名为1.jpg, 2.jpg, 3.jpg, 4.jpg. 

    那么上传路径的前缀应该类似于

    ```
    lost/upload/things/123456/201904/C6E131A9-E8C9-4223-9D0B-E92AD01580D0/   ①路径
    ```

    接下来，计算上述字符串的SHA1, 使用类似下面的函数进行计算

    ```java
        public static String sha1Encode(String content) {
            byte[] hash;
            try {
                hash = MessageDigest.getInstance("SHA-1").digest(content.getBytes("UTF-8"));
             } catch (Exception e) {
                throw new RuntimeException("NoSuchAlgorithmException", e);
            }
            StringBuilder hex = new StringBuilder(hash.length * 2);
            for (byte b : hash) {
                if ((b & 0xFF) < 0x10) {
                    hex.append("0");
                }
                hex.append(Integer.toHexString(b & 0xFF));
            }
            return hex.toString();
        }
    ```

    假设计算得到了以下结果
    ```
    b60d121b438a380c343d5ec3c2037564b82ffef3
    ```

    因为软件限制一个失物招领信息最多发9张图片，那么从这条SHA1计算结果中，从左往右，以4个字符为一组，不断截取出一小段字符作为文件名的前缀，拼合原本的文件名，如下例子：


    b60d-1.jpg

    121b-2.jpg

    438a-3.jpg

    380c-4.jpg

    这四个文件。最后拼合①的路径，作为上传至存储桶的对象键。
    以第一个文件为例，最终对象键应该设置为:
    ```
    lost/upload/things/123456/201904/C6E131A9-E8C9-4223-9D0B-E92AD01580D0/b60d-1.jpg
    ```

    这样的一波操作主要是为了迎合存储桶的索引方式，加快寻址文件的速度。


    对于2数据，路径构造为:

    ```
    lost/upload/idvalidate/<user-id>
    ```

    只不过这次不需要再附带年月字符串和GUID了。直接把这个路径扔去SHA1.得到的结果作为前缀，拼合文件名即可。

    <strong>值得注意的是，这样截取文件前缀，是有几率出现两个前缀重复的。所以不要完全依赖前缀来保证在某个文件夹下的文件名的唯一性。</strong>

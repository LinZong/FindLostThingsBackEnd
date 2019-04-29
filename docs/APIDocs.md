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
        ```

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
    ```
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
    ```
        //以上是支持预置的全部联系方式。想要更新哪个field，就往这个json object里塞不同的field即可。
        //比如说只想更新QQ号和微信号，就只添加QQ和WxID即可

        服务器就会更新相应的field名和状态码
        {
            "StatusCode":x, -- int
            "Updated":["这个JSON数组返回已经更新了的资料field。一般情况下原路返回申请的field."] -- string数组
        }

    (3) 获取账户个人资料
        地址 /user/info
        方式 GET
        请求Header添加 actk=xxxxxx, userid=snowflakeid

        服务器接收到后，将返回
        {
            "StatusCode":状态码 -- int,
            "UserInfo":{
                "Id":"同上面描述的UserId", -- long
                "NickName":"同上面描述的NickName", -- string
                "QQ":"",//这些和上面更新预置信息的类型是一致的，不再赘述
                "WxID":"",
                "PhoneNumber":"",
                "Email":"",
                "RealPersonValid":1, -- int 描述这个账号有没有经过真实身份认证，有为1，没有为0.
                "RealPersonIdentity":"" -- string数组，内部是这个账号上传的有关身份认证的图片信息（如果有）
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




  4. 获取支持的学校信息

    (1) 获取支持的学校列表

    ```
    地址  /school/list/
    GET

    服务器返回
    {
        "SupportSchools": [
            {
                "Id": 1, -- int 
                "Name": "华南理工大学(大学城校区)" -- string
            }
        ],
        "StatusCode": 0 -- int
    }
    ```

    (2) 根据当前选择的学校获取所有可选的建筑物名称

    地址  /school/building?id=上面获取到的SchoolId
    GET

    服务器返回

    {
        "SchoolBuildings": [
            {
                "Id": 1, -- int 
                "BuildingName": "C10学生公寓", -- string
                "Latitude": 10, -- double
                "Longitude": 10, -- double
                "BuilddingAddress": "这个建筑的地址" -- string
            }
        ],
        "StatusCode": 0 -- int
    }

    
  5. 上传/获取失物信息
    虽然说这个是接口文档，但是这里还是要讲一下上传失物信息的流程。
    因为团队非常穷，主服务器的上传带宽只有1Mbps，那么图片资源完全不可能传在主服务器上，只能是传到存储桶上。故整个发布步骤大致为
    填写失物信息 --> 拍照 --> 照片传到存储桶 --> 上传完成后，得到照片位于存储桶的URL地址 --> 将照片URL放入数组，转成JSON数组之后连同失物信息一起上传给服务器
    这样一来，服务器上就只存储照片的URL，获取失物信息的时候客户端应该解析这些照片的URL，再根据这些URL去下载图片资源。


    (1) 发布失物信息

    地址/thing/publish
    POST -- body -- application/json
    请求头部携带actk和userid验证
    向服务器发送以下JSON字符串:
    注:非必须的字段前以&号标注。
    {
        "Id":"GUID字符串，唯一代表了本条失物记录，在客户端生成",
        "Title":"本条失物信息的标题",
        "ThingCatId":该物品的大类Id,
        "ThingDetailId":该物品的细分类别Id,
        "FoundTime":捡到这个东西的时间,
        "PublishTime":发布此条记录的时间,
        "FoundAddress":"捡到丢失物的地址,有格式要求，下面会讲解1",
        "Publisher":发布此失物信息的账号UserID,
        &"PublisherContacts":"捡到东西的人的联系方式, 不是必须，下面会讲解2",
        "ThingPhotoUrls":"失物照片资源URL，上面提及",
        &"FoundAddrDescription":"捡到物品的地址的附加描述",
        &"ThingAddiDescription":"对捡到的物品的附加描述",
    }
    每个条目的数据格式参见(5)中对此完整的失物对象的描述。在实现的过程中其实也只需要创建一个(5)中描述的数据结构，然后只填充必填部分即可。服务器会自动区分处理填好的和没填好的Fields.

    注解:
    1. 捡到丢失物的地址，由`学校id-建筑物id`表示。如果建筑物选择的是其他，则建筑物ID为0。目前不提供在未受支持的校园环境中使用此软件。举例说明，上文提供的例子中，华南理工大学大学城校区的学校ID为1，C10学生公寓的建筑物ID也为1, 则此项应为字符串"1-1"(不含双引号)。如果想填写失物是在B7实验室捡到的，但是B7实验室尚未出现在支持的建筑物列表中，那么需要设置为"1-0"，表示建筑物ID为其他，然后在FoundAddrDescription一项中补充说明此地址和建筑物的特征等信息。

    2. 捡到东西的人的联系方式。这项是非必须。如果此项为null，那么客户端应该根据Publisher的ID发起查询某人个人信息的请求，得到他的全部联系方式，展示出来。另外，有一种很常见的情况是发布者并不想用自己预置在软件中的联系方式，希望在发布某条信息的时候临时使用别的联系方式。这时候此项起作用。发布者希望使用别的联系方式的时候，需要在上传失物信息的时候设置此项为以下的JSON对象:
    {
        "QQ":"新的QQ号"
        "WxID",""
        //等等，最多支持设置上文所述的四种联系方式。不再赘述。可以想留哪种联系方式就设哪种联系方式的field。
        "AdditionalDescription":"此项填写一些联系方式的附加信息。比如留一句话，加QQ的时候备注是来认领东西的。"
    }
    如果PublisherContacts中的JSON对象仅仅设置了AdditionalDescription，没有其他的联系方式。则客户端仍然应该按照**没有设置本条失物信息的其他临时联系方式**处理，去请求这个Publisher的个人信息，展现出他预留在后台的联系方式。

    (2) 按时间轴逆序获取失物信息（离当前时间越近的条目越前，离当前时间越远的条目越后）（首页瀑布流用）
        地址 /thing/list
        GET 
        请求头带上actk和userid
        actk和userid不再赘述
        以QueryString的形式给出以下参数:

        EndItemId -- string 客户端目前获取的整条瀑布流中最后一个Item的GUID。
        HaveFetchedItemCount -- int --客户端从第一条请求至今一共接受了多少条数据。
        Count -- int

        例如:

        /thing/list?EndItemId=000F55E6-6BCD-4C60-99C3-FEDA9AC7AF0C&HaveFetchedItemCount=10&Count=100

        服务器返回:
        正常情况将返回一个里面全是(5)中描述的对象的JSON数组。
        出现异常的话返回空数组

        注解：
        已发布的失物的数量是会越来越多的，假设在某一时间，客户端获取了瀑布流的第1-100条数据。用户刷了一段时间之后，此时需要分段加载第101-200的接下来的100条数据。
        如果此时有新的失物信息发布，那么很明显，获取瀑布流中的第101-200条数据中肯定就存在之前看过的信息。因为新信息的出现使得他们按照时间的排名变后了。

        为了使得用户在往下刷的过程中不会加载到重复的数据，就必须要维护一个类似于“版本机制”的东西。为了简单起见，客户端告诉服务器当前这条瀑布流的起点在哪里，服务器就能够根据客户端提供的“起点”准确定位所请求的段真实应该在哪里。
        count参数控制了想往下加载多少条数据。如果在三个参数的控制下，获取的失物信息段超过了服务器中存储的全部失物信息。例如服务器只有150条信息，客户端要求从第100条开始获取100条，那么服务器只返回50条信息。如果客户端要求从200条开始获取100条信息，服务器将返回一个空数组。以此类推。

        可以不设EndItemId，那么服务器将以当前时刻的最新瀑布流为基准，从头开始的Count条元素。
        HaveFetchedItemCount不允许为负数, 但允许不设。如果传入了非法输入，HaveFetchedItemCount自动变为0.
        Count不允许负数或者不传，如果传入了非法输入，Count自动变为0。

        如果是客户端初始化，第一次请求整瀑布流，则不传EndItemId，将HaveFetchedItemCount设为0，Count通常设置为一个适中的值，如50。
        否则应该设置EndItemId为当前客户端获得的整条瀑布流中最后一个Item的GUID，HaveFetchedItemCount设置为目前为止客户端一共获取到的失物信息条目的数量，Count设置为一个适中的值。

        样例返回:
        ```
        [
            {
                "Id": "4CFB9502-1E98-418C-BC0E-96DA923662C1",
                "Title": "测试用的第二条失物",
                "ThingCatId": 1,
                "ThingDetailId": 1,
                "PublishTime": 1555227532,
                "FoundTime": 1555227522,
                "GivenTime": null,
                "Isgiven": null,
                "FoundAddress": "1-2",
                "FoundAddrDescription": null,
                "ThingAddiDescription": "这是第二条傻逼东西，说个屁，屁明",
                "ThingPhotoUrls": null,
                "PublisherContacts": null,
                "GivenContacts": null,
                "Publisher": 36767411659079680,
                "Given": null
            },
            {
                "Id": "7CBC7947-244D-4890-8F2B-D098829E3123",
                "Title": "测试用的第一条失物",
                "ThingCatId": 1,
                "ThingDetailId": 1,
                "PublishTime": 1555227522,
                "FoundTime": 1555227522,
                "GivenTime": null,
                "Isgiven": null,
                "FoundAddress": "1-1",
                "FoundAddrDescription": null,
                "ThingAddiDescription": "没什么好说的，就是个傻逼东西",
                "ThingPhotoUrls": null,
                "PublisherContacts": null,
                "GivenContacts": null,
                "Publisher": 37535048114634752,
                "Given": null
                }
        ]
        ```

(3) 获取一个指定账户发布的全部失物信息（我发布的）
    地址     /thing/mylist?type=1 (1为发布，0为认领的失物信息)
    GET 
    请求头带上actk和userid。
    get

    服务器返回:
    返回一个里面全是(5)中描述的对象的JSON数组。可能是空数组。
    注解：
    由于一个人一般不会好心到发布的失物多到需要做分段加载。所以这个接口会一次性返回用户发布的全部失物信息。客户端做展示的时候可以不必一次性把全部的信息加载到View上。
        
(4) 获取一个指定账户认领的失物信息（我找回的）
    描述同上，不再赘述

(5) 编辑已经发布的失物信息
    地址  /thing/update
    POST -- body -- application/json
    带actk和userid。

    服务器返回:
    ```
    {
        "StatusCode":本次更新操作的状态码，请参照后端返回错误码一览表。
    }
    ```
    注解：
    这里给出失物信息对象的完整Fields
    ```
    {
        *"Id":"GUID字符串，唯一代表了本条失物记录，在客户端生成", -- string
        "Title":"本条失物信息的标题", -- string
        "ThingCatId":该物品的大类Id, -- int
        "ThingDetailId":该物品的细分类别Id, -- int
        "FoundTime":捡到这个东西的时间,  -- long
        *"PublishTime":发布此条记录的时间, -- long
        "GivenTime":这个物品被归还给失主的时间, -- long 
        "Isgiven":是否被归还。0为否 1为是, -- int
        "FoundAddress":"捡到丢失物的地址,有格式要求，下面会讲解1", -- string
        *"Publisher":发布此失物信息的账号UserID, -- long 
        "PublisherContacts":"捡到东西的人的联系方式, 不是必须，下面会讲解2", -- string
        "GivenContacts":"失主的联系方式。在失物被归还之后补充此field", -- string
        "Given":失主的UserID, -- long
        "ThingPhotoUrls":"失物照片资源URL，上面提及", -- string数组
        "FoundAddrDescription":"捡到物品的地址的附加描述", -- string
        "ThingAddiDescription":"对捡到的物品的附加描述", -- string
    }
     ```

每当调用接口获取失物的信息时，服务器将返回以上描述的JSON对象。
当需要更新一件失物的信息时，需要修改以上对象的相关Fields。然后将整个JSON对象POST回给服务器即可。
请注意，此对象有一些部分不允许修改，已用*标出。如果客户端上传的对象修改了不能修改的条目，则服务器将拒绝此次更新。
仅当Header上的UserID和对象中的Publisher一致时才进行修改操作。   

  (6) 搜索功能:查询发布到平台上的失物记录
  地址: /thing/search
  GET -- body -- application/json 携带actk和userid
  这个功能没什么好解释的，刚需，用就是了。高级搜索还没法做，我还得去熟悉一下网上的分词库。
  搜索页第一屏会给出一个跟发布失物信息页差不多的下拉栏，用于对失物进行条件搜索，需要把用户填入下拉栏的信息发送到服务器，服务器根据这些信息过滤数据库，返回结果。有大部分的参数与(5)中提及的参数的含义和类型都是完全一样的，对于这类属性，将不再赘述。
  {
      "ThingCatId":"同上，必填", -- int
      "ThingDetailId":"同上，选填", -- int
      "SchoolId":"学校校区的ID,对应于(5)中FoundAddress格式中-的前面的数字, 必填", -- int
      "SchoolBuildingId":"选定学校校区的建筑物ID,对应于(5)中FoundAddress格式中-的后面的数字, 选填", -- int
      "ItemStatus":设置搜索的失物信息的状态。1为未完成，2为已完成，3为混合已完成和未完成。 -- int 
      "SortType":设置服务器回传这些数据时的排序方式,0为按照发布日期倒序排序，1为按照捡到失物的日期倒序排序, -- int 
      "IsAdvancedSort":是否为高级搜索，打汉字那种，0为不是，1为是。考虑到现在还没做高级搜索，请只赋值0， -- int 
      "AdvancedSortText":高级搜索的具体内容，即文本框中的全部输入。由于目前没做高级搜索，所以此项为空。 -- string
      "FoundDateBeginUnix":失物被捡到的时间段的开始时间, -- long
      "FoundDateEndUnix":失物被捡到的时间段的结束时间。举例，如果大概是2019年4月15到4月20之间丢的，转换2019/4/15 0:00 和2019/4/21 0:00的Unix时间戳，分别赋值到Begin和End，构造出一个Time period.这样就刚好包括了整个时间段。 -- long
  }

  服务器返回:
  如果搜到了信息，将返回一个内部元素为(5)中提及的对象的数组，其中这个元素的属性满足搜索时下达的条件要求，否则，返回空数组。

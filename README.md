## 国创项目PC端WPF代码和服务端PHP代码
### 服务器
##### 数据库表：

用户信息表：
用户手机号 加密过的密码 是否登陆
UserInfo(username,password,is_login)

用户手机基本信息表：
用户id 手机地理坐标 手机当前电量 
PhoneInfo(username,location,power)

手机未读短信表
用户id 发信人电话号码 短信内容 接收时间
ShortMessage(username,sender_number,content,receive_time)

待发送短信表：
用户手机号 接收人号码 内容
SendingMessage(username,receiver_num,content)

手机联系人表
用户id 联系人姓名 电话号码 归属地
Contacter(username,contacter_name,contacter_num,contacter_location)

手机系统通知表
用户id 通知发起者 内容
Notice(username,sender,content)

摄像头拍摄的图片
Photo(username,picture,time)

操作指令表
Instructor(username,content)
##### 操作指令代码：
1：发送短信
2：一键锁屏
3：删除数据
4：打开摄像头拍照（照片文件名以时间命名，并读取拍摄时间datetime形式）
##### 剩下的介绍：
用户表中is_login字段用于手机端登录后指示电脑端可进行操作，phone_login.php用于手机端登录，
phone_signout.php用于手机端退出（两者均为修改is_login字段）。电脑端的登陆检测页面为pc_check.php
该页面仅用于检测PC端是否具有操作权限。

test.php页面用于测试。其中使用表单发送http请求，手机与PC客户端则根据这个页面模拟同样的表单数据。

各个页面返回的结果均用json表示，其中大部分的错误信息用两个字段表示：result 这个字段为一个整数，
具体含义见相关页面；message 表示错误信息字符串

有些操作返回的结果json字符串不太相同，例如手机端读取操作指令时得到的json数组会不一样，查看源代码
可得到具体形式，注意解析问题

注册过程：只能在电脑端注册，手机端登录，判断是否有该用户名，不存在则无法使用；手机端登录后立刻上传本手机基本信息，
并定时更新，电脑端开启新线程，定时访问服务器获取手机端信息；

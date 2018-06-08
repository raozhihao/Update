# Update 使用方法
为Winform实时更新数据库的更改<br>
此例主要使用的是socket<br>
在打开多个winform客户端时,此时多个winform客户端都连接着同一数据库<br>
当其中一个客户端有诸如增加,删除,更新数据库中数据时,其它客户端也会有实时的数据更新表现<br>


文件构成
-------

服务端--此项为Winform服务端,<br>
客户端--此项为Demo客户端<br>
windows服务端--此项为windows服务<br>
NowUpdate--此项为自己封装的一个类库<br>
DemoNow--此项为使用NowUpdate的一个Demo<br>

使用方法
-------

方法1<br>
直接使用服务端打开后,填入要绑定的ip地址与端口号,开启侦听即可<br>
在打开客户端程序之前需要在配置文件中配置与服务端同样的ip和端口号<br>
打开多个客户端程序后增加信息即可看到其中一个客户端改变数据后其它客户端也随之改变<br>

方法2<br>
在打开windows服务端之前也先配置好需要绑定的ip地址和端口号<br>
其它同方法2<br>
windows服务端请自行上网搜索安装到系统中的方法<br>

方法3<br>
在配置好ip地址与端口号后,打开服务端或安装好windows服务端后<br>
可以在自己的程序中引用NowUpdate<br>
最好在Form_Load事件中新建Now对象,并给事件DoSomeThing赋值<br>
其中DoSomeThing事件即为你在项目中对数据库进行了操作后需要服务器为你及其它客户端所做的事<br>
在示例程序DemoNow中为刷新当前界面上的表格<br>
当你的程序对数据库做了更改后,请勿必使用Now对象的send方法通知服务器,否则更新之后其它客户端无法得知你的更改.
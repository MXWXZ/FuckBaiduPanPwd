# FuckBaiduPanPwd
一个暴力破解百度网盘密码的小工具\
 A simple tool to fuck baidu pan password\
![Language][1]
[![GitHub release][2]][3]
[![Github Download][4]][5]
![License][6]

本软件基于GPL-3.0开源协议，简单来讲，如果您（部分）使用本软件的代码，您必须在分发软件时提供源代码（开源），对代码修改部分需要进行声明，修改后的软件也要使用GPL-3.0开源协议

## 实际测试
实际测试因为没有可靠（速度快）的代理IP池，我使用了某个可以定时切换IP的国内代理V.P.N软件，定时40s切换一次IP，这家服务的切换时间在2s以内还是不错的，500线程。总共用时7小时左右（一个晚上），尝试了133万共169万多的密码，速度大概在60个/min左右，也就是裸连跑的40%，成功爆破出密码。

仅供学习研究使用，想要保证速度就需要一定的成本，鉴于HTTP代理国内市场水很深，不少IP都用烂了速度极差，不建议使用（如果有优质代理可自测），或者尝试我测试使用的国内V.P.N代理的方法，这个可能没有短期的套餐需要你自己去谈，还有一种就是各种混拨线路，这个没有测试过，理论上是可行的。

## Feature
- 多线程破解
- 随机尝试密码
- 日志记录，断点续破
- 支持分布式破解
- 支持代理IP（指定文件名/API自动获取）
- 失效代理自动切换

## 运行说明
### 有python3环境
运行crack.py文件即可

    python crack.py

### 无python环境
- windows10 x64用户在release下载`crack-for-win10`解压运行即可（x86未测试）
- 其他用户请下载`crack-for-win_LOWER`解压后如果无法运行，安装对应版本`vc_redist`即可

## 软件使用
### 参数说明：
- URL地址：保证格式为`https://pan.baidu.com/share/init?xxx`即可，如果不是请在浏览器打开看看会不会自动跳转。
- 破解字典文件名：包含待尝试密码的文件名，文件格式一行一个密码，默认为目录下的`allpwd.dic`文件，可以使用superdic等软件生成
- 忽略密码文件名：包含所有跳过的密码的文件名，文件格式一行一个密码，默认不忽略，程序已经尝试的密码默认记录在`log.dic`文件中
- 延时毫秒数：每个线程每两次尝试延时毫秒（建议50-100）
- 线程数：尝试线程数（根据配置建议200-500）
- 代理IP文件名：包含所有代理IP的文件名，文件格式一行一个IP:`ip:port`，例如`1.1.1.1:1111`
- 代理IPAPI地址：代理ip提供商的api地址，格式请选择txt，保证获取到的数据和代理IP文件名格式相同，建议一次获取数量不要太多以免失效
- 切换IP频率：代理ip的切换频率，单位为秒（根据破解速度30s-480s之间，速度快就需要频繁切换ip）

### 日志文件
- 程序默认日志文件为`log.dic`，内容为尝试失败的密码，格式和破解字典相同，可以直接用于之后程序的忽略密码文件名参数。写入方式为追加写入，不会对已有数据造成影响。
- 找到的密码将会输出在屏幕上并且保存在`password.ans`文件中，使用文本编辑器打开即可看到。

## 文件目录
- .gitignore：Git忽略文件配置
- LICENSE：开源协议
- README.md：本说明
- allpwd.dic：包含全部可能密码的字典文件
- crack.py：破解程序主文件
- release.bat：pyinstaller批处理生成exe文件

[1]:https://img.shields.io/badge/Language-Python3-red.svg
[2]:https://img.shields.io/github/release/mxwxz/FuckBaiduPanPwd.svg
[3]:https://github.com/MXWXZ/FuckBaiduPanPwd/releases
[4]:https://img.shields.io/github/downloads/mxwxz/FuckBaiduPanPwd/total.svg
[5]:https://github.com/MXWXZ/FuckBaiduPanPwd/releases
[6]:https://img.shields.io/badge/License-GPL--3.0-yellow.svg

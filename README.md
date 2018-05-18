# FuckBaiduPanPwd
一个暴力破解百度网盘密码的小工具\
 A simple tool to fuck baidu pan password\
![Language][1]
[![GitHub release][2]][3]
[![Github All Releases][4]][5]

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
- 破解字典文件名：包含待尝试密码的文件名，文件格式一行一个密码，默认为目录下的`allpwd.dic`文件
- 忽略密码文件名：包含所有跳过的密码的文件名，文件格式一行一个密码，默认不忽略，程序已经尝试的密码默认记录在`log.dic`文件中
- 延时毫秒数：每个线程每两次尝试延时毫秒（建议50-100）
- 线程数：尝试线程数（根据配置建议200-500）
- 代理IP文件名：包含所有代理IP的文件名，文件格式一行一个IP:`ip:port`，例如`1.1.1.1:1111`
- 代理IPAPI地址：代理ip提供商的api地址，格式请选择txt，保证获取到的数据和代理IP文件名格式相同，建议一次获取数量不要太多以免失效
- 切换IP频率：代理ip的切换频率，单位为秒（根据破解速度30s-480s之间，速度快就需要频繁切换ip）

### 日志文件
- 程序默认日志文件为`log.dic`，内容为尝试失败的密码，格式和破解字典相同，可以直接用于之后程序的忽略密码文件名参数。写入方式为追加写入，不会对已有数据造成影响。
- 找到的密码将会输出在屏幕上并且保存在`password.ans`文件中，使用文本编辑器打开即可看到。

[1]:https://img.shields.io/badge/Language-Python3-red.svg
[2]:https://img.shields.io/github/release/mxwxz/FuckBaiduPanPwd.svg
[3]:https://github.com/MXWXZ/FuckBaiduPanPwd/releases
[4]:https://img.shields.io/github/downloads/mxwxz/FuckBaiduPanPwd/total.svg
[5]:https://github.com/MXWXZ/FuckBaiduPanPwd/releases
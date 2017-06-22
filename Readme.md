# 一个unity语音转换为文字的解决方案 #

----------
## 制作平台 ##
 unity5.5.0
## 使用 ##
用unity打开项目，运行
点start开始录音，end录音完毕，然后大概等待1~2秒（看网速）就会把转换的文字Debug出来

别的脚本调用方法：把VoiceToString.cs脚本挂到任务物体，然后在需要调用的脚本中找到此脚本

StartMic:开始录音

EndMic:结束录音
## 说明 ##
此项目所用技术来自百度语音api，需要自己获取百度语音开发者App ID和Secret Key

官网[http://yuyin.baidu.com/](http://yuyin.baidu.com/)

App ID和Secret Key获取地址[http://yuyin.baidu.com/app](http://yuyin.baidu.com/app)

官方API文档[http://yuyin.baidu.com/docs/asr/57](http://yuyin.baidu.com/docs/asr/57)
# 小摩摩 v3.0
全新的项目架构，全新的模式。使用docker-compose部署，应用react、asp.net core、signalr等。

## 组成部分
1. MoMoBot.Api: api服务
2. MoMoBot.Client: 钉钉E应用客户端
3. MoMoBot.Portal: 管理员后台

## 地址及端口
1. api: 10.0.1.46:4567
2. portal: 10.0.1.46:4568
3. client: 10.0.1.46: 3000

version: '3.4'

services:
  momobot.client:
    image: momobotclient
    build:
      context: ./MoMoBot.Client/
      dockerfile: ./MoMoBot.Client/Dockerfile
  momobot.api:
    image: momobotapi
    build: 
      context: .
      dockerfile: ./MoMoBot.Api/Dockerfile


PGP@ssw0rd

```
admin@yunstorm.com
Pass@word1
```

`账号：lingxi7@outlook.com`

`密码：TEST@msdn`
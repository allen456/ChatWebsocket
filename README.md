# .netcore3.1-websocket-chat

Chat web application using .net core 3.1 and javascript

## live demo
```
https://chat.sermeno.xyz/
```

## run using docker
```
docker run -p 5001:80 -d --restart unless-stopped allen456/chatwebsocket:latest
```
open localhost:5001

## run using .net core 3.1 sdk

```
dotnet run
```

## publishing to iis

```
dotnet publish -c Release -o ./publish
```
copy all files in /publish folder to iis directory

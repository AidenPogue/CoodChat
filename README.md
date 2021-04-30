# CoodChat
_Tired of your messaging apps not being Turing complete? This is the software for you!_

CoodChat is a simple, janky, and extremely unsafe program that allows you to "chat" with all your friends by remotely executing C# code on thier computer.
If it can compile and run without any exceptions on your machine, the source code will be sent to all connected clients, where it will be compiled and executed, no questions asked.
As if it needs to be said, don't use this with someone you wouldn't trust with your wallet.

## Building
Since I'm a stupid new age kid, I don't know what to tell you other than use VS 2019.

## Usage
This app isn't anything fancy, so it just uses a simple TCP connection. The person hosting the server will need to forward a port, or you'll all need something like [LogMeIn Hamachi](https://vpn.net/).
After everyone's connected, it should be quite self explanitory. Make sure to keep the console window in view so you can see what's going on.

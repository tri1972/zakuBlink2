/*
 * seeduinoライブラリ
 * https://github.com/Seeed-Studio/ArduinoCore-samd
 */
#include <TimerTC3.h>
/*
 * C:\Users\sakai\AppData\Local\Arduino15\packages\Seeeduino\hardware\samd\1.8.1\libraries\TimerTC3
 */

/* Sweep
  by BARRAGAN <http://barraganstudio.com>
  This example code is in the public domain.

  modified 8 Nov 2013
  by Scott Fitzgerald
  http://www.arduino.cc/en/Tutorial/Sweep
*/

#include <Servo.h>

Servo myservo;  // create servo object to control a servo
// twelve servo objects can be created on most boards

int pos = 0;    // variable to store the servo position
int counter=0;
int duty = 0;
bool initTime=true;//ザクヘッドの初期動作を行うフラグ
bool monoEyeBlink=false;//モノアイが点滅するか否か
bool monoEyeAutoBlink=false; //モノアイが自動的に点滅するか否か
long noAccessPeriod=0;
long timerPreScolor=0;

void timerIsr()
{  
    if(monoEyeBlink==true){ 
      analogWrite(9, duty);
      counter+=1;
      if(counter<20){
        duty+=5;
      }else if(20 <= counter && counter <=38){
        duty-=5;
      }else{
        counter=0;
        duty=0;
      }
    //Serial.println(counter,DEC);
    //Serial.println(duty,DEC);
    }
    if(timerPreScolor % 10){//1sごとにカウントするようTimer割り込みを10分周
      noAccessPeriod++;
    }
    timerPreScolor++;
}

void setup() {
  Serial.begin( 9600 );
  myservo.attach(10);  // attaches the servo on pin 9 to the servo object
  Serial.println( "Hello Arduino!" );   // 最初に1回だけメッセージを表示する
  pinMode(9,OUTPUT);
  TimerTc3.initialize(100000);    
}

void loop() {
  if ( initTime ) {       // 初回？
    //myservo.writeMicroseconds(1500);  // サーボを中間点に設定します
    pos=20;
    myservo.write(pos);
    
    Serial.println( pos,DEC );
    for (pos = 30; pos <= 150; pos += 1) { // goes from 0 degrees to 180 degrees
      // in steps of 1 degree
      myservo.write(pos);              // tell servo to go to position in variable 'pos'
      delay(15);                       // waits 15ms for the servo to reach the position
      
    }
    for (pos = 150; pos >= 30; pos -= 1) { // goes from 180 degrees to 0 degrees
      myservo.write(pos);              // tell servo to go to position in variable 'pos'
      delay(15);                       // waits 15ms for the servo to reach the position
    }
    
    for (pos = 30; pos <= 90; pos += 1) { // goes from 180 degrees to 0 degrees
      myservo.write(pos);              // tell servo to go to position in variable 'pos'
      delay(15);                       // waits 15ms for the servo to reach the position
    }
    //myservo.writeMicroseconds(1500);  // サーボを中間点に設定します
    monoEyeBlink=true;
    TimerTc3.attachInterrupt(timerIsr);
    
    initTime=false;
  }
  byte buf;
  byte key;     // 受信データを格納するchar型の変数
  int i;
  // 受信データがあった時だけ、処理を行う
  if ( Serial.available() ) {       // 受信データがあるか？
      pinMode(9,OUTPUT);
    noAccessPeriod=0;
    monoEyeBlink=false;
    while((buf = Serial.read())!='\n'){// 1文字だけ読み込む
      key=buf;
      Serial.write( key );            // 1文字送信。受信データをそのまま送り返す。
      }
      if(30 <= key && key<=150){//モノアイの動作範囲が30度から150度までしかダメなので制限する
        myservo.write(key); 
      }else if(key==0xff){
        digitalWrite(9, HIGH);   // 点灯
      }else if(key==0x00){
        digitalWrite(9, LOW);    // 消灯
      }else{
    }
    //monoEyeBlink=true;
  }else{
    if(noAccessPeriod>=10){
      monoEyeBlink=true;
    }
  }
}

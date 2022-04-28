#include "BluetoothSerial.h"
#include "M5Lite.h"

const int ANALOG_PIN = 33;
const int DIGITAL_PIN = 23;
float           bat_vol             = 0;        // バッテリー電圧
BluetoothSerial SerialBT;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(115200);
  SerialBT.begin("ESP32test"); //Bluetooth device name
  Serial.println("The device started, now you can pair it with bluetooth!");
  M5Lite.begin();
  M5Lite.Axp.begin();
}

void loop() {
  // put your main code here, to run repeatedly:
    int value;
    byte sensingOn;
    String parseString;
    int i;

    bat_vol =M5Lite.Axp.GetBatVoltage();
    
    value = analogRead( ANALOG_PIN );
    sensingOn=digitalRead(DIGITAL_PIN);
    
    Serial.print( "Value: " );
    Serial.print( value );
    Serial.print( "\n" );
    Serial.print( "Voltage: " );
    Serial.print( bat_vol );
    Serial.print( "\n" );
    Serial.print( "digitalOut: " );
    Serial.print( sensingOn );
    Serial.print( "\n" );
    parseString=String(value, DEC);
    for(int i=0;i<parseString.length();i++){
      SerialBT.write(parseString.charAt(i));
    }
    SerialBT.write(',');
  /*
  if (SerialBT.available()) {
    Serial.write(value);
  }*/
  delay(20);

    

    delay( 500 );

}

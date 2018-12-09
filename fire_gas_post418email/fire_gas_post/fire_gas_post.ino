int buzzer = 8; // 用Pin8 輸出方波至蜂鳴器
int gas_din=2;
int gas_ain=A0;
int ad_value;
int photocellPin = 3;
int val = 0;
String sensor_value[4] ;
int UVOUT = A2;
int REF_3V3 = A1;
void setup()
{
  pinMode(buzzer,OUTPUT);
  pinMode(gas_din,INPUT);
  pinMode(gas_ain,INPUT);
  pinMode(UVOUT, INPUT);
  pinMode(REF_3V3, INPUT);
  Serial.begin(9600);
  delay(5);
}
void loop()
{
  //GAS
  if(digitalRead(gas_din)==LOW)
  {
    ad_value = analogRead(gas_ain);
    sensor_value[0] = String("Gas leak");
    sensor_value[1] = String(ad_value);
    b(); 
  }
  else
  {
    sensor_value[0] = String("Gas not leak");
    sensor_value[1] = String("0");
  }
  //FIRE
  val = digitalRead(photocellPin);         
  if(val==HIGH)
  {   
    sensor_value[2] = String("NO Fire");    
  }     
  else
  {         
   sensor_value[2] = String("ON Fire"); 
   b();  
  }
  //UV
  int uvLevel = averageAnalogRead(UVOUT);
  if(uvLevel<50)
  {
    sensor_value[3] = String("0");
  }
  else if(50 < uvLevel < 318)
  {
    sensor_value[3] = String("LOW");
  }
  else if(318 < uvLevel < 606)
  {
    sensor_value[3] = String("in");
  }
  else if(606 < uvLevel < 795)
  {
    sensor_value[3] = String("high");
  }
  else if(795 < uvLevel < 1079)
  {
    sensor_value[3] = String("excess");
  }
  else
  {
    sensor_value[3] = String("Dangerous");
  }
  sendpy();
}

void b()
{
  for ( int ii=0; ii<10; ii++ ) {
        tone(buzzer,1000);
        delay(50);
        tone(buzzer,500);
        delay(50);
    }
    noTone(buzzer);
    delay(2000);
}

void sendpy()
  {  
  if (Serial.available())
    {
      if (Serial.read() == 'a')
      {
        for (int i = 0; i < 4; i++)
        {
          Serial.print(sensor_value[i]);
          Serial.print(",");
        }
          Serial.println("");
      }
    }
  }
int averageAnalogRead(int pinToRead)
{
  byte numberOfReadings = 8;
  unsigned int runningValue = 0; 

  for(int x = 0 ; x < numberOfReadings ; x++)
    runningValue += analogRead(pinToRead);
  runningValue /= numberOfReadings;

  return(runningValue);  
}

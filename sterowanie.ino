void setup() {
  Serial.begin(9600);
}

void process(String com)
{
  if(com.length() == 16)
  {
    for(int i = 1; i <= (com.length() - 2); i++)
    {
      if(com.charAt(i) == '0')
        digitalWrite(i-1, LOW);
      else
        digitalWrite(i-1, HIGH);
    }
  }
}

void loop() {
  if(Serial.available())
  {
    String command = "";
    char znak;

    while(Serial.available() > 0)
    {
      znak = (char)Serial.read();
      command += znak;
    }

    process(command);
  }

}

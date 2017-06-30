String com_prev;
bool start;

void setup() 
{
  Serial.begin(9600);

  reset();
}

void reset()
{
  for(int i = 0; i <= 13; i++)
  {
    pinMode(i, OUTPUT);
    digitalWrite(i, LOW);
  }
  com_prev = "#00000000000000#";
  start = true;
}

String to_send()
{
  String com_send = "#";
  bool reader;
  
  for(int i = 1; i <= (com_prev.length() - 2); i++)
  {
    if(com_prev.charAt(i) != 'I')
      com_send += 'X';
    else
    {
      reader = digitalRead(i-1);

      if(reader)
        com_send += '1';
      else
        com_send += '0';
    }
  }
  com_send += '#';

  return com_send;
}

void process(String com)
{
  if(com.length() == 16 && com.charAt(0) == '#' && com.charAt(15) == '#')
  {
    for(int i = 1; i <= (com.length() - 2); i++)
    {
      if(com.charAt(i) == '0')
      {
        if(com_prev.charAt(i) == '1')
          digitalWrite(i-1, LOW);
        else if(com_prev.charAt(i) == 'I')
        {
          pinMode(i-1, OUTPUT);
          digitalWrite(i-1, LOW);
        }
      }
      else if(com.charAt(i) == '1')
      {
        if(com_prev.charAt(i) == '0')
          digitalWrite(i-1, HIGH);
        
        else if(com_prev.charAt(i) == 'I')
        {
          pinMode(i-1, OUTPUT);
          digitalWrite(i-1, HIGH);
        }
      }
      else if(com.charAt(i) == 'I')
      {
        if(com_prev.charAt(i) != 'I')
          pinMode(i-1, INPUT_PULLUP);
      }
    }
  }
  
  com_prev = com;
}

void loop() 
{
  Serial.println(to_send());
  
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

  delay(75);
}

#include <Arduino.h>
#include "state_machine.h"

#define LED_R 13
#define LED_G 12
#define LED_B 11

#define GPIO_BTN 7

SM_DEFINE_STATE(STOP);
SM_DEFINE_STATE(PREPARE_ON);
SM_DEFINE_STATE(PREPARE_OFF);
SM_DEFINE_STATE(RUN);

SM_DECLARE_FUNCTION(runMachine);

SM_state_t arduinoState;

void setup()
{
  pinMode(LED_R, OUTPUT);
  pinMode(LED_G, OUTPUT);
  pinMode(LED_B, OUTPUT);
  pinMode(GPIO_BTN, INPUT_PULLUP);

  Serial.begin(9600);

  SM_INIT(&arduinoState, STOP);
}

void loop()
{
  runMachine(&arduinoState);
}

SM_timer_t timer1 = SM_timer_DEFAULT;
SM_timer_t timer2 = SM_timer_DEFAULT;

int buttonPressed(SM_state_t *state)
{
  return digitalRead(GPIO_BTN) == LOW;
}

void stopAction(SM_state_t *state, int phase)
{
  if (phase == SM_PHASE_ENTERING)
  {
    digitalWrite(LED_R, HIGH);
  }
  else if (phase == SM_PHASE_EXIT)
  {
    digitalWrite(LED_R, LOW);
  }
}

void prepareOnAction(SM_state_t *state, int phase)
{
  if (phase == SM_PHASE_ENTERING)
  {
    digitalWrite(LED_B, HIGH);
    smTimerStart(&timer1);
  }
  else if (phase == SM_PHASE_EXIT)
  {
    digitalWrite(LED_B, LOW);
  }
}

void prepareOffAction(SM_state_t *state, int phase)
{
  if (phase == SM_PHASE_ENTERING)
  {
    smTimerStart(&timer2);
  }
}

void runAction(SM_state_t *state, int phase)
{
  if (phase == SM_PHASE_ENTERING)
  {
    digitalWrite(LED_G, HIGH);
  }
  else if (phase == SM_PHASE_EXIT)
  {
    digitalWrite(LED_G, LOW);
  }
}

SM_DEFINE_FUNCTION_BEGIN(runMachine)
SM_STATE_BEGIN(STOP, stopAction)
SM_TRASITION(buttonPressed, PREPARE_ON)
SM_STATE_END()

SM_STATE_BEGIN(PREPARE_ON, prepareOnAction)
SM_TRASITION(buttonPressed, RUN)
SM_TRASITION_INLINE(smTimerElapsed(&timer1, 1500), PREPARE_OFF)
SM_STATE_END()

SM_STATE_BEGIN(PREPARE_OFF, prepareOffAction)
SM_TRASITION(buttonPressed, RUN)
SM_TRASITION_INLINE(smTimerElapsed(&timer2, 1000), PREPARE_ON)
SM_STATE_END()

SM_STATE_BEGIN(RUN, runAction)
SM_TRASITION(buttonPressed, STOP)
SM_STATE_END()
SM_DEFINE_FUNCTION_END()

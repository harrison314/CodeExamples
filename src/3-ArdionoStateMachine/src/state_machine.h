#pragma once
#include <Arduino.h>

#define SM_PHASE_ENTERING 1
#define SM_PHASE_DO 2
#define SM_PHASE_EXIT 3

typedef struct
{
    int state;
    int phase;
} SM_state_t;

typedef void(SM_action_fn)(SM_state_t *, int);

#define SM_DEFINE_STATE(name) const int name = __COUNTER__
#define SM_STATE_BEGIN(p_state, action)         \
    if (p_state == _state->state)               \
    {                                           \
        action(_state, _state->phase);          \
        if (_state->phase == SM_PHASE_ENTERING) \
        {                                       \
            _state->phase = SM_PHASE_DO;   \
        }                                       \
        _last_action = (action);

#define SM_STATE_END() \
    return;            \
    }

#define SM_TRASITION_INLINE(condition, p_state)     \
    if (condition)                                  \
    {                                               \
        if (_state->state != p_state)               \
        {                                           \
            _state->phase = SM_PHASE_ENTERING;      \
            _last_action(_state, SM_PHASE_EXIT); \
        }                                           \
        _state->state = p_state;                    \
        return;                                     \
    }

#define SM_TRASITION(condition, p_state) SM_TRASITION_INLINE(condition(_state), p_state)

#define SM_DEFINE_FUNCTION_BEGIN(name) \
    void name(SM_state_t *_state)      \
    {                                  \
        SM_action_fn *_last_action = NULL;
#define SM_DEFINE_FUNCTION_END() }

#define SM_DECLARE_FUNCTION(name) void name(SM_state_t *_state)

#define SM_INIT(state_t_ptr, p_state) \
    (state_t_ptr)->state = p_state;   \
    (state_t_ptr)->phase = SM_PHASE_ENTERING

typedef struct
{
    unsigned long value;

} SM_timer_t;

#define SM_timer_DEFAULT {0};

void smTimerStart(SM_timer_t *timer)
{
    timer->value = millis();
    if (timer->value == 0)
    {
        timer->value = 1;
    }
}

int smTimerElapsed(SM_timer_t *timer, unsigned long duration)
{
    if (timer->value != 0 && millis() - timer->value >= duration)
    {
        timer->value = 0;
        return 1;
    }

    return 0;
}

/* mbed Microcontroller Library
 * Copyright (c) 2019 ARM Limited
 * SPDX-License-Identifier: Apache-2.0
 */

#include "mbed.h"
#include "rtos.h"
#include "EthernetInterface.h"

// Blinking rate in milliseconds
#define NUM_BUTTONS 6


typedef struct Preset {
    bool sending;
    PwmOut* led;
    char number;
} Preset;

extern "C" void mbed_reset();

EthernetInterface net;
UDPSocket sock;
Endpoint broadcast;
Serial pc(USBTX, USBRX);
DigitalOut green(p28), yellow(p27);
LocalFileSystem local("local");
char readFile[16] = {'/', 'l', 'o', 'c', 'a', 'l', '/', 'a', 'r', 't', '0', '.', 't', 'x', 't', 0x00};
char outputBuffer[530];
DigitalIn buttons[NUM_BUTTONS] = {DigitalIn(p20), DigitalIn(p19), DigitalIn(p18), DigitalIn(p17), DigitalIn(p16), DigitalIn(p15)};
bool prevState[NUM_BUTTONS] = {true, true, true, true, true, true};
bool currState[NUM_BUTTONS] = {true, true, true, true, true, true};
Mutex FileIO;

Preset createPreset(PwmOut* led, char number) {
    Preset newPreset;
    newPreset.led = led;
    newPreset.sending = false;
    newPreset.number = number;
    return newPreset;
    
}

void handleInput(Preset* preset) {
    preset->sending = !preset->sending;
    *(preset->led) = preset->sending;
    FileIO.lock();
    if (!preset->sending) {
        readFile[10] = '0';
    } else {
        readFile[10] = preset->number;
    }
    FileIO.unlock();
}



PwmOut led1(p21), led2(p22), led3(p23), led4(p24), led5(p25), led6(p26);
Preset preset1 = createPreset(&led1, '1');
Preset preset2 = createPreset(&led2, '2');
Preset preset3 = createPreset(&led3, '3');
Preset preset4 = createPreset(&led4, '4');
Preset preset5 = createPreset(&led5, '5');
Preset preset6 = createPreset(&led6, '6');
Preset* presets[6] = {&preset1, &preset2, &preset3, &preset4, &preset5, &preset6};


void fetchButtons() {
while(1) {
    for (int i = 0; i < NUM_BUTTONS; ++i) {
        prevState[i] = currState[i];
        currState[i] = buttons[i];
        if (currState[i] == 0 && prevState[i] == 1) {
        handleInput(presets[i]);
        for (int j = 0; j < NUM_BUTTONS; ++j) {
            if (j != i) {
                *presets[j]->led = 0.0;
                presets[j]->sending = false;
            }
        }
        Thread::wait(200);
        }
    }
}
}

void sendArtDmx(void const *args) {

  broadcast.set_address("255.255.255.255", 6454);
  FILE* outputFile;
    
    while(1) {
        FileIO.lock();
        outputFile = fopen(readFile, "rb");
        if (outputFile != NULL) {
            while(fread(outputBuffer, sizeof(char), 1, outputFile) == 1) {
                if (outputBuffer[0] == 0x41) {
                    if (fread(outputBuffer + 1, sizeof(char), 529, outputFile) == 529) {
                                        yellow = 1;
                sock.sendTo(broadcast, outputBuffer, sizeof(outputBuffer));
                yellow = 0;
                    }
                }
            }
        }
        fclose(outputFile);
        FileIO.unlock();
        Thread::wait(100);
    }
}

void checkForReset(void const * args) {
    while(1) {
    if (pc.readable()) {
        if (pc.getc() == 'R') {
            if (pc.getc() == 'e') {
                if (pc.getc() == 's') {
                    if (pc.getc() == 'e') {
                        if (pc.getc() == 't') {
                            mbed_reset();
                        }
                    }
                }
            }
        }
    }
    }
}

void PULSE_LEDS(void const* args) {
    while(1) {
        for(float i = 0.0f; i <= 1.0f; i += 0.05f) {
            for (int j = 0; j < NUM_BUTTONS; ++j) {
                *presets[j]->led = i;
            }
            Thread::wait(50);
        }
        for(float i = 1.0f; i >= 0.0f; i -= 0.05f) {
            for (int j = 0; j < NUM_BUTTONS; ++j) {
                *presets[j]->led = i;
            }
            Thread::wait(50);
        }
    }
}

int main() {
    Thread temp(PULSE_LEDS, NULL);
    for (int i = 0; i < NUM_BUTTONS; ++i) {
        buttons[i].mode(PullUp);
    }
    net.init("2.0.0.1", "255.255.255.255", "0.0.0.0");
    net.connect();
    sock.init();
    sock.set_broadcasting();
    temp.terminate();
    for (int i = 0; i < NUM_BUTTONS; ++i) {
        *presets[i]->led = 0.0f;
    }
    green = 1;

    Thread t1(sendArtDmx, NULL);
    Thread t2(checkForReset, NULL);

    while(1) {
    fetchButtons();
    return 1;
}
}